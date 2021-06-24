using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model;
using Parse;

public class EvolutionaryNeuralLearner : Brain
{

    // The model
    public NeuralNet model;

    // The model code
    public string model_code = "EvoNN:in-4-4-Tanh>ot-4-0-Tanh";

    // Start is called before the first frame update
    void Start()
    {
        if (model == null)
        {
            // Parse the model code and set the model 
            model = (NeuralNet)Parser.CodeToModel(model_code);
            Mutate();
        }
    }

    public override void Setup()
    {
        // Parse the model code and set the model 
        model = (NeuralNet)Parser.CodeToModel(model_code);
    }

    public override void Setup(BaseModel nn)
    {
        // Setup and mutate the model
        model = (NeuralNet)nn;
        Mutate();
    }

    public void SetCode(string code)
    {
        model_code = code;
    }

    public override BaseModel GetModel()
    {
        return model;
    }

    public override List<float> GetAction(List<float> obs)
    {
        return model.FeedForward(obs);
    }

    public int GetTotalNeurons()
    {
        return model.total_neurons;
    }

    public override void Mutate()
    {
        if (TryGetComponent<BaseAgent>(out BaseAgent a))
        {
            //Model.NeuralNet.MutateWeights(model, GetComponent<BaseAgent>().genes.weight_mutation_prob, GetComponent<BaseAgent>().genes.dropout_prob);
            string log = NeuralNet.Mutate(model,
                base_mutation_rate: a.genes.base_mutation_rate,
                weight_mutation_rate: a.genes.weight_mutation_prob,
                neuro_mutation_rate: a.genes.neuro_mutation_prob,
                bias_mutation_rate: a.genes.bias_mutation_prob,
                dropout_rate: a.genes.dropout_prob);
        }
        else
        {
            Model.NeuralNet.MutateWeights(model, 0.01f, 0.001f);
        }
        model_code = model.GetCode();
    }
}
