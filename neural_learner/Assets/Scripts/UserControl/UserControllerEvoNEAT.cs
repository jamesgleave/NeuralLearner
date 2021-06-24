using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserControllerEvoNEAT : MonoBehaviour
{
    // Used as a baseline
    public float lookSpeedH = 2f;
    public float lookSpeedV = 2f;
    public float zoomSpeed = 2f;
    public float dragSpeed = 6f;

    // The controller used to control the HUD
    public TextController text;
    //public NeuralNetworkUIController GUI;

    // The true values used to control the camera
    private float zoom_speed;
    private float drag_speed;

    public DisplayEvoNEAT nn;

    [Header("Activates editor mode")]
    public bool editor = false;

    // The neuron we have currently selected
    public GameObject selected;

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
                NEATDisplayNeuron n = selected.GetComponent<NEATDisplayNeuron>();
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
            RaycastHit2D hit = Physics2D.CircleCast(Camera.main.ScreenToWorldPoint(mousePos), 0.25f, transform.forward);

            if (hit.collider != null)
            {
                // If we have something selected then deselect it
                if (selected != null)
                {
                    selected.GetComponent<NEATDisplayNeuron>().Deselect();
                }

                // Set the selected
                selected = hit.collider.gameObject;

                // Get the neuron object
                var n = hit.collider.gameObject.GetComponent<NEATDisplayNeuron>();

                // Select the neuron
                n.Select();


                // Get every artist and fade the ones not connected
                var to_not_fade = n.GetAllConnectedArtists();
                foreach (var x in nn.GetAllNeurons())
                {
                    foreach (var a in x.artists)
                    {
                        if (!to_not_fade.Contains(a))
                        {
                            a.Fade(false);
                        }
                        else
                        {
                            a.Fade(true);
                        }
                    }
                }

                // Only fade out in control mode
                if (editor == false)
                {
                    // Isolate it and unfade
                }
            }
            else
            {

                // If we have something selected then deselect it
                if (selected != null)
                {
                    selected.GetComponent<NEATDisplayNeuron>().Deselect();
                }

                //FadeInAll();
                selected = null;

                // Fade in all weights
                foreach (var x in nn.GetAllNeurons())
                {
                    x.FadeInWeights();
                }
            }
        }
    }

    void WatchHotKeys()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GetComponent<SimulationSceneManager>().ToSim();
        }
    }

    void CalculateCamera()
    {
        float relative_scale = Mathf.Max(Mathf.Abs(transform.position.x / 100), 1);
        zoom_speed = zoomSpeed * relative_scale;
        drag_speed = dragSpeed * relative_scale;
    }

    private void OnDrawGizmos()
    {
        // Lets check if we selected anything
        var mousePos = Input.mousePosition;
        mousePos.z = 500;
        var pos = Camera.main.ScreenToWorldPoint(mousePos);
        Gizmos.DrawSphere(pos, 1);
    }
}
