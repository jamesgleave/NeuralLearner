using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveAgentGUI : MonoBehaviour
{
    public InputField input_field;
    public Button save_button;
    public Button cancel_button;

    public void Start()
    {
        // Setup the buttons
        cancel_button.onClick.AddListener(Cancel);
        save_button.onClick.AddListener(Save);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {

            if (input_field.text.Length < 0)
            {
                return;
            }

            Saving.SaveAgent(GameObject.Find("User").GetComponent<UserControllerEvoSim>().most_recently_selected_agent, Saving.path + "/" + input_field.text);
            this.gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            this.gameObject.SetActive(false);
        }

        GetComponentInChildren<DisplayWobbit>().Setup(GameObject.Find("User").GetComponent<UserControllerEvoSim>().most_recently_selected_agent.genes);
    }

    public void Save()
    {
        if (input_field.text.Length < 0)
        {
            return;
        }

        Saving.SaveAgent(GameObject.Find("User").GetComponent<UserControllerEvoSim>().most_recently_selected_agent, Saving.path + "/" + input_field.text);
        this.gameObject.SetActive(false);
    }

    public void Cancel()
    {
        this.gameObject.SetActive(false);
    }
}
