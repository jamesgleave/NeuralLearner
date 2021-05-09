using UnityEngine;
using System.Collections;

public class UserControllerEvoNN : MonoBehaviour
{

    // Used as a baseline
    public float lookSpeedH = 2f;
    public float lookSpeedV = 2f;
    public float zoomSpeed = 2f;
    public float dragSpeed = 6f;

    // The controller used to control the HUD
    public TextController text;


    // The true values used to control the camera
    private float look_speed_H;
    private float look_speed_V;
    private float zoom_speed;
    private float drag_speed;

    public DisplayEvoNN nn;

    private Camera camera;
    private float yaw = 90f;
    private float pitch = 0f;

    // The mouse coords
    [SerializeField]
    private Vector3 mouse;

    // The neuron we have currently selected
    public GameObject selected;

    void Start()
    {
        camera = GetComponent<Camera>();
    }

    void MouseController()
    {
        //Look around with left Mouse
        if (Input.GetMouseButton(1))
        {
            yaw += look_speed_H * Input.GetAxis("Mouse X");
            pitch -= look_speed_V * Input.GetAxis("Mouse Y");

            transform.eulerAngles = new Vector3(pitch, yaw, 0f);
        }

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
        if (Input.GetMouseButtonDown(0))
        {
            // if left button pressed...
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // the object identified by hit.transform was clicked
                // do whatever you want
                var n = hit.collider.gameObject.GetComponent<Neuron>();
                var info = n.GetInfo();

                print(info["layer"] + ", " + info["value"] + ", " + info["bias"]);
                //nn.AddNeuron((int)info["layer"] - 1, (int)info["position"]);

                // If we have something selected then deselect it
                if (selected != null)
                {
                    selected.GetComponent<Neuron>().Deselect();
                }

                // Select the gameobject that the ray collided with
                n.Select();
                selected = n.gameObject;
            }
            else if (selected != null)
            {
                selected.GetComponent<Neuron>().Deselect();
                selected = null;
            }
        }
    }

    void Update()
    {

        // Calculates the speed for camera movement given how far you are from the origin
        CalculateCamera();

        // Update the mouse coords
        float distance_to_screen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        mouse = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen));

        // Check if we select a neuron
        DetectNeuronPressed();

        // Controll the camera position
        MouseController();

        // Check for hotkey
        WatchHotKeys();

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
        else if (selected != null && nn.neurons[(int)selected.GetComponent<Neuron>().GetInfo()["layer"]].Count == 1 && Input.GetKeyDown(KeyCode.Backspace))
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

            Model.NeuralNet.MutateWeights(nn.model, 0.1f, 0.01f);
            nn.ForceDrawLines();

            if (Random.value < 0.3f)
            {
                int where = Random.Range(0, nn.model.GetLayers().Length);
                int pos = Random.Range(0, nn.model.GetLayers()[where].neurons.Count);
                nn.AddNeuron(where, pos);
            }

            if (Random.value < 0.03f)
            {
                int where = Random.Range(0, nn.model.GetLayers().Length);
                int pos = Random.Range(0, nn.model.GetLayers()[where].neurons.Count);
                if (nn.model.GetLayers()[where].neurons.Count > 1)
                {
                    nn.RemoveNeuron(where, pos);
                }
            }
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
}