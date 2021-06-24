using System.Collections;
using System.Collections.Generic;
using Model;
using UnityEngine;

public class EvolutionaryNEATLearner : Brain
{
    // The model 
    public NEATNetwork model;

    // The genome that creates the model
    public Genome genome;

    /// <summary>
    /// The input size of the NEAT network
    /// </summary>
    public int input_size;
    private string[] input_names;

    /// <summary>
    /// The output size of the NEAT network
    /// </summary>
    public int output_size;
    private string[] output_names;

    /// <summary>
    /// The log of the last mutation
    /// </summary>
    public string log;

    /// <summary>
    /// Whether or not to mutate 25 times on load
    /// </summary>
    public bool mutate_on_load;

    public string gcode;

    // Start is called before the first frame update
    void Start()
    {
        if (model == null)
        {

            genome = new Genome();
            //genome.AddNode(new NodeGene(NodeGeneType.Input, 0));

            //genome.AddNode(new NodeGene(NodeGeneType.Hidden, 1));
            //genome.AddNode(new NodeGene(NodeGeneType.Hidden, 2));
            //genome.AddNode(new NodeGene(NodeGeneType.Hidden, 3));

            //genome.AddNode(new NodeGene(NodeGeneType.Output, 4));
            //genome.AddNode(new NodeGene(NodeGeneType.Output, 5));


            //genome.AddConnection(new ConnectionGene(0, 1, Random.value - 0.5f, true, genome.GetNextInnovation()));
            //genome.AddConnection(new ConnectionGene(0, 2, Random.value - 0.5f, true, genome.GetNextInnovation()));
            //genome.AddConnection(new ConnectionGene(0, 3, Random.value - 0.5f, true, genome.GetNextInnovation()));

            //genome.AddConnection(new ConnectionGene(1, 4, Random.value - 0.5f, true, genome.GetNextInnovation()));
            //genome.AddConnection(new ConnectionGene(1, 2, Random.value - 0.5f, true, genome.GetNextInnovation()));

            //genome.AddConnection(new ConnectionGene(2, 3, Random.value - 0.5f, true, genome.GetNextInnovation()));

            //genome.AddConnection(new ConnectionGene(4, 5, Random.value - 0.5f, true, genome.GetNextInnovation()));

            //for (int i = 0; i < 22; i++)
            //{
            //    genome.AddNode(new NodeGene(NodeGeneType.Input, i));
            //}

            //for (int i = 0; i < 8; i++)
            //{
            //    genome.AddNode(new NodeGene(NodeGeneType.Output, i + 22));
            //}

            //// Parse the model code and set the model 
            //model = new NEATNetwork(genome);
            //print(model.num_times_recompiled);
        }
    }

    public override void Setup()
    {
        // Create the lists of names for input output nodes
        input_names = new string[input_size];
        output_names = new string[output_size];

        genome = new Genome();
        for (int i = 0; i < input_size; i++)
        {
            genome.AddNode(new NodeGene(NodeGeneType.Input, i));
        }
        for (int i = 0; i < output_size; i++)
        {
            genome.AddNode(new NodeGene(NodeGeneType.Output, i + input_size));
        }

        if (mutate_on_load)
        {
            for (int i = 0; i < 25; i++)
            {
                Mutate();
            }
        }

        // Parse the model code and set the model
        model = new NEATNetwork(genome);
    }

    public override void Setup(BaseModel nn)
    {
        // Setup and mutate the model
        model = (NEATNetwork)nn;
        Mutate();
    }

    public override List<float> GetAction(List<float> obs)
    {
        return model.Infer(obs);
    }

    public override BaseModel GetModel()
    {
        return model;
    }


    /// <summary>
    /// Sets the model and genome
    /// </summary>
    /// <param name="new_genome"></param>
    public void SetModel(Genome new_genome)
    {
        genome = new_genome;
        model = new NEATNetwork(genome);
    }

    public override void Mutate()
    {
        if (TryGetComponent<BaseAgent>(out BaseAgent a))
        {
            Genome new_genome = model.CopyGenome();
            log = Genome.Mutate(new_genome, a.genes.base_mutation_rate, a.genes.weight_mutation_prob, a.genes.neuro_mutation_prob, a.genes.bias_mutation_prob, a.genes.dropout_prob);
            model = new NEATNetwork(new_genome);
            genome = new_genome;
        }
        else
        {
            log = Genome.Mutate(genome, 1, 0.8f, 0.8f, 0.8f, 0.8f);
            model = new NEATNetwork(genome);
        }
    }
}
