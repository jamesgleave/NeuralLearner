using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestNEAT : MonoBehaviour
{

    public Genome g = new Genome();

    public float input;
    public List<int> unprocessed = new List<int>();
    public List<float> outputs = new List<float>();

    public List<float> input_values = new List<float>();
    public List<bool> input_recurrence = new List<bool>();
    public List<float> output_weights = new List<float>();
    public List<float> hidden_state = new List<float>();
    public List<int> out_ids = new List<int>();
    public List<int> execution_order = new List<int>();

    public float neuron_output = 0;
    public bool neuron_ready = false;
    public bool neuron_activated = false;
    public int depth = 0;
    public int lookat = 0;

    public NEATNetwork nn;

    // Start is called before the first frame update
    void Start()
    {
        g.AddNode(new NodeGene(NodeGeneType.Input, 0));
        g.AddNode(new NodeGene(NodeGeneType.Output, 1));


        g.AddNode(new NodeGene(NodeGeneType.Hidden, 2));
        g.AddNode(new NodeGene(NodeGeneType.Hidden, 3));

        g.AddConnection(new ConnectionGene(0, 2, 1, true, g.GetNextInnovation()));

        g.AddConnection(new ConnectionGene(2, 3, 1, true, g.GetNextInnovation()));

        g.AddConnection(new ConnectionGene(3, 2, 1, true, g.GetNextInnovation()));
        g.AddConnection(new ConnectionGene(3, 1, 1, true, g.GetNextInnovation()));


        // Setup nn
        nn = new NEATNetwork(g);

        List<float> inputs = new List<float>();
        inputs.Add(input);

        CalculateDepth();
        nn.FindRecurrence(inputs);

        output_weights = nn.GetNeurons()[lookat].GetWeights();
        out_ids = nn.GetNeurons()[lookat].GetOutputIDs();
        input_recurrence = nn.GetNeurons()[lookat].GetRecurrence();
        input_values = nn.GetNeurons()[lookat].GetInputs();
        neuron_output = nn.GetNeurons()[lookat].GetOutput();
        execution_order = nn.GetNeurons()[lookat].order;
        neuron_ready = nn.GetNeurons()[lookat].IsReady();
        hidden_state = nn.GetNeurons()[lookat].GetHiddenStateList();
        depth = nn.GetNeurons()[lookat].GetDepth();
    }

    void Update()
    {
        lookat = Mathf.Clamp(lookat, 0, g.GetNodes().Count - 1);
        if (Input.GetKeyDown(KeyCode.D))
        {
            // Setup nn
            List<float> inputs = new List<float>();
            inputs.Add(input);
            outputs = nn.Infer(inputs);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            List<float> inputs = new List<float>();
            inputs.Add(input);
        }
        output_weights = nn.GetNeurons()[lookat].GetWeights();
        out_ids = nn.GetNeurons()[lookat].GetOutputIDs();
        input_recurrence = nn.GetNeurons()[lookat].GetRecurrence();
        input_values = nn.GetNeurons()[lookat].GetInputs();
        neuron_output = nn.GetNeurons()[lookat].GetOutput();
        execution_order = nn.GetNeurons()[lookat].order;
        neuron_ready = nn.GetNeurons()[lookat].IsReady();
        hidden_state = nn.GetNeurons()[lookat].GetHiddenStateList();
        depth = nn.GetNeurons()[lookat].GetDepth();
    }

    public void CalculateDepth()
    {
        Dictionary<int, NEATNeuron> neurons = nn.GetNeurons();
        Dictionary<NEATNeuron, int> buffer = new Dictionary<NEATNeuron, int>();

        bool cont = true;
        int iterations = 0;
        int total_loops = 0;
        while (cont)
        {
            cont = false;
            iterations++;
            foreach (KeyValuePair<int, NEATNeuron> pair in neurons)
            {
                total_loops++;
                if (pair.Value.GetDepth() != int.MinValue)
                {
                    foreach (int output_id in pair.Value.GetOutputIDs())
                    {
                        if (neurons[output_id].GetDepth() == int.MinValue)
                        {
                            neurons[output_id].SetDepth(pair.Value.GetDepth() + 1);
                        }
                        total_loops++;
                    }
                }
                else
                {
                    cont = true;
                }
            }
        }
        print("iterations: " + iterations + " with a total of " + total_loops + " loops");
    }
}
