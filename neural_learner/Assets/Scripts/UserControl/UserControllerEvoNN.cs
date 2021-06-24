using UnityEngine;
using System.Collections.Generic;

public class UserControllerEvoNN : MonoBehaviour
{

    // Used as a baseline
    public float lookSpeedH = 2f;
    public float lookSpeedV = 2f;
    public float zoomSpeed = 2f;
    public float dragSpeed = 6f;

    // The controller used to control the HUD
    public TextController text;
    public NeuralNetworkUIController GUI;


    // The true values used to control the camera
    private float look_speed_H;
    private float look_speed_V;
    private float zoom_speed;
    private float drag_speed;

    public DisplayEvoNN nn;

    [Header("Activates editor mode")]
    public bool editor = false;

    // The neuron we have currently selected
    public GameObject selected;

    void Start()
    {

    }

    void Update()
    {
        if (editor == false)
        {
            ControlMode();
        }
        else
        {
            EditorMode();
        }
    }

    public void EditorMode()
    {
        // Force the lines when in editor mode
        nn.ForceDrawLines();

        // If we have something selected then...
        if (selected != null)
        {
            // Grab the neuron object
            Neuron n = selected.GetComponent<Neuron>();

            // Draw a line from the neuron to the mouse
            float distance_to_screen = Camera.main.WorldToScreenPoint(selected.transform.position).z;
            Vector2 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen));

            // Use the edit pen to draw our line and change the value of the weight using the mousewheel
            float delta_weight_value = Input.mouseScrollDelta.y;
            n.editor_value -= delta_weight_value / 100;

            // Get the scale of the weight
            float scale = Mathf.Abs(n.editor_value) * n.line_scaler;

            // Clamp the scale 
            scale = Mathf.Clamp(scale, n.min_line_width, n.line_scaler);

            // Set the pen line up 
            n.editor_pen.SetFeatures(scale, scale);
            n.editor_pen.SetColour(n.editor_value);
            n.editor_pen.SimpleLine(pos, n.editor_pen.transform.position);
        }
        else
        {
            //Zoom in and out with Mouse Wheel if we dont have anything selected
            transform.Translate(0, 0, Input.GetAxis("Mouse ScrollWheel") * zoom_speed, Space.Self);
        }

        // Calculates the speed for camera movement given how far you are from the origin
        CalculateCamera();

        // Controll the camera position
        //drag camera around with right Mouse
        if (Input.GetMouseButton(0) && selected == null)
        {
            // Move 
            transform.Translate(-Input.GetAxisRaw("Mouse X") * Time.deltaTime * drag_speed, -Input.GetAxisRaw("Mouse Y") * Time.deltaTime * drag_speed, 0);
            // Check if we have pressed on a neuron
            DetectNeuronPressed();
        }
        // If we have a selected neuron and click another then we connect them
        else if (Input.GetMouseButton(0) && selected != null)
        {
            // The currently selected neuron
            Neuron current_selected = selected.GetComponent<Neuron>();

            // Check if we click
            DetectNeuronPressed();

            // If we have selected another neuron...
            if (selected != null)
            {
                // Get the one we just clicked
                Neuron next_selected = selected.GetComponent<Neuron>();

                // connect the weights
                if (next_selected != current_selected && current_selected.children.Contains(next_selected))
                {
                    // The index of where the weights are in relation to the currently selected value
                    int index = current_selected.children.IndexOf(next_selected);
                    current_selected.weights[index] = current_selected.editor_value;

                    // Deselect the selected
                    selected = null;
                }
            }

            // Clear the pen
            current_selected.editor_pen.Clear();
        }
    }

    public void ControlMode()
    {
        // Calculates the speed for camera movement given how far you are from the origin
        CalculateCamera();

        // Update the mouse coords
        float distance_to_screen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        Vector3 mouse = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen));

        // Check if we select a neuron
        DetectNeuronPressed();

        // Controll the camera position
        MouseController();

        // Check for hotkey
        WatchHotKeys();

        if (text)
            if (selected != null)
            {
                // Set the text
                Neuron n = selected.GetComponent<Neuron>();
                text.UpdateText((int)n.layer, (int)n.position, n.value, n.bias);

                // Turn on the text
                text.Activate();
            }
            else
            {
                // Turn off the text
                text.Deactivate();
            }
    }

    void MouseController()
    {

        //drag camera around with right Mouse
        if (Input.GetMouseButton(0) && selected == null)
        {
            transform.Translate(-Input.GetAxisRaw("Mouse X") * Time.deltaTime * drag_speed, -Input.GetAxisRaw("Mouse Y") * Time.deltaTime * drag_speed, 0);
        }
        else if (Input.GetMouseButton(0) && selected != null)
        {
            // If we have something selected clicked we can drag it around lol
            float distance_to_screen = Camera.main.WorldToScreenPoint(selected.transform.position).z;
            selected.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen));
        }

        //Zoom in and out with Mouse Wheel
        transform.Translate(0, 0, Input.GetAxis("Mouse ScrollWheel") * zoom_speed, Space.Self);
    }

    void DetectNeuronPressed()
    {
        // If we click and the gui is not on (to avoid deselecting before button press)
        if (Input.GetMouseButtonDown(0))
        {
            // Lets check if we selected anything
            var mousePos = Input.mousePosition;
            mousePos.z = -transform.position.z; // select distance = 10 units from the camera
            RaycastHit2D hit = Physics2D.CircleCast(Camera.main.ScreenToWorldPoint(mousePos), 1, transform.forward);
            if (hit.collider != null)
            {
                print(hit.collider.name);
                // If we have something selected then deselect it
                if (selected != null)
                {
                    selected.GetComponent<Neuron>().Deselect();
                }

                // Set the selected
                selected = hit.collider.gameObject;

                // Get the neuron object
                var n = hit.collider.gameObject.GetComponent<Neuron>();

                // Select the neuron
                n.Select();

                // Only fade out in control mode
                if (editor == false)
                {
                    // Isolate it and unfade
                    IsolateWeights();
                }
            }
            else
            {
                FadeInAll();
                selected = null;
            }
        }
    }

    void FadeInAll()
    {
        // Now deactivate all of the next layers as well
        for (int i = 0; i < nn.neurons.Count; i++)
        {
            // Look at each neuron
            foreach (GameObject n in nn.neurons[i])
            {
                // Look at each aritst of each future neuron
                foreach (var artist in n.GetComponent<Neuron>().artists)
                {
                    artist.Fade(fade_in: true);
                }
            }
        }
    }

    void IsolateWeights()
    {
        // The list of all neurons connected the selected neuron
        List<Neuron> connected_too = new List<Neuron>();

        // The selected neuron
        Neuron s = selected.GetComponent<Neuron>();
        // The layer of the selected neuron
        int selected_layer = (int)s.layer;

        // Add the neuron s to the connected to list since it should be isolated as well
        connected_too.Add(s);

        for (int layer = (int)(selected_layer - 1f); layer >= 0; layer--)
        {
            // Get all the neurons in the layer
            List<GameObject> neuron_layer = nn.neurons[layer];

            // Look at each neuron in that layer
            foreach (GameObject neuron_gameobj in neuron_layer)
            {
                // Get the neuron component
                Neuron n = neuron_gameobj.GetComponent<Neuron>();

                // Look at each child of n
                int n_index = 0;
                foreach (Neuron child in n.children)
                {
                    if (n.weights[n_index] * n.weights[n_index] > 0 && connected_too.Contains(child))
                    {
                        // Fade it in if it is not already
                        n.artists[n_index].Fade(fade_in: true);

                        // Add to the connected to list
                        connected_too.Add(n);
                    }
                    else
                    {
                        // Fade out the others
                        n.artists[n_index].Fade(fade_in: false);
                    }
                    n_index++;
                }
            }
        }

        // Now deactivate all of the next layers as well
        for (int i = selected_layer; i < nn.neurons.Count; i++)
        {
            // Look at each neuron
            foreach (GameObject n in nn.neurons[i])
            {
                // Look at each aritst of each future neuron
                foreach (var artist in n.GetComponent<Neuron>().artists)
                {
                    artist.Fade(fade_in: false);
                }
            }
        }
    }

    void WatchHotKeys()
    {
        if (Input.GetKeyDown(KeyCode.Return) && selected != null)
        {
            // Insert a neuron 
            nn.AddNeuron((int)selected.GetComponent<Neuron>().GetInfo()["layer"] - 1, (int)selected.GetComponent<Neuron>().GetInfo()["position"]);
        }

        if (Input.GetKeyDown(KeyCode.Backspace) && selected != null && nn.neurons[(int)selected.GetComponent<Neuron>().GetInfo()["layer"]].Count > 1)
        {
            // Delete and select new neuron
            int pos = (int)selected.GetComponent<Neuron>().GetInfo()["position"];
            int layer = (int)selected.GetComponent<Neuron>().GetInfo()["layer"];
            nn.RemoveNeuron(layer - 1, pos);

            // Seleect a new neuron
            selected = nn.neurons[layer][(int)nn.neurons[layer].Count / 2];
            selected.GetComponent<Neuron>().Select();
        }
        else if (nn != null && selected != null && nn.neurons[(int)selected.GetComponent<Neuron>().GetInfo()["layer"]].Count == 1 && Input.GetKeyDown(KeyCode.Backspace))
        {
            // If we have selected a layer/neuron, and we try to delete the last one, delete the layer!
            int layer = (int)selected.GetComponent<Neuron>().GetInfo()["layer"];
            nn.RemoveLayer(layer);
            print(nn.model.GetCode());
        }

        if (Input.GetKeyDown(KeyCode.L) && selected != null)
        {
            // Delete and select new neuron
            int layer = (int)selected.GetComponent<Neuron>().GetInfo()["layer"];
            nn.AddLayer(layer, 3);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            var agent = nn.learner.GetComponent<BaseAgent>();
            print(nn.Mutate(agent.genes.base_mutation_rate, agent.genes.weight_mutation_prob, agent.genes.neuro_mutation_prob, agent.genes.base_mutation_rate, agent.genes.dropout_prob));
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GetComponent<SimulationSceneManager>().ToSim();
        }
    }

    void CalculateCamera()
    {
        float relative_scale = Mathf.Max(Mathf.Abs(transform.position.x / 100), 1);
        zoom_speed = zoomSpeed * relative_scale;
        drag_speed = dragSpeed * relative_scale;
        look_speed_H = lookSpeedH * relative_scale;
        look_speed_V = lookSpeedV * relative_scale;
    }

    private void OnDrawGizmos()
    {
        var mousePos = Input.mousePosition;
        Gizmos.DrawCube(Camera.main.ScreenToWorldPoint(mousePos) - Vector3.up * 1000, Vector3.one * 10);
    }
}