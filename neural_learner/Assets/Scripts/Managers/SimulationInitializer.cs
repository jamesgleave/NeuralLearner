using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationInitializer : MonoBehaviour
{
    private void Start()
    {
        if (StateManager.manager != null)
        {
            // Move the manager from the saved state to our scene
            SimulationSceneManager.Move(StateManager.manager.gameObject);

            // Destroy the manager in this scene
            Destroy(GameObject.FindGameObjectWithTag("manager").gameObject);

            // Activate the manager
            StateManager.manager.gameObject.SetActive(true);

            // Move the manager to this scene
            StateManager.manager.transform.SetParent(gameObject.transform);

            // Reselect the interactable
            Camera.main.GetComponent<UserControllerEvoSim>().selected = StateManager.interactable;
            Camera.main.transform.position = StateManager.camera_position;

            // Resume as normal...
        }
        else
        {
            GameObject.FindGameObjectWithTag("manager").GetComponent<Manager>().Setup();
        }
    }
}
