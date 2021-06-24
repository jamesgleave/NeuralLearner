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

    [Tooltip("How tightly the pellets distrobuted")]
    public float pellet_distrobution;

    [Tooltip("How many pellet clusters there will be")]
    public int pellet_clusters;

    [Tooltip("The max food pellet size")]
    public float food_pellet_size;

    [Tooltip("The amount of energy in one pellet")]
    public float food_pellet_energy;

    [Tooltip("The food pellet prefab")]
    public FoodPellet pellet;

    [Tooltip("The list of all food pelletes present")]
    public List<Interactable> food_pellets = new List<Interactable>();     // The list of food pellets

    [Header("Simulation Settings")]
    [Tooltip("The manager's update rate in time per update (seconds)")]
    public float update_rate;
    private float clock = 0;

    [Tooltip("The size of our simulation area")]
    public int gridsize;

    [Header("Adaptive Grid Settings")]
    [Tooltip("Use an adaptive grid which expands and contracts with the number of agents")]
    public bool adaptive_grid;
    public float adaptive_gridsize;
    public int adaptive_grid_agent_threshold;

    [Header("Adaptive Food Settings")]
    public bool adaptive_food;
    public int agent_pellet_threshold;

    [Header("Agent Settings")]
    [Tooltip("The Agent")]
    public BaseAgent agent;
    public Egg egg;

    [Tooltip("The number of initial agents to spawn in.")]
    public float starting_agents;
    public int min_agents;

    [Tooltip("The scale rate for the agent metabolism. Increase this value to increate the lifetime of each agent.")]
    public float metabolism_scale_rate;

    [Tooltip("The list of all present agents")]
    public List<Interactable> agents = new List<Interactable>();

    [Header("Simulation info")]
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

    [Header("Ancesory")]
    public AncestorManager anc_manager = new AncestorManager();

    [Header("Garbage Collection")]
    public bool use_auto_manual_collection;

    // These are values the manager uses to manage the simulation (does not need to be seen)
    // The cluster pos is the center positions of all the clusters 
    private List<Vector2> cluster_pos = new List<Vector2>();
    // List of all activate agents 
    private List<BaseAgent> all_agents = new List<BaseAgent>();

    public void Setup()
    {
        // Create clusters for the spawning
        CreateClusters();

        // Spawn in agents!
        // This spawns the agents and calculates the required energy to spawn in the food.
        // It will return the total energy required for the system
        initial_energy += SpawnAgents();

        // This spawns in all of the food pellets
        SpawnFoodPellets();

        // Setup the stat manager
        stat_manager.Setup(initial_energy);
    }

    private void Update()
    {

        if (clock <= 0)
        {
            clock = update_rate;
            ManageFoodPellets();
            ManageClusters();
            ManageAgents();
            CalculateEnergyInEth();
            combined = energy_in_agents + energy_in_ether + energy_in_pellets;

            stat_manager.SetOtherEnergy(percent_energy_in_pellets, percent_energy_in_ether);
            stat_manager.Update();
        }

        else
        {
            clock -= Time.deltaTime;
        }
    }

    public void ManageClusters()
    {
        // If we are using the adaptive grid, perform these calculations
        if (adaptive_grid)
        {
            // Calculate the ratio of agents to max agents
            float adaption_factor = stat_manager.num_agents / (float)adaptive_grid_agent_threshold;

            // Get a temp gridsize for comparison
            float potential_gridsize = Mathf.Max(adaption_factor * ((float)gridsize), gridsize);

            // If we pretty much have the same grid, dont bother updating anything
            if (!Mathf.Approximately(potential_gridsize, adaptive_gridsize) && potential_gridsize > adaptive_gridsize || stat_manager.num_agents < (float)adaptive_grid_agent_threshold)
            {
                // update the gridsize
                adaptive_gridsize += ((potential_gridsize - adaptive_gridsize) / 30) * Time.deltaTime;

                if (Mathf.Abs(potential_gridsize - adaptive_gridsize) > 5)
                {
                    // Move the clusters
                    for (int i = 0; i < cluster_pos.Count; i++)
                    {
                        cluster_pos[i] = Random.insideUnitCircle * adaptive_gridsize;
                    }
                }
            }
        }
    }

    public void CreateClusters()
    {
        int num_clusters = pellet_clusters;
        cluster_pos.Clear();

        // Set up the clusters
        for (int i = 0; i < num_clusters; i++)
        {
            cluster_pos.Add(Random.insideUnitCircle * gridsize);
        }
    }

    public void ManageFoodPellets()
    {
        // Reset the energy in pellets to recalculate
        percent_energy_in_pellets = 0;

        // Look at each pellet
        foreach (FoodPellet pellet in food_pellets)
        {
            // If the pellet has been eaten then respawn if we have enough energy
            // As the number of agents increase, the probability of a food pellet respawning drops
            if (pellet.eaten) //  && Random.value < max_agents / (1 + stat_manager.num_agents)
            {
                Vector2 center = cluster_pos[Random.Range(0, pellet_clusters)];
                Vector2 offset = Random.insideUnitCircle * pellet_distrobution;
                pellet.Respawn(center + offset, food_pellet_energy, food_growth_rate, food_pellet_size);
            }

            // Add the energy in that pellet
            percent_energy_in_pellets += pellet.energy;
        }

        // Get the ratio of total energy in the system and the energy in the pellets
        energy_in_pellets = percent_energy_in_pellets;
        percent_energy_in_pellets /= initial_energy;
    }

    /// <summary>
    /// Returns the time each pellet has to wait before it may respawn. If adaptive food is not on, it will return zero
    /// </summary>
    /// <returns></returns>
    public float CalculatePelletRespawnTime()
    {
        return adaptive_food ? Mathf.Lerp(10, 500, stat_manager.num_agents / agent_pellet_threshold) : 0;
    }

    public void SpawnFoodPellets()
    {

        // Calculate the amount of energy needed for all of the food
        energy = food_pellet_energy * total_food;

        // Spawn in the pellets
        for (int i = 0; i < total_food; i++)
        {
            Vector2 center = cluster_pos[Random.Range(0, cluster_pos.Count)];
            Vector2 offset = Random.insideUnitCircle * pellet_distrobution;
            FoodPellet p = Instantiate(pellet, center + offset, Quaternion.identity, transform);

            p.Setup((int)ID.FoodPellet, food_pellet_energy, food_growth_rate, food_pellet_size, this);
            p.energy_consumed = food_pellet_energy;
            p.energy = food_pellet_energy;
            energy -= food_pellet_energy;
            food_pellets.Add(p);

        }
    }

    // Returns the amount of energy needed to create the agents
    public float SpawnAgents()
    {
        // Spawn in the agents
        for (int i = 0; i < starting_agents; i++)
        {
            // Spawn in the egg object
            Vector2 center = cluster_pos[Random.Range(0, cluster_pos.Count)];
            Vector2 offset = Random.insideUnitCircle * pellet_distrobution;
            Vector2 pos = center + offset;
            var e = Instantiate(egg, pos, transform.rotation, transform);
            e.Setup(this, (int)ID.WobbitEgg);
        }

        // If the agents used all the energy then make up for it
        float needed_for_food = total_food * food_pellet_energy;
        if (energy < needed_for_food)
        {
            return needed_for_food + (-energy);
        }
        else
        {
            return -energy;
        }
    }

    public void ManageAgents()
    {
        float in_agents = 0;
        float in_meat = 0;
        float in_eggs = 0;
        percent_energy_in_agents = 0;

        // Spawn more agents if we dont have enough
        if (stat_manager.num_agents < min_agents)
        {
            float energy_required = agent.base_health * Mathf.Pow(Genes.GetBaseGenes().size, 2);
            if (energy >= energy_required)
            {
                Vector2 center = cluster_pos[Random.Range(0, cluster_pos.Count)];
                Vector2 offset = Random.insideUnitCircle * pellet_distrobution;
                Vector2 pos = center + offset;
                var e = Instantiate(egg, pos, transform.rotation, transform);
                e.Setup(this, (int)ID.WobbitEgg);
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
        percent_energy_in_ether = 1 - percent_energy_in_pellets - percent_energy_in_agents;
        energy_in_ether = initial_energy - energy_in_agents - energy_in_pellets;

        // Set the energy (in case we lose precision due to floating point rounding)
        energy = energy_in_ether;
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

    public void ToggleRender()
    {

    }

    public void OnDrawGizmos()
    {
        foreach (var c in cluster_pos)
        {
            Gizmos.DrawSphere(c, 0.1f);
        }

        Gizmos.DrawWireSphere(transform.position, adaptive_gridsize);
    }

}
