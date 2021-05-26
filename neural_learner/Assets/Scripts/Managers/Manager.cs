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

    [Header("Testing")]
    public float scale;

    [Header("Agent Settings")]
    [Tooltip("The Agent")]
    public BaseAgent agent;

    [Tooltip("The number of initial agents to spawn in.")]
    public float starting_agents;

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

    // These are values the manager uses to manage the simulation
    // The spawn map contains probabilities for spawning food pellets
    private List<List<float>> spawnmap = new List<List<float>>();

    // The cluster pos is the center positions of all the clusters 
    private List<Vector2> cluster_pos = new List<Vector2>();


    public void Setup()
    {
        // Create clusters for the spawning
        CreateClusters();

        // This generates a heatmap of probabilities based on perlin noise
        GenerateMap();

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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleRender();
        }
    }

    public void CreateClusters()
    {
        int num_clusters = pellet_clusters;
        cluster_pos.Clear();

        // Set up the clusters
        for (int i = 0; i < num_clusters; i++)
        {
            cluster_pos.Add(new Vector2((Random.Range(-1f, 1f)) * (gridsize / 2), Random.Range(-1f, 1f) * (gridsize / 2)));
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
            // TODO Come up with a better way to manage the number of agents
            if (pellet.eaten && agents.Count < starting_agents)
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

    public void RespawnFoodPellets()
    {
        int num_clusters = pellet_clusters;
        cluster_pos.Clear();

        // Set up the clusters
        for (int i = 0; i < num_clusters; i++)
        {
            cluster_pos.Add(new Vector2((Random.Range(-1f, 1f)) * (gridsize / 2), Random.Range(-1f, 1f) * (gridsize / 2)));
        }

        // Spawn in the pellets
        foreach (FoodPellet p in food_pellets)
        {
            Vector2 center = cluster_pos[Random.Range(0, num_clusters)];
            Vector2 offset = Random.insideUnitCircle * pellet_distrobution;
            p.Respawn(center + offset, food_pellet_energy, food_growth_rate, food_pellet_size);
        }
    }

    // Returns the amount of energy needed to create the agents
    public float SpawnAgents()
    {
        // Spawn in the agents
        for (int i = 0; i < starting_agents; i++)
        {

            Vector2 center = cluster_pos[Random.Range(0, cluster_pos.Count)];
            Vector2 offset = Random.insideUnitCircle * pellet_distrobution;
            BaseAgent a = Instantiate(agent, center + offset, transform.rotation, transform);
            a.manager = this;

            // Create names and base genes for the creatures
            Genes g = Genes.RandomGenes(); // TODO Replace the random genes here with some kind of default
            string identifier = "<" + Random.Range(1000, 10000).ToString("X") + ">";
            string full_name = identifier + NameGenerator.GenerateFullName();
            g.genus = full_name.Split(' ')[0];
            g.species = full_name.Split(' ')[1];

            // Set the genes' spritemap (establish the look of the agent randomly)
            GetComponent<SpriteManager>().SetRandomComponents(g);

            // Setup the agent
            a.Setup((int)ID.Wobbit, g);
            a.genes.genetic_drift = 0;
            a.age = a.maturity_age;

            // Give them randomized brains
            for (int j = Random.Range(1, 100); j > 0; j--)
            {
                try
                {
                    Model.NeuralNet.MutateWeights((Model.NeuralNet)a.brain.GetModel(), a.genes.weight_mutation_prob, a.genes.dropout_prob);
                }
                catch
                {
                    // Do nothing
                    print("oof");
                }
            }

            // To track the ancestory, we use an ancestor manager object which is a tree like structure using nodes
            AncestorNode node = new AncestorNode(parent: null, genes: g, name: full_name);
            anc_manager.UpdatePopulation(a);
            anc_manager.AddGenus(node);

            a.node = node; // Add the agents ancestory (which is nothing yet)
            energy -= a.energy;
            AddAgent(a);
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

        List<BaseAgent> all_agents = new List<BaseAgent>();
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
    }

    public void GenerateMap()
    {
        // The offset to make the perlin noise different each time it is generated
        float px;
        float py;
        float offset = Random.Range(-1000f, 1000f);

        for (int x = 0; x < gridsize; x++)
        {
            List<float> row = new List<float>();
            for (int y = 0; y < gridsize; y++)
            {
                px = (float)x / (float)gridsize * scale;
                py = (float)y / (float)gridsize * scale;
                row.Add(Mathf.PerlinNoise(px + offset, py + offset));
            }
            spawnmap.Add(row);
        }
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

    public void ToggleRender()
    {

    }

    public void OnDrawGizmos()
    {

    }

}
