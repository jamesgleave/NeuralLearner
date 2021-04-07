using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model;
using Layers;

public class DisplayEvoNN : MonoBehaviour
{
    // the learner to display
    public EvolutionaryNeuralLearner learner;
    public GameObject neuron;
    public NeuralNet model;
    public float update_rate = 1f;

    public GameObject[] neurons;
    public float cooldown;
    public bool activated = false;
    public float[] x = new float[5];
    int total_updates = 0;



    // Start is called before the first frame update
    void Activate()
    {
        // Get the model from the learner
        model = learner.GetModel();

        // Set the cooldown
        cooldown = update_rate;

        // Initialize neruons
        int hidden_layer_count = 0;
        neurons = new GameObject[learner.GetTotalNeurons()];

        // Create a new list for the layers including the input and output
        int num_layers = model.GetLayers().Length + 2;
        Layer[] layers = new Layer[num_layers];
        layers[0] = model.input;
        layers[num_layers - 1] = model.output;
        // Add the rest of the layers
        for (int i = 1; i < num_layers - 1; i++)
        {
            layers[i] = model.GetLayers()[i - 1];
        }

        int unit_tracker = 0;
        foreach (var layer in layers)
        {
            print(layer);
            print(hidden_layer_count);
            print("");
            for (int i = 0; i < layer.GetUnits(); i++)
            {
                neurons[unit_tracker] = Instantiate(neuron, new Vector3(0, 0 + i * 1 - layer.GetUnits() / 2, 0 + hidden_layer_count * 3), transform.rotation, transform);
                unit_tracker += 1;
            }
            hidden_layer_count += 1;
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
            // Update the model
            if (cooldown <= 0)
            {
                model = learner.GetModel();
                float[] model_out = model.FeedForward(x);
                float m = Mathf.Max(x);
                print("Model Input: " + x[0].ToString() + "," + x[1].ToString() + " -> Model Output: " + model_out[0].ToString() + "," + model_out[1].ToString());
                cooldown = update_rate;
                UpdateNeurons();
            }
            else
            {
                cooldown -= Time.deltaTime;
            }

            total_updates += 1;
        }
    }

    void UpdateNeurons()
    {
        float[] model_neurons = model.GetNeuronArray();
        for (int i = 0; i < neurons.Length; i++)
        {
            float size = 0.5f + model_neurons[i];
            neurons[i].GetComponent<Transform>().localScale = new Vector3(size, size, size);
        }
    }
}
// EvoNN:in-5-10-Tanh>fc-10-10-Tanh>fc-10-10-Tanh>fc-10-10-Tanh>fc-10-10-Tanh>fc-10-10-Tanh>fc-10-2-Tanh>ot-2-2-Tanh