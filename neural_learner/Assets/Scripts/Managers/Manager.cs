using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [Header("Food Settings")]

    [Tooltip("Max amount of food to spawn in")]
    public int total_food;

    [Tooltip("The growth rate of the food pellets")]
    public float food_growth_rate;

    //[Tooltip("How tightly the pellets distrobuted")]
    //public float pellet_distrobution;

    //[Tooltip("How many pellet clusters there will be")]
    //public int pellet_clusters;

    [Tooltip("The max food pellet size")]
    public float food_pellet_size;

    [Tooltip("The amount of energy in one pellet")]
    public float food_pellet_energy;

    [Tooltip("The food pellet prefab")]
    public FoodPellet pellet;

    [Tooltip("The manager for the food pellets")]
    public FoodPelletManager food_pellet_manager;

    [Tooltip("The list of all food pelletes present")]
    public List<FoodPellet> food_pellets = new List<FoodPellet>();     // The list of food pellets

    [Header("Simulation Settings")]
    [Tooltip("The manager's update rate in time per update (seconds)")]
    public float update_rate;
    [Tooltip("If true, the agents will be teleported to the other side of the map when moving too far away from the grid. If false, the agents are left to wander the void.")]
    public bool teleport;
    private float clock = 0;

    [Tooltip("The size of our simulation area")]
    public int gridsize;

    //[Header("Adaptive Food Settings")]
    //public bool adaptive_food;
    //public int agent_pellet_threshold;
    [Space]
    [Header("Agent Settings")]
    public BaseAgent agent;
    public Egg egg;

    [Tooltip("The number of initial agents to spawn in.")]
    public float starting_agents;
    [Tooltip("The min number of initial agents in the simulation. Once the number of agents drops below this value, more are spawned in.")]
    public int min_agents;

    [Tooltip("The scale rate for the agent metabolism. Increase this value to decrease the basal metobalolic rate of the agents.")]
    public float metabolism_scale_rate = 3;

    [Tooltip("The scale rate for each agent's movement cost. Increasing this value will reduce the cost of movement for all agents.")]
    public float movement_cost_scale_rate = 500f;

    [Tooltip("Increasing this value decreases the amount of energy is used by the agents brain")]
    public float brain_cost_reduction_factor = 250;

    [Space]
    [Header("Simulation info (readonly)")]
    [Tooltip("The amount of energy present in the simulation")]
    public float energy;
    public float initial_energy;

    [Header("Energy Values")]
    public float energy_in_pellets;
    public float energy_in_agents;
    public float energy_in_ether;
    public float combined;

    [Header("Energy Percentages")]
    public float percent_energy_in_pellets;
    public float percent_energy_in_agents;
    public float percent_energy_in_ether;

    [Header("Statistics")]
    public StatisticManager stat_manager = new StatisticManager();
    public int num_active_food = 0;

    [Header("Ancesory")]
    public AncestorManager anc_manager = new AncestorManager();

    [Header("Spawner")]
    public SpawnManager spawn_manager;

    /// <summary>
    /// If true, all starting agents will evolve movement
    /// </summary>
    [Header("Evolution Settings")]
    public static bool mobile_start;

    //[Header("Garbage Collection")]
    //public bool use_auto_manual_collection;

    // These are values the manager uses to manage the simulation (does not need to be seen)
    // The cluster pos is the center positions of all the clusters
    private List<Vector2> cluster_pos = new List<Vector2>();
    // List of all activate agents
    private List<BaseAgent> all_agents = new List<BaseAgent>();

    [Tooltip("The list of all present agents")]
    public List<Interactable> agents = new List<Interactable>();

    public void Setup()
    {
        // Setup the spawner
        spawn_manager.Setup();

        // This spawns the agents.
        SpawnAgents();

        // Calculate the energy in the agents
        ManageAgents();

        // This spawns in all of the food pellets
        SpawnFoodPellets();

        // Set the initial energy
        initial_energy = energy + energy_in_agents;

        // Setup the stat manager
        stat_manager.Setup(initial_energy);
    }

    private void Update()
    {

        if (clock <= 0)
        {
            clock = update_rate;
            ManageFoodPellets();
            ManageAgents();
            CalculateEnergyInEth();
            combined = energy_in_agents + energy_in_ether + energy_in_pellets;

            percent_energy_in_pellets *= 100;
            percent_energy_in_ether *= 100;
            percent_energy_in_agents *= 100;

            stat_manager.SetOtherEnergy(percent_energy_in_pellets, percent_energy_in_ether);
            stat_manager.Update();

            // To deal with rounding errors, we inject a energy into the system when it fades away
            energy = Mathf.Max(initial_energy - combined + energy, 0);

        }
        else
        {
            clock -= Time.deltaTime;
        }
    }

    //public void CreateClusters()
    //{
    //    int num_clusters = pellet_clusters;
    //    cluster_pos.Clear();

    //    // Set up the clusters
    //    for (int i = 0; i < num_clusters; i++)
    //    {
    //        cluster_pos.Add(Random.insideUnitCircle * gridsize);
    //    }
    //}

    public List<Vector2> GetClusters()
    {
        return cluster_pos;
    }

    /// <summary>
    /// Calculates the energy in the food pellets
    /// </summary>
    public void ManageFoodPellets()
    {
        float percent = 0;
        num_active_food = 0;
        for (int i = 0; i < food_pellets.Count; i++)
        {
            // Add the energy of the food pellet
            percent += food_pellets[i].energy;
            num_active_food += food_pellets[i].eaten ? 0 : 1;
        }

        // Get the ratio of total energy in the system and the energy in the pellets
        energy_in_pellets = percent;
        percent_energy_in_pellets = percent / initial_energy;
    }

    public void SpawnFoodPellets()
    {
        // Spawn in the pellets
        for (int i = 0; i < total_food; i++)
        {
            FoodPellet p = Instantiate(pellet, spawn_manager.GetFoodSpawnLocation(), Quaternion.identity, transform);

            p.Setup((int)ID.FoodPellet, food_pellet_energy, food_growth_rate, food_pellet_size, this);
            RecycleEnergy(food_pellet_energy);
            food_pellets.Add(p);
        }
    }

    // Returns the amount of energy needed to create the agents
    public void SpawnAgents()
    {
        // Spawn in the agents
        for (int i = 0; i < starting_agents; i++)
        {
            // Spawn an agent and set it up
            var e = Instantiate(egg, spawn_manager.GetRandomSpawnLocation(), transform.rotation, transform);
            e.Setup(this, (int)ID.WobbitEgg);
            //RecycleEnergy(e.energy);
        }
    }

    public void ManageAgents()
    {
        float in_agents = 0;
        float in_meat = 0;
        float in_eggs = 0;
        percent_energy_in_agents = 0;

        // Spawn more agents if we dont have enough
        if (agents.Count < min_agents)
        {
            float energy_required = agent.base_health * Mathf.Pow(Genes.GetBaseGenes().size, 2);
            if (energy >= energy_required)
            {
                var e = Instantiate(egg, spawn_manager.GetRandomSpawnLocation(), transform.rotation, transform);
                e.Setup(this, (int)ID.WobbitEgg);
                ExtractEnergy(e.energy);
            }
        }

        // Clear the list of agents
        all_agents.Clear();
        foreach (Interactable a in agents)
        {
            if (a.GetID() == (int)ID.Wobbit)
            {
                percent_energy_in_agents += ((BaseAgent)a).energy;
                in_agents += ((BaseAgent)a).energy;

                // Add to this temp list to send to the stats manager
                all_agents.Add((BaseAgent)a);
            }
            else if (a.GetID() == (int)ID.Meat)
            {
                percent_energy_in_agents += ((Meat)a).energy;
                in_meat += ((Meat)a).energy;
            }
            else if (a.GetID() == (int)ID.WobbitEgg)
            {
                percent_energy_in_agents += ((Egg)a).energy;
                in_eggs += ((Egg)a).energy;
            }
        }
        energy_in_agents = percent_energy_in_agents;
        percent_energy_in_agents /= initial_energy;

        // Update the stat manager
        stat_manager.SetAgentEnergy(percent_energy_in_agents, in_agents / initial_energy, in_meat / initial_energy, in_eggs / initial_energy);
        stat_manager.CalculateAverageGenes(all_agents);
    }

    public void AddAgent(Interactable a)
    {
        agents.Add(a);
    }

    public void CalculateEnergyInEth()
    {
        energy_in_ether = energy;
        percent_energy_in_ether = energy / initial_energy;
    }

    public float Round(float value, int digits)
    {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }

    public void RecycleEnergy(float amount)
    {
        energy += amount;
    }

    public float ExtractEnergy(float amount)
    {
        if (energy - amount > 0)
        {
            energy -= amount;
            return amount;
        }
        else
        {
            return 0;
        }

    }

    public BaseAgent GetAgent(int id)
    {
        if (id == (int)ID.Wobbit)
        {
            return agent;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Setup the sprite by setting the child sprite renderer object's sprite image and setting the points on the polygon colider
    /// </summary>
    /// <param name="a"></param>
    public void SetSprite(BaseAgent a, float r, float g, float b)
    {
        // Setup the sprite by setting the child sprite renderer object's sprite image and setting the points on the polygon collider
        GetComponent<SpriteManager>().SetSprite(a.genes.spritemap["head"], a.genes.spritemap["body"], a, new Color(r, g, b));
    }

    /// <summary>
    /// Setup the sprite for a child which was created from sexual reproduction
    /// </summary>
    /// <param name="a"></param>
    public void SetSprite(BaseAgent a, Color c1, Color c2)
    {
        // Sets the body and head to be two different colours (one from each parent)
        GetComponent<SpriteManager>().SetBiChromaticAgent(a, c1, c2);
    }

    /// <summary>
    /// Returns a certain agent depending on the mode.
    /// The current modes are as follows: {'o' -> Oldest Agent, 'v' -> highest velocity Agent, 'r' -> Random Agent, 'y' -> Youngest Agent, 'e' -> Agent With Most Eggs Layed}
    /// </summary>
    /// <param name="mode"></param>
    public BaseAgent GetAgent(char mode)
    {
        BaseAgent to_be_returned = null;
        foreach (BaseAgent a in all_agents)
        {
            switch (mode)
            {
                case 'o':
                    to_be_returned = (to_be_returned == null || to_be_returned.age < a.age) ? a : to_be_returned;
                    break;
                case 'r':
                    return all_agents[Random.Range(0, all_agents.Count)];
                case 'e':
                    to_be_returned = (to_be_returned == null || to_be_returned.eggs_layed < a.eggs_layed) ? a : to_be_returned;
                    break;
                case 'y':
                    to_be_returned = (to_be_returned == null || to_be_returned.age > a.age) ? a : to_be_returned;
                    break;
                case 'v':
                    to_be_returned = (to_be_returned == null || to_be_returned.GetRB().velocity.magnitude < a.GetRB().velocity.magnitude) ? a : to_be_returned;
                    break;
            }
        }

        return to_be_returned;
    }
}
