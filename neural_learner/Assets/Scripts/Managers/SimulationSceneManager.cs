using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimulationSceneManager : MonoBehaviour
{
    public void ToSim()
    {
        SceneManager.LoadScene("TestInteractables + UI");

        // Activate the manager if it is not active 
        if (!StateManager.manager.gameObject.activeInHierarchy)
        {
            // Set the manager to inactive for now 
            StateManager.manager.gameObject.SetActive(true);
        }
    }

    public void ToAncestory(GameObject agent_obj, BaseAgent agent, Manager manager, GameObject gui)
    {
        // Save the values to the state manager
        StateManager.interactable = agent_obj;
        StateManager.selected_agent = agent;
        StateManager.manager = manager;
        StateManager.ancestor_manager = manager.anc_manager;
        StateManager.camera_position = Camera.main.transform.position;

        // Detatch the manager from the parent
        manager.transform.SetParent(null);

        // Move the manager to the "Do not destroy scene"
        DontDestroyOnLoad(manager);

        // Move the gui to the "Do not destroy scene"

        // Move scenes
        SceneManager.LoadScene("TestAncestory");

        // Set the manager to inactive for now 
        StateManager.manager.gameObject.SetActive(false);
    }

    public void ToNeural(GameObject agent_obj, BaseAgent agent, Manager manager, GameObject gui)
    {
        // Save the values to the state manager
        StateManager.interactable = agent_obj;
        StateManager.selected_agent = agent;
        StateManager.manager = manager;
        StateManager.ancestor_manager = manager.anc_manager;
        StateManager.camera_position = Camera.main.transform.position;

        // Detatch the manager from the parent
        manager.transform.SetParent(null);

        // Move the manager to the "Do not destroy scene"
        DontDestroyOnLoad(manager);

        // Move scenes
        // TODO this is temporary to test NEAT
        if (agent.TryGetComponent<EvolutionaryNEATLearner>(out EvolutionaryNEATLearner learner))
        {
            SceneManager.LoadScene("NEATVisualization");
        }
        else
        {
            SceneManager.LoadScene("TestVisualization + UI");
        }

        // Set the manager to inactive for now 
        //StateManager.manager.gameObject.SetActive(false);
    }

    public static void Move(GameObject obj)
    {
        // Move an object to the current scene
        SceneManager.MoveGameObjectToScene(obj, SceneManager.GetActiveScene());
    }

}

