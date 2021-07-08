using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadAgentController : MonoBehaviour
{

    private Dropdown dropdown;
    private Saving save;
    public DisplayWobbit dw;

    public Button cancel_button;
    public Button load_button;

    // Start is called before the first frame update
    void Start()
    {
        save = GetComponent<Saving>();

        // Setup the buttons
        cancel_button.onClick.AddListener(Cancel);
        load_button.onClick.AddListener(Load);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            this.gameObject.SetActive(false);
            dropdown = null;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Saving.LoadAgent(GameObject.Find("User").GetComponent<UserControllerEvoSim>().most_recently_selected_agent, Saving.path + "/" + dropdown.options[dropdown.value].text);
            dropdown = null;
            this.gameObject.SetActive(false);
        }

        if (dropdown == null)
        {
            dropdown = GetComponentInChildren<Dropdown>();
            dropdown.ClearOptions();
            List<string> opts = new List<string>();
            foreach (string name in save.agents.Keys)
            {
                opts.Add(name);
            }

            dropdown.AddOptions(opts);
        }

        dw.Setup(Saving.LoadGenes("./Assets/SaveData/" + dropdown.options[dropdown.value].text + "/genes.txt"));
    }

    /// <summary>
    /// Loads the selected agent
    /// </summary>
    public void Load()
    {
        Saving.LoadAgent(GameObject.Find("User").GetComponent<UserControllerEvoSim>().most_recently_selected_agent, Saving.path + "/" + dropdown.options[dropdown.value].text);
        dropdown = null;
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Cancel the load screen
    /// </summary>
    public void Cancel()
    {
        this.gameObject.SetActive(false);
        dropdown = null;
    }
}
