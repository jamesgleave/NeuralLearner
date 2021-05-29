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

    // The neurons of the network
    private Dictionary<int, NEATNeuron> neurons;

    // The genome for this neat network
    private Genome genome;

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
            NEATNeuron neuron = new NEATNeuron(new Linear(), node.GetID());

            // Look at each key (id)
            if (node.IsInput())
            {
                // Setup the input neuron
                neuron.AddInput();
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
            connection_receiver.AddInput();
        }
    }

    public string GetInfo()
    {
        return "Input: " + inputs.Count + ", " + "Output: " + outputs.Count;
    }

    /// <summary>
    /// Finds recurrence calculations
    /// </summary>
    /// <param name="inputs"></param>
    /// <returns></returns>
    public void FindRecurrence(List<float> inputs)
    {
        // Check the input size
        if (inputs.Count != this.inputs.Count)
        {
            throw new System.Exception("Number of inputs must match number of input neurons! Expected: " + this.inputs.Count + ", Received: " + inputs.Count);
        }

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
            input_neuron.FeedInput(inputs[i]);

            // Calculate the output
            input_neuron.Calculate();

            // For each of the input's outputs (lol) give the ouptut of this neuron
            for (int r = 0; r < input_neuron.GetOutputIDs().Count; r++)
            {
                // Get each of the neuron's outputs (receiver because it receives the output)
                NEATNeuron receiver = neurons[input_neuron.GetOutputIDs()[r]];

                // Get the product of the input's value and its weight (forward propogate)
                receiver.FeedInput(input_neuron.GetOutput() * input_neuron.GetWeights()[r]);
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
                // Could not propogate the network (too many iterations)
                List<bool> rec_inputs = new List<bool>();
                foreach (NEATNeuron neuron in unprocessed_neurons)
                {
                    // Clear the input bool array
                    rec_inputs.Clear();

                    // Populate the array
                    foreach (float f in neuron.GetInputs())
                    {
                        // Check if the connection is recurrent
                        bool is_recurrent = float.IsNaN(f);

                        // Add to bool array
                        rec_inputs.Add(is_recurrent);

                        // Add a new key-value pair to the hidden state
                        if (is_recurrent)
                        {
                            neuron.SetHiddenState(neuron.GetNeuronID(), 0.0f);
                        }
                    }

                    // Setup certain inputs as recurrent
                    neuron.SetRecurrentInputs(rec_inputs);
                }
                break;
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
                        // TODO use dictionary to store inputs or pass the input neurons ID to store
                        neurons[receiverID].FeedInput(receiver_value);

                        // Set the reciever neuron with a depth
                        neurons[receiverID].SetDepth(n.GetDepth() + 1);
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

        // Reset each neuron before running
        foreach (NEATNeuron n in neurons.Values)
        {
            n.Reset();
            n.order.Clear();
        }

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
            input_neuron.FeedInput(inputs[i]);

            // Calculate the output
            input_neuron.Calculate();

            // For each of the input's outputs (lol) give the ouptut of this neuron
            for (int r = 0; r < input_neuron.GetOutputIDs().Count; r++)
            {
                // Get each of the neuron's outputs (receiver because it receives the output)
                NEATNeuron receiver = neurons[input_neuron.GetOutputIDs()[r]];

                // Get the product of the input's value and its weight (forward propogate)
                receiver.FeedInput(input_neuron.GetOutput() * input_neuron.GetWeights()[r]);
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

                        // Now, we look at the input and check if it is recurrent
                        if (neurons[receiverID].InputRecurrent(receiverID))
                        {
                            // Set the hidden state :)
                            neurons[receiverID].SetHiddenState(n.GetNeuronID(), receiver_value);
                        }
                        else
                        {
                            // Give the neuron the input value calculated above
                            neurons[receiverID].FeedInput(receiver_value);
                        }
                    }

                    // If the calculation is done, we can remove this node from the unprocessed list
                    n.order.Add(order++);
                    unprocessed_neurons.RemoveAt(x);
                }
            }
        }

        // Gather all outputs..
        List<float> output_values = new List<float>();
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
    private List<float> inputs;
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

    public NEATNeuron(Activation activation, int id)
    {
        this.id = id;
        inputs = new List<float>();
        output_ids = new List<int>();
        output_weights = new List<float>();
        recurrent_inputs = new List<bool>();
        hidden_states = new Dictionary<int, float>();

        this.activation = activation;
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
        if (network_depth != int.MinValue)
        {
            throw new System.Exception("Cannot set a neuron's depth twice.");
        }
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
    public void AddInput()
    {
        // Add a new input
        inputs.Add(float.NaN);
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
        for (int i = 0; i < inputs.Count; i++)
        {
            if (recurrent_inputs[i] == false)
                sum += inputs[i];
        }

        // If we have any hidden states, they should be included in the summation
        if (hidden_states.Count > 0)
        {
            // Sum up the values in the hidden state
            foreach (float hidden_value in hidden_states.Values)
            {
                sum += hidden_value;
            }
        }

        // Activate the sum
        output = activation.activate(sum);

        // return the final value
        return output;
    }

    /// <summary>
    /// Returns true if there are no NaN values in the inputs
    /// </summary>
    /// <returns></returns>
    public bool IsReady()
    {
        // Ready means that there is no non-recurrent connections waiting
        for (int i = 0; i < inputs.Count; i++)
        {
            if (float.IsNaN(inputs[i]) && recurrent_inputs[i] == false)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Adds an input to the first neuron available
    /// </summary>
    /// <param name="input"></param>
    public void FeedInput(float input)
    {
        // Assign the input to the first available input slot
        bool found = false;
        for (int i = 0; i < inputs.Count; i++)
        {
            if (float.IsNaN(inputs[i]) && recurrent_inputs[i] == false)
            {
                inputs[i] = input;
                found = true;
                break;
            }
        }

        // If nothing was found, raise exception
        if (!found)
        {
            throw new System.Exception("Shape Mismatch: Too Many Inputs");
        }
    }

    /// <summary>
    /// Adds an input to the first neuron available
    /// </summary>
    /// <param name="input"></param>
    public void FeedInput(float input, int input_id)
    {
        // Assign the input to the first available input slot
        bool found = false;
        for (int i = 0; i < inputs.Count; i++)
        {
            if (float.IsNaN(inputs[i]) && recurrent_inputs[i] == false)
            {
                inputs[i] = input;
                found = true;
                break;
            }
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
    /// Get the input values of this neuron
    /// </summary>
    /// <returns></returns>
    public List<float> GetInputs()
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
        for (int i = 0; i < inputs.Count; i++)
        {
            // Keep recurrent input (hidden value)
            inputs[i] = float.NaN;
        }
        // Reset the ouput
        output = 0;
    }
}
