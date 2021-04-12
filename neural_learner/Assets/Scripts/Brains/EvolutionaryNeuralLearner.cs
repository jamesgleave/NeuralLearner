using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model;
using Parse;

public class EvolutionaryNeuralLearner : MonoBehaviour
{

    // The model
    public NeuralNet model;

    // The model code
    public string model_code = "EvoNN:in-2-44-LeakyRelu.3>fc-44-43-LeakyRelu.3>fc-43-10-LeakyRelu.3>ot-10-3-Linear";

    // Start is called before the first frame update
    void Start()
    {
        // Parse the model code and set the model 
        model = (NeuralNet)Parser.CodeToModel(model_code);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetCode(string code)
    {
        model_code = code;
    }

    public NeuralNet GetModel()
    {
        return model;
    }

    public int GetTotalNeurons()
    {
        return model.total_neurons;
    }
}
