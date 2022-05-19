using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Senses : MonoBehaviour
{

    public float vision_width;
    public float vision_distance;
    public List<Interactable> detected;

    [Space]
    public FoodPellet closest_pellet;
    public float closest_pellet_dist_magnitude;
    public int num_pellets;

    [Space]
    public Meat closest_meat;
    public float closest_meat_dist_magnitude;
    public int num_meats;

    [Space]
    public Egg closest_egg;
    public float closest_egg_dist_magnitude;
    public int num_eggs;

    [Space]
    public BaseAgent closest_agent;
    public float closest_agent_dist_magnitude;
    public int num_agents;

    [Space]
    public Pheromone red;
    public Pheromone green;
    public Pheromone blue;
    public float red_distance = 0, blue_distance = 0, green_distance = 0;

    // The list of observations
    public List<float> observations = new List<float>();
    public List<string> observation_names = new List<string>();

    // The buffer for the circlecast
    public RaycastHit2D[] buffer;
    [Tooltip("The amount of time until the agent forgets something")]
    public float sense_memory;

    ContactFilter2D filter;

    /// <summary>
    /// The list of agents seen (used for behaviour)
    /// </summary>
    [Space]
    public List<GameObject> agent_context;

    /// <summary>
    /// A static variable to hold which observation maps to which index of the observation list.
    /// </summary>
    public static Dictionary<string, int> observation_indices;

    /// <summary>
    /// Weights assosiated with sensory connections for mutations (defaulted to 1/length)
    /// </summary>
    public static Dictionary<string, float> observation_weights;

    public void Setup(float dist, int id, Transform t)
    {
        // Set the vision radius 
        vision_distance = dist;
        vision_width = dist / 2;

        // Setup the names of all observations in the right order (the ith observation lines up with the ith observation_name)
        SetupNames();

        // Create the buffer
        buffer = new RaycastHit2D[20];
        filter = new ContactFilter2D();
        //sense_memory = Mathf.Log(GetComponent<BaseAgent>());
    }

    public void SetupNames()
    {
        // Clear the names
        observation_names.Clear();

        // Add observation names for the pellets
        observation_names.Add("Distance To Closest Pellet");
        observation_names.Add("Number Of Pellets Seen");
        observation_names.Add("Is Facing Closest Pellet");

        // Add observation names for the meat
        observation_names.Add("Distance To Closest Meat");
        observation_names.Add("Number Of Meat Seen");
        observation_names.Add("Is Facing Closest Meat");

        // Add observation names for the Agent
        observation_names.Add("Distance To Closest Agent");
        observation_names.Add("Number Of Agents Seen");
        observation_names.Add("Is Facing Closest Agent");

        // The next 4 are all about the closest agent seen
        observation_names.Add("Red Value Of Closest Agent Seen");
        observation_names.Add("Green Value Of Closest Agent Seen");
        observation_names.Add("Blue Value Of Closest Agent Seen");
        observation_names.Add("Color Difference Between Self & Closest Agent");

        // Add observation names for the egg
        observation_names.Add("Distance To Closest Egg");
        observation_names.Add("Number Of Eggs Seen");
        observation_names.Add("Is Facing Closest Egg");
        observation_names.Add("Color Difference Between Self & Closest Egg");


        // Next are all about the agent's state
        observation_names.Add("Agent's Energy");
        observation_names.Add("Agent's Health");
        observation_names.Add("Agent's Lifespan");
        observation_names.Add("Agent's Speed");
        observation_names.Add("Agent's Rotation");
        observation_names.Add("Agent Has Grabbed Object");

        // Pheromones
        observation_names.Add("Is Facing Red Pheromone");
        observation_names.Add("Is Facing Green Pheromone");
        observation_names.Add("Is Facing Blue Pheromone");

        // The constant value
        observation_names.Add("Constant Value");
    }

    public List<float> GetObservations(BaseAgent agent)
    {
        // Clear obs
        observations.Clear();

        // An embedded funciton to add things to our observation list
        void AddTo(Interactable i, float dist, float scaled_magnitude)
        {
            // Add the distance
            observations.Add(dist);

            // Add the number of interactables found (scaled)
            observations.Add(scaled_magnitude);

            // If i is not null, we add the angle too, but if it is null then we add 0
            if (i != null)
            {
                // Add the normalized angle (between -1 and 1)
                observations.Add(Vector3.SignedAngle(transform.up, i.transform.position - transform.position, transform.forward) / 180f);
            }
            else
            {
                observations.Add(0);
            }
        }

        // The max number of this seen is the size of the buffer array used to store the collisions from the overlap circle and therefore we can use its lenght to normalize each value
        float scaled_num_agents = num_agents / (float)buffer.Length;
        float scaled_num_eggs = num_eggs / (float)buffer.Length;
        float scaled_num_meats = num_meats / (float)buffer.Length;
        float scaled_num_pellets = num_pellets / (float)buffer.Length;

        // Add the values found with Sense()
        // The first 8 values will be to do with sight
        // 1-3
        AddTo(closest_pellet, closest_pellet_dist_magnitude, scaled_num_pellets);
        // 4-6
        AddTo(closest_meat, closest_meat_dist_magnitude, scaled_num_meats);
        // 7-9
        AddTo(closest_agent, closest_agent_dist_magnitude, scaled_num_agents);
        // The next 3 are all about the closest agent seen
        if (closest_agent != null)
        {
            // 10-11-12-13
            // Note: I have changed this to the colour of the agent for testing
            observations.Add(closest_agent.genes.colour_r);
            observations.Add(closest_agent.genes.colour_g);
            observations.Add(closest_agent.genes.colour_b);
            observations.Add(closest_agent.genes.ComputeColorDifference(agent.genes));
        }
        else
        {
            // If there is nothing, then just add default values
            // Add the code of the other agent
            //  10-11-12-13
            observations.Add(0);
            observations.Add(0);
            observations.Add(0);
            observations.Add(0);
        }
        // 14-15-16
        AddTo(closest_egg, closest_egg_dist_magnitude, scaled_num_eggs);

        // The next is the colour difference between the closest egg and the agent
        // 17
        if (closest_egg != null)
        {
            observations.Add(closest_egg.genes.ComputeColorDifference(agent.genes));
        }
        else
        {
            observations.Add(0);
        }

        // The next things to add are about the agent's own state
        // Add the agent's energy % (18)
        observations.Add(agent.energy / (2 * agent.max_energy));

        // Add the agent's health % (19)
        observations.Add(Mathf.Clamp(agent.health / agent.max_health, 0f, 1f));

        // Add the agent's lifetime % (20)
        observations.Add(agent.age / agent.lifespan);

        // Add the agent's speed % (21)
        observations.Add(1 - 1 / (1 + agent.speed));

        // Rotation % (22)
        observations.Add(Mathf.Abs(agent.transform.rotation.z));

        // Whether or not the agent has something grabbed (23)
        if (agent.grabbed != null)
        {
            // Let the agent know it has something grabbed
            observations.Add(1);
        }
        else
        {
            observations.Add(0);
        }

        // 24
        // Add red pheromone
        if (red != null)
        {
            observations.Add(Vector3.SignedAngle(transform.up, red.transform.position - transform.position, transform.forward) / 180f);
        }
        else
        {
            observations.Add(0);
        }

        // 25
        // Add green pheromone
        if (green != null)
        {
            observations.Add(Vector3.SignedAngle(transform.up, green.transform.position - transform.position, transform.forward) / 180f);
        }
        else
        {
            observations.Add(0);
        }

        // 26
        // Add blue pheromone
        if (blue != null)
        {
            observations.Add(Vector3.SignedAngle(transform.up, blue.transform.position - transform.position, transform.forward) / 180f);
        }
        else
        {
            observations.Add(0);
        }

        // Constant 27
        observations.Add(1);
        return observations;
    }

    public List<Interactable> Sense()
    {

        // Clear the list of detected game objects
        detected.Clear();

        // Clear the agents seen
        agent_context.Clear();

        // Clear the buffer
        System.Array.Clear(buffer, 0, buffer.Length);

        // Clear all of the closest values
        closest_agent = null;
        closest_egg = null;
        closest_meat = null;
        closest_pellet = null;
        red = null;
        green = null;
        blue = null;

        // Reset all distances
        closest_pellet_dist_magnitude = closest_meat_dist_magnitude = closest_egg_dist_magnitude = closest_agent_dist_magnitude = red_distance = green_distance = blue_distance = 0;

        // Reset counts
        num_pellets = num_meats = num_eggs = num_agents = 0;

        // Look at stuff!
        Physics2D.CircleCast(origin: transform.position + transform.up * vision_width, radius: vision_width, direction: transform.up, distance: vision_distance, results: buffer, contactFilter: filter.NoFilter());
        for (int i = 0; i < buffer.Length; i++)
        {
            // Grab the object from the buffer
            RaycastHit2D c = buffer[i];

            // If we have a null value, skip!
            if (c.collider == null)
            {
                continue;
            }

            // Define the game object we are looking at
            // We only detect the agent's body... This is to avoid adding it twice to the detected list (avoid checking as well)
            GameObject g;
            if (c.collider.CompareTag("Body"))
            {
                // Check if we have collided with an agent's body component and if so we must look at the parent (the agent)
                g = c.collider.transform.parent.gameObject;
            }
            else
            {
                // If not, then it is something else and we close with g just being equal to the colliders game object
                g = c.collider.gameObject;
            }

            if (g.TryGetComponent<Interactable>(out Interactable obj) && obj.gameObject != this.gameObject)
            {
                // Now update the closest values
                // Use a switch case cuz it be faster
                float raw_dist = Vector2.Distance(obj.transform.position, transform.position);
                float dist = raw_dist / (vision_distance + vision_width * 2f);
                detected.Add(obj);

                switch ((ID)obj.GetID())
                {
                    case ID.FoodPellet:
                        // If the closest food pellet is null then take the first option
                        // If it is not null, then we should check againts the stored value
                        if (closest_pellet == null || dist < closest_pellet_dist_magnitude)
                        {
                            closest_pellet = (FoodPellet)obj;
                            closest_pellet_dist_magnitude = dist;
                        }
                        num_pellets++;
                        break;
                    case ID.Meat:
                        // If the closest food pellet is null then take the first option
                        // If it is not null, then we should check againts the stored value
                        if (closest_meat == null || dist < closest_meat_dist_magnitude)
                        {
                            closest_meat = (Meat)obj;
                            closest_meat_dist_magnitude = dist;
                        }
                        num_meats++;
                        break;
                    case ID.WobbitEgg:
                        // If the closest food pellet is null then take the first option
                        // If it is not null, then we should check againts the stored value
                        if (closest_egg == null || dist < closest_egg_dist_magnitude)
                        {
                            closest_egg = (Egg)obj;
                            closest_egg_dist_magnitude = dist;
                        }
                        num_eggs++;
                        break;
                    case ID.Wobbit:
                        // If the closest food pellet is null then take the first option
                        // If it is not null, then we should check againts the stored value
                        if (closest_agent == null || dist < closest_agent_dist_magnitude)
                        {
                            closest_agent = (BaseAgent)obj;
                            closest_agent_dist_magnitude = dist;
                        }
                        // Add the gameobject to the list
                        agent_context.Add(obj.gameObject);
                        // Increment the number of agents seen
                        num_agents++;
                        break;
                    case ID.red_pharomone:
                        if (red == null || dist < red_distance)
                        {
                            red = (Pheromone)obj;
                            red_distance = dist;
                        }
                        break;
                    case ID.green_pharomone:
                        if (green == null || dist < green_distance)
                        {
                            green = (Pheromone)obj;
                            green_distance = dist;
                        }
                        break;
                    case ID.blue_pharomone:
                        if (blue == null || dist < blue_distance)
                        {
                            blue = (Pheromone)obj;
                            blue_distance = dist;
                        }
                        break;
                }
            }
        }

        return detected;
    }

    public void ClearDetection()
    {
        detected.Clear();
        observations.Clear();
    }

    public void OnDrawGizmos()
    {
        for (int i = 0; i < detected.Count; i++)
        {
            if (detected[i] != null)
            {
                Gizmos.DrawLine(transform.position, detected[i].transform.position);
            }
        }
    }
}
