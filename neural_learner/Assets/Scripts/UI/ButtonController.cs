using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonController : MonoBehaviour
{

    public UserControllerEvoNN display;
    public Button add_neuron;

    void Start()
    {
        Button b1 = add_neuron.GetComponentInChildren<Button>();
        b1.onClick.AddListener(AddNeuron);
    }

    void AddNeuron()
    {
        display = display.gameObject.GetComponent<UserControllerEvoNN>();
        display.nn.AddNeuron((int)display.selected.GetComponent<Neuron>().layer, (int)display.selected.GetComponent<Neuron>().position);
    }
}

