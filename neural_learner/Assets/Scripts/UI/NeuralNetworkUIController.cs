using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NeuralNetworkUIController : MonoBehaviour
{
    [Header("GUIs")]
    public GameObject neuronGUI;
    public GameObject networkGUI;

    [Header("Values For Neurons")]
    public Text activation;
    public Text value_value;
    public Text bias_value;
    public Text connections_value;
    public Text children_value;

    [Header("Values For Neural Network")]
    public Text layers_value;
    public Text neurons_value;
    public Text complexity_value;
    public Text total_mutations_value;
    public Text code_value;

    [Header("Buttons")]
    public Button add_neuron;
    public Button remove_neuron;
    public Button add_layer;
    public Button remove_layer;
    public Button mutate;
    public Button editor;

    [Header("Manager")]
    public DisplayEvoNN display;
    public UserControllerEvoNN user;

    // Used to store the selected neuron
    private Neuron most_recent_selected;


    public void Start()
    {
        editor.onClick.AddListener(EditorButton);
    }

    public void Update()
    {
        // Toggle the menu
        if (Input.GetKeyDown(KeyCode.M))
        {
            networkGUI.SetActive(!networkGUI.activeInHierarchy);
            if (networkGUI.activeInHierarchy)
            {
                user.editor = false;
            }
        }

        // If we have something selected then enable the neuron gui
        neuronGUI.SetActive(user.selected != null);
        if (user.selected != null)
        {
            activation.text = user.selected.GetComponent<Neuron>().activation;
            value_value.text = user.selected.GetComponent<Neuron>().value.ToString();
            bias_value.text = user.selected.GetComponent<Neuron>().bias.ToString();
            connections_value.text = "ph";
            children_value.text = user.selected.GetComponent<Neuron>().children.Count.ToString();

            // Update the most recent selection
            most_recent_selected = user.selected.GetComponent<Neuron>();
        }

        // Update only if the model is present
        if (display.model != null)
        {
            // Update values of the NN
            layers_value.text = display.model.GetLayers().Length.ToString();
            neurons_value.text = "ph";
            complexity_value.text = "ph";
            total_mutations_value.text = "ph";
            code_value.text = display.model.GetCode();
        }
    }

    public void EditorButton()
    {
        user.editor = true;
        networkGUI.SetActive(!networkGUI.activeInHierarchy);
    }
}
