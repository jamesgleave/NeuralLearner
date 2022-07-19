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
    public Vector3 cooldown;

    // Update is called once per frame
    void Update()
    {
        cooldown -= Time.deltaTime * Vector3.one;
    }

    public void SpawnRed()
    {
        if (cooldown.x > 0) return;
        Instantiate(red, transform.position, transform.rotation);
        cooldown.x = cooldown_time;
    }

    public void SpawnGreen()
    {
        if (cooldown.y > 0) return;
        Instantiate(green, transform.position, transform.rotation);
        cooldown.y = cooldown_time;
    }

    public void SpawnBlue()
    {
        if (cooldown.z > 0) return;
        Instantiate(blue, transform.position, transform.rotation);
        cooldown.z = cooldown_time;
    }
}
