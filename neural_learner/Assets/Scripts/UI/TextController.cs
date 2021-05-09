using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextController : MonoBehaviour
{

    public Text layer, neuron, value, bias, controls;
    // Start is called before the first frame update
    void Start()
    {
        Deactivate();
    }

    public void Deactivate()
    {
        layer.enabled = false;
        neuron.enabled = false;
        value.enabled = false;
        bias.enabled = false;
        controls.enabled = false;
    }

    public void Activate()
    {
        layer.enabled = true;
        neuron.enabled = true;
        value.enabled = true;
        bias.enabled = true;
        controls.enabled = true;
    }

    public void UpdateText(int layer, int neuron, float value, float bias)
    {
        this.layer.text = "Layer: " + layer.ToString();
        this.neuron.text = "Neuron: " + neuron.ToString();
        this.value.text = "Value: " + value.ToString();
        this.bias.text = "Bias: " + bias.ToString();
    }
}
