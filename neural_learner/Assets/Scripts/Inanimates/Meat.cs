using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meat : Interactable
{
    /// <summary>
    ///   <para>The energy containted in the meat</para>
    /// </summary>
    public float energy;
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

    public void Setup(int id, float e, float rr, float s, Genes genes, Manager m)
    {
        // Set values
        rot_rate = rr * Random.value;
        manager = m;
        energy = e;
        initial_energy = e;
        size = s / 10 + Random.value;
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
        float cost = Time.deltaTime * rot_rate * initial_energy;
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
        size -= 0.01f * Time.deltaTime;
        float new_size = Mathf.Max(0.1f, size);
        transform.localScale = new Vector3(new_size, new_size, new_size);

    }

    public float Eat()
    {
        // Temp store the energy so it can be returned
        float temp_energy = energy;

        // Set the energy of this pellet to zero since it has been eaten
        energy = 0;

        return temp_energy;
    }

    public float Eat(float consumption_rate)
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
