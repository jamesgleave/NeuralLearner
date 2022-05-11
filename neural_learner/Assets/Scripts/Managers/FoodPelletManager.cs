using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodPelletManager : MonoBehaviour
{
    // Keeps track of food pellets within the lists (processes 1 per frame)
    public int tick_pointer;

    /// <summary>
    /// The rate which the pellets are fed thru the manager
    /// </summary>
    public int feed_rate;

    /// <summary>
    /// The higher this value, the less food will respawn
    /// </summary>
    [Range(0.0f, 1)]
    public float retention;

    /// <summary>
    /// The lower this value, the less food will respawn
    /// </summary>
    [Range(0f, 1000f)]
    public float intensity;

    /// <summary>
    /// The number of pellets we have calculated
    /// </summary>
    public int calculated_pellets;

    /// <summary>
    /// The number of pellets we want
    /// </summary>
    public int target_pellets;

    // This manager has a reference to the main manager (to access the food list)
    public Manager general_manager;
    private float percent;
    private int storage;

    /// <summary>
    /// If true, we use a hardware accelerated version of the food manager and food pellet scripts
    /// using compute shaders.
    /// EXPERIMENTAL
    /// </summary>
    [Header("Hardware Acceleration")]
    public bool hardware_accelerated;
    public ComputeShader cs;
    struct FoodPelletStruct
    {
        public float size;
        public int update_scale;

        public int recycle;

        public float energy;
        public float max_energy;
        public float energy_consumed;
        public float energy_extraction_ratio;

        public Vector2 position;
    };
    ComputeBuffer buffer;
    FoodPelletStruct[] food_pellet_structs;

    void Start()
    {
        // Set up our values for our buffer
        // First, calculate the size of our struct
        int vector2_size = 2 * sizeof(float);
        int struct_size = sizeof(float) * 5 + sizeof(int) * 2 + vector2_size;

        // Create our struct array
        food_pellet_structs = new FoodPelletStruct[general_manager.food_pellets.Count];
        for (int i = 0; i < food_pellet_structs.Length; i++)
        {
            food_pellet_structs[i] = new FoodPelletStruct();
        }

        // Create and set our buffer
        buffer = new ComputeBuffer(general_manager.food_pellets.Count, struct_size);
        cs.SetBuffer(0, "pellets", buffer);
    }

    // Start is called before the first frame update
    void Update()
    {

        // Update all of the pellets
        if (hardware_accelerated)
        {
            SetBufferValues();
            UpdatePelletsHardwareAccelorated();
        }
        else
        {
            UpdatePellets();

        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            foreach (FoodPellet pellet in general_manager.food_pellets)
            {
                general_manager.RecycleEnergy(pellet.Eat());
            }
        }
    }

    public void CalculateSpawnFunc()
    {
        // Calculate the new value 
        float calculated = Mathf.Max((int)((general_manager.total_food / (general_manager.stat_manager.num_agents / (1 - retention))) * intensity), 0);
        target_pellets = (int)calculated;

        // Increment the old value based on the new one
        if (calculated > calculated_pellets)
        {
            calculated_pellets++;
        }
        else if (calculated < calculated_pellets)
        {
            calculated_pellets--;
        }
    }

    public void UpdatePellets()
    {

        // Lock in the feed rate
        if (feed_rate > general_manager.total_food)
        {
            feed_rate = general_manager.total_food - 1;
        }

        if (general_manager.food_pellets.Count <= 0)
        {
            return;
        }

        // Iterate through using the feed rate
        for (int i = 0; i < (general_manager.total_food - feed_rate); i++)
        {


            // Check if tick pointer is too big
            if (tick_pointer >= general_manager.total_food)
            {
                tick_pointer = 0;

                // Reset the energy in pellets to recalculate
                general_manager.percent_energy_in_pellets = percent;

                // Reset percent
                percent = 0;

                // Get the ratio of total energy in the system and the energy in the pellets
                general_manager.energy_in_pellets = general_manager.percent_energy_in_pellets;
                general_manager.percent_energy_in_pellets /= general_manager.initial_energy;

                // Look at each pellet
                general_manager.num_active_food = storage;
                storage = general_manager.total_food;
            }

            // Grab the food pellet
            FoodPellet pellet = (FoodPellet)general_manager.food_pellets[tick_pointer];

            // Update the pellet
            pellet.UpdatePellet();

            // If the pellet has been eaten then respawn if we have enough energy
            // As the number of agents increase, the probability of a food pellet respawning dr
            if (pellet.eaten && general_manager.num_active_food < calculated_pellets)
            {
                pellet.Respawn(general_manager.spawn_manager.GetFoodSpawnLocation(), general_manager.food_pellet_energy, general_manager.food_growth_rate, general_manager.food_pellet_size);
                general_manager.num_active_food++;
            }
            else if (pellet.eaten)
            {
                storage--;
            }

            // Add the energy in that pellet
            percent += pellet.energy;


            // Increment tick pointer
            tick_pointer++;
        }

        CalculateSpawnFunc();
    }

    public void SetBufferValues()
    {
        // Send our delta time value to the GPU
        cs.SetFloat("delta_time", Time.deltaTime);

        //// If true, the manager will be managing food pellet teleportation
        //bool manager_teleport;
        cs.SetBool("manager_teleport", general_manager.teleport);

        //// A buffer given while calculating the manager teleport value.
        //// This is multiplied by the gridsize to determine the whether or not to teleport. Defaults to 1.25.
        cs.SetFloat("distance_threshold_buffer", 1.25f);

        //// The position of the manager
        cs.SetVector("manager_position", general_manager.transform.position);

        //// The growth rate of the pellets
        cs.SetFloat("growth_rate", general_manager.food_growth_rate);

        //// The gridsize determined by the manager
        cs.SetFloat("gridsize", general_manager.gridsize);
    }

    public void UpdatePelletsHardwareAccelorated()
    {
        // Lock in the feed rate
        if (feed_rate > general_manager.total_food)
        {
            feed_rate = general_manager.total_food - 1;
        }

        if (general_manager.food_pellets.Count <= 0)
        {
            return;
        }

        // Do all of the compute shader stuff...
        for (int i = 0; i < food_pellet_structs.Length; i++)
        {
            // Update all values in the compute shader
            food_pellet_structs[i].size = general_manager.food_pellets[i].size;
            food_pellet_structs[i].energy = general_manager.food_pellets[i].energy;
            food_pellet_structs[i].max_energy = general_manager.food_pellets[i].max_energy;
            food_pellet_structs[i].energy_consumed = general_manager.food_pellets[i].energy_consumed;
            food_pellet_structs[i].position = general_manager.food_pellets[i].transform.position;

            // Fill these in (they outputs so these dont matter)
            food_pellet_structs[i].update_scale = -1;
            food_pellet_structs[i].energy_extraction_ratio = -1.0f;
        }

        // Create a position and force buffer
        buffer.SetData(food_pellet_structs);

        // Run the thing
        int num_threads = Mathf.Max(food_pellet_structs.Length / 32, food_pellet_structs.Length);
        cs.Dispatch(0, num_threads, 1, 1);

        // Get the data back!
        buffer.GetData(food_pellet_structs);

        // Do all of the compute shader stuff...
        for (int i = 0; i < food_pellet_structs.Length; i++)
        {
            // Update all values in the compute shader
            general_manager.food_pellets[i].SetAttributes(
                food_pellet_structs[i].recycle == 1,
                food_pellet_structs[i].update_scale == 1,
                food_pellet_structs[i].energy_extraction_ratio,
                food_pellet_structs[i].size);
        }
    }

    public void OnDestroy()
    {
        // Get rid of the buffer
        buffer.Dispose();
    }
}
