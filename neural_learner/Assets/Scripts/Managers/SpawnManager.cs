using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    /// <summary>
    /// The manager of the sim
    /// </summary>
    public Manager manager;

    /// <summary>
    /// The radius at which the pellet looks in
    /// </summary>
    public int r;

    /// <summary>
    /// The size of the grid
    /// </summary>
    private int s;

    /// <summary>
    /// How many times to try to find a new location
    /// </summary>
    public int k;

    private Collider2D[] buffer;
    private ContactFilter2D filter;


    // Start is called before the first frame update
    public void Setup()
    {
        buffer = new Collider2D[1];
        filter = new ContactFilter2D();
        s = manager.gridsize;
    }

    public Vector2 GetFoodSpawnLocation()
    {
        // Find random position
        Vector2 pos = Random.insideUnitCircle * s;
        for (int i = 0; i < k; i++)
        {
            // Check to see if it is a viable spot
            if (Scan(pos))
            {
                break;
            }

            // If not, re randomize position
            pos = Random.insideUnitCircle * s;
        }

        // Finally return position
        return pos;
    }

    public Vector2 GetRandomSpawnLocation()
    {
        return Random.insideUnitCircle * s;
    }

    public bool Scan(Vector2 point)
    {
        // Clear the buffer
        buffer[0] = null;

        // Populate buffer
        Physics2D.OverlapCircle(point, r, results: buffer, contactFilter: filter.NoFilter());

        return buffer[0] != null && buffer[0].CompareTag("Food");
    }
}
