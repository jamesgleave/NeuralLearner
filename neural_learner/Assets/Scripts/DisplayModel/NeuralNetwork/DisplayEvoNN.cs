using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model;
using Layers;
using Activations;
using System.Linq;

public class DisplayEvoNN : Display
{
    // the learner to display
    public EvolutionaryNeuralLearner learner;
    public GameObject neuron;
    public NeuralNet model;

    public List<List<GameObject>> neurons;
    public List<float> x = new List<float>();
    public float cooldown;

    // Start is called before the first frame update
    public override void Activate()
    {

        // Grab the agent passed into the state manager
        if (StateManager.selected_agent != null)
        {
            learner = (EvolutionaryNeuralLearner)StateManager.selected_agent.brain;
        }
        else
        {
            learner.Setup();
            //Model.NeuralNet.RandomizeWeights(learner.model);
        }

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

        // Run the base activate method
        base.Activate();
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
        }
        else if (cooldown <= 0)
        {
            model = (NeuralNet)learner.GetModel();
            // Set the model input to the selected agent's
            UpdateNeurons();
            cooldown = update_rate;
        }

        // Tick down the timer!
        cooldown -= Time.deltaTime;
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
        List<GameObject> new_layer = new List<GameObject>();
        model.AddLayer(layer_pos - 1, "fc", new Tanh());
        neurons.Insert(layer_pos, new_layer);
        for (int i = 0; i < size; i++)
        {
            AddNeuron(layer_pos - 1, 0);
        }
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

    public void AddNeuron(int layer, int position, bool full = false)
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

    public string Mutate(float base_mutation_rate, float weight_mutation_rate, float neuro_mutation_rate, float bias_mutation_rate, float dropout_rate)
    {
        // A string value to track the mutations
        string log = "<";

        // Tracks how many individual mutations occured
        int mutation_tracker = 0;

        // In supremely rare cases, it may mutate and add another layer (this is probably not benificial)
        if (Random.value < neuro_mutation_rate)
        {
            // Not implemented yet
            log += "M" + mutation_tracker + "->" + "LayerAdded, ";
            mutation_tracker++;
        }

        if (Random.value < neuro_mutation_rate)
        {
            // Not implemented yet
            log += "M" + mutation_tracker + "->" + "LayerAdded, ";
            mutation_tracker++;
        }

        // Get all of the layers
        List<Layer> all_layers = model.GetAllLayers().ToList(); ;

        // Remove and store the last layer (it has no weights)
        Layer output = all_layers[all_layers.Count - 1];
        all_layers.RemoveAt(all_layers.Count - 1);

        // Look at each layer...
        int layer_index = 0;
        foreach (Layer layer in all_layers)
        {
            // Looking at each layer, we will use the neuro_mutation_rate and dropout_rate to see if we mutate
            // Here we look at the layer and for each time we have a triggered neuro mutation event we add a neuron
            int where = 0;
            while (Random.value < neuro_mutation_rate && layer_index > 0)  // Note that we cannot add neurons to the input layer
            {
                // If triggered, we add a neuron to this layer
                where = Random.Range(0, layer.GetUnits());
                AddNeuron(layer_index - 1, where);

                // Update the log
                log += "M" + mutation_tracker + "->" + "NeuronAdded@<L:" + layer_index + ", P:" + where + ">, ";
                mutation_tracker++;
            }

            // If the dropout was triggered, we remove a neuron if there is no weights
            // We count how many occurence of successful dropouts we have
            int weight_index;
            while (Random.value < dropout_rate)
            {
                // We only remove a neuron if the connection is zero or the dropout rate is triggered twice (a significant event)
                where = Random.Range(0, layer.GetUnits());

                try
                {
                    // Which weight to look at
                    weight_index = Random.Range(0, layer.GetWeights()[where].Count);
                }
                catch
                {
                    return "Error: " + where + ", " + layer.GetUnits() + ", " + layer.GetWeights().Count + ", " + layer_index;
                }

                // If we have triggered another event or the sum of the weights are zero (no connections) it will be dropped
                float sum = layer.weights[where].Sum();
                bool drop = Random.value < dropout_rate || Mathf.Approximately(sum, 0);
                if (drop && layer_index > 0)  // Note that we cannot add neurons to the input layer
                {
                    // Remove a neuron
                    RemoveNeuron(layer_index - 1, where);

                    // Update the log
                    log += "M" + mutation_tracker + "->" + "NeuronDropped@<L:" + layer_index + ", P:" + where + "> , ";
                    mutation_tracker++;
                }
                else if (layer_index > 0)
                {
                    // It is not ready to dropout unless the value is zero so here we set it to zero
                    layer.weights[where][weight_index] = 0;
                    layer.biases[where] = 0;

                    // Update the log
                    log += "M" + mutation_tracker + "->" + "SynapseRemoved@<L:" + layer_index + ", P:(" + where + "," + weight_index + ")>, ";
                    mutation_tracker++;
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
                weight_index = Random.Range(0, layer.GetWeights()[where].Count);
                // Store old weight
                old_value = layer.weights[where][weight_index];
                layer.weights[where][weight_index] += Random.Range(-base_mutation_rate * 10f, base_mutation_rate * 10f);

                // Update the log
                log += "M" + mutation_tracker + "->" + "WeightMutated@<L:" + layer_index + ", P:(" + where + "," + weight_index + ")>; E{" + old_value + "->" + layer.weights[where][weight_index] + "}, ";
                mutation_tracker++;
            }

            layer_index++;
        }
        log += "END>";
        return log;
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
                n.SetActivation(layer.activation.name);
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
                n.min_line_width = sigma_prime;

                // Redraw the lines if the neuron has moved
                n.DrawLine(false);

                // Update the size of the neuron
                n.UpdateSize();

                // Calculate the desired position based on the neurons neighbours
                n.CalculateDesiredPosition(layer_index, i, layer.GetUnits(), alpha, lambda);

                // Update the movement of the neuron
                n.UpdateMovement(gamma, phi);

            }
            // Increment the layer
            layer_index++;
        }
    }
}
// EvoNN:in-5-5-Tanh>fc-5-10-Tanh>fc-10-25-Tanh>fc-25-20-Tanh>fc-20-15-Tanh>fc-15-10-Tanh>ot-1-0-Tanh
// EvoNN:in-5-5-Tanh>fc-5-1-Tanh>ot-1-0-Tanh