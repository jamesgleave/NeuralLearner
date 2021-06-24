using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadAgentController : MonoBehaviour
{

    private Dropdown dropdown;
    private Saving save;
    public DisplayWobbit dw;

    // Start is called before the first frame update
    void Start()
    {
        save = GetComponent<Saving>();
    }

    // Update is called once per frame
    void Update()
    {
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
}
