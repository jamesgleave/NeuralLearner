using System.IO.Compression;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Senses : MonoBehaviour
{

    public float vision_distance;
    public float field_of_view;
    public List<Interactable> detected;

    [Space]
    public FoodPellet closest_pellet;
    public float closest_pellet_dist_magnitude;
    private float facing_direction_with_most_pellets;
    private Vector3 average_pellet_position_in_observation;
    public float is_facing_direction_with_most_pellets;
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

    // The buffer for the circle cast
    public Collider2D[] buffer;
    public int buffer_size;
    [Tooltip("The amount of time until the agent forgets something")]
    public float sense_memory;

    ContactFilter2D filter;

    /// <summary>
    /// The list of agents seen (used for behaviour)
    /// </summary>
    [Space]
    public List<GameObject> agent_context;

    /// <summary>
    /// Weights assosiated with sensory connections for mutations (defaulted to 1/length)
    /// </summary>
    public static Dictionary<string, float> observation_weights;

    // Static values used by other classes
    public enum NEATNeuronIndices{
        DistanceToClosestPellet = 0,
        NumberOfPelletsSeen = 1,
        IsFacingClosestPellet = 2,
        DistanceToClosestMeat = 3,
        NumberOfMeatSeen = 4,
        IsFacingClosestMeat = 5,
        DistanceToClosestAgent = 6,
        NumberOfAgentsSeen = 7,
        IsFacingClosestAgent = 8,
        RedValueOfClosestAgentSeen = 9,
        GreenValueOfClosestAgentSeen = 10,
        BlueValueOfClosestAgentSeen = 11,
        ColorDifferenceBetweenSelfAndClosestAgent = 12,
        DistanceToClosestEgg = 13,
        NumberOfEggsSeen = 14,
        IsFacingClosestEgg = 15,
        ColorDifferenceBetweenSelfAndClosestEgg = 16,
        AgentsEnergy = 17,
        AgentsHealth = 18,
        AgentsLifespan = 19,
        AgentsSpeed = 20,
        AgentsRotation = 21,
        AgentHasGrabbedObject = 22,

        IsFacingRedPheromone = 23,
        RedPheromoneStrength = 24,
        IsFacingGreenPheromone = 25,
        GreenPheromoneStrength = 26,
        IsFacingBluePheromone = 27,
        BluePheromoneStrength = 28,

        ConstantValue = 29,

        AgentFacingDirectionWithMostPellets = 30,
        RelativeHeadingOfClosestAgent = 31,
        RelativeSizeOfClosestAgent = 32,
        AgentsFullness = 33,

        // Add the indices for the outputs as well
        MoveForward = 34,
        Rotate = 35,
        WantsToReproduce = 36,
        WantsToEat = 37,
        WantsToGrab = 38,
        WantsToAttack = 39,
        ProduceRedPheromone = 40,
        ProduceGreenPheromone = 41,
        ProduceBluePheromone = 42,
    }

    public void Setup(float dist, float fov, int id, Transform t)
    {
        // Set the vision radius 
        vision_distance = dist;

        // Set up the FOV
        // 1.6log(2.4x+1)
        field_of_view = 1.6f * Mathf.Log10(1 + 2.4f * fov);

        // Create the buffer
        buffer_size = (int)(vision_distance * 10);
        buffer = new Collider2D[buffer_size];
    }

    public List<float> GetObservations(BaseAgent agent)
    {
        // Clear obs
        observations.Clear();

        // An embedded funciton to add things to our observation list
        void AddTo(Interactable i, float dist, float scaled_magnitude)
        {
            // Add the distance
            // TODO testing the inverse (increase signal as the agent gets closer)
            observations.Add(1f - Mathf.Clamp01(dist));

            // Add the number of interactables found (scaled)
            observations.Add(scaled_magnitude);

            // If i is not null, we add the angle too, but if it is null then we add 0
            if (i != null)
            {
                // Add the normalized angle (between -1 and 1)
                // Multiply by the field of view to get a range between 0 and 1 (where in the field of view the object is)
                observations.Add((Vector3.SignedAngle(transform.up, i.transform.position - transform.position, transform.forward) / 180f) / field_of_view);
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
            // 1 if they are the same, 0 if they are opposites
            observations.Add(1f - closest_egg.genes.ComputeColorDifference(agent.genes));
        }
        else
        {
            observations.Add(0);
        }

        // The next things to add are about the agent's own state
        // Add the agent's energy % (18)
        observations.Add(agent.energy / agent.max_energy);

        // Add the agent's health % (19)
        observations.Add(Mathf.Clamp(agent.health / agent.max_health, 0f, 1f));

        // Add the agent's lifetime % (20)
        observations.Add(agent.age / agent.lifespan);

        // Add the agent's speed % (21)
        observations.Add(1 - 1 / (1 + agent.speed));

        // Rotation % (22)
        observations.Add((agent.transform.rotation.z + 1f) / 2f);

        // Whether or not the agent has something grabbed (23)
        if (agent.grabbed != null)
        {
            // Let the agent know it has something grabbed
            observations.Add(1);
        }
        else
        {
            observations.Add(-1);
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
        observations.Add(red_distance);

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
        observations.Add(green_distance);

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
        observations.Add(blue_distance);

        // Constant 27
        observations.Add(1);

        // Extra observations

        // Is the agent facing the direction with the most detected pellets? (28)
        is_facing_direction_with_most_pellets = Vector3.SignedAngle(transform.up, average_pellet_position_in_observation - transform.position, transform.forward) / 180f;
        observations.Add(is_facing_direction_with_most_pellets);

        // Add the relative heading of the closest agent (the angle between the closest agent and this agent) (29)
        // Add closest agent's size ratio (this agent's size / closest agent's size) (30)
        if (closest_agent != null)
        {
            observations.Add(1f - ((Vector3.Dot(transform.up, closest_agent.transform.up) + 1) / 2));

            // If this agent is larger, the value will approach zero, else it will approach one
            observations.Add(1f / (Mathf.Exp(-(closest_agent.transform.localScale.x / agent.transform.localScale.x)) + 1));
        }
        else
        {
            observations.Add(0);
            observations.Add(0);
        }

        // Add agent's stomach fullness ratio (31)
        observations.Add(agent.stomach.GetFullnessPercentage());

        return observations;
    }

    public List<Interactable> Sense()
    {
        // Initialize dist (temp variable we use to store distances)
        float dist;

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
        closest_pellet_dist_magnitude = closest_meat_dist_magnitude = closest_egg_dist_magnitude = closest_agent_dist_magnitude = 1;

        // Pheromones start at zero because they are "get stronger as you get closer"
        red_distance = green_distance = blue_distance = 0;

        // Reset counts
        num_pellets = num_meats = num_eggs = num_agents = 0;

        // Reset the average position of all pellets seen and if it is looking at the most pellets
        average_pellet_position_in_observation = Vector3.zero;
        facing_direction_with_most_pellets = 0;

        // Look at stuff!
        Physics2D.OverlapCircleNonAlloc(point: transform.position, radius: vision_distance, results: buffer);
        for (int i = 0; i < buffer.Length; i++)
        {
            // Grab the object from the buffer
            Collider2D c = buffer[i];

            // If it is not null
            if(c == null){continue;}

            // Check if the object is within the agent's field of view
            if(Vector3.SignedAngle(transform.up, c.transform.position - transform.position, transform.up) / 180f > field_of_view) 
            {
                // If the object is outside of the fov, still check if it is a pheromone
                if(c.TryGetComponent<Pheromone>(out Pheromone pheromone)){
                    
                    // Find the scaled distance of the pheromone
                    dist = Vector3.Distance(transform.position, pheromone.transform.position) / vision_distance;

                    // Add the pheromone to the list of detected
                    detected.Add(pheromone);

                    // If it is a pheromone, check if it is red
                    if(pheromone.type == PheromoneType.red && (red == null || dist < red_distance)){
                        // If it is red, set the red pheromone to the pheromone
                        red = pheromone;

                        // Set the red distance to the distance between the agent and the pheromone
                        // The 1 - dist is because the strength of the pheromone is inverse to the distance
                        red_distance = 1f - dist;
                    }
                    // If it is a pheromone, check if it is green
                    else if(pheromone.type == PheromoneType.green && (green == null || dist < green_distance)){
                        // If it is green, set the green pheromone to the pheromone
                        green = pheromone;

                        // Set the green distance to the distance between the agent and the pheromone
                            // The 1 - dist is because the strength of the pheromone is inverse to the distance
                        green_distance = 1f - dist;
                    }
                    // If it is a pheromone, check if it is blue
                    else if(pheromone.type == PheromoneType.blue && (blue == null || dist < blue_distance)){
                        // If it is blue, set the blue pheromone to the pheromone
                        blue = pheromone;

                        // Set the blue distance to the distance between the agent and the pheromone
                        // The 1 - dist is because the strength of the pheromone is inverse to the distance
                        blue_distance = 1f - dist;
                    }
                }
                continue;
            }

            // Define the game object we are looking at
            // We only detect the agent's body... This is to avoid adding it twice to the detected list (avoid checking as well)
            GameObject g;
            if (c.CompareTag("Body"))
            {
                // Check if we have collided with an agent's body component and if so we must look at the parent (the agent)
                g = c.transform.parent.gameObject;
            }
            else
            {
                // If not, then it is something else and we close with g just being equal to the colliders game object
                g = c.gameObject;
            }

            if (g.TryGetComponent<Interactable>(out Interactable obj) && obj.gameObject != this.gameObject)
            {
                // Now update the closest values
                // Use a switch case cuz it be faster
                float raw_dist = Vector2.Distance(obj.transform.position, transform.position);
                dist = raw_dist / vision_distance;
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
                        average_pellet_position_in_observation += obj.transform.position;
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

        // Create an average position for the pellets
        if(num_pellets > 0) average_pellet_position_in_observation /= num_pellets;

        return detected;
    }

    public void ClearDetection()
    {
        detected.Clear();
        observations.Clear();
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for (int i = 0; i < detected.Count; i++)
        {
            if (detected[i] != null)
            {
                Gizmos.DrawLine(transform.position, detected[i].transform.position);
            }
        }


        float angle = 360 * (1 - field_of_view);
        float rayRange = vision_distance;
        float halfFOV = angle / 2.0f;
        float coneDirection = 180;

        Quaternion upRayRotation = Quaternion.AngleAxis(-halfFOV + coneDirection, Vector3.forward);
        Quaternion downRayRotation = Quaternion.AngleAxis(halfFOV + coneDirection, Vector3.forward);

        Vector3 upRayDirection = upRayRotation * transform.up * rayRange;
        Vector3 downRayDirection = downRayRotation * transform.up * rayRange;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, upRayDirection);
        Gizmos.DrawRay(transform.position, downRayDirection);

        // Add a ball to show the direction facing the most pellets
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(average_pellet_position_in_observation, Vector3.one * 0.3f);
        
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, vision_distance);
    }
}
