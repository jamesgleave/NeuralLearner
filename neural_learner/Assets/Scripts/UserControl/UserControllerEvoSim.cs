using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserControllerEvoSim : MonoBehaviour
{
    // For movement
    public float drag_speed = 2;
    public float zoom_speed = 10;

    // For selecting interactable
    public GameObject selected;
    public BaseAgent most_recently_selected_agent;

    // The origin of our drag
    private Vector3 dragOrigin;

    // The Agent GUI
    public GameObject gui;
    public GameObject save_gui;
    public GameObject load_gui;

    /// <summary>
    /// The time before idle movements begin to play
    /// </summary>
    public int idle_time_threshold = 30;

    /// <summary>
    /// The current idle time
    /// </summary>
    public float idle_time;

    // The control mode of the controller
    public enum ControlMode
    {
        manual, animated
    }
    public ControlMode control_mode = ControlMode.manual;

    /// <summary>
    /// If true, then after 30 seconds (idle_time_threshold) the user goes into idle control
    /// </summary>
    public bool enable_idle;

    /// <summary>
    /// The manager of the scene
    /// </summary>
    private Manager manager;


    // Help from: https://forum.unity.com/threads/click-drag-camera-movement.39513/
    // Thx :)
    void Update()
    {

        // Manage idle time
        ManageIdleTime();

        // If the manager is null, find it!
        if (manager == null)
        {
            manager = GameObject.FindGameObjectWithTag("manager").GetComponent<Manager>();
        }

        // Takes care of everything if we have an interactable selected
        HandleSelection();

        // If the gui is open, return after this point
        if (gui != null && (gui.activeInHierarchy || save_gui.activeInHierarchy || load_gui.activeInHierarchy))
        {
            return;
        }

        // Check to see if we have selected anything new
        if (Input.GetMouseButtonDown(0) && gui != null && gui.activeInHierarchy == false)
        {
            CheckSelect();
        }

        // Follow the selected interactable
        FollowSelected();

        // Takes care of user movement
        HandleUserMovement();

        // Check if we should be automatically controlling
        if (control_mode == ControlMode.animated)
        {
            HandleAutomaticControl();
        }

        // Take care of hotkeys
        HandleHotKeys();
    }

    void HandleHotKeys()
    {
        // Select the fastest agent
        if (Input.GetKeyDown(KeyCode.V) && manager.stat_manager.num_agents > 0)
        {
            selected = manager.GetAgent('v').head;
        }

        // Select the oldest agent
        if (Input.GetKeyDown(KeyCode.O) && manager.stat_manager.num_agents > 0)
        {
            selected = manager.GetAgent('o').head;
        }

        // Select the youngest agent
        if (Input.GetKeyDown(KeyCode.Y) && manager.stat_manager.num_agents > 0)
        {
            selected = manager.GetAgent('y').head;
        }

        // Select the agent with most eggs layed 
        if (Input.GetKeyDown(KeyCode.E) && manager.stat_manager.num_agents > 0)
        {
            selected = manager.GetAgent('e').head;
        }

        // Select random agent
        if (Input.GetKeyDown(KeyCode.R) && manager.stat_manager.num_agents > 0)
        {
            selected = manager.GetAgent('r').head;
        }
    }

    void HandleSelection()
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
    }

    void FollowSelected()
    {
        // If the selected thing is not null then follow it
        if (selected != null)
        {
            // Follow the selected thing!
            float z = transform.position.z;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(selected.transform.position.x, selected.transform.position.y, z), (Vector2.Distance(transform.position, selected.transform.position) + drag_speed) * Time.deltaTime);
        }
    }

    void HandleUserMovement()
    {
        // Zoom in and out with Mouse Wheel
        transform.Translate(0, 0, Input.GetAxis("Mouse ScrollWheel") * zoom_speed * transform.position.z/10f, Space.Self);

        if (Input.GetMouseButtonDown(0))
        {
            // Set the drag origin
            dragOrigin = Input.mousePosition;

            // Upon clicken the mouse button, reset from idle
            ResetFromIdle();

            // Return from the func
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

    void ManageIdleTime()
    {
        // Incrememnt idle time
        idle_time += Time.deltaTime;

        // If we have been idle for longer than the threshold
        if (idle_time > idle_time_threshold && enable_idle)
        {
            control_mode = ControlMode.animated;
        }
        else
        {
            control_mode = ControlMode.manual;
        }
    }

    void ResetFromIdle()
    {
        // If we were idle and click then deselect whatever we had selected
        if (idle_time > idle_time_threshold)
        {
            selected = null;
        }
        idle_time = 0;
        control_mode = ControlMode.manual;
    }

    void HandleAutomaticControl()
    {
        // Check if selected is null or is dead, and if it is, then choose a new one
        bool trigger = 0.01f * Random.value * idle_time > 0.5f;
        if ((selected == null || selected.TryGetComponent<BaseAgent>(out BaseAgent agent) && agent.energy < 0 || trigger) && control_mode != ControlMode.manual)
        {
            selected = manager.GetAgent('r').head;
            idle_time = idle_time_threshold;
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        Gizmos.DrawRay(mousePos2D, Vector2.zero);
    }
}
