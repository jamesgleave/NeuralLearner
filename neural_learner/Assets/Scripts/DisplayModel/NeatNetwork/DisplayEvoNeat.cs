using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayEvoNEAT : Display
{

    // Store each input, hidden, and output neuron
    public List<NEATDisplayNeuron> input_neurons;
    public List<NEATDisplayNeuron> hidden_neurons;
    public List<NEATDisplayNeuron> output_neurons;
    public int input_index = 0;
    public int hidden_index = 0;
    public int output_index = 0;

    /// <summary>
    /// The names of each input neuron (what their inputs correspond to)
    /// </summary>
    public List<string> input_names = null;
    /// <summary>
    /// The names of each output neuron (what their outputs correspond to)
    /// </summary>
    public List<string> output_names = null;

    // The depth tracker, where the key is the depth value and the value is the number of neurons found at that depth
    public Dictionary<int, int> depth_counter = new Dictionary<int, int>();

    // the learner to display
    public EvolutionaryNEATLearner learner;
    public GameObject neuron;
    public NEATNetwork model;

    public List<float> x = new List<float>();
    public float cooldown;

    // The ID of the selected neuron
    public int selected_id;

    // Scalers for display
    [Space]
    public float display_x_seperation_scaler;
    public float display_y_seperation_scaler;
    public float vert_offset;
    public float hori_offset;

    // Start is called before the first frame update
    public override void Activate()
    {

        // Increments or adds a value to the depth counter dictionary
        void Assist(NEATNeuron neuron)
        {
            if (depth_counter.ContainsKey(neuron.GetDepth()))
            {
                depth_counter[neuron.GetDepth()]++;
            }
            else
            {
                depth_counter[neuron.GetDepth()] = 1;
            }
        }

        // Grab the agent passed into the state manager
        if (StateManager.selected_agent != null)
        {
            // Grab the learner
            learner = (EvolutionaryNEATLearner)StateManager.selected_agent.brain;

            // Setup the input names
            input_names = StateManager.selected_agent.senses.observation_names;

            // Setup the output names
            output_names = new List<string>();
            output_names.Add("Move Forward");
            output_names.Add("Rotate");
            output_names.Add("Wants To Reproduce");
            output_names.Add("Wants To Eat");
            output_names.Add("Wants To Grab");
            output_names.Add("Wants To Attack");
            output_names.Add("Produce Red Pheromone");
            output_names.Add("Produce Green Pheromone");
            output_names.Add("Produce Blue Pheromone");

        }
        else
        {
            learner.Setup();
        }

        // Get the model from the learner
        model = (NEATNetwork)learner.GetModel();

        // Set each neuron
        foreach (KeyValuePair<int, NEATNeuron> n in model.GetNeurons())
        {
            // Add to inputs
            if (model.GetInputs().Contains(n.Key))
            {
                var x = Instantiate(neuron, transform).GetComponent<NEATDisplayNeuron>();
                x.Setup(n.Value, this);
                input_neurons.Add(x);
                Assist(x.neuron);

                // Since this is an input neuron, we give it a name
                if (input_names != null && input_names.Count > 0)
                {
                    x.SetIOName(input_names[0]);
                    input_names.RemoveAt(0);
                }
            }
            // Add to outputs
            else if (model.GetOutputs().Contains(n.Key))
            {
                var x = Instantiate(neuron, transform).GetComponent<NEATDisplayNeuron>();
                x.Setup(n.Value, this);
                output_neurons.Add(x);
                Assist(x.neuron);

                // Since this is an output neuron, we give it a name
                if (output_names != null && output_names.Count > 0)
                {
                    x.SetIOName(output_names[0]);
                    output_names.RemoveAt(0);
                }
            }
            // Add to hidden
            else
            {
                var x = Instantiate(neuron, transform).GetComponent<NEATDisplayNeuron>();
                x.Setup(n.Value, this);
                hidden_neurons.Add(x);
                Assist(x.neuron);
                x.SetIOName("Hidden Neuron");
            }
        }

        // Set the cooldown
        cooldown = update_rate;

        // Setup all children
        SetupChildren();

        // Run the base activate method
        base.Activate();

        print(model.GetComplexity());
    }

    private void SetupChildren()
    {
        // Get each child neuron
        List<NEATDisplayNeuron> combined = new List<NEATDisplayNeuron>();
        combined.AddRange(input_neurons);
        combined.AddRange(hidden_neurons);
        combined.AddRange(output_neurons);

        // Sadly, we must go O(n^3) to add the children :( im dumb and lazy but my mom said its okay so
        foreach (NEATDisplayNeuron disp_neuron_parent in combined)
        {
            foreach (NEATDisplayNeuron disp_neuron_child in combined)
            {
                // Check if the output ids contain the id of the child neuron
                bool is_child = disp_neuron_parent.neuron.GetOutputIDs().Contains(disp_neuron_child.neuron.GetNeuronID());
                // If the neuron is a child, add it 
                if (is_child)
                {
                    disp_neuron_parent.AddChild(disp_neuron_child);
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {

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
            model = (NEATNetwork)learner.GetModel();
            // Set the model input to the selected agent's
            UpdateNeurons();
            // Reset the update rate
            cooldown = update_rate;
        }

        // Tick down the timer!
        cooldown -= Time.deltaTime;

        // Run inference
        if (Input.GetKeyDown(KeyCode.D))
        {
            // Setup nn
            List<float> inputs = new List<float>();
            for (int i = 0; i < 22; i++)
            {
                inputs.Add(Random.Range(-1f, 1f));
            }
            print(model.Infer(inputs));
        }
    }

    private void UpdateNeurons()
    {
        // Set indices to zero
        input_index = 0;
        output_index = 0;
        hidden_index = 0;

        for (int i = 0; i < Mathf.Max(input_neurons.Count, output_neurons.Count, hidden_neurons.Count); i++)
        {
            // Instead of looping through every neuron in each frame, we update a single neuron from each list per frame
            // Update the positions (the index in the list and update interal values by reading the NEAT neuron associated with this display neuron
            if (input_index < input_neurons.Count)
            {
                input_neurons[input_index].position = input_index;
                input_neurons[input_index].UpdateInternalState();
                input_index++;

            }
            if (hidden_index < hidden_neurons.Count)
            {
                hidden_neurons[hidden_index].position = hidden_index % depth_counter[hidden_neurons[hidden_index].neuron.GetDepth()];
                hidden_neurons[hidden_index].UpdateInternalState();
                hidden_index++;
            }
            if (output_index < output_neurons.Count)
            {
                output_neurons[output_index].position = output_index;
                output_neurons[output_index].UpdateInternalState();
                output_index++;
            }
        }
    }

    public List<NEATDisplayNeuron> GetAllNeurons()
    {
        List<NEATDisplayNeuron> output = new List<NEATDisplayNeuron>();
        output.AddRange(input_neurons);
        output.AddRange(hidden_neurons);
        output.AddRange(output_neurons);
        return output;
    }
}
