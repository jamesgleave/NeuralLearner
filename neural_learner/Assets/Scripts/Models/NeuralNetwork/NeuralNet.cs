using Layers;
using Callbacks;

namespace Model
{
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
            foreach (var x in layers) { total += x.GetNeurons().Length; }
            total += input.GetNeurons().Length + output.GetNeurons().Length;
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
            combined[layers.Length + 2] = output;

            // Returns the layers 
            return combined;
        }

        public float[] GetNeuronArray()
        {
            /// <summary>
            // Summary: Returns an array of all of the neurons flattened
            /// </summary>
            float[] n = new float[total_neurons];
            int input_neurons = input.GetNeurons().Length;
            int output_neurons = output.GetNeurons().Length;
            for (int i = 0; i < input_neurons; i++)
            {
                n[i] = input.GetNeurons()[i];
            }

            int loc = input_neurons;
            foreach (var l in layers)
            {
                for (int i = 0; i < l.GetNeurons().Length; i++)
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

        public float[] FeedForward(float[] inputs)
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
                    activation = current_layer.Activate(linear_combination + current_layer.bias);
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
                output.SetNeuron(i, current_layer.Activate(linear_combination + output.bias));
            }

            // Apply the final activation to the ouput layer
            output.Activate();
            return output.neurons;
        }

        public float[] FeedForward(float[] inputs, BaseCallback callback)
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
                    activation = current_layer.Activate(linear_combination + current_layer.bias);
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
                output.SetNeuron(i, current_layer.Activate(linear_combination));
            }

            // Apply the final activation to the ouput layer
            output.Activate();
            return output.neurons;
        }
    }
}



