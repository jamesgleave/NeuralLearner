using System.Collections;
using System.Collections.Generic;
using Model;
using UnityEngine;
using Activations;

public class NEATNetwork : Model.BaseModel
{
    // ID of inputs for the neat network
    private List<int> inputs;

    // ID of outputs for the neat network
    private List<int> outputs;

    // A list to store neurons temporarily during the feed forward calculation
    public List<NEATNeuron> unprocessed_neurons;

    // A list that stores neurons in the order they are executed
    public List<int> neuron_execution_order = new List<int>();

    // The neurons of the network
    private Dictionary<int, NEATNeuron> neurons;

    // The genome for this neat network
    private Genome genome;

    public int num_times_recompiled = 0;

    // The list of output values
    private List<float> output_values = new List<float>();

    /// <summary>
    /// The complexity of the neat network
    /// </summary>
    private float network_complexity;

    public NEATNetwork(Genome genome)
    {
        // Set the genome
        this.genome = genome;

        // Setup the lists and dict
        inputs = new List<int>();
        outputs = new List<int>();
        neurons = new Dictionary<int, NEATNeuron>();
        unprocessed_neurons = new List<NEATNeuron>();

        // Set up the input / output neurons
        foreach (int id in genome.GetNodes().Keys)
        {
            // Get the node with the id
            NodeGene node = genome.GetNodes()[id];

            // TODO Make sure we have the ability to mutate activation in the genome
            NEATNeuron neuron = new NEATNeuron(Parse.Parser.ReadActivation(node.GetActivationString()), node.GetID(), node.GetCalculationMethod());

            // Look at each key (id)
            if (node.IsInput())
            {
                // Setup the input neuron
                // Input neurons do not have 'inputs', so we represent the input ID with a '-1'
                neuron.AddInput(-1, new NEATInputContainer());
                inputs.Add(id);

                // Set the depth of the input neuron (it is always zero)
                neuron.SetDepth(0);
            }
            else if (node.IsOutput())
            {
                // Set up as an output
                outputs.Add(id);
            }
            // Add neuron to dict (mapped with node id)
            neurons.Add(id, neuron);
        }

        // Loop through connections now
        foreach (int id in genome.GetConnections().Keys)
        {
            // Get the connection with the ID 'id'
            ConnectionGene connection = genome.GetConnections()[id];

            // Check if the connection is expressed or not
            if (connection.IsExpressed() == false)
            {
                continue;
            }

            // Get the neuron that leads into the connection (where the connection comes from)
            NEATNeuron connection_input_neuron = neurons[connection.getInNode()];

            // Add the node to the output with its weight
            connection_input_neuron.AddOutput(connection.getOutNode(), connection.GetWeight());

            // Now do the same for the output (where the connection leads to)
            NEATNeuron connection_receiver = neurons[connection.getOutNode()];

            // Add an input (the connection)
            connection_receiver.AddInput(connection_input_neuron.GetNeuronID(), new NEATInputContainer());
        }

        // Calculate each neurons depth (distance from an input)
        CalculateDepth();

        // Find recurrent connections
        FindRecurrence();

        // Calculate the complexity of the netwok
        CalculateComplexity();
    }

    /// <summary>
    /// Returns a copy of the genome
    /// </summary>
    /// <returns></returns>
    public Genome CopyGenome()
    {
        return genome.Copy();
    }

    public Genome GetGenome()
    {
        return genome;
    }

    public void Recompile(Genome genome)
    {
        // Increment the recompile counter
        num_times_recompiled++;

        // Set the new genome
        this.genome = genome;

        // Clear everything
        inputs.Clear();
        outputs.Clear();
        neurons.Clear();
        unprocessed_neurons.Clear();

        // Set up the input / output neurons
        foreach (int id in genome.GetNodes().Keys)
        {
            // Get the node with the id
            NodeGene node = genome.GetNodes()[id];
            NEATNeuron neuron = new NEATNeuron(Parse.Parser.ReadActivation(node.GetActivationString()), node.GetID(), node.GetCalculationMethod());

            // Look at each key (id)
            if (node.IsInput())
            {
                // Setup the input neuron
                // Input neurons do not have 'inputs', so we represent the input ID with a '-1'
                neuron.AddInput(-1, new NEATInputContainer());
                inputs.Add(id);

                // Set the depth of the input neuron (it is always zero)
                neuron.SetDepth(0);
            }
            else if (node.IsOutput())
            {
                // Set up as an output
                outputs.Add(id);
            }
            // Add neuron to dict (mapped with node id)
            neurons.Add(id, neuron);
        }

        // Loop through connections now
        foreach (int id in genome.GetConnections().Keys)
        {
            // Get the connection with the ID 'id'
            ConnectionGene connection = genome.GetConnections()[id];

            // Check if the connection is expressed or not
            if (connection.IsExpressed() == false)
            {
                continue;
            }

            // Get the neuron that leads into the connection (where the connection comes from)
            NEATNeuron connection_input_neuron = neurons[connection.getInNode()];

            // Add the node to the output with its weight
            connection_input_neuron.AddOutput(connection.getOutNode(), connection.GetWeight());

            // Now do the same for the output (where the connection leads to)
            NEATNeuron connection_receiver = neurons[connection.getOutNode()];

            // Add an input (the connection)
            connection_receiver.AddInput(connection_input_neuron.GetNeuronID(), new NEATInputContainer());
        }

        // Calculate each neurons depth (distance from an input)
        CalculateDepth();

        // Calculate the complexity of the netwok
        CalculateComplexity();
    }

    /// <summary>
    /// Calculates the depth of each neuron, and also removes all orphan and useless neurons (prunes the network)
    /// </summary>
    public void CalculateDepth()
    {
        List<int> to_be_pruned = new List<int>();
        bool cont = true;
        int iterations = 0;
        int max_depth = 0;
        while (cont)
        {
            cont = false;
            iterations++;
            // Look at each neuron in the dict
            foreach (KeyValuePair<int, NEATNeuron> pair in neurons)
            {
                // If the neuron's depth is not the min value
                if (pair.Value.GetDepth() != int.MinValue || pair.Value.GetOutputIDs().Count == 0)
                {
                    // Check to see if we update the max depth value
                    if (pair.Value.GetDepth() > max_depth)
                    {
                        // If the max depth is less than the depth of this neuron, we update max depth
                        max_depth = pair.Value.GetDepth();
                    }

                    // Loop through the outputs of the neuron in pair
                    foreach (int output_id in pair.Value.GetOutputIDs())
                    {
                        // If the output has not been assigned a depth then assign it this neurons depth + 1
                        if (neurons[output_id].GetDepth() == int.MinValue)
                        {
                            neurons[output_id].SetDepth(pair.Value.GetDepth() + 1);
                        }
                    }
                }
                else
                {
                    cont = true;
                }

                // Check to see if the neuron has any outputs and it is hidden, if not, it must be pruned
                bool is_hidden = inputs.Contains(pair.Value.GetNeuronID()) == false && outputs.Contains(pair.Value.GetNeuronID()) == false;
                if (pair.Value.GetOutputIDs().Count == 0 && is_hidden)
                {
                    to_be_pruned.Add(pair.Value.GetNeuronID());
                }
            }

            // Check to see if the neuron has any outputs and it is hidden, if not, we must recompile
            if (to_be_pruned.Count > 0)
            {
                // Remove each node from the genome
                foreach (int id in to_be_pruned)
                {
                    // Remove neuron from genome
                    genome.RemoveNode(id);
                }
                // Recompile the network
                Recompile(genome);
            }


            if (iterations > neurons.Count * 2)
            {
                // Get every single neuron that does not have a proper path to the output
                foreach (KeyValuePair<int, NEATNeuron> pair in neurons)
                {
                    // Remove any neuron that does not have a depth value assigned, as long as it is not an input or output neuron
                    bool is_hidden = inputs.Contains(pair.Value.GetNeuronID()) == false && outputs.Contains(pair.Value.GetNeuronID()) == false;
                    if (pair.Value.GetDepth() == int.MinValue && is_hidden)
                    {
                        // Remove neuron from genome
                        genome.RemoveNode(pair.Value.GetNeuronID());
                    }
                }
                // Recompile the network
                cont = false;
                Recompile(genome);
            }

        }

        // Set each output neuron to the highest depth if it has not been set already
        foreach (int id in outputs)
        {
            neurons[id].ForceSetDepth(max_depth + 1);
        }
    }

    /// <summary>
    /// Finds recurrence calculations
    /// </summary>
    /// <param name="inputs"></param>
    /// <returns></returns>
    public void FindRecurrence()
    {
        // Clear the unprocessed list and add all neurons to it
        unprocessed_neurons.Clear();
        unprocessed_neurons.AddRange(neurons.Values);
        unprocessed_neurons.Reverse();

        // Populate the inputs with their values
        for (int i = 0; i < inputs.Count; i++)
        {
            // Get the input from our neuron list
            NEATNeuron input_neuron = neurons[this.inputs[i]];

            // Give the input neuron its value
            // Again, -1 represents an input neuron (an ID that does not truly exist)
            input_neuron.FeedInput(-1, 1);

            // Calculate the output
            input_neuron.Calculate();

            // For each of the input's outputs (lol) give the ouptut of this neuron
            for (int r = 0; r < input_neuron.GetOutputIDs().Count; r++)
            {
                // Get each of the neuron's outputs (receiver because it receives the output)
                NEATNeuron receiver = neurons[input_neuron.GetOutputIDs()[r]];

                // Get the product of the input's value and its weight (forward propogate)
                receiver.FeedInput(input_neuron.GetNeuronID(), input_neuron.GetOutput() * input_neuron.GetWeights()[r]);
            }

            // Remove the input neuron from the unprocessed neurons (since we just processed it)
            unprocessed_neurons.Remove(input_neuron);
        }

        // We now loop over each neuron until there are none left to process
        int num_iterations = 0;
        while (unprocessed_neurons.Count > 0)
        {
            num_iterations++;
            if (num_iterations > 2 * neurons.Count)
            {
                // Setup values to determine the recurrence
                int input_id, current_id, input_depth, current_depth;

                // Could not propogate the network (too many iterations)
                foreach (NEATNeuron neuron in unprocessed_neurons)
                {
                    // Populate the array
                    foreach (KeyValuePair<int, NEATInputContainer> pair in neuron.GetInputs())
                    {
                        // Get input values
                        input_id = pair.Key;
                        input_depth = neurons[input_id].GetDepth();

                        // Get output values
                        current_id = neuron.GetNeuronID();
                        current_depth = neuron.GetDepth();

                        // If the current depth is less than where the input came from, it must be recurrent since it is moving backwards
                        if (input_depth >= current_depth)
                        {
                            // Set input type as recurrent :)
                            pair.Value.SetInputType(NEATInputContainer.NEATInputType.Recurrent);
                        }
                    }
                }
                break;
            }

            // Loop through the unprocessed nodes
            NEATNeuron n; int receiverID;
            for (int x = unprocessed_neurons.Count - 1; x >= 0; x--)
            {

                // Get the current neuron
                n = unprocessed_neurons[x];

                // If all values are filled...
                if (n.IsReady())
                {
                    // Get the output
                    n.Calculate();

                    // Look at each output
                    for (int i = 0; i < n.GetOutputIDs().Count; i++)
                    {
                        // Get the ith output for n
                        receiverID = n.GetOutputIDs()[i];

                        // Perform the product calculation and give the neuron the input value calculated.
                        neurons[receiverID].FeedInput(n.GetNeuronID(), n.GetOutput() * n.GetWeights()[i]);
                    }

                    // If the calculation is done, we can remove this node from the unprocessed list
                    unprocessed_neurons.RemoveAt(x);
                }
            }
        }

        // Reset each neuron before running
        foreach (NEATNeuron n in neurons.Values)
        {
            n.Reset();
        }
    }

    public string GetInfo()
    {
        return "Input: " + inputs.Count + ", " + "Output: " + outputs.Count;
    }

    /// <summary>
    /// Returns a list of outputs given the like of inputs
    /// </summary>
    /// <param name="inputs"></param>
    /// <returns></returns>
    public override List<float> Infer(List<float> inputs)
    {
        int order = 0;
        // Check the input size
        if (inputs.Count != this.inputs.Count)
        {
            throw new System.Exception("Number of inputs must match number of input neurons! Expected: " + this.inputs.Count + ", Received: " + inputs.Count);
        }

        // Here, if the execution order is known, we can use the smart inference method which is pretty much twice as fast
        if (neuron_execution_order.Count > 0)
        {
            return SmartInfer(inputs);
        }

        // Reset each neuron before running
        foreach (NEATNeuron n in neurons.Values)
        {
            n.Reset();
            n.order.Clear();
        }

        // Clear the unprocessed list and add all neurons to it
        unprocessed_neurons.Clear();
        neuron_execution_order.Clear();
        unprocessed_neurons.AddRange(neurons.Values);
        unprocessed_neurons.Reverse();

        // Populate the inputs with their values
        for (int i = 0; i < inputs.Count; i++)
        {
            // Get the input from our neuron list
            NEATNeuron input_neuron = neurons[this.inputs[i]];

            // Testing
            neuron_execution_order.Add(input_neuron.GetNeuronID());

            // Give the input neuron its value
            input_neuron.FeedInput(-1, inputs[i]);

            // Calculate the output
            input_neuron.Calculate();

            // For each of the input's outputs (lol) give the ouptut of this neuron
            for (int r = 0; r < input_neuron.GetOutputIDs().Count; r++)
            {
                // Get each of the neuron's outputs (receiver because it receives the output)
                NEATNeuron receiver = neurons[input_neuron.GetOutputIDs()[r]];

                // Get the product of the input's value and its weight (forward propogate)
                receiver.FeedInput(input_neuron.GetNeuronID(), input_neuron.GetOutput() * input_neuron.GetWeights()[r]);
            }

            // Remove the input neuron from the unprocessed neurons (since we just processed it)
            unprocessed_neurons.Remove(input_neuron);
            input_neuron.order.Add(order++);
        }

        // We now loop over each neuron until there are none left to process
        int num_iterations = 0;
        while (unprocessed_neurons.Count > 0)
        {
            num_iterations++;
            if (num_iterations > 2 * neurons.Count)
            {
                string s = "";
                foreach (NEATNeuron x in unprocessed_neurons)
                {
                    s += x.GetNeuronID() + ", ";
                }
                // Could not propogate the network (too many iterations)
                throw new System.Exception("A Cycle was deteted! Number of neurons in cycle: " + s);
            }

            // Loop through the unprocessed nodes
            NEATNeuron n;
            for (int x = unprocessed_neurons.Count - 1; x >= 0; x--)
            {

                // Get the current neuron
                n = unprocessed_neurons[x];

                // If all values are filled...
                if (n.IsReady())
                {
                    // Get the output
                    n.Calculate();
                    // Look at each output
                    for (int i = 0; i < n.GetOutputIDs().Count; i++)
                    {
                        // Get the ith output for n
                        int receiverID = n.GetOutputIDs()[i];
                        // Perform the product calculation
                        float receiver_value = n.GetOutput() * n.GetWeights()[i];

                        // Give the neuron the input value calculated above
                        neurons[receiverID].FeedInput(n.GetNeuronID(), receiver_value);
                    }

                    // If the calculation is done, we can remove this node from the unprocessed list
                    n.order.Add(order++);
                    // Testing
                    neuron_execution_order.Add(n.GetNeuronID());
                    unprocessed_neurons.RemoveAt(x);
                }
            }
        }

        // Gather all outputs..
        output_values.Clear();
        for (int i = 0; i < outputs.Count; i++)
        {
            output_values.Add(neurons[outputs[i]].GetOutput());
        }

        // Finally, return the inference
        return output_values;
    }

    public List<float> SmartInfer(List<float> inputs)
    {

        // Reset each neuron before running TODO Move into lower loop
        foreach (NEATNeuron n in neurons.Values)
        {
            n.Reset();
            n.order.Clear();
        }

        output_values.Clear();
        for (int i = 0, input_size = inputs.Count; i < neuron_execution_order.Count; i++)
        {
            int to_execute = neuron_execution_order[i];

            // If the neuron is an input, feed it the input values
            if (to_execute < input_size)
            {
                // Get the input from our neuron list
                NEATNeuron input_neuron = neurons[i];

                // Give the input neuron its value
                input_neuron.FeedInput(-1, inputs[i]);

                // Calculate the output
                input_neuron.Calculate();

                // For each of the input's outputs (lol) give the ouptut of this neuron
                for (int r = 0; r < input_neuron.GetOutputIDs().Count; r++)
                {
                    // Get each of the neuron's outputs (receiver because it receives the output)
                    NEATNeuron receiver = neurons[input_neuron.GetOutputIDs()[r]];

                    // Get the product of the input's value and its weight (forward propogate)
                    receiver.FeedInput(input_neuron.GetNeuronID(), input_neuron.GetOutput() * input_neuron.GetWeights()[r]);
                }
            }
            else
            {
                // If all values are filled...
                if (neurons[to_execute].IsReady())
                {
                    NEATNeuron n = neurons[to_execute];

                    // Get the output
                    n.Calculate();

                    // Look at each output
                    for (int r = 0; r < n.GetOutputIDs().Count; r++)
                    {
                        // Get the ith output for n
                        int receiverID = n.GetOutputIDs()[r];
                        // Perform the product calculation
                        float receiver_value = n.GetOutput() * n.GetWeights()[r];

                        // Give the neuron the input value calculated above
                        neurons[receiverID].FeedInput(n.GetNeuronID(), receiver_value);
                    }
                }
            }
        }

        // Gather all outputs.. TODO Move into upper loop
        for (int i = 0; i < outputs.Count; i++)
        {
            output_values.Add(neurons[outputs[i]].GetOutput());
        }

        // Finally, return the inference
        return output_values;

    }

    /// <summary>
    /// Returns a copy of this neat network instance
    /// </summary>
    /// <returns></returns>
    public override BaseModel Copy()
    {
        return new NEATNetwork(this.genome.Copy());
    }

    public Dictionary<int, NEATNeuron> GetNeurons()
    {
        return neurons;
    }

    public List<int> GetInputs()
    {
        return inputs;
    }

    public List<int> GetOutputs()
    {
        return outputs;
    }

    /// <summary>
    /// Returns the complexity of the brain
    /// </summary>
    /// <returns></returns>
    public override float GetComplexity()
    {
        return network_complexity;
    }

    /// <summary>
    /// Calculates the complexity of the brain
    /// </summary>
    private void CalculateComplexity()
    {
        // Tally up the expressed connections
        int num_connections = 0;
        foreach (ConnectionGene con in genome.GetConnections().Values)
        {
            num_connections += con.IsExpressed() ? 1 : 0;
        }

        // the number of connections + the number of hidden neurons
        network_complexity = num_connections + (neurons.Count - inputs.Count - outputs.Count);
    }
}

public class NEATNeuron
{
    // The output value of this neuron
    private float output;

    // The id of the neuron
    private int id;

    // The depth of the neuron
    private int network_depth = int.MinValue;

    // The input values going into this neuron
    private Dictionary<int, NEATInputContainer> inputs;

    // Indicates if an input is recurrent or not
    private List<bool> recurrent_inputs;
    // The hidden values that are stored by the recurrent connections
    private Dictionary<int, float> hidden_states;

    // The execution order (temp)
    public List<int> order = new List<int>();

    // The IDs of the output nodes
    private List<int> output_ids;

    // The output weights
    private List<float> output_weights;

    // The activation funciton
    private Activation activation;

    /// <summary>
    /// The method for calculation
    /// </summary>
    private NodeCalculationMethod method;

    public NEATNeuron(Activation activation, int id)
    {
        this.id = id;
        inputs = new Dictionary<int, NEATInputContainer>();
        output_ids = new List<int>();
        output_weights = new List<float>();
        recurrent_inputs = new List<bool>();
        hidden_states = new Dictionary<int, float>();

        this.activation = activation;
        this.method = NodeCalculationMethod.LinComb;
    }

    public NEATNeuron(Activation activation, int id, NodeCalculationMethod method)
    {
        this.id = id;
        inputs = new Dictionary<int, NEATInputContainer>();
        output_ids = new List<int>();
        output_weights = new List<float>();
        recurrent_inputs = new List<bool>();
        hidden_states = new Dictionary<int, float>();

        this.activation = activation;
        this.method = method;
    }

    /// <summary>
    /// Set which inputs are recurrent and which are not
    /// </summary>
    /// <param name="ri"></param>
    public void SetRecurrentInputs(List<bool> ri)
    {
        recurrent_inputs.Clear();
        recurrent_inputs.AddRange(ri);
    }

    /// <summary>
    /// Returns which inputs are recurrent (one to one with inputs)
    /// </summary>
    /// <returns></returns>
    public List<bool> GetRecurrence()
    {
        return recurrent_inputs;
    }

    /// <summary>
    /// Set a value for hidden state
    /// </summary>
    /// <param name="other_id"></param>
    /// <param name="value"></param>
    public void SetHiddenState(int other_id, float value)
    {
        // If we have the key already then just update the value
        if (hidden_states.ContainsKey(other_id))
        {
            hidden_states[other_id] = value;
        }
        else
        {
            // Else just add a new value
            hidden_states.Add(other_id, value);
        }
    }

    /// <summary>
    /// Set the depth of the neuron. This can only be down once per neuron.
    /// </summary>
    /// <param name="new_depth"></param>
    public void SetDepth(int new_depth)
    {
        // Get the actual depth (closest)
        if (network_depth == int.MinValue)
        {
            network_depth = new_depth;
        }
    }

    /// <summary>
    /// Forces the depth setting (overrides the set depth limitation)
    /// </summary>
    /// <param name="new_depth"></param>
    public void ForceSetDepth(int new_depth)
    {
        network_depth = new_depth;
    }

    /// <summary>
    /// Returns the depth of the neuron instance
    /// </summary>
    /// <returns></returns>
    public int GetDepth()
    {
        return network_depth;
    }

    /// <summary>
    /// Returns the activation object
    /// </summary>
    /// <returns></returns>
    public Activation GetActivation()
    {
        return activation;
    }

    /// <summary>
    /// Adds a new connection
    /// </summary>
    /// <param name="output_id"></param>
    /// <param name="weight"></param>
    public void AddOutput(int output_id, float weight)
    {
        // Add the output id to the list of outputs
        output_ids.Add(output_id);

        // Add the weight to the output weight list
        output_weights.Add(weight);
    }

    /// <summary>
    /// Add an input from another neuron
    /// </summary>
    public void AddInput(int in_id, NEATInputContainer input_container)
    {
        // Add a new input
        inputs[in_id] = input_container;
        recurrent_inputs.Add(false);
    }

    /// <summary>
    /// Performs the weighted sum, followed by the activation of the inputs
    /// </summary>
    /// <returns></returns>
    public float Calculate()
    {
        // Get the sum of the inputs
        float sum = 0;
        // Look at each key value pair and sum all values
        foreach (KeyValuePair<int, NEATInputContainer> pair in inputs)
        {
            if (pair.Value.GetInputType() == NEATInputContainer.NEATInputType.Recurrent)
            {
                // Add up each value
                sum += (float)System.Math.Tanh(pair.Value.GetInputValue());
            }
            else
            {
                // Add up each value
                sum += pair.Value.GetInputValue();
            }
        }

        switch (method)
        {
            // check to see if this is a LinComb neuron
            case NodeCalculationMethod.LinComb:
                // Activate the sum
                output = activation.activate(sum);
                break;

            // check to see if this is a latch neuron
            case NodeCalculationMethod.Latch:
                // A latch neuron will always return 1 if sum squish is 0.80 or higher (latched) and input is greater than zero.
                // If the output is less than or equal to zero, output zero

                // Activate the sum
                float activ = activation.activate(sum);
                //Debug.Log(activ + ", " + sum + ", " + activation.name);

                if (output == 1 && activ > 0 || activ >= 0.80f)
                {
                    output = 1;
                }
                else
                {
                    output = 0;
                }
                break;
        }


        // return the final value
        return output;
    }

    /// <summary>
    /// Returns the calculation method
    /// </summary>
    /// <returns></returns>
    public NodeCalculationMethod GetMethod()
    {
        return method;
    }

    /// <summary>
    /// Set the method of the neuron's output calculation
    /// </summary>
    /// <param name="new_method"></param>
    public void SetMethod(NodeCalculationMethod new_method)
    {
        this.method = new_method;
    }

    /// <summary>
    /// Returns true if there are no NaN values in the inputs
    /// </summary>
    /// <returns></returns>
    public bool IsReady()
    {
        // Ready means that there is no non-recurrent connections waiting
        foreach (KeyValuePair<int, NEATInputContainer> pair in inputs)
        {
            // If a single input container is not ready, we return false
            if (!pair.Value.IsReady())
            {
                return false;
            }
        }
        // If we get through every input without any non-ready issues, we are ready!
        return true;
    }

    /// <summary>
    /// Updates the input from the 'from_id' neuron with the value 'input'
    /// </summary>
    /// <param name="input"></param>
    public void FeedInput(int from_id, float input)
    {
        // Assign the input to the first available input slot
        bool found = false;
        // If we have the input...
        if (inputs.ContainsKey(from_id))
        {
            // We have found an input matching the ID
            found = true;
            // Set the input value
            inputs[from_id].SetValue(input);
        }

        // If nothing was found, raise exception
        if (!found)
        {
            throw new System.Exception("Shape Mismatch: Too Many Inputs");
        }
    }

    /// <summary>
    /// Returns the output of this neuron instance
    /// </summary>
    /// <returns></returns>
    public float GetOutput()
    {
        return output;
    }

    /// <summary>
    /// Returns the list of output ids
    /// </summary>
    /// <returns></returns>
    public List<int> GetOutputIDs()
    {
        return output_ids;
    }

    /// <summary>
    /// Returns the weights
    /// </summary>
    /// <returns></returns>
    public List<float> GetWeights()
    {
        return output_weights;
    }

    /// <summary>
    /// Returns the weight of the output with the ID 'id'
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public float GetSpecifiedWeight(int id)
    {
        return output_weights[output_ids.IndexOf(id)];
    }

    /// <summary>
    /// Get the input values of this neuron
    /// </summary>
    /// <returns></returns>
    public Dictionary<int, NEATInputContainer> GetInputs()
    {
        return inputs;
    }

    /// <summary>
    /// Get the id of this neuron
    /// </summary>
    /// <returns></returns>
    public int GetNeuronID()
    {
        return id;
    }

    /// <summary>
    /// Returns true if the input with the id 'id' is recurrent
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool InputRecurrent(int id)
    {
        return hidden_states.ContainsKey(id);
    }

    public List<float> GetHiddenStateList()
    {
        List<float> x = new List<float>();
        x.AddRange(hidden_states.Values);
        return x;
    }

    /// <summary>
    /// Reset the inputs and the calculation
    /// </summary>
    public void Reset()
    {
        // Set each input to NaN
        foreach (KeyValuePair<int, NEATInputContainer> pair in inputs)
        {
            pair.Value.ResetValue();
        }

        // Here we look at the different calculation methods to see what to do during a reset.
        switch (method)
        {
            // If we have a lincomb, then we want to reset the output value at each reset
            case NodeCalculationMethod.LinComb:
                output = 0;
                break;

            // If we have a latch neuron, dont do anything.
            // The point of a latch neuron is to stay 1 when the input is nonzero and it has been activated.
            case NodeCalculationMethod.Latch:
                break;
        }
    }
}
