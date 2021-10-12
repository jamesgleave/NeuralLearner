using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PheromoneGland : MonoBehaviour
{
    /// <summary>
    /// The pheromone game objects
    /// </summary>
    public Pheromone red, green, blue;
    public float cooldown_time;

    /// <summary>
    /// The cooldown (time between pheromone deployment)
    /// </summary>
    public float cooldown;

    // Update is called once per frame
    void Update()
    {
        cooldown -= Time.deltaTime;
    }

    public void SpawnRed()
    {
        if (cooldown > 0) return;
        Instantiate(red, transform.position, transform.rotation);
        cooldown = cooldown_time;
    }

    public void SpawnGreen()
    {
        if (cooldown > 0) return;
        Instantiate(green, transform.position, transform.rotation);
        cooldown = cooldown_time;
    }

    public void SpawnBlue()
    {
        if (cooldown > 0) return;
        Instantiate(blue, transform.position, transform.rotation);
        cooldown = cooldown_time;
    }
}
