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
    public int storage;

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
    public int chunk_size = 1024;
    bool dispatch_frame = true;
    bool coroutine_running = false;

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
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            foreach (FoodPellet pellet in general_manager.food_pellets)
            {
                general_manager.RecycleEnergy(pellet.Eat());
            }
        }

        UpdatePelletManager();
    }

    public void UpdatePelletManager()
    {
        // Update all of the pellets
        if (hardware_accelerated)
        {
            SetBufferValues();
            if (coroutine_running == false)
            {
                StartCoroutine(PelletUpdateTest());
            }
        }
        else
        {
            UpdatePellets();
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
        for (int i = 0; i < (feed_rate); i++)
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
                pellet.Respawn(general_manager.spawn_manager.GetFoodSpawnLocation(), general_manager.food_pellet_energy_density, general_manager.food_growth_rate, general_manager.food_pellet_size);
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

    public void UpdatePelletsHardwareAccelerated()
    {
        if (dispatch_frame)
        {
            // Set to total food
            storage = general_manager.total_food;

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
            int num_threads = Mathf.Max(food_pellet_structs.Length / 16, food_pellet_structs.Length);
            cs.Dispatch(0, num_threads, 2, 2);
            dispatch_frame = false;
        }
        else
        {
            // Get the data back!
            buffer.GetData(food_pellet_structs);

            // Set values of the pellets and calculate the percent energy in food pellets
            percent = 0;
            for (int i = 0; i < food_pellet_structs.Length; i++)
            {
                //print(food_pellet_structs[i].size);
                // Update all values in the compute shader
                general_manager.food_pellets[i].SetAttributes(
                    food_pellet_structs[i].recycle == 1,
                    food_pellet_structs[i].update_scale == 1,
                    food_pellet_structs[i].energy_extraction_ratio,
                    food_pellet_structs[i].size);

                // Add the energy of the food pellet
                percent += general_manager.food_pellets[i].energy;
                storage -= general_manager.food_pellets[i].eaten ? 1 : 0;
            }

            // Get the ratio of total energy in the system and the energy in the pellets
            general_manager.energy_in_pellets = percent;
            general_manager.percent_energy_in_pellets = percent / general_manager.initial_energy;

            // Look at each pellet
            general_manager.num_active_food = storage;
            dispatch_frame = true;
        }

    }

    private IEnumerator UpdatePelletsHardwareAcceleratedCoroutine()
    {
        // Run in an update loop
        coroutine_running = true;
        print("Started Coroutine For GPU Pellets");
        while (hardware_accelerated)
        {

            // Set to total food
            storage = general_manager.total_food;

            // Lock in the feed rate
            if (feed_rate > general_manager.total_food)
            {
                feed_rate = general_manager.total_food - 1;
            }

            if (general_manager.food_pellets.Count <= 0)
            {
                continue;
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

                // We only process one chunk per frame
                if (i % chunk_size == 0)
                {
                    yield return new WaitForEndOfFrame();
                }
            }

            // Create a position and force buffer
            buffer.SetData(food_pellet_structs);

            // Run the thing
            int num_threads = Mathf.Max(food_pellet_structs.Length / 16, food_pellet_structs.Length);
            cs.Dispatch(0, num_threads, 2, 2);

            // We have finished half of the work, now take a break
            //yield return new WaitForEndOfFrame();

            // Get the data back!
            buffer.GetData(food_pellet_structs);

            // Set values of the pellets and calculate the percent energy in food pellets
            percent = 0;
            for (int i = 0; i < food_pellet_structs.Length; i++)
            {
                //print(food_pellet_structs[i].size);
                // Update all values in the compute shader
                general_manager.food_pellets[i].SetAttributes(
                    food_pellet_structs[i].recycle == 1,
                    food_pellet_structs[i].update_scale == 1,
                    food_pellet_structs[i].energy_extraction_ratio,
                    food_pellet_structs[i].size);

                // Add the energy of the food pellet
                percent += general_manager.food_pellets[i].energy;
                storage -= general_manager.food_pellets[i].eaten ? 1 : 0;

                // We only process one chunk per frame
                //if (i % chunk_size == 0)
                //{
                //    yield return new WaitForEndOfFrame();
                //}
            }

            // Get the ratio of total energy in the system and the energy in the pellets
            general_manager.energy_in_pellets = percent;
            general_manager.percent_energy_in_pellets = percent / general_manager.initial_energy;

            // Look at each pellet
            general_manager.num_active_food = storage;
            dispatch_frame = true;

            yield return null;
            print("Hello");
        }
        coroutine_running = false;
        print("Stopped Coroutine For GPU Pellets");
    }

    private IEnumerator PelletUpdateTest()
    {
        // Run in an update loop
        int start_index = 0;
        int max_index = general_manager.food_pellets.Count;
        coroutine_running = true;
        print("Started Coroutine For GPU Pellets");
        while (hardware_accelerated)
        {

            // Set to total food
            storage = general_manager.total_food;

            // Calculate our max index
            int end_index = Mathf.Min(start_index + chunk_size, max_index);

            // Do all of the compute shader stuff...
            for (int i = start_index; i < end_index; i++)
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
            int num_threads = Mathf.Max(food_pellet_structs.Length / 16, food_pellet_structs.Length);
            cs.Dispatch(0, num_threads, 2, 2);

            // We have finished half of the work, now take a break
            //yield return new WaitForEndOfFrame();

            // Get the data back!
            buffer.GetData(food_pellet_structs);

            // Set values of the pellets and calculate the percent energy in food pellets

            for (int i = start_index; i < end_index; i++)
            {
                //print(food_pellet_structs[i].size);
                // Update all values in the compute shader
                general_manager.food_pellets[i].SetAttributes(
                    food_pellet_structs[i].recycle == 1,
                    food_pellet_structs[i].update_scale == 1,
                    food_pellet_structs[i].energy_extraction_ratio,
                    food_pellet_structs[i].size);

                // If the pellet has been eaten then respawn if we have enough energy
                if (general_manager.food_pellets[i].eaten && general_manager.num_active_food < target_pellets)
                {
                    general_manager.food_pellets[i].Respawn(general_manager.spawn_manager.GetFoodSpawnLocation(), general_manager.food_pellet_energy_density, general_manager.food_growth_rate, general_manager.food_pellet_size);
                }
            }

            // Reset start index if we exceed the size of our array
            start_index += chunk_size;
            if (start_index >= general_manager.food_pellets.Count)
            {
                // Reset index
                start_index = 0;
            }

            yield return null;

            CalculateSpawnFunc();
        }
        coroutine_running = false;
        print("Stopped Coroutine For GPU Pellets");
    }


    public void OnDestroy()
    {
        // Get rid of the buffer
        //buffer.Dispose();
    }
}
