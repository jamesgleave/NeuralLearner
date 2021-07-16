using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserControllerEvoSim : MonoBehaviour
{
    // For movement
    public float drag_speed = 2;

    // For selecting interactable
    public GameObject selected;
    public BaseAgent most_recently_selected_agent;

    // The origin of our drag
    private Vector3 dragOrigin;

    // The Agent GUI
    public GameObject gui;
    public GameObject save_gui;
    public GameObject load_gui;

    // Help from: https://forum.unity.com/threads/click-drag-camera-movement.39513/
    // Thx :)
    void Update()
    {
        // Check to see if we transition a scene
        if (Input.GetKeyDown(KeyCode.Return) && load_gui.activeInHierarchy == false)
        {
            // Check the type of interactable we have selected
            if (selected != null && selected.transform.parent.TryGetComponent<BaseAgent>(out BaseAgent a) && !gui.activeInHierarchy)
            {
                // Set the most recently selected agent
                most_recently_selected_agent = a;

                // Give the gui this user (for scene control)
                gui.GetComponent<AgentUIController>().SetUser(this);

                // If it is a base agent...
                gui.GetComponent<AgentUIController>().Setup(a);

                gui.GetComponent<AgentUIController>().SetupBarChart();

                // Turn on GUI
                gui.SetActive(true);
            }
            else if (gui.activeInHierarchy)
            {
                // Turn off GUI
                gui.SetActive(false);
            }
        }

        // If the selected thing is not null then follow it
        if (selected != null)
        {
            // Follow the selected thing!
            float z = transform.position.z;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(selected.transform.position.x, selected.transform.position.y, z), (Vector2.Distance(transform.position, selected.transform.position) + drag_speed) * Time.deltaTime);
        }

        // If the gui is open, return after this point
        if (gui != null && (gui.activeInHierarchy || save_gui.activeInHierarchy || load_gui.activeInHierarchy))
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) && gui != null && gui.activeInHierarchy == false)
        {
            CheckSelect();
        }

        // Zoom in and out with Mouse Wheel
        transform.Translate(0, 0, Input.GetAxis("Mouse ScrollWheel") * drag_speed, Space.Self);

        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        // Return if we have not clicked at this point
        if (!Input.GetMouseButton(0)) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = new Vector3(pos.x * drag_speed, pos.y * drag_speed, 0);
        transform.Translate(move, Space.World);
    }

    bool CheckSelect()
    {
        var mousePos = Input.mousePosition;
        mousePos.z = -transform.position.z; // select distance = 10 units from the camera
        RaycastHit2D hit = Physics2D.CircleCast(Camera.main.ScreenToWorldPoint(mousePos), 0.1f, transform.forward);
        if (hit.collider != null)
        {
            selected = hit.collider.gameObject;
        }
        else
        {
            selected = null;
        }
        return selected != null;
    }

    public void MoveToBrain()
    {
        GetComponent<SimulationSceneManager>().ToNeural(selected, most_recently_selected_agent, most_recently_selected_agent.manager, gui);
    }

    public void MoveToAncestory()
    {
        GetComponent<SimulationSceneManager>().ToAncestory(selected, most_recently_selected_agent, most_recently_selected_agent.manager, gui);
    }

    private void OnDrawGizmos()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        Gizmos.DrawRay(mousePos2D, Vector2.zero);
    }
}
