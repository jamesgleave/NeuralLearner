using UnityEngine;
using Activations;

namespace Layers
{

    public abstract class Layer
    {
        public float[][] weights;    //weights
        public float[] neurons;    //neurons
        public float bias;  // The bias of course
        public Activation activation;   // The activation function

        // All funcitons that must be implemented
        public abstract void Activate();
        public abstract void SetNeuron(int i, float value);
        public abstract void SetName(string new_name);
        public abstract void SetWeights(float[] new_weights);
        public abstract void SetNeurons(float[] new_neurons);

        public abstract float[] GetNeurons();
        public abstract float[][] GetWeights();

        public abstract int GetUnits();
        public abstract float GetNeuron(int i);
        public abstract float Activate(float value);
        public abstract float GetWeight(int i, int j);
    }

    public class BaseLayer : Layer
    {
        // The number of neurons
        public int units = 0;
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

        public override float[][] GetWeights()
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

        public override float[] GetNeurons()
        {
            return neurons;
        }

        public override float GetNeuron(int i)
        {
            return neurons[i];
        }

        public override void SetWeights(float[] new_weights) { }

        public override void SetNeurons(float[] new_neurons)
        {
            for (int i = 0; i < units; i++)
            {
                neurons[i] = new_neurons[i];
            }
        }

        public override void SetNeuron(int i, float value)
        {
            neurons[i] = value;
        }
    }

    public class FullyConnected : BaseLayer
    {
        public FullyConnected(int units, Activation activation, int output_size)
        {

            // Set the number of units
            this.units = units;
            weights = new float[units][];
            for (int w = 0; w < units; w++)
            {
                // Initialize the weights randomly
                neurons = new float[units];
                weights[w] = new float[output_size];
                for (int i = 0; i < output_size; i++)
                {
                    // Set weight to random value
                    weights[w][i] = Random.Range(-0.5f, 0.5f);
                }

                // Set this layer's bias value randomly
                bias = Random.Range(-0.5f, 0.5f);

                // Set the activation function
                this.activation = activation;
            }
        }
    }
}
