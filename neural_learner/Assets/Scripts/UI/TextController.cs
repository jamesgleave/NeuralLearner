using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextController : MonoBehaviour
{

    public Text layer, neuron, value, bias, controls;
    public Camera target_cam;


    // Start is called before the first frame update
    void Start()
    {

    }

    public void Deactivate()
    {

    }

    public void Activate()
    {

    }

    public void UpdateText(int layer, int neuron, float value, float bias)
    {

    }

    public void Update()
    {
        // If the selected agent is not null, then follow it around
        if (StateManager.selected_agent != null)
        {
            // Set the position 
            target_cam.transform.position = StateManager.selected_agent.transform.position + Vector3.forward * -5;
        }
    }
}
