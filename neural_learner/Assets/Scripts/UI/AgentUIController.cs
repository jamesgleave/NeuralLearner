using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentUIController : MonoBehaviour
{
    // The list of text values
    public GameObject display_box;
    public List<string> state_display_values;

    // The agent we are looking at
    public BaseAgent agent;

    // The scene manager that belongs to the simulation
    public UserControllerEvoSim user;

    // The two buttons for control
    public Button to_brain;
    public Button to_tree;

    // Buttons to save and load
    public Button save_agent;
    public Button load_agent;
    public GameObject save_agent_gui;
    public GameObject load_agent_gui;

    // The bar chart for the genes
    public GameObject bar_chart;

    public Camera rendercam;

    // Start is called before the first frame update
    void Start()
    {
        // Setup the brain button
        to_brain.onClick.AddListener(OnClickBrain);
        // Setup the tree button
        to_tree.onClick.AddListener(OnClickTree);

        // Setup load and save
        save_agent.onClick.AddListener(SaveAgent);
        load_agent.onClick.AddListener(LoadAgent);
    }

    // Update is called once per frame
    void Update()
    {

        if (agent != null)
        {
            // First, we setup again
            Setup(agent);

            // Update all values
            int i = 0;
            foreach (var t in display_box.GetComponentsInChildren<Text>())
            {
                // Only take the first 5 values
                string feild = state_display_values[i];

                // Here we will check to see if we have a prefix % to denote a percentage
                if (feild.Contains("%"))
                {

                    // If it is a number than pad it
                    if (feild.Contains("."))
                    {
                        feild = feild.PadRight(10, '0');
                    }

                    // Get the first 5 chars
                    feild = feild.Substring(0, Mathf.Min(feild.Length, 6));
                    feild = feild.Substring(1);
                    feild += "%";
                }
                else if (feild.Contains("/"))
                {

                }
                else
                {

                    // If it is a number than pad it
                    if (feild.Contains("."))
                    {
                        feild = feild.PadRight(10, '0');
                    }

                    // Get the first 5 chars
                    feild = feild.Substring(0, Mathf.Min(feild.Length, 6));
                }

                // We take the first 5 chars
                t.text = feild;
                i++;

                // Set the position of the render camera
                rendercam.transform.position = agent.transform.position + Vector3.forward * agent.max_size.x;
            }
        }
    }

    void OnClickBrain()
    {
        // Move to the neural view
        user.MoveToBrain();
    }

    void OnClickTree()
    {
        user.MoveToAncestory();
    }

    public void SetUser(UserControllerEvoSim u)
    {
        user = u;
    }

    public void Setup(BaseAgent a)
    {
        // Set the agent
        agent = a;

        // Setup all values
        state_display_values.Clear();

        // Energy
        int display_size = Mathf.Min(agent.energy.ToString().Length, 5);
        try
        {
            state_display_values.Add(agent.energy.ToString().Substring(0, display_size) + "/" + (agent.max_energy * 2f).ToString().Substring(0, display_size));
        }
        catch
        {
            state_display_values.Add(agent.energy.ToString() + "/" + (agent.max_energy * 2f).ToString());
        }
        state_display_values.Add(agent.true_metabolic_cost.ToString());

        // Setup the diet
        string diet;
        if (agent.genes.diet > 0.5f)
        {
            diet = "Meat";
        }
        else
        {
            diet = "Pellet";
        }
        state_display_values.Add(diet);

        // The health is displayed in a specific way like the energy (current/max)
        display_size = Mathf.Min(agent.health.ToString().Length, 5);
        state_display_values.Add(agent.health.ToString().Substring(0, display_size) + "/" + (agent.max_health).ToString().Substring(0, display_size));

        // Attack and defense
        state_display_values.Add(agent.attack.ToString());
        state_display_values.Add(agent.defense.ToString());

        // Speed
        state_display_values.Add(agent.speed.ToString());

        // Decisions
        state_display_values.Add(agent.wants_to_eat.ToString());
        state_display_values.Add(agent.wants_to_grab.ToString());
        state_display_values.Add(agent.wants_to_attack.ToString());
        state_display_values.Add(agent.wants_to_breed.ToString());

        // Age stuff
        state_display_values.Add(agent.age.ToString());
        state_display_values.Add(agent.lifespan.ToString());
        state_display_values.Add(agent.maturity_age.ToString());

        // What generation is this agent in its species?
        state_display_values.Add(agent.generation.ToString());

        // How many others of this agent's species are there?
        state_display_values.Add((-1).ToString());

        // How many eggs they have layed
        state_display_values.Add(agent.eggs_layed.ToString());

        // Setup the name
        transform.Find("Name Title").GetComponent<Text>().text = agent.GetRawName();
    }

    public void SetupBarChart()
    {
        List<string> gl = new List<string>();

        gl.Add("Base-Mutation Rate");
        gl.Add("Colour-Mutation Probability");
        gl.Add("Attribute-Mutation Rate");
        gl.Add("Neuro-Mutation Probability");
        gl.Add("Weight-Mutation Probability");
        gl.Add("Bias-Mutation Probability");
        gl.Add("Dropout Probability");

        gl.Add("Speed");
        gl.Add("Diet");
        gl.Add("Attack");
        gl.Add("Defense");
        gl.Add("Vitality");
        gl.Add("Size");
        gl.Add("Perception");

        gl.Add("Clockrate");
        gl.Add("Gestation Time");
        gl.Add("Maturity Time");
        gl.Add("Colour-R");
        gl.Add("Colour-G");
        gl.Add("Colour-B");
        bar_chart.GetComponent<GeneBarChart>().Setup(gl.ToArray(), agent.genes.GetGeneList());
    }

    public void SaveAgent()
    {
        save_agent_gui.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
        save_agent_gui.gameObject.SetActive(true);
    }

    public void LoadAgent()
    {

        load_agent.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
        load_agent.gameObject.SetActive(true);

    }
}
