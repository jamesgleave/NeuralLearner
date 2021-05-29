using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    /// Returns true if there is a cycle between node 1 and node 2
    /// </summary>
    /// <param name="new_connection_id"></param>
    /// <param name="original_node"></param>
    /// <returns></returns>
    public bool CyclicConnection(int new_connection_id, int original_node)
    {
        // Find outgoing connections from the new connection
        foreach (ConnectionGene connection in connections.Values)
        {
            // Grab all connections starting at 'new_connection'
            if (connection.getInNode() == new_connection_id)
            {
                // We can see it loops back
                if (connection.getOutNode() == original_node)
                {
                    return true;
                }
                else
                {
                    return CyclicConnection(connection.getOutNode(), original_node);
                }
            }
        }

        throw new System.Exception("Made it to end");
        return false;
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
    /// Returns the next innovation number & increments the genome's innovation generator
    /// </summary>
    /// <returns></returns>
    public int GetNextInnovation()
    {
        return innovation.GetInnovation();
    }

    /// <summary>
    /// Adds a connection between two random nodes
    /// </summary>
    public void AddConnectionMutation()
    {
        // Create a list of all keys in the nodes list
        List<int> keys = new List<int>(nodes.Count);
        foreach (var item in nodes)
        {
            keys.Add(item.Key);
        }

        // Create a new random variable
        System.Random r = new System.Random();

        // Grab the two nodes that we will be connecting
        NodeGene node1 = nodes[keys[r.Next(keys.Count)]];
        NodeGene node2 = nodes[keys[r.Next(keys.Count)]];

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
        if (connection_exists || node1 == node2 || node1.IsInput() && node2.IsInput() || node1.IsOutput() && node2.IsOutput())
        {
            return;
        }

        // TODO Check for cycles!!!!


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
    }

    /// <summary>
    /// Adds a new node inplace of an existing connection
    /// </summary>
    public void AddNodeMutation()
    {

        // We need connections to create a node
        if (connections.Count == 0)
        {
            return;
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

        // Create the two new connections
        ConnectionGene going_to_new = new ConnectionGene(in_node.GetID(), new_node.GetID(), 1f, true, innovation.GetInnovation());
        ConnectionGene new_to_next = new ConnectionGene(new_node.GetID(), out_node.GetID(), connection.GetWeight(), true, innovation.GetInnovation());

        // Add the new node to the dict
        nodes.Add(new_node.GetID(), new_node);

        // Add the new connections to the dict
        connections.Add(going_to_new.GetInnovation(), going_to_new);
        connections.Add(new_to_next.GetInnovation(), new_to_next);
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

        // Return the new genome
        return new_genome;
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

    public static string Mutate(Genome model, float mutation_amount, float weight_mutation_prob, float neuro_mutation_prob, float bias_mutation_prob, float dropout_prob)
    {
        // There is a base 80% chance of a genome mutation
        if (0.80f < Random.value)
        {
            return "";
        }

        // Loop through each connection and mutate the weights according to the passed weight mutation probability
        foreach (ConnectionGene con in model.connections.Values)
        {
            // If weight_mutation_prob is triggered, perturb the weight by a random value determined by the passed mutation amount
            if (weight_mutation_prob > Random.value)
            {
                con.SetWeight(con.GetWeight() * Random.Range(-2 * mutation_amount, 2 * mutation_amount));
            }
            // If the weight mutation is not triggered, then we assign the weights a new value (as from the NEAT paper)
            else
            {
                // If not, then set the weight value 
                con.SetWeight(Random.Range(-2 * mutation_amount, 2 * mutation_amount));
            }
        }

        if (neuro_mutation_prob > Random.value)
        {
            model.AddConnectionMutation();
        }

        if (neuro_mutation_prob > Random.value)
        {
            model.AddNodeMutation();
        }

        return "";
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
}
