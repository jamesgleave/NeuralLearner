using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model;
using Layers;
using Activations;

public class DisplayEvoNN : MonoBehaviour
{
    // the learner to display
    public EvolutionaryNeuralLearner learner;
    public GameObject neuron;
    public NeuralNet model;
    public float update_rate = 1f;

    public List<List<GameObject>> neurons;
    public List<float> x = new List<float>();
    public float cooldown;

    [Header("Horizontal Layer Seperation ")]
    public float alpha = 3;

    [Header("Vertical Neuron Seperation ")]
    public float lambda = 3;

    [Header("Max Neuron Size")]
    public float beta = 0.76f;

    [Header("Neuron Scale Rate")]
    public float theta = 6.9f;

    [Header("Max Weight Width")]
    public float sigma = 0.31f;

    [Header("Neuron Movement Speed")]
    public float gamma = 30f;

    [Header("The Visualizer Has Been Activated (Click To Reset)")]
    public bool activated = false;


    // Start is called before the first frame update
    void Activate()
    {
        if (neurons != null)
        {
            // Delete each neuron
            foreach (var n in neurons)
            {
                n.ForEach(Destroy);
            }
            neurons.Clear();
        }

        // Get the model from the learner
        model = (NeuralNet)learner.GetModel();

        // Set the cooldown
        cooldown = update_rate;

        // Create the arraylist to hold our neuron layers
        neurons = new List<List<GameObject>>();

        // Loop through each layer
        for (int i = 0; i < model.GetAllLayers().Length; i++)
        {
            List<GameObject> neuron_layer = new List<GameObject>();
            for (int j = 0; j < model.GetAllLayers()[i].GetNeurons().Count; j++)
            {
                neuron_layer.Add(Instantiate(neuron, transform));
            }
            neurons.Add(neuron_layer);
        }

        // Add children to all nodes
        AddChildren();

        // Set the neighbour nodes
        SetNeighbours();
    }

    void AddChildren()
    {
        // Loop through each layer
        for (int i = 0; i < neurons.Count - 1; i++)
        {
            // Grab the neuron layer
            List<GameObject> neuron_layer = neurons[i];

            // Loop through each neuron in the layer
            for (int j = 0; j < neuron_layer.Count; j++)
            {
                // Get the current neuron
                GameObject n = neuron_layer[j];

                // Clear the neurons children if it has any
                n.GetComponent<Neuron>().children.Clear();

                // Add every neuron from the next layer as a child
                List<GameObject> next_layer = neurons[i + 1];
                for (int k = 0; k < next_layer.Count; k++)
                {
                    n.GetComponent<Neuron>().AddChild(next_layer[k].GetComponent<Neuron>());
                }
            }
        }
    }

    void SetNeighbours()
    {
        foreach (List<GameObject> neuron_layer in neurons)
        {
            for (int i = 0; i < neuron_layer.Count; i++)
            {
                // If we are looking at the first neuron and the number of neurons are greater than one...
                if (i == 0 && neuron_layer.Count > 1)
                {
                    neuron_layer[i].GetComponent<Neuron>().neighbours["upper"] = neuron_layer[i + 1];
                }
                // If we are looking at any neuron in the middle 
                else if (i > 0 && neuron_layer.Count - 1 > i)
                {
                    // Add the upper neighbour
                    neuron_layer[i].GetComponent<Neuron>().neighbours["upper"] = neuron_layer[i + 1];

                    // Add the lower neighbour
                    neuron_layer[i].GetComponent<Neuron>().neighbours["lower"] = neuron_layer[i - 1];
                }
                // If we are looking at the last neuron
                else if (neuron_layer.Count - 1 == i && neuron_layer.Count > 1)
                {
                    // Add the lower neighbour
                    neuron_layer[i].GetComponent<Neuron>().neighbours["lower"] = neuron_layer[i - 1];
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Activate on first update
        if (activated == false)
        {
            Activate();
            activated = true;
        }
        else
        {
            model = (NeuralNet)learner.GetModel();

            //for (int i = 0; i < x.Count; i++)
            //{
            //    x[i] = Mathf.PerlinNoise(i, Time.realtimeSinceStartup);
            //}
            List<float> model_out = model.FeedForward(x);
            UpdateNeurons();
        }
    }

    public void ForceDrawLines()
    {
        // Forces all lines to be redrawn
        Neuron n;
        int layer_index = 0;
        foreach (Layer layer in model.GetAllLayers())
        {
            // Look at each neuron in the layer
            for (int i = 0; i < layer.GetUnits(); i++)
            {
                // Get the neuron
                n = neurons[layer_index][i].GetComponent<Neuron>();
                // Redraw the lines
                n.DrawLine(true);
            }
            // Increment the layer
            layer_index++;
        }
    }

    public void AddLayer(int layer_pos, int size)
    {

        print("Before: " + model.GetCode());
        List<GameObject> new_layer = new List<GameObject>();
        model.AddLayer(layer_pos - 1, "fc", new Tanh());
        neurons.Insert(layer_pos, new_layer);
        for (int i = 0; i < size; i++)
        {
            AddNeuron(layer_pos - 1, 0);
        }

        print("After: " + model.GetCode());
    }

    public void RemoveLayer(int layer_pos)
    {
        // Destroy each neuron
        for (int i = 0; i < neurons[layer_pos].Count; i++)
        {
            RemoveNeuron(layer_pos - 1, 0);
        }

        // Remove the layer from the display
        neurons.RemoveAt(layer_pos);

        // Remove the layer from the model
        print("Removing hidden layer " + (layer_pos - 1).ToString());
        model.RemoveLayer(layer_pos - 1);
        AddChildren();
    }

    public void AddNeuron(int layer, int position)
    {
        print("Adding Neuron To Layer " + layer + " at position " + position);
        // Add a neuron to the neural net's hidden layer 
        model.AddNeuron(layer, position);

        // Add a neuron to the display
        // Since we cannot add a neuron to the input or output, we must increment the layer by 1
        layer++;
        neurons[layer].Insert(position, Instantiate(neuron, transform));

        Neuron n = neurons[layer][position].GetComponent<Neuron>();

        // Setup the neighbours
        if (position - 1 > 0)
        {
            n.neighbours["lower"] = neurons[layer][position - 1];
        }

        if (position + 1 < neurons[layer].Count)
        {
            n.neighbours["upper"] = neurons[layer][position + 1];
        }

        // Recompute children nodes
        AddChildren();

        // The node will have the same children as its neighbours
        neurons[layer][position].GetComponent<Neuron>().SetupInsertedNode();
    }


    public void RemoveNeuron(int layer, int position)
    {
        // Add a neuron to the neural net's hidden layer 
        model.RemoveNeuron(layer, position);

        // Add a neuron to the display
        // Since we cannot add a neuron to the input or output, we must increment the layer by 1
        layer++;
        Destroy(neurons[layer][position]);
        neurons[layer].RemoveAt(position);

        // Recompute children nodes
        AddChildren();
    }

    void UpdateNeurons()
    {
        // Look at each layer
        Neuron n;
        int layer_index = 0;
        foreach (Layer layer in model.GetAllLayers())
        {
            // Look at each neuron in the layer
            for (int i = 0; i < layer.GetUnits(); i++)
            {
                // Get the neuron
                n = neurons[layer_index][i].GetComponent<Neuron>();

                // Update the weights, bias, value, etc
                n.SetWeights(w: layer.GetWeights()[i]);
                n.SetValue(layer.GetNeurons()[i]);
                n.SetBias(layer.biases[i]);
                n.SetLayer(layer_index);
                n.SetPosition(i);

                // Set the max size, scale rate, and weight edge thickness
                // If we scale the alpha and lambda factors by a great deal, we should also scale beta by a small amount
                float scale_factor = (alpha * lambda) / ((alpha + lambda) * 2);
                n.max_size = beta + scale_factor;
                n.scale_rate = theta;
                n.line_scaler = sigma;


                // Redraw the lines if the neuron has moved
                n.DrawLine(false);

                // Update the size of the neuron
                n.UpdateSize();

                // Calculate the desired position based on the neurons neighbours
                n.CalculateDesiredPosition(layer_index, i, layer.GetUnits(), alpha, lambda);

                // Update the movement of the neuron
                n.UpdateMovement(gamma);

            }
            // Increment the layer
            layer_index++;
        }
    }
}
// EvoNN:in-5-5-Tanh>fc-5-10-Tanh>fc-10-25-Tanh>fc-25-20-Tanh>fc-20-15-Tanh>fc-15-10-Tanh>ot-1-0-Tanh
// EvoNN:in-5-5-Tanh>fc-5-1-Tanh>ot-1-0-Tanh