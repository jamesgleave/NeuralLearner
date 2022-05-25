using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodPellet : Edible
{
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

    // Constants to speed up calculations
    private float max_energy_growth_rate_ratio;
    private Vector3 target_size;

    public float energy_density;

    public void Setup(int id, float me, float gr, float s, Manager m)
    {
        // Set values
        float scaler = Random.Range(0.5f, 1.5f);
        growth_rate = gr;
        size = s * scaler;
        max_energy = size * me;
        manager = m;
        energy = 0;

        transform.localScale = Vector3.zero;
        base.Setup(id);

        // Set up constant ratio
        max_energy_growth_rate_ratio = growth_rate;
        target_size = new Vector3(size, size, size);
    }

    public void Respawn(Vector2 pos, float me, float gr, float s)
    {

        // Set values
        float scaler = Random.Range(0.5f, 1.5f);
        growth_rate = gr;
        size = s * scaler;
        max_energy = size * me;
        energy = 0;

        sprite.enabled = true;
        rb.simulated = true;
        col.enabled = true;

        transform.localScale = Vector3.zero;
        transform.localPosition = pos;
        eaten = false;

        max_energy_growth_rate_ratio = growth_rate;
        target_size = new Vector3(size, size, size);

    }

    public void UpdatePellet()
    {
        if(eaten){return;}
        // Scale the ratio with delta time
        float max_energy_growth_rate_ratio_time_scaled = growth_rate * Time.deltaTime;
        if (max_energy > energy + max_energy_growth_rate_ratio_time_scaled && max_energy > energy_consumed + max_energy_growth_rate_ratio_time_scaled)
        {
            // Extract energy from the manager
            float energy_delta = manager.ExtractEnergy(max_energy_growth_rate_ratio_time_scaled);

            // Add energy
            energy += energy_delta;
            energy_consumed += energy_delta;

            // Find the delta energy and the goal size
            goal_scale = target_size * (energy / max_energy);

            // Scale up
            transform.localScale = goal_scale;

            energy_density = GetEnergyDensity();
        }
        else
        {
            goal_scale = target_size * (energy / max_energy);

            // Scale up
            transform.localScale = goal_scale;
        }

        // If the pellet is too far, respawn it
        if (manager.teleport && Vector2.Distance(rb.position, manager_position) > (manager.gridsize) * 1.25f)
        {
            manager.RecycleEnergy(Eat());
        }
    }

    /// <summary>
    /// Used by the computer shader version of the food system
    /// </summary>
    /// <param name="recycle"></param>
    public void SetAttributes(bool recycle, bool update_scale, float energy_extraction_ratio, float new_scale)
    {
        // If the compute shader determined we are ready to recycle, we do.
        // recycle = manager.teleport && Vector2.Distance(rb.position, manager_position) > (manager.gridsize) * 1.25f
        if (recycle) { manager.RecycleEnergy(Eat()); }

        // goal_scale = new Vector3(size, size, size) * Mathf.Max((energy / max_energy), 0.1f) + Vector3.one / 10f;
        // new_scale = Vector3.Lerp(Vector3.zero, goal_scale, energy / max_energy);
        if (!float.IsNaN(new_scale) && !float.IsInfinity(new_scale))
        {
            transform.localScale = Vector3.one * new_scale;
        }

        // Check whether or not to update the scale/take more energy from the manager
        // update_scale = max_energy > energy + (max_energy / growth_rate) * Time.deltaTime && max_energy > energy_consumed + (max_energy / growth_rate) * Time.deltaTime
        if (update_scale)
        {
            // Extract energy from the manager
            // energy_extraction_ratio = (max_energy / growth_rate) * Time.deltaTime
            float energy_delta = manager.ExtractEnergy(energy_extraction_ratio);
            // Add energy
            energy += energy_delta;
            energy_consumed += energy_delta;
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

    public override float Eat()
    {
        // Temp store the energy so it can be returned
        float temp_energy = energy;

        // Set time to zero
        time_since_eaten = 0;

        // Set the energy of this pellet to zero since it has been eaten
        energy = 0;
        max_energy = 0;

        if (energy <= 0.1F)
        {
            // Set eaten to true obviously 
            eaten = true;
        }

        // Deactivate all components
        Deactivate();

        return temp_energy;
    }

    public override float Eat(float consumption_rate)
    {

        // Set time to zero
        time_since_eaten = 0;

        // If we have enough energy after consuming then we do not destroy
        if (energy - consumption_rate > 0.1F)
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
