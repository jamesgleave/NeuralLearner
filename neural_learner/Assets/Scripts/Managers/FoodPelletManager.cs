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
    /// The number of pellets we have calculated
    /// </summary>
    public int target_pellets;

    // This manager has a reference to the main manager (to access the food list)
    public Manager general_manager;

    private float percent;
    private int storage;


    public void Start()
    {

    }


    // Start is called before the first frame update
    void Update()
    {
        // Update all of the pellets
        UpdatePellets();
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
            feed_rate = general_manager.total_food;
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
}
