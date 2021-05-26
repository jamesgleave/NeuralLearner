using System.Collections.Generic;
using UnityEngine;
using Layers;
using Activations;
using System.Linq;

namespace Model
{

    // A Sequential Feed Forward Neural Net
    public class NeuralNet : BaseModel
    {
        private Layer[] layers;   //layers
        public Layer input;    // This is the input layer
        public Layer output;    // This is the input layer
        public int total_neurons;   // Total number of neurons in all combined layers

        public NeuralNet(Layer input, Layer[] layers, Layer output)
        {
            // The main constructor
            this.input = input; // Set input layer
            this.layers = layers;  // Set hidden layer(s)
            this.output = output;  // Set output layer

            // Calculate the total number of neurons present in the network
            CalculateTotalNeurons();
        }

        // Placeholder constructor
        public NeuralNet()
        {

        }

        private void CalculateTotalNeurons()
        {
            int total = 0;
            foreach (var x in layers) { total += x.GetNeurons().Count; }
            total += input.GetNeurons().Count + output.GetNeurons().Count;
            total_neurons = total;
        }

        public Layer[] GetLayers()
        {
            // Returns the hidden layers 
            return layers;
        }

        public Layer[] GetAllLayers()
        {
            // Create new array of layers
            Layer[] combined = new Layer[layers.Length + 2];

            // Set the first element to the input
            combined[0] = input;

            // Set the second to the (n-1)th value to the hidden layers
            layers.CopyTo(combined, 1);

            // Set the final nth element the the output
            combined[layers.Length + 1] = output;

            // Returns the layers 
            return combined;
        }

        public List<float> GetNeuronArray()
        {
            /// <summary>
            // Summary: Returns an array of all of the neurons flattened
            /// </summary>
            List<float> n = new List<float>();
            int input_neurons = input.GetNeurons().Count;
            int output_neurons = output.GetNeurons().Count;
            for (int i = 0; i < input_neurons; i++)
            {
                n[i] = input.GetNeurons()[i];
            }

            int loc = input_neurons;
            foreach (var l in layers)
            {
                for (int i = 0; i < l.GetNeurons().Count; i++)
                {
                    n[loc] = l.GetNeurons()[i];
                    loc += 1;
                }
            }

            for (int i = 0; i < output_neurons; i++)
            {
                n[loc] = output.GetNeurons()[i];
                loc += 1;
            }

            return n;
        }

        public void AddLayer(int layer_position, string code, Activation activ)
        {
            // This method adds a new empty layer to the model.
            // The add neuron method should be used with this method to populate the new layer.

            // Create list of layers from the current layers
            List<Layer> new_layers = new List<Layer>(layers);

            // The previous layer
            Layer prev_layer;
            if (layers.Length == 0 || layer_position == 0)
            {
                prev_layer = input;
            }
            else
            {
                prev_layer = layers[layer_position - 1];
            }

            // The layer at layer_position will be the output layer for the newly added layer
            Layer next_layer;
            if (layers.Length > 0 && layer_position < layers.Length)
            {
                next_layer = layers[layer_position];
            }
            else
            {
                // If there are no hidden layers, then the output layer is the output layer
                next_layer = output;
            }

            // The output size for the new and previous layer
            int new_layer_out_size = next_layer.GetUnits();
            for (int i = 0; i < prev_layer.weights.Count; i++)
            {
                prev_layer.weights[i].Clear();
            }

            // Insert new layer with no neurons with an output size that matches the next layer's number of neurons
            new_layers.Insert(layer_position, BaseLayer.FromCode(code, activ, 0, new_layer_out_size));


            // Turn the layers list back into an array 
            layers = new_layers.ToArray();
        }

        public void RemoveLayer(int layer_position)
        {
            // Create list of layers from the current layers
            List<Layer> new_layers = new List<Layer>(layers);

            // Set the new previous layer
            Layer prev;
            if (layer_position == 0)
            {
                prev = input;
            }
            else
            {
                prev = new_layers[layer_position - 1];
            }

            // set the next layer
            Layer next;
            if (layer_position + 1 >= new_layers.Count)
            {
                next = output;
            }
            else
            {
                next = new_layers[layer_position + 1];
            }

            // Clear the previous weights
            for (int i = 0; i < prev.GetUnits(); i++)
            {
                prev.weights[i].Clear();
                for (int j = 0; j < next.GetUnits(); j++)
                {
                    prev.weights[i].Add(0);
                }
            }

            // Now we remove the layer
            new_layers.RemoveAt(layer_position);
            layers = new_layers.ToArray();
        }

        public void AddNeuron(int layer, int position)
        {

            // Evacuate if layer is invalid
            if (layer < 0 || layer >= layers.Length)
            {
                return;
            }

            // Add a neuron with a specific value to a specified layer
            total_neurons += 1;

            // Add the neuron
            layers[layer].neurons.Insert(position, 0);

            // Add the bias
            layers[layer].biases.Insert(position, 0);

            // Adjust the number of units in the layer
            layers[layer].IncrementUnitCount(1);

            // Get the number of weights this neuron should have by looking the layer's output size
            int num_weights = layers[layer].GetOutputSize();

            // Add new weights
            List<float> new_weights = new List<float>();
            for (int i = 0; i < num_weights; i++) { new_weights.Add(0); }
            layers[layer].weights.Insert(position, new_weights);

            // If the layer is the first layer we must add weights to the input
            if (layer == 0)
            {
                for (int i = 0; i < input.GetUnits(); i++)
                {
                    input.weights[i].Insert(position, 0);
                }
            }
            else
            {
                // We must add weights to the previous layer
                for (int i = 0; i < layers[layer - 1].neurons.Count; i++)
                {
                    layers[layer - 1].weights[i].Insert(position, 0);
                }
            }
        }

        public void RemoveNeuron(int layer, int position)
        {

            // Evacuate if layer is invalid
            if (layer < 0 || layer >= layers.Length)
            {
                return;
            }

            // Remove a neuron
            total_neurons += -1;

            // Remove the neuron
            layers[layer].neurons.RemoveAt(position);

            // Remove the Bias for that neuron
            layers[layer].biases.RemoveAt(position);

            // Adjust the number of units in the layer
            layers[layer].IncrementUnitCount(-1);

            // Remove the weights
            layers[layer].weights.RemoveAt(position);

            // If the layer is the first layer we must remove the weight from the input
            if (layer == 0)
            {
                for (int i = 0; i < input.GetUnits(); i++)
                {
                    input.weights[i].RemoveAt(position);
                }
            }
            else
            {
                // We must remove weight from the previous layer
                for (int i = 0; i < layers[layer - 1].neurons.Count; i++)
                {
                    layers[layer - 1].weights[i].RemoveAt(position);
                }
            }
        }

        public override List<float> Infer(List<float> inputs)
        {
            return FeedForward(inputs);
        }

        public List<float> FeedForward(List<float> inputs)
        {
            // Update the input's neurons to the inputs
            input.SetNeurons(inputs);
            Layer current_layer = input;
            Layer next_layer;

            // Create these two floats outside the loop
            float activation;
            float linear_combination;

            // Go through every layer
            for (int layer = 0; layer < layers.Length; layer++)
            {
                // Set the next layer
                next_layer = layers[layer];

                // Go through each neuron in the next layer
                for (int i = 0; i < next_layer.GetUnits(); i++)
                {
                    // Create the linear combination
                    linear_combination = 0;

                    // Get the dot product between the weights and the neurons
                    for (int j = 0; j < current_layer.GetUnits(); j++)
                    {
                        linear_combination += current_layer.GetWeight(j, i) * current_layer.GetNeuron(j);
                    }

                    // Set the neuron for the next layer
                    activation = current_layer.Activate(linear_combination + next_layer.biases[i]);
                    next_layer.SetNeuron(i, activation);
                }

                // Update the current layer
                current_layer = next_layer;
            }

            // Now go though the output layer
            for (int i = 0; i < output.GetUnits(); i++)
            {
                // Create the linear combination
                linear_combination = 0;

                // Get the dot product between the weights and the neurons
                for (int j = 0; j < current_layer.GetUnits(); j++)
                {
                    linear_combination += current_layer.GetWeight(j, i) * current_layer.GetNeuron(j);
                }

                // Set the neuron for the next layer
                // TODO For testing, I am removing the bias term from the calculation of the final layer
                //output.SetNeuron(i, current_layer.Activate(linear_combination + output.biases[i]));
                output.SetNeuron(i, current_layer.Activate(linear_combination));

            }

            // Apply the final activation to the ouput layer
            output.Activate();
            return output.neurons;
        }

        // Returns a copy of a model
        public override BaseModel Copy()
        {
            return NeuralNet.Copy(this);
        }


        public static NeuralNet Copy(NeuralNet nn)
        {
            // Returns a deep copy of a NeuralNet Object

            // Create a new network with the exact same architecture
            NeuralNet new_model = (NeuralNet)Parse.Parser.CodeToModel(nn.GetCode());

            // Get all of the layers
            Layer[] new_layers = new_model.GetAllLayers();
            Layer[] old_layers = nn.GetAllLayers();

            // Memberwise clone
            for (int i = 0; i < new_layers.Length; i++)
            {
                // Set each weight
                for (int j = 0; j < new_layers[i].weights.Count; j++)
                {
                    new_layers[i].weights[j].Clear();
                    new_layers[i].weights[j].AddRange(old_layers[i].weights[j]);
                }

                // Set the neuron and bias values 
                for (int j = 0; j < new_layers[i].neurons.Count; j++)
                {
                    new_layers[i].neurons[j] = old_layers[i].neurons[j];
                    new_layers[i].biases[j] = old_layers[i].biases[j];
                }

            }

            return new_model;
        }


        public static string Mutate(NeuralNet nn, float base_mutation_rate, float weight_mutation_rate, float neuro_mutation_rate, float bias_mutation_rate, float dropout_rate)
        {
            // A string value to track the mutations
            string log = "<";

            // Tracks how many individual mutations occured
            int mutation_tracker = 0;

            // Scale the rates down by half (smooths things out a bit)
            base_mutation_rate *= 0.5f;
            weight_mutation_rate *= 0.5f;
            neuro_mutation_rate *= 0.5f;
            bias_mutation_rate *= 0.5f;
            dropout_rate *= 0.5f;

            // Get all of the layers
            List<Layer> all_layers = nn.GetAllLayers().ToList();

            // Remove and store the last layer (it has no weights)
            Layer output = all_layers[all_layers.Count - 1];
            all_layers.RemoveAt(all_layers.Count - 1);

            // Since the output layer has no weights, we only want to mutate the activation funciton
            //if (Random.value < neuro_mutation_rate)
            //{
            //    // Get a random activation funciton
            //    string code = ActivationHelper.GetRandomActivation();

            //    // Update log
            //    log += "M" + mutation_tracker + "->" + "ActivationChanged@<L:" + "Output" + ">; E{" + output.activation.name + "->" + code + "}, ";

            //    // Change activation
            //    output.activation = Parse.Parser.ReadActivation(code);
            //}

            // In supremely rare cases, it may mutate and add another layer (this is probably not benificial)
            if (Random.value < neuro_mutation_rate)
            {
                // Not implemented yet
                log += "M" + mutation_tracker + "->" + "LayerAdded, ";
                mutation_tracker++;
            }

            // Look at each layer...
            int layer_index = 0;
            foreach (Layer layer in all_layers)
            {

                // Mutate the activation function
                if (Random.value < neuro_mutation_rate)
                {
                    // Get a random activation funciton
                    string code = ActivationHelper.GetRandomActivation();

                    // Update log
                    log += "M" + mutation_tracker + "->" + "ActivationChanged@<L:" + layer_index + ">; E{" + layer.activation.name + "->" + code + "}, ";

                    // Change activation
                    layer.activation = Parse.Parser.ReadActivation(code);
                }

                // Looking at each layer, we will use the neuro_mutation_rate and dropout_rate to see if we mutate
                // Here we look at the layer and for each time we have a triggered neuro mutation event we add a neuron
                int where = 0;
                while (Random.value * Random.value < neuro_mutation_rate && layer_index > 0)  // Note that we cannot add neurons to the input layer
                {
                    // If triggered, we add a neuron to this layer
                    where = Random.Range(0, layer.GetUnits());
                    nn.AddNeuron(layer_index - 1, where);

                    // Update the log
                    log += "M" + mutation_tracker + "->" + "NeuronAdded@<L:" + layer_index + ", P:" + where + ">, ";
                    mutation_tracker++;
                }

                // If the dropout was triggered, we remove a neuron if there is no weights
                // We count how many occurence of successful dropouts we have
                int weight_index;
                while (Random.value * Random.value < dropout_rate)
                {
                    // We only remove a neuron if the connection is zero or the dropout rate is triggered twice (a significant event)
                    where = Random.Range(0, layer.GetUnits());

                    try
                    {
                        // Which weight to look at
                        weight_index = Random.Range(0, layer.GetWeights()[where].Count);
                        // If we have triggered another event or the sum of the weights are zero (no connections) it will be dropped
                        float sum = layer.weights[where].Sum();
                        bool drop = Random.value < dropout_rate || Mathf.Approximately(sum, 0);
                        if (drop && layer_index > 0)  // Note that we cannot add neurons to the input layer
                        {

                            // If there is more than one neuron then delete it else remove layer
                            if (nn.layers[layer_index - 1].GetNeurons().Count > 1)
                            {
                                // Remove a neuron
                                nn.RemoveNeuron(layer_index - 1, where);
                                log += "M" + mutation_tracker + "->" + "NeuronDropped@<L:" + layer_index + ", P:" + where + "> , ";
                            }
                            else
                            {
                                nn.RemoveLayer(layer_index - 1);
                                log += "M" + mutation_tracker + "->" + "LayerDropped@<L:" + layer_index + ", P:" + where + "> , ";
                            }


                            // Update the log
                            mutation_tracker++;
                        }
                        else
                        {
                            // It is not ready to dropout unless the value is zero so here we set it to zero
                            layer.weights[where][weight_index] = 0;
                            layer.biases[where] = 0;

                            // Update the log
                            log += "M" + mutation_tracker + "->" + "SynapseRemoved@<L:" + layer_index + ", P:(" + where + "," + weight_index + ")>, ";
                            mutation_tracker++;
                        }
                    }
                    catch
                    {
                        break;
                    }
                }

                // We now mutate biases
                float old_value;
                while (Random.value < bias_mutation_rate)
                {
                    // Which neuron 
                    where = Random.Range(0, layer.GetUnits());
                    // Store old value
                    old_value = layer.biases[where];
                    // We nudge the value by using the base mutation rate (how much will it mutate?)
                    layer.biases[where] += Random.Range(-base_mutation_rate * 10f, base_mutation_rate * 10f);

                    // Update the log
                    log += "M" + mutation_tracker + "->" + "BiasMutated@<L:" + layer_index + ", P:" + where + ">; E{" + old_value + "->" + layer.biases[where] + "}, ";
                    mutation_tracker++;
                }

                // We now mutate weights 
                while (Random.value < weight_mutation_rate)
                {
                    // Which neuron 
                    where = Random.Range(0, layer.GetUnits());
                    // We nudge the value by using the base mutation rate (how much will it mutate?)
                    try
                    {
                        weight_index = Random.Range(0, layer.GetWeights()[where].Count);
                        // Store old weight
                        old_value = layer.weights[where][weight_index];

                        layer.weights[where][weight_index] += Random.Range(-base_mutation_rate * 10f, base_mutation_rate * 10f);

                        // Update the log
                        log += "M" + mutation_tracker + "->" + "WeightMutated@<L:" + layer_index + ", P:(" + where + "," + weight_index + ")>; E{" + old_value + "->" + layer.weights[where][weight_index] + "}, ";
                        mutation_tracker++;
                    }
                    catch
                    {
                        // Do nothing
                        break;
                    }
                }

                layer_index++;
            }
            log += "END>";

            return log;
        }


        public static void MutateWeights(NeuralNet nn, float mutation_rate, float dropout_rate)
        {
            // Look at each layer...
            foreach (Layer layer in nn.GetAllLayers())
            {
                // For each unit in the layer...
                for (int i = 0; i < layer.GetUnits(); i++)
                {
                    // Look at each weight
                    for (int j = 0; j < layer.weights[i].Count; j++)
                    {
                        // If the mutation is triggered, change the weight (the chances are proportional to the layers size)
                        if (Random.value < (mutation_rate / layer.GetUnits()))
                        {
                            layer.weights[i][j] += 2 * (Random.value - 0.5f);
                        }
                        // If the dropout is triggered and the mutation is not, drop the connection
                        else if (Random.value < (dropout_rate / layer.GetUnits()))
                        {
                            layer.weights[i][j] = 0;
                        }
                    }
                }
            }
        }


        public static void RandomizeWeights(NeuralNet nn)
        {
            // Look at each layer...
            foreach (Layer layer in nn.GetAllLayers())
            {
                // For each unit in the layer...
                for (int i = 0; i < layer.GetUnits(); i++)
                {
                    // Look at each weight
                    for (int j = 0; j < layer.weights[i].Count; j++)
                    {
                        layer.weights[i][j] += Random.value - 0.5f;
                    }
                }
            }
        }


        public string GetCode()
        {
            string code = "EvoNN:in-" + input.GetUnits() + "-" + input.GetOutputSize() + "-" + input.activation.name;
            foreach (var l in layers)
            {
                code += ">" + l.code + "-" + l.GetUnits() + "-" + l.GetOutputSize() + "-" + l.activation.name;
            }
            code += ">ot-" + output.GetUnits() + "-" + output.GetOutputSize() + "-" + output.activation.name;
            return code;
        }
    }
}
// EvoNN:in-5-20-Tanh>fc-20-20-Tanh>fc-20-20-Tanh>fc-20-20-Tanh>fc-20-10-Tanh>fc-10-10-Tanh>fc-10-10-Tanh>fc-10-10-Tanh>fc-10-10-Tanh>fc-10-2-Tanh>ot-2-2-Tanh

// EvoNN:in-3-1-Tanh>fc-1-1-Tanh>fc-1-1-Tanh>fc-1-3-Tanh>ot-3-0-Tanh

// EvoNN:in-5-3-Tanh>fc-3-3-Tanh>fc-3-3-Tanh>ot-5-0-Tanh

