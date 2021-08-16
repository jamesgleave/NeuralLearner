using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFood : MonoBehaviour
{
    /// <summary>
    /// The food pellet
    /// </summary>
    public GameObject food;

    /// <summary>
    /// How many times to try to find a new location
    /// </summary>
    public int k;

    /// <summary>
    /// The radius at which the pellet looks in
    /// </summary>
    public int r;

    /// <summary>
    /// The size of the grid
    /// </summary>
    public int s;

    /// <summary>
    /// Number of pellets 
    /// </summary>
    public int n;

    /// <summary>
    /// Click to respawn all pellets
    /// </summary>
    public bool respawn;

    private Collider2D[] buffer;
    private ContactFilter2D filter;


    // Start is called before the first frame update
    void Start()
    {
        buffer = new Collider2D[1];
        filter = new ContactFilter2D();
    }

    // Update is called once per frame
    void Update()
    {
        if (respawn)
        {
            Respawn();
        }
    }

    public void TrySpawn()
    {
        Instantiate<GameObject>(food, GetSpawnLocation(), Quaternion.identity);
    }

    public Vector2 GetSpawnLocation()
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

    public void Respawn()
    {
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Food"))
        {
            Destroy(g);
        }
        respawn = false;

        for (int i = 0; i < n; i++)
        {
            TrySpawn();
        }
    }
}
