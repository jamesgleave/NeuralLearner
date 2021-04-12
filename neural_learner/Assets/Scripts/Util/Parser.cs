using System;
using Layers;
using Model;

namespace Parse
{
    public static class Parser
    {
        public static BaseModel CodeToModel(string code)
        {
            // Break apart the code
            string[] components = BreakCode(code);
            int num_components = components.Length;

            // Get the model type
            string model_type = components[0];

            // Check the model
            if (model_type.Equals("EvoNN"))
            {
                Layer input = new BaseLayer();
                Layer output = new BaseLayer();
                string[] info;

                // Loop through the code
                Layer[] hidden_layers = new BaseLayer[num_components - 3];
                int current_hidden_layer = 0;
                for (int i = 1; i < num_components; i++)
                {
                    // Break the info apart
                    info = Parser.ParseLayer(components[i]);

                    // Get the components
                    string name = info[0];
                    int num_units = Int32.Parse(info[1]);
                    int output_size = Int32.Parse(info[2]);
                    string activation_function = info[3];

                    // Check if layer is the input layer
                    if (name.Contains("in"))
                    {
                        input = new FullyConnected(num_units, Parser.ReadActivation(activation_function), output_size);
                    }

                    // Check if layer is the output layer
                    if (name.Contains("ot"))
                    {
                        output = new FullyConnected(num_units, Parser.ReadActivation(activation_function), output_size);
                    }

                    // Check if the layer is fully connected
                    if (name.Contains("fc"))
                    {
                        hidden_layers[current_hidden_layer] = new FullyConnected(num_units, Parser.ReadActivation(activation_function), output_size);
                        current_hidden_layer++;
                    }
                }

                // Return and build the model
                return new NeuralNet(input, hidden_layers, output);
            }

            return null;
        }

        public static string[] BreakCode(string code)
        {
            char[] delim = { '>', ':' };
            return code.Split(delim);
        }

        public static string[] ParseLayer(string layer_code)
        {
            char[] delim = { '-' };
            return layer_code.Split(delim);
        }

        public static Activations.Activation ReadActivation(string activ)
        {
            // Return a linear activation
            if (activ.Equals("Linear"))
            {
                return new Activations.Linear();
            }

            // Return a Relu activation
            if (activ.Equals("Relu"))
            {
                return new Activations.Relu();
            }

            // Return a LeakyRelu activation
            if (activ.Contains("LeakyRelu"))
            {
                char delim = '.';
                float alpha = float.Parse(activ.Split(delim)[1]);
                return new Activations.LeakyRelu(alpha);
            }

            // Return a LeakyRelu activation
            if (activ.Contains("Tanh"))
            {
                return new Activations.Tanh();
            }

            // If invalid return linear
            return new Activations.Linear();

        }

    }
}
