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


    public void Setup(float dist, int id, Transform t)
    {
        // Set the vision radius 
        vision_distance = dist;
        vision_width = dist / 2;
    }

    public List<float> GetObservations(BaseAgent agent)
    {

        // Create the obs list
        List<float> observations = new List<float>();

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
                // Add the dot product
                observations.Add(Vector3.Dot(transform.up, (i.transform.position - transform.position).normalized));
            }
            else
            {
                observations.Add(0);
            }
        }

        // We normalize the num_* proportionally to each value (min max scaling)
        float max = Mathf.Max(num_agents, num_eggs, num_meats, num_pellets);
        float min = Mathf.Min(num_agents, num_eggs, num_meats, num_pellets);
        float scaled_num_agents = (num_agents - min) / Mathf.Max(max - min, 1);
        float scaled_num_eggs = (num_eggs - min) / Mathf.Max(max - min, 1);
        float scaled_num_meats = (num_meats - min) / Mathf.Max(max - min, 1);
        float scaled_num_pellets = (num_pellets - min) / Mathf.Max(max - min, 1);

        // Add the values found with Sense()
        // The first 8 values will be to do with sight
        // 1-3
        AddTo(closest_pellet, closest_pellet_dist_magnitude, scaled_num_pellets);
        // 4-6
        AddTo(closest_meat, closest_meat_dist_magnitude, scaled_num_meats);
        // 7-9
        AddTo(closest_egg, closest_egg_dist_magnitude, scaled_num_eggs);
        // 10 - 12
        AddTo(closest_agent, closest_agent_dist_magnitude, scaled_num_agents);

        // The next five are all about the closest agent seen
        if (closest_agent != null)
        {
            // Add the code of the other agent
            // 13, 14, 15
            // Note: I have changed this to the colour of the agent for testing
            Vector3 normalized_code = closest_agent.genes.code;
            observations.Add(closest_agent.genes.colour_r);
            observations.Add(closest_agent.genes.colour_g);
            observations.Add(closest_agent.genes.colour_b);
        }
        else
        {
            // If there is nothing, then just add default values
            // Add the code of the other agent
            // 13, 14, 15
            observations.Add(0);
            observations.Add(0);
            observations.Add(0);
        }

        // The next things to add are about the agent's own state
        // Add the agent's energy % (16)
        observations.Add(agent.energy / (2 * agent.max_energy));

        // Add the agent's health % (17)
        observations.Add(Mathf.Clamp(agent.health / agent.max_health, 0f, 1f));

        // Add the agent's lifetime % (18)
        observations.Add(agent.age / agent.lifespan);

        // Add the agent's speed % (18)
        observations.Add(1 - 1 / (1 + agent.speed));

        // Rotation % (20)
        observations.Add(Mathf.Abs(agent.transform.rotation.z));

        // Whether or not the agent has something grabbed (21)
        if (agent.grabbed != null)
        {
            // Add the normalized ID of the object we have grabbed
            observations.Add(agent.grabbed.GetID() / (int)ID.MaxID);
        }
        else
        {
            observations.Add(0);
        }

        // Constant (22)
        observations.Add(1);

        return observations;
    }

    public List<Interactable> Sense()
    {

        // Clear the list of detected game objects
        detected.Clear();

        // Clear all of the closest values
        closest_agent = null;
        closest_egg = null;
        closest_meat = null;
        closest_pellet = null;

        // Reset all distances
        closest_pellet_dist_magnitude = closest_meat_dist_magnitude = closest_egg_dist_magnitude = closest_agent_dist_magnitude = 0;

        // Reset counts
        num_pellets = num_meats = num_eggs = num_agents = 0;

        // Look at stuff!
        foreach (RaycastHit2D c in Physics2D.CircleCastAll(transform.position + transform.up * vision_width / 1.5f, vision_width, transform.up, vision_distance))
        {
            if (c.collider.TryGetComponent<Interactable>(out Interactable obj) && obj.gameObject != this.gameObject)
            {
                detected.Add(obj);

                // Now update the closest values
                // Use a switch case cuz it be faster
                float dist = Vector2.Distance(obj.transform.position, transform.position) / (vision_distance + vision_width * 2f);
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
                        num_agents++;
                        break;
                }
            }
        }

        return detected;
    }

    public string Touch(Collision2D collision, Vector3 facing)
    {
        if (collision.gameObject.TryGetComponent(out Interactable i))
        {
            // If the dot product between the forward vector of an agent and the interactable is greater than 0.5 then it is facing the object
            bool is_facing = Vector3.Dot(facing, (i.transform.position - transform.position).normalized) > 0.5f;

            if (is_facing)
            {
                return "Front";
            }
            else
            {
                return "Back";
            }
        }
        else
        {
            return "Unknown";
        }
    }

    public string Touch(Collider2D collision, Vector3 facing)
    {
        if (collision.gameObject.TryGetComponent(out Interactable i))
        {
            // If the dot product between the forward vector of an agent and the interactable is greater than 0.5 then it is facing the object
            bool is_facing = Vector3.Dot(facing, (i.transform.position - transform.position).normalized) > 0.5f;

            if (is_facing)
            {
                return "Front";
            }
            else
            {
                return "Back";
            }
        }
        else
        {
            return "Unknown";
        }
    }

    public void OnDrawGizmos()
    {
        //Gizmos.DrawWireSphere(transform.position + transform.up * vision_width / 1.5f, vision_width);
        //Gizmos.DrawWireSphere(transform.position + transform.up * vision_distance, vision_width);

        foreach (Interactable i in detected)
        {
            if (i != null)
            {
                Gizmos.DrawLine(transform.position, i.transform.position);
            }
        }

        if (closest_agent != null)
        {
            Gizmos.DrawSphere(closest_agent.transform.position, 0.1f);
        }
        if (closest_meat != null)
        {
            Gizmos.DrawSphere(closest_meat.transform.position, 0.1f);
        }
        if (closest_pellet != null)
        {
            Gizmos.DrawSphere(closest_pellet.transform.position, 0.1f);
        }
        if (closest_egg != null)
        {
            Gizmos.DrawSphere(closest_egg.transform.position, 0.1f);
        }
    }
}
