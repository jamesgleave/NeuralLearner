using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Genome
{
    /// <summary>
	/// The list of connections, each of which refers to two node genes being connected. The connections are stored in a dictionary, mapped by the connection's ID (int).
	/// </summary>
    private Dictionary<int, ConnectionGene> connections;

    /// <summary>
	/// The list of nodes. The nodes are stored in a dictionary, mapped by the node's ID (int).
    /// </summary>
    private Dictionary<int, NodeGene> nodes;

    /// <summary>
    /// Generates innovation numbers
    /// </summary>
    private InnovationGenerator innovation;

    public Genome()
    {
        connections = new Dictionary<int, ConnectionGene>();
        nodes = new Dictionary<int, NodeGene>();
        innovation = new InnovationGenerator();
    }

    /// <summary>
    /// Add a node to a genome
    /// </summary>
    /// <param name="node"></param>
    public void AddNode(NodeGene node)
    {
        nodes.Add(node.GetID(), node);
    }

    /// <summary>
    /// Add a connection to a genome
    /// </summary>
    /// <param name="node"></param>
    public void AddConnection(ConnectionGene con)
    {
        connections.Add(con.GetInnovation(), con);
    }

    /// <summary>
    /// Returns the connections dict
    /// </summary>
    /// <returns></returns>
    public Dictionary<int, ConnectionGene> GetConnections()
    {
        return connections;
    }

    /// <summary>
    /// Returns the nodes dict
    /// </summary>
    /// <returns></returns>
    public Dictionary<int, NodeGene> GetNodes()
    {
        return nodes;
    }

    /// <summary>
    /// Remove a node from the genome
    /// </summary>
    /// <returns></returns>
    public void RemoveNode(int id)
    {
        // Remove the node
        nodes.Remove(id);

        // Remove any connection using the node
        // Create temp list of keys
        List<int> keys = new List<int>();
        foreach (int key in connections.Keys)
        {
            keys.Add(key);
        }

        // Look at each key and remove any connection
        foreach (int key in keys)
        {
            if (connections[key].getInNode() == id || connections[key].getOutNode() == id)
            {
                connections.Remove(key);
            }
        }
    }


    /// <summary>
    /// Returns the next innovation number & increments the genome's innovation generator
    /// </summary>
    /// <returns></returns>
    public int GetNextInnovation()
    {
        return innovation.GetInnovation();
    }

    public bool ChangeActivationMutation()
    {
        // Create a list of all keys in the nodes list
        List<int> keys = new List<int>(nodes.Count);
        foreach (var item in nodes)
        {
            keys.Add(item.Key);
        }

        // Create a new random variable
        System.Random r = new System.Random();

        // Grab the node that we will be changing
        NodeGene node = nodes[keys[r.Next(keys.Count)]];

        // Only change the activation if the node is not an output node (they must stay a sigmoid)
        if (node.IsOutput() == false)
        {
            node.SetActivationString(Activations.ActivationHelper.GetRandomActivation());
        }

        return true;
    }

    /// <summary>
    /// Mutate the calculation method
    /// </summary>
    /// <returns></returns>
    public bool ChangeCalculationMethodMutation()
    {
        // Create a list of all keys in the nodes list
        List<int> keys = new List<int>(nodes.Count);
        foreach (var item in nodes)
        {
            keys.Add(item.Key);
        }

        // Create a new random variable
        System.Random r = new System.Random();

        // Grab the node that we will be changing
        NodeGene node = nodes[keys[r.Next(keys.Count)]];

        // Only change method of hidden nodes
        if (!node.IsHidden())
        {
            return false;
        }

        // Change the calculation method
        node.SetCalculationMethod(MethodHelp.GetRandomMethod());

        //throw new System.Exception(node.GetCalculationMethod().ToString());

        return true;
    }

    /// <summary>
    /// Adds a connection between two random nodes. Returns true if successful
    /// </summary>
    public bool AddConnectionMutation()
    {
        // Create a list of all keys in the nodes list
        List<int> keys = new List<int>(nodes.Count);
        List<int> non_input_ids = new List<int>();
        List<int> non_output_ids = new List<int>();
        foreach (var item in nodes)
        {
            keys.Add(item.Key);

            // Add to these lists (in case we get a double up, where node is attatch to another of the same node given its an input or output)
            if (item.Value.IsHidden() || item.Value.IsOutput())
            {
                non_input_ids.Add(item.Key);
            }
            else if (item.Value.IsHidden() || item.Value.IsInput())
            {
                non_output_ids.Add(item.Key);
            }
        }

        // If we have no connections, there is a 90% chance that the first connection formed will be to the movement neuron
        NodeGene node1, node2;
        if (connections.Count == 0 && (0.90f > Random.value || Manager.mobile_start))
        {
            // Grab the two nodes that we will be connecting
            node1 = nodes[keys[Random.Range(0, keys.Count)]];
            node2 = nodes[25]; // 25 is the index of the movement neuron
        }
        else
        {
            // Grab the two nodes that we will be connecting
            node1 = nodes[keys[Random.Range(0, keys.Count)]];
            node2 = nodes[keys[Random.Range(0, keys.Count)]];
        }

        // If we fail and have an input connecting to an input or an output connecting to an output we search and find all outputs/inputs and select one at random that works
        if (node1.IsInput() && node2.IsInput())
        {
            node1 = nodes[non_input_ids[Random.Range(0, non_input_ids.Count)]];
        }
        else if (node1.IsOutput() && node2.IsOutput())
        {
            node1 = nodes[non_output_ids[Random.Range(0, non_output_ids.Count)]];
        }

        // Reversed, meaning swapping the outgoing and incomming neuron
        bool reversed = false;

        // If the outgoing neuron is hidden and the incomming neuron is an input neuron, we should reverse the connection
        if (node1.IsHidden() && node2.IsInput())
        {
            reversed = true;
        }
        // If the outgoing neuron is an ouput it should be reversed
        else if (node1.IsOutput() && node2.IsHidden())
        {
            reversed = true;
        }
        else if (node1.IsOutput() && node2.IsInput())
        {
            reversed = true;
        }

        // If the connection is already there...
        bool connection_exists = false;
        // Loop through each connection and check if we have already have the connection
        foreach (ConnectionGene cg in connections.Values)
        {
            // Check to see if the connection already exists
            if (cg.getInNode() == node1.GetID() && cg.getOutNode() == node2.GetID())
            {
                connection_exists = true;
                break;
            }
            else if (cg.getOutNode() == node1.GetID() && cg.getInNode() == node2.GetID())
            {
                connection_exists = true;
                break;
            }
        }

        // Return if the connection exists, because we would not want to create another connection if one exists!
        // Also, inputs should not be able to connect to inputs, and same with outputs!
        // TODO Remove from the if statement. This allows for recurrent output nodes
        if (connection_exists || node1.IsInput() && node2.IsInput() || node1.IsOutput() && node2.IsOutput())
        {
            return false;
        }

        // if it is reversed, we must swap node 1 and node 2
        float weight = (Random.value * 2f) - 1f;
        ConnectionGene new_connection;
        if (reversed)
        {
            new_connection = new ConnectionGene(node2.GetID(), node1.GetID(), weight, true, innovation.GetInnovation());
        }
        else
        {
            new_connection = new ConnectionGene(node1.GetID(), node2.GetID(), weight, true, innovation.GetInnovation());
        }

        // Add the new connection to our connections
        connections.Add(new_connection.GetInnovation(), new_connection);
        return true;
    }

    /// <summary>
    /// Drops an active connection and reactivates a dropped connection
    /// </summary>
    /// <returns></returns>
    public bool InvertConnectionMutation()
    {
        // Get a random connection and disable it
        // Create a list of all keys in the connections list
        List<int> keys = new List<int>(connections.Count);
        foreach (var item in connections)
        {
            keys.Add(item.Key);
        }

        // If we have no connections, return false
        if (keys.Count == 0)
        {
            return false;
        }

        // Create a new random variable
        System.Random r = new System.Random();

        // Disable connection
        ConnectionGene connection = connections[keys[r.Next(keys.Count)]];

        // Swap the expression status
        if (connection.IsExpressed())
        {
            connection.SetExpressedStatus(false);
            return true;
        }
        else
        {
            connection.SetExpressedStatus(true);
            return false;
        }
    }

    /// <summary>
    /// Adds a new node inplace of an existing connection. Returns true if successful
    /// </summary>
    public bool AddNodeMutation()
    {

        // We need connections to create a node
        if (connections.Count == 0)
        {
            return false;
        }

        // Get a random connection and disable it
        // Create a list of all keys in the connections list
        List<int> keys = new List<int>(connections.Count);
        foreach (var item in connections)
        {
            keys.Add(item.Key);
        }

        // Create a new random variable
        System.Random r = new System.Random();

        // Grab the two nodes that we will be connecting
        ConnectionGene connection = connections[keys[r.Next(keys.Count)]];

        // Get two random nodes
        // Create a list of all keys in the nodes list
        keys.Clear();
        foreach (var item in nodes)
        {
            keys.Add(item.Key);
        }

        // Grab the two nodes that we will be connecting with another
        NodeGene in_node = nodes[connection.getInNode()];
        NodeGene out_node = nodes[connection.getOutNode()];


        connection.SetExpressedStatus(false);

        // Create a new node
        NodeGene new_node = new NodeGene(NodeGeneType.Hidden, nodes.Count);

        // Increment the node's ID until we have a new one
        while (nodes.ContainsKey(new_node.GetID()))
        {
            new_node.IncrementID();
        }

        // Create the two new connections
        ConnectionGene going_to_new = new ConnectionGene(in_node.GetID(), new_node.GetID(), 1f, true, innovation.GetInnovation());
        ConnectionGene new_to_next = new ConnectionGene(new_node.GetID(), out_node.GetID(), connection.GetWeight(), true, innovation.GetInnovation());

        // Add the new node to the dict
        nodes.Add(new_node.GetID(), new_node);

        // Add the new connections to the dict
        connections.Add(going_to_new.GetInnovation(), going_to_new);
        connections.Add(new_to_next.GetInnovation(), new_to_next);

        return true;
    }

    public bool RemoveNodeMutation()
    {
        // Create a list of all keys in the nodes list
        List<int> keys = new List<int>(nodes.Count);
        foreach (var item in nodes)
        {
            keys.Add(item.Key);
        }

        // Create a new random variable
        System.Random r = new System.Random();

        // Grab the node we will be removing
        NodeGene node = nodes[keys[r.Next(keys.Count)]];

        // Only remove a hidden neuron
        if (!node.IsHidden())
        {
            return false;
        }

        // Remove the node
        RemoveNode(node.GetID());
        return true;
    }

    /// <summary>
    /// Returns a copy of the genome
    /// </summary>
    /// <returns></returns>
    public Genome Copy()
    {
        // Create new
        Genome new_genome = new Genome();

        // Copy each node
        foreach (NodeGene node in nodes.Values)
        {
            new_genome.AddNode(node.Copy());
        }

        // Copy each connection
        foreach (ConnectionGene connection in connections.Values)
        {
            new_genome.AddConnection(connection.Copy());
        }

        // Set the innovation generator
        new_genome.SetInnovationProgress(innovation.Query());

        // Return the new genome
        return new_genome;
    }

    public void SetInnovationProgress(int inno)
    {
        innovation.SetProgress(inno);
    }

    /// <summary>
    /// Crosses the genome of two parents. The first parameter MUST be the more fit parent
    /// </summary>
    /// <param name="more_fit"></param>
    /// <param name="less_fit"></param>
    /// <returns></returns>
    public static Genome Crossover(Genome more_fit, Genome less_fit)
    {
        // Create the child
        Genome child = new Genome();

        // Add each node from the more fit parent
        foreach (NodeGene node in more_fit.GetNodes().Values)
        {
            child.AddNode(node.Copy());
        }

        // Add matching genes
        foreach (ConnectionGene connection in more_fit.GetConnections().Values)
        {
            // If we have matching connection genes...
            if (less_fit.GetConnections().ContainsKey(connection.GetInnovation()))
            {
                // Is matching
                bool from_more_fit = Random.value > 0.5f;
                // If we take from more fit parent, chose 'connection' else chose the less fit with same innovation number
                ConnectionGene child_con = from_more_fit ? connection.Copy() : less_fit.GetConnections()[connection.GetInnovation()].Copy();

                // Add the connection to the child
                child.AddConnection(child_con);
            }
            else
            {
                // Is disjoint
                // Comes from more fit
                child.AddConnection(connection.Copy());
            }
        }
        // Return the child
        return child;
    }

    public string SaveGenome(string save_path)
    {
        int id;
        string method;
        string code = "";
        string activation;
        NodeGeneType type;

        code += "Genome:Innovation=" + innovation.Query() + "\n";

        code += "Nodes: ";
        foreach (NodeGene node in nodes.Values)
        {
            id = node.GetID();
            method = node.GetCalculationMethod().ToString();
            type = node.GetNodeType();
            activation = node.GetActivationString();
            code += id + "," + method + "," + type + "," + activation + "; ";
        }

        code += "\n";
        code += "Connections: ";
        foreach (ConnectionGene conneciton in connections.Values)
        {
            code += conneciton.GetInnovation() + "," + conneciton.getInNode() + "," + conneciton.getOutNode() + "," + conneciton.GetWeight() + "," + conneciton.IsExpressed() + "; ";
        }

        File.WriteAllText(save_path, code);
        return code;
    }

    public static Genome LoadFrom(string path_to_saved)
    {
        // Create the new genome
        Genome new_genome = new Genome();

        // Read the lines
        string[] lines = System.IO.File.ReadAllLines(path_to_saved);

        // Display the file contents by using a foreach loop.
        foreach (string line in lines)
        {
            // Look at each part of the Genome
            if (line.Contains("Genome:"))
            {
                string[] comps = line.Split(':');
                foreach (string comp in comps)
                {
                    // Set innovation
                    if (comp.Contains("Innovation"))
                    {
                        new_genome.innovation.SetProgress(int.Parse(comp.Split('=')[1]));
                    }
                }
            }
            // Add new nodes
            else if (line.Contains("Nodes:"))
            {
                // Pattern:
                // id + "," + method + "," + type + "," + activation + "; "
                string data = line.Split(':')[1];


                // Look at each node
                int id;
                string method;
                string activation;
                NodeGeneType type;
                foreach (string comp in data.Split(';'))
                {
                    // Skip out if the string is empty
                    if (comp.Equals(" "))
                    {
                        continue;
                    }
                    // Look at each data point in each node
                    var node_data = comp.Split(',');  // break the string
                    id = int.Parse(node_data[0]);  // Get the ID
                    method = node_data[1]; // Get the method
                    // Determine the neuron type
                    type = NodeGeneType.Hidden;
                    switch (node_data[2])
                    {
                        case "Input":
                            type = NodeGeneType.Input;
                            break;
                        case "Hidden":
                            type = NodeGeneType.Hidden;
                            break;
                        case "Output":
                            type = NodeGeneType.Output;
                            break;
                    }
                    // Get the activation code
                    activation = node_data[3];

                    // Add node to genome
                    new_genome.AddNode(new NodeGene(type, id, activation, MethodHelp.FromName(method)));
                }
            }
            else if (line.Contains("Connections:"))
            {
                // Pattern:
                // Look at each connection
                string data = line.Split(':')[1];

                // Look at each node
                int innovation, in_node_id, out_node_id;
                float weight;
                bool is_expressed;
                foreach (string comp in data.Split(';'))
                {

                    // Skip out if the string is empty
                    if (comp.Equals(" "))
                    {
                        continue;
                    }

                    // Look at each data point in each node
                    var c_data = comp.Split(',');  // break the string
                    innovation = int.Parse(c_data[0]);
                    in_node_id = int.Parse(c_data[1]);
                    out_node_id = int.Parse(c_data[2]);
                    weight = float.Parse(c_data[3]);
                    is_expressed = bool.Parse(c_data[4]);
                    new_genome.AddConnection(new ConnectionGene(in_node_id, out_node_id, weight, is_expressed, innovation));
                }
            }
        }

        return new_genome;
    }

    public static string Mutate(Genome model, float mutation_amount, float weight_mutation_prob, float neuro_mutation_prob, float bias_mutation_prob, float dropout_prob)
    {
        string log = "";

        // There is a base 80% chance of a genome mutation
        if (0.80f > Random.value)
        {

            // Mutate add node
            float prob_trigger = Random.Range(0f, 1f);
            bool trigger = (neuro_mutation_prob > prob_trigger);
            if (trigger)
            {
                log += "Added Node: <" + neuro_mutation_prob + " > " + prob_trigger + "> " + model.AddNodeMutation() + ", ";
            }

            // Mutate activations
            prob_trigger = Random.Range(0f, 1f);
            trigger = (neuro_mutation_prob > prob_trigger);
            if (trigger)
            {
                log += "Changed Activation: " + ", ";
                model.ChangeActivationMutation();
            }

            // Mutate neuron types
            prob_trigger = Random.Range(0f, 1f);
            trigger = (bias_mutation_prob > prob_trigger);
            if (trigger)
            {
                log += "Changed Neuron Type: " + ", ";
                model.ChangeCalculationMethodMutation();
            }

            // Mutate the connections by adding a few more
            float num_added_connections = 3 * neuro_mutation_prob;
            // We can only add up to 5 new connections per mutation
            prob_trigger = Random.Range(0f, 1f);
            trigger = (weight_mutation_prob > prob_trigger);
            while (trigger && num_added_connections > 0)
            {
                // Add new connection
                log += "Added Connection: " + model.AddConnectionMutation() + ", ";
                num_added_connections -= 1;

                // Recalculate trigger
                prob_trigger = Random.Range(0f, 1f);
                trigger = (weight_mutation_prob > prob_trigger);
            }

            // Loop through each connection and mutate the weights according to the passed weight mutation probability
            foreach (ConnectionGene con in model.connections.Values)
            {
                // First, we must trigger if we want to mutate this connection at all
                if (weight_mutation_prob > Random.value)
                {
                    // If weight_mutation_prob is triggered, perturb the weight. There is a 75% chance of this happening
                    if (0.75f > Random.value)
                    {
                        con.SetWeight(con.GetWeight() + Random.Range(-2 * mutation_amount, 2 * mutation_amount));
                        // Only add to the log if our value is greater than zero
                        if (con.GetWeight() > 0)
                        {
                            log += "Updated weight, ";
                        }
                    }
                    // If the weight mutation is not triggered, then we assign the weights a new value (as from the NEAT paper)
                    // If the weight is not pertubed, there is a 25% chance of the weight being set to an all new value
                    else if (0.25f > Random.value)
                    {
                        // If not, then set the weight value
                        con.SetWeight(Random.Range(-2 * mutation_amount, 2 * mutation_amount));
                        log += "Set weight, ";
                    }
                }
            }


            // Recalculate trigger but this time for dropping a connection
            prob_trigger = Random.Range(0f, 1f);
            trigger = (dropout_prob > prob_trigger);
            if (trigger)
            {
                // Drop the connection
                model.InvertConnectionMutation();
                log += "Dropped connection, ";
            }
        }
        return log;
    }

    /// <summary>
    /// Counts the number of matching genes between g1 and g2
    /// </summary>
    /// <param name="g1"></param>
    /// <param name="g2"></param>
    /// <returns></returns>
    /// TODO optimize this stuff
    public static int CountMatchingGenes(Genome g1, Genome g2)
    {
        // Initialize our values
        int matching = 0;
        List<int> keys_1 = new List<int>();
        List<int> keys_2 = new List<int>();

        // Create a list of keys from g1
        foreach (int i in g1.GetNodes().Keys)
        {
            keys_1.Add(i);
        }

        // Create a list of keys from g2
        foreach (int i in g2.GetNodes().Keys)
        {
            keys_2.Add(i);
        }

        // Sort each list and grab the highest innovation number
        keys_1.Sort();
        keys_2.Sort();
        int max_inno = Mathf.Max(keys_1[keys_1.Count - 1], keys_2[keys_2.Count - 1]);

        // Loop over all node genes to find matching genes
        for (int i = 0; i <= max_inno; i++)
        {
            // If g1 and g2 both contain the innovation number i, it is a matching gene
            if (g1.GetNodes().ContainsKey(i) && g2.GetNodes().ContainsKey(i))
            {
                matching++;
            }
        }

        // Now we do the same thing for the connection genes
        // Clear the key lists
        keys_1.Clear();
        keys_2.Clear();

        // Create a list of keys from g1
        foreach (int i in g1.GetConnections().Keys)
        {
            keys_1.Add(i);
        }

        // Create a list of keys from g2
        foreach (int i in g2.GetConnections().Keys)
        {
            keys_2.Add(i);
        }

        // Sort each list and grab the highest innovation number
        keys_1.Sort();
        keys_2.Sort();
        max_inno = Mathf.Max(keys_1[keys_1.Count - 1], keys_2[keys_2.Count - 1]);

        // Loop over all node genes to find matching genes
        for (int i = 0; i <= max_inno; i++)
        {
            // If g1 and g2 both contain the innovation number i, it is a matching gene
            if (g1.GetConnections().ContainsKey(i) && g2.GetConnections().ContainsKey(i))
            {
                matching++;
            }
        }
        return matching;
    }

    // TODO optimize this!!!
    public static int CountDisjointGenes(Genome g1, Genome g2)
    {
        // Initialize our values
        int disjoint = 0;
        List<int> keys_1 = new List<int>();
        List<int> keys_2 = new List<int>();

        // Create a list of keys from g1
        foreach (int i in g1.GetNodes().Keys)
        {
            keys_1.Add(i);
        }

        // Create a list of keys from g2
        foreach (int i in g2.GetNodes().Keys)
        {
            keys_2.Add(i);
        }

        // Sort each list and grab the highest innovation number
        keys_1.Sort();
        keys_2.Sort();
        int max_inno = Mathf.Max(keys_1[keys_1.Count - 1], keys_2[keys_2.Count - 1]);

        // Loop over all node genes to find matching genes
        for (int i = 0; i <= max_inno; i++)
        {
            // If g1 has a gene that g2 does not, or vice versa, and i is less than the max innovation number, we add to disjoint
            if (max_inno > i && (g1.GetNodes().ContainsKey(i) && !g2.GetNodes().ContainsKey(i) || !g1.GetNodes().ContainsKey(i) && g2.GetNodes().ContainsKey(i)))
            {
                disjoint++;
            }
        }

        // Now we do the same thing for the connection genes
        // Clear the key lists
        keys_1.Clear();
        keys_2.Clear();

        // Create a list of keys from g1
        foreach (int i in g1.GetConnections().Keys)
        {
            keys_1.Add(i);
        }

        // Create a list of keys from g2
        foreach (int i in g2.GetConnections().Keys)
        {
            keys_2.Add(i);
        }

        // Sort each list and grab the highest innovation number
        keys_1.Sort();
        keys_2.Sort();
        max_inno = Mathf.Max(keys_1[keys_1.Count - 1], keys_2[keys_2.Count - 1]);

        // Loop over all node genes to find matching genes
        for (int i = 0; i <= max_inno; i++)
        {
            // If g1 has a gene that g2 does not, or vice versa, and i is less than the max innovation number, we add to disjoint
            if (max_inno > i && (g1.GetConnections().ContainsKey(i) && !g2.GetConnections().ContainsKey(i) || !g1.GetConnections().ContainsKey(i) && g2.GetConnections().ContainsKey(i)))
            {
                disjoint++;
            }
        }
        return disjoint;
    }

    // TODO optimize this!!!
    public static int CountExcessGenes(Genome g1, Genome g2)
    {
        // Initialize our values
        int excess = 0;
        List<int> keys_1 = new List<int>();
        List<int> keys_2 = new List<int>();

        // Create a list of keys from g1
        foreach (int i in g1.GetNodes().Keys)
        {
            keys_1.Add(i);
        }

        // Create a list of keys from g2
        foreach (int i in g2.GetNodes().Keys)
        {
            keys_2.Add(i);
        }

        // Sort each list and grab the highest innovation number
        keys_1.Sort();
        keys_2.Sort();
        int max_inno = Mathf.Max(keys_1[keys_1.Count - 1], keys_2[keys_2.Count - 1]);

        // Loop over all node genes to find matching genes
        for (int i = 0; i <= max_inno; i++)
        {
            // If we have the innovation number i in g1 and not g2 or vice versa, and the highest innovation number for g* without the inno number then it is excess
            if (!g1.GetNodes().ContainsKey(i) && g2.GetNodes().ContainsKey(i) && keys_1[keys_1.Count - 1] < i || !g2.GetNodes().ContainsKey(i) && g1.GetNodes().ContainsKey(i) && keys_2[keys_2.Count - 1] < i)
            {
                excess++;
            }
        }

        // Now we do the same thing for the connection genes
        // Clear the key lists
        keys_1.Clear();
        keys_2.Clear();

        // Create a list of keys from g1
        foreach (int i in g1.GetConnections().Keys)
        {
            keys_1.Add(i);
        }

        // Create a list of keys from g2
        foreach (int i in g2.GetConnections().Keys)
        {
            keys_2.Add(i);
        }

        // Sort each list and grab the highest innovation number
        keys_1.Sort();
        keys_2.Sort();
        max_inno = Mathf.Max(keys_1[keys_1.Count - 1], keys_2[keys_2.Count - 1]);

        // Loop over all node genes to find matching genes
        for (int i = 0; i <= max_inno; i++)
        {
            // If we have the innovation number i in g1 and not g2 or vice versa, and the highest innovation number for g* without the inno number then it is excess
            if (!g1.GetConnections().ContainsKey(i) && g2.GetConnections().ContainsKey(i) && keys_1[keys_1.Count - 1] < i || !g2.GetConnections().ContainsKey(i) && g1.GetConnections().ContainsKey(i) && keys_2[keys_2.Count - 1] < i)
            {
                excess++;
            }
        }
        return excess;
    }

    // TODO optimize this!!!
    public static float AverageWeightDifference(Genome g1, Genome g2)
    {
        // Initialize our values
        int matching = 0;
        float diff = 0;
        List<int> keys_1 = new List<int>();
        List<int> keys_2 = new List<int>();

        // Create a list of keys from g1
        foreach (int i in g1.GetConnections().Keys)
        {
            keys_1.Add(i);
        }

        // Create a list of keys from g2
        foreach (int i in g2.GetConnections().Keys)
        {
            keys_2.Add(i);
        }

        // Sort each list and grab the highest innovation number
        keys_1.Sort();
        keys_2.Sort();
        int max_inno = Mathf.Max(keys_1[keys_1.Count - 1], keys_2[keys_2.Count - 1]);

        // Loop over all node genes to find matching genes
        for (int i = 0; i <= max_inno; i++)
        {
            // If g1 and g2 both contain the innovation number i, it is a matching gene
            if (g1.GetConnections().ContainsKey(i) && g2.GetConnections().ContainsKey(i))
            {
                matching++;
                diff += Mathf.Abs(g1.GetConnections()[i].GetWeight() - g2.GetConnections()[i].GetWeight());
            }
        }
        return diff / matching;

    }

    public static float CompatibilityDistance(Genome g1, Genome g2, float c1, float c2, float c3)
    {
        int excess = Genome.CountExcessGenes(g1, g2);
        int disjoint = Genome.CountDisjointGenes(g1, g2);
        float weight_difference = Genome.AverageWeightDifference(g1, g2);
        return excess * c1 + disjoint * c2 + weight_difference * c3;
    }

    public int GetNumConnections()
    {
        return this.connections.Count;
    }

}
