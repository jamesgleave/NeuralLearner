using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityPoolManager : MonoBehaviour
{
    /// <summary>
    /// The pool of instantiated agents
    /// </summary>
    [SerializeField]
    private List<BaseAgent> agent_pool;

    /// <summary>
    /// The pool of instantiated eggs
    /// </summary>
    [SerializeField]
    private List<Egg> egg_pool;

    /// <summary>
    /// The pool of instantiated pheromones
    /// </summary>
    [SerializeField]
    private List<Pheromone> pheromone_pool;

    // Start is called before the first frame update
    void Start()
    {
        pheromone_pool = new List<Pheromone>();
        agent_pool = new List<BaseAgent>();
        egg_pool = new List<Egg>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Destroy(Interactable i)
    {
        // Deactivate the interactable
        i.gameObject.SetActive(false);

        // Add the interactable to their respective pools
        switch (i.GetID())
        {
            case (int)ID.Wobbit:
                agent_pool.Add((BaseAgent)i);
                break;
            case (int)ID.WobbitEgg:
                egg_pool.Add((Egg)i);
                break;
            case (int)ID.red_pharomone:
                pheromone_pool.Add((Pheromone)i);
                break;
            case (int)ID.green_pharomone:
                pheromone_pool.Add((Pheromone)i);
                break;
            case (int)ID.blue_pharomone:
                pheromone_pool.Add((Pheromone)i);
                break;
        }
    }

    public Egg InstantiateEgg(Egg egg, Vector3 position, Quaternion rotation, Transform parent)
    {
        // If the pool is empty, create a new egg
        if (egg_pool.Count == 0)
        {
            return Instantiate(egg, position, rotation, parent);
        }
        else
        {
            // Grab the zeroth egg and remove it from the pool
            Egg repurposed_egg = egg_pool[0];
            repurposed_egg.transform.position = position;
            repurposed_egg.transform.rotation = rotation;
            repurposed_egg.gameObject.SetActive(true);
            egg_pool.RemoveAt(0);

            repurposed_egg.genes = null;
            repurposed_egg.parent = null;
            repurposed_egg.initial_population = false;

            return repurposed_egg;
        }
    }

    public BaseAgent InstantiateAgent(BaseAgent agent, Vector3 position, Quaternion rotation, Transform parent)
    {
        // If the pool is empty, create a new agent
        if (agent_pool.Count == 0)
        {
            return Instantiate(agent, position, rotation, parent);
        }
        else
        {
            // Grab the zeroth agent and remove it from the pool
            BaseAgent repurposed_agent = agent_pool[0];
            // Setup the agents position and rotation and clear its observations
            repurposed_agent.transform.SetPositionAndRotation(position, rotation);
            repurposed_agent.gameObject.SetActive(true);
            repurposed_agent.senses.ClearDetection();
            agent_pool.RemoveAt(0);

            return repurposed_agent;
        }
    }
}
