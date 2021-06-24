using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodPellet : Interactable
{

    /// <summary>
    ///   <para>The energy containted in food pellet</para>
    /// </summary>
    public float energy;

    /// <summary>
    ///   <para>The max energy containted in food pellet when fully grown</para>
    /// </summary>
    public float max_energy;

    /// <summary>
    ///   <para>The amount of energy this pellet has consumed</para>
    /// </summary>
    public float energy_consumed;

    /// <summary>
    ///   <para>The time take to grow to max size</para>
    /// </summary>
    public float growth_rate;

    /// <summary>
    ///   <para>The scale of the pellet</para>
    /// </summary>
    public float size;

    /// <summary>
    ///   <para>If the pellet has been eaten</para>
    /// </summary>
    public bool eaten;

    /// <summary>
    ///   <para>The manager associated with this food pellet</para>
    /// </summary>
    public Manager manager;

    /// <summary>
    ///   <para>The goal size of the pellet</para>
    /// </summary>
    Vector3 goal_scale;

    // Ready is true when the cooldown time is less than zero meaning it can respawn
    public float cooldown_time;
    public bool ready;

    // The time since they have last been eaten
    // When this is 30, they can start to regrow
    public float time_since_eaten = 0;

    public void Setup(int id, float me, float gr, float s, Manager m)
    {
        // Set values
        growth_rate = gr * Random.Range(0.9f, 1.1f);
        size = s * Random.Range(0.5f, 1.5f);
        max_energy = me;
        manager = m;
        energy = 0;

        transform.localScale = Vector3.zero;
        base.Setup(id);
    }

    public void Respawn(Vector2 pos, float me, float gr, float s)
    {
        if (ready)
        {
            transform.position = pos;
            max_energy = me;
            growth_rate = gr * Random.Range(0.9f, 1.1f);
            eaten = false;
            energy = 0;
            size = s * Random.Range(0.5f, 1.5f);

            sprite.enabled = true;
            rb.simulated = true;
            col.enabled = true;

            transform.localScale = Vector3.zero;

            ready = false;
        }
    }

    // Update is called once per frame
    public void Update()
    {
        if (max_energy > energy + (max_energy / growth_rate) * Time.deltaTime && max_energy > energy_consumed + (max_energy / growth_rate) * Time.deltaTime)
        {
            // Extract energy from the manager
            float energy_delta = manager.ExtractEnergy((max_energy / growth_rate) * Time.deltaTime);

            // Find the delta energy and the goal size
            goal_scale = new Vector3(size, size, size) * Mathf.Max((energy / max_energy), 0.1f) + Vector3.one / 10f;

            // Scale up
            transform.localScale = Vector3.Lerp(Vector3.zero, goal_scale, energy / max_energy);

            // Add energy
            energy += energy_delta;
            energy_consumed += energy_delta;
        }
        else
        {
            // Find the delta energy and the goal size
            goal_scale = new Vector3(size, size, size) * Mathf.Max((energy / max_energy), 0.1f) + Vector3.one / 10f;
            transform.localScale = Vector3.Lerp(transform.localScale, goal_scale, growth_rate);

            // Count down if we are not ready
            if (cooldown_time > 0 && ready == false)
            {
                cooldown_time -= Time.deltaTime;
            }
            else
            // If we are ready, set a respawn timer equal to the proportion of max_agents
            {
                // The cooldown time exponentially increases as the number of agents reach the max threshold
                cooldown_time = Mathf.Min(manager.CalculatePelletRespawnTime(), 300);
                ready = true;
            }
        }
    }

    void Deactivate()
    {
        energy_consumed = 0;
        max_energy = 0;
        energy = 0;
        sprite.enabled = false;
        rb.simulated = false;
        col.enabled = false;
        rb.velocity = Vector2.zero;
    }

    public float Eat()
    {
        // Temp store the energy so it can be returned
        float temp_energy = energy;

        // Set time to zero
        time_since_eaten = 0;

        // Set the energy of this pellet to zero since it has been eaten
        energy = 0;
        max_energy = 0;

        if (energy <= 0)
        {
            // Set eaten to true obviously 
            eaten = true;
        }

        // Deactivate all components
        Deactivate();

        return temp_energy;
    }

    public float Eat(float consumption_rate)
    {

        // Set time to zero
        time_since_eaten = 0;

        // If we have enough energy after consuming then we do not destroy
        if (energy - consumption_rate > 0)
        {
            energy -= consumption_rate;
            return consumption_rate;
        }
        else
        {
            max_energy = 0;
            eaten = true;
            Deactivate();
            return energy;
        }
    }

}
