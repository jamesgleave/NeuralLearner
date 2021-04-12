using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model;
using Parse;


public class testing : MonoBehaviour
{
    NeuralNet model;

    // Start is called before the first frame update
    void Start()
    {
        string code = "EvoNN:in-2-44-LeakyRelu.3>fc-44-43-LeakyRelu.3>fc-43-10-LeakyRelu.3>ot-10-3-Linear";
        model = (NeuralNet)Parser.CodeToModel(code);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
