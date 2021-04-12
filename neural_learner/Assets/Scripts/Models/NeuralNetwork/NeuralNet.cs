using System.Collections.Generic;
using UnityEngine;
using Layers;
using Activations;
using Callbacks;

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

        public void AddLayer(int layer_position, string code, Activation activ, int num_units)
        {
            // Maybe I will add this
        }

        public void RemoveLayer(int layer)
        {
            // Maybe I will add this
        }

        public void AddNeuron(int layer, int position)
        {
            // Add a neuron with a specific value to a specified layer
            total_neurons += 1;

            // Add the neuron
            layers[layer].neurons.Insert(position, 0);

            // Add the bias
            layers[layer].biases.Insert(position, Random.value - 0.5f);

            // Adjust the number of units in the layer
            layers[layer].IncrementUnitCount(1);

            // Get the number of weights this neuron should have by looking the layer's output size
            int num_weights = layers[layer].GetOutputSize();

            // Add new weights
            List<float> new_weights = new List<float>();
            for (int i = 0; i < num_weights; i++) { new_weights.Add(Random.Range(-1f, 1f)); }
            layers[layer].weights.Insert(position, new_weights);

            // If the layer is the first layer we must add weights to the input
            if (layer == 0)
            {
                for (int i = 0; i < input.GetUnits(); i++)
                {
                    input.weights[i].Insert(position, Random.Range(-1f, 1f));
                }
            }
            else
            {
                // We must add weights to the previous layer
                for (int i = 0; i < layers[layer - 1].neurons.Count; i++)
                {
                    layers[layer - 1].weights[i].Insert(position, Random.Range(-1f, 1f));
                }
            }
        }

        public void RemoveNeuron(int layer, int position)
        {
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
                output.SetNeuron(i, current_layer.Activate(linear_combination + output.biases[i]));
            }

            // Apply the final activation to the ouput layer
            output.Activate();
            return output.neurons;
        }
    }
}
// EvoNN:in-5-20-Tanh>fc-20-20-Tanh>fc-20-20-Tanh>fc-20-20-Tanh>fc-20-10-Tanh>fc-10-10-Tanh>fc-10-10-Tanh>fc-10-10-Tanh>fc-10-10-Tanh>fc-10-2-Tanh>ot-2-2-Tanh

// EvoNN:in-3-1-Tanh>fc-1-1-Tanh>fc-1-1-Tanh>fc-1-3-Tanh>ot-3-0-Tanh

// EvoNN:in-5-200-Tanh>fc-200-200-Tanh>fc-200-3-Tanh>ot-3-0-Tanh

