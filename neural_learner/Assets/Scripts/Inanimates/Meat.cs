using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meat : Edible
{
    public float initial_energy;

    /// <summary>
    ///   <para>The rate this meat piece rots</para>
    /// </summary>
    public float rot_rate;

    /// <summary>
    ///   <para>The scale of the meat</para>
    /// </summary>
    public float size;

    /// <summary>
    ///   <para>The manager associated with this meat</para>
    /// </summary>
    public Manager manager;

    public Genes genes;

    public float energy_density;

    public void Setup(int id, float e, float rr, float s, Genes genes, Manager m)
    {
        // Set values
        rot_rate = rr;
        manager = m;
        energy = e;
        initial_energy = e;
        size = s;
        transform.localScale = new Vector3(size, size, size);
        base.Setup(id);

        rb.velocity = Random.insideUnitCircle;
        this.genes = genes;
    }

    // Update is called once per frame
    public void Update()
    {
        // If the energy has run out, get rid of the meat
        if (energy <= initial_energy * 0.1f)
        {
            // Remove this agent from the manager's list
            manager.agents.Remove(this);

            // Recycle and destroy object
            manager.RecycleEnergy(energy);
            Destroy(gameObject);
        }

        // Reduce the size and energy of the meat
        float cost = initial_energy * (Time.deltaTime / rot_rate);
        if (energy - cost < 0)
        {
            cost = energy;
            energy -= cost;
        }
        else
        {
            energy -= cost;
        }
        // If there is any wasted energy, it is returned to the system
        manager.RecycleEnergy(cost);

        // Find the new size
        transform.localScale = new Vector3(size, size, size) * (energy / initial_energy);

        energy_density = GetEnergyDensity();
    }

    public override float Eat()
    {
        // Temp store the energy so it can be returned
        float temp_energy = energy;

        // Set the energy of this pellet to zero since it has been eaten
        energy = 0;

        // Remove this agent from the manager's list
        manager.agents.Remove(this);

        return temp_energy;
    }

    public override float Eat(float consumption_rate)
    {
        // If we have enough energy after consuming then we do not destroy
        if (energy - consumption_rate > 0)
        {
            energy -= consumption_rate;
            return consumption_rate;
        }
        else
        {
            return Eat();
        }
    }
}
