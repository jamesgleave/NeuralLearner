using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveAgentGUI : MonoBehaviour
{
    public InputField input_field;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Saving.SaveAgent(GameObject.Find("User").GetComponent<UserControllerEvoSim>().most_recently_selected_agent, Saving.path + input_field.text);
            print(GameObject.Find("User").GetComponent<UserControllerEvoSim>().most_recently_selected_agent);
            print(Saving.path + input_field.text);
            this.gameObject.SetActive(false);
        }
    }
}
