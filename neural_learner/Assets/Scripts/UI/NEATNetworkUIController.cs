using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NEATNetworkUIController : MonoBehaviour
{
    [Header("GUIs")]
    public GameObject neuronGUI;
    public GameObject networkGUI;

    [Header("Values For Neurons")]
    public Text activation;
    public Text neuron_name;
    public Text value_value;
    public Text bias_value;
    public Text connections_value;
    public Text children_value;

    [Header("Values For Neural Network")]
    public Text depth;
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
    public DisplayEvoNEAT display;
    public UserControllerEvoNEAT user;

    // Used to store the selected neuron
    private NEATDisplayNeuron most_recent_selected;

    public void Start()
    {
        //editor.onClick.AddListener(EditorButton);
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
            activation.text = user.selected.GetComponent<NEATDisplayNeuron>().activation;
            neuron_name.text = user.selected.GetComponent<NEATDisplayNeuron>().io_name;
            value_value.text = user.selected.GetComponent<NEATDisplayNeuron>().value.ToString();
            bias_value.text = user.selected.GetComponent<NEATDisplayNeuron>().bias.ToString();
            connections_value.text = (user.selected.GetComponent<NEATDisplayNeuron>().children.Count + user.selected.GetComponent<NEATDisplayNeuron>().parents.Count).ToString();
            children_value.text = user.selected.GetComponent<NEATDisplayNeuron>().children.Count.ToString();

            // Update the most recent selection
            most_recent_selected = user.selected.GetComponent<NEATDisplayNeuron>();
        }

        // Update only if the model is present
        if (display.model != null)
        {
            // Update depth of the NN
            int max_d_key = 0;
            foreach (int d in display.depth_counter.Keys)
            {
                max_d_key = Mathf.Max(d, max_d_key);
            }
            depth.text = max_d_key.ToString();

            neurons_value.text = display.GetAllNeurons().Count.ToString();
            complexity_value.text = "ph";
            total_mutations_value.text = "ph";
            code_value.text = "";
        }
    }

    public void EditorButton()
    {
        user.editor = true;
        networkGUI.SetActive(!networkGUI.activeInHierarchy);
    }
}
