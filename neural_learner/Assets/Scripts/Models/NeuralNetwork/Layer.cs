using System.Collections.Generic;
using UnityEngine;
using Activations;

namespace Layers
{

    public abstract class Layer
    {
        public List<List<float>> weights;    //weights
        public List<float> neurons;    //neurons
        public float bias;  // The bias of course
        public List<float> biases;
        public Activation activation;   // The activation function
        public string code;

        // All funcitons that must be implemented
        public abstract void Activate();
        public abstract void SetNeuron(int i, float value);
        public abstract void SetName(string new_name);
        public abstract void SetWeights(List<float> new_weights);
        public abstract void SetNeurons(List<float> new_neurons);
        public abstract void IncrementUnitCount(int i);

        public abstract List<float> GetNeurons();
        public abstract List<List<float>> GetWeights();

        public abstract int GetUnits();
        public abstract int GetOutputSize();
        public abstract float GetNeuron(int i);
        public abstract float Activate(float value);
        public abstract float GetWeight(int i, int j);
    }

    public class BaseLayer : Layer
    {
        // The number of neurons
        public int units = 0;
        public int output_size = 0;
        public string name = "Layer";

        public override void SetName(string new_name)
        {
            name = new_name;
        }

        public override float Activate(float value)
        {
            return activation.activate(value);
        }

        public override void Activate()
        {
            neurons = activation.activate(neurons);
        }

        public override List<List<float>> GetWeights()
        {
            return weights;
        }

        public override float GetWeight(int i, int j)
        {
            return weights[i][j];
        }

        public override int GetUnits()
        {
            return units;
        }

        public override int GetOutputSize()
        {
            if (weights.Count > 0 && weights[0].Count != output_size)
            {
                output_size = weights[0].Count;
            }
            return output_size;
        }

        public override List<float> GetNeurons()
        {
            return neurons;
        }

        public override float GetNeuron(int i)
        {
            return neurons[i];
        }

        public override void SetWeights(List<float> new_weights)
        {
        }

        public override void SetNeurons(List<float> new_neurons)
        {
            neurons = new_neurons;
        }

        public override void SetNeuron(int i, float value)
        {
            neurons[i] = value;
        }

        public override void IncrementUnitCount(int i)
        {
            units += i;
        }

        public static Layer FromCode(string code, Activation activ, int num_units, int output_size)
        {
            // Compare the codes
            switch (code)
            {
                case "fc":
                    return new FullyConnected(num_units, activ, output_size);
            }

            return new BaseLayer();
        }
    }

    public class FullyConnected : BaseLayer
    {
        public FullyConnected(int units, Activation activation, int output_size)
        {

            // Set the number of units
            this.units = units;
            this.output_size = output_size;

            // Create the weights and neurons
            weights = new List<List<float>>();
            neurons = new List<float>();
            biases = new List<float>();
            code = "fc";

            var temp_weights = new float[units][];
            for (int w = 0; w < units; w++)
            {
                // Initialize the weights
                temp_weights[w] = new float[output_size];

                // Add to the neurons
                neurons.Add(0);

                // Add a bias for each neuron
                biases.Add(0);

                for (int i = 0; i < output_size; i++)
                {
                    // Set weight
                    temp_weights[w][i] = 0;
                }

                weights.Add(new List<float>(temp_weights[w]));
            }
            // Set the activation function
            this.activation = activation;
        }

        public List<float> GetOutput(List<float> inputs)
        {
            // Performs the feedforward algorithm for this layer
            List<float> outputs = new List<float>();

            // Loop through the inputs and set the neurons to the inputs
            for (int i = 0; i < neurons.Count; i++)
            {
                SetNeuron(i, inputs[i]);
            }

            foreach (List<float> wv in weights)
            {

            }
            return outputs;
        }
    }
}
