using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pheromone : Interactable
{
    /// <summary>
    /// How long the pheromone lasts for
    /// </summary>
    public float lifespan;
    private float max_lifespan;

    /// <summary>
    /// The type of this pheromone
    /// </summary>
    public PheromoneType type;

    /// <summary>
    /// The initial size of the pheromone
    /// </summary>
    private float init_size = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        if (type == PheromoneType.red)
        {
            Setup((int)ID.red_pharomone);
        }
        else if (type == PheromoneType.blue)
        {
            Setup((int)ID.blue_pharomone);
        }
        else if (type == PheromoneType.green)
        {
            Setup((int)ID.green_pharomone);
        }

        max_lifespan = lifespan;
        transform.localScale = new Vector3(lifespan / max_lifespan * init_size, lifespan / max_lifespan * init_size, lifespan / max_lifespan * init_size);
    }

    protected void Update()
    {
        // Reduce cooldown
        lifespan -= Time.deltaTime;

        // If we have gone on longer than the lifespan, destroy
        if (lifespan < 0)
        {
            Destroy(gameObject);
        }
    }
}

/// <summary>
/// The three different types of pheromone
/// </summary>
public enum PheromoneType
{
    red, blue, green
}
