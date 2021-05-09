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
        void AddTo(Interactable i, float dist)
        {
            // Add the distance
            observations.Add(dist);

            // If i is not null, we add the angle too, but if it is null then we add 0
            if (i != null)
            {
                // Add the dot product normalized between 0 and 1
                observations.Add((Vector3.Dot(transform.up, (i.transform.position - transform.position).normalized) + 1) / 2);
            }
            else
            {
                observations.Add(0);
            }
        }

        // Add the values found with Sense()
        // The first 8 values will be to do with sight
        // 1-2
        AddTo(closest_pellet, closest_pellet_dist_magnitude);
        // 2-4
        AddTo(closest_meat, closest_meat_dist_magnitude);
        // 4-6
        AddTo(closest_egg, closest_egg_dist_magnitude);
        // 7 - 8
        AddTo(closest_agent, closest_agent_dist_magnitude);

        // The next five are all about the closest agent seen
        if (closest_agent != null)
        {
            // Add the code of the other agent
            // 8, 9, 10
            observations.Add(closest_agent.genes.code.x);
            observations.Add(closest_agent.genes.code.y);
            observations.Add(closest_agent.genes.code.z);

            // Add the size and health for the agent
            // 11, 12
            observations.Add(closest_agent.genes.size);
            observations.Add(closest_agent.health / closest_agent.max_health);
        }
        else
        {
            // If there is nothing, then just add default values
            // Add the code of the other agent
            // 8, 9, 10
            observations.Add(0);
            observations.Add(0);
            observations.Add(0);

            // Add the size and health for the agent
            // 11, 12
            observations.Add(0);
            observations.Add(0);
        }

        // The next things to add are about the agent's own state
        // Add the agent's energy % (13)
        observations.Add(agent.energy / (2 * agent.max_energy));

        // Add the agent's health % (14)
        observations.Add(Mathf.Clamp(agent.health / agent.max_health, 0f, 1f));

        // Add the agent's lifetime % (15)
        observations.Add(agent.age / agent.lifespan);

        // Add the agent's speed % (16)
        observations.Add(1 - 1 / (1 + agent.speed));

        // Rotation % (17)
        observations.Add(Mathf.Abs(agent.transform.rotation.z));

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
        closest_pellet_dist_magnitude = closest_meat_dist_magnitude = closest_egg_dist_magnitude = closest_agent_dist_magnitude = 1;

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

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + transform.up * vision_width / 1.5f, vision_width);
        Gizmos.DrawWireSphere(transform.position + transform.up * vision_distance, vision_width);

        foreach (Interactable i in detected)
        {
            Gizmos.DrawLine(transform.position, i.transform.position);
        }
    }
}
