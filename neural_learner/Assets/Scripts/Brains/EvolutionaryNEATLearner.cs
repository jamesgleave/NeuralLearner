using System.Collections;
using System.Collections.Generic;
using Model;
using UnityEngine;
using System.Threading;

public class EvolutionaryNEATLearner : Brain
{
    // The model 
    public NEATNetwork model;

    // The genome that creates the model
    public Genome genome;

    public BaseAgent agent;

    /// <summary>
    /// The input size of the NEAT network
    /// </summary>
    public int input_size;

    /// <summary>
    /// The output size of the NEAT network
    /// </summary>
    public int output_size;

    /// <summary>
    /// The log of the last mutation
    /// </summary>
    public string log;

    /// <summary>
    /// Whether or not to mutate 25 times on load
    /// </summary>
    public bool mutate_on_load;

    public string gcode;

    Thread t;
    public bool use_threading_model_creation;

    // Start is called before the first frame update
    void Start()
    {
        if (model == null)
        {
            genome = new Genome();
        }
    }

    public override void Setup()
    {
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
            for (int i = 0; i < 100; i++)
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
        if (agent != null)
        {
            Genome new_genome = model.CopyGenome();
            log = Genome.Mutate(new_genome, agent.genes.base_mutation_rate, agent.genes.weight_mutation_prob, agent.genes.neuro_mutation_prob, agent.genes.bias_mutation_prob, agent.genes.dropout_prob);
            genome = new_genome;

            // Set the model using multi threading or not
            if (use_threading_model_creation)
            {
                t = new Thread(SetNewModel);
                t.Start();
            }
            else
            {
                SetNewModel();
            }

        }
        else
        {
            log = Genome.Mutate(genome, 1, 0.8f, 0.8f, 0.8f, 0.8f);
            model = new NEATNetwork(genome);
        }
    }

    /// <summary>
    /// Sets the new model with the new genome
    /// </summary>
    private void SetNewModel()
    {
        model = new NEATNetwork(genome);
    }
}
