using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A struct to return from Boid
/// </summary>
public struct BoidWrap
{
    public Vector2 velocity;
    public float torque;

    public BoidWrap(Vector2 v, float t)
    {
        velocity = v;
        torque = t;
    }
}

public class BehaviouralNeuron : MonoBehaviour
{
    public static BoidWrap Boid(float cohesion, float separation, float allignment, float avoidance, float matching, List<GameObject> context, GameObject boid, float degree = 1, float torque_force = 0.01f)
    {

        // Initialize the vectors
        Vector2 average_position = Vector2.zero;
        Vector2 average_heading = boid.transform.up;
        Vector2 average_velocity = boid.GetComponent<Rigidbody2D>().velocity;
        Vector2 avoidance_dir = Vector2.zero;

        // The number of other boid agents within the passed boid's avoidance circle
        int avoid = 0;

        // Look at each boid agent
        foreach (GameObject agent in context)
        {
            // increment the average heading and position
            average_position += (Vector2)agent.transform.position;
            average_heading += (Vector2)agent.transform.up;
            average_velocity += agent.GetComponent<Rigidbody2D>().velocity;

            // Increment the number of boids to avoid and adjust the average avoidance direction
            if (Vector2.Distance(boid.transform.position, agent.transform.position) < avoidance)
            {
                avoid++;
                avoidance_dir += (Vector2)(boid.transform.position - agent.transform.position);
            }
        }

        // If we have anything in the avoidance area we divide (avoid infinity by x/0)
        if (avoid > 0)
        {
            avoidance_dir /= avoid;
        }

        // If we have any boid we average
        if (context.Count > 0)
        {
            // Get the average position
            average_position /= context.Count;
            average_heading /= context.Count;
            average_velocity /= context.Count;
        }

        Vector2 dir = BoidBehaviour.Compound(cohesion, separation, allignment, average_position, average_heading, avoidance_dir, boid) * degree;
        Vector2 velocity = BoidBehaviour.Matching(matching, average_velocity, boid) * degree;

        return new BoidWrap(v: velocity, GetTorque(dir, torque_force, boid.transform.up));
    }

    private static float GetTorque(Vector2 dir, float force, Vector2 up)
    {
        //get the angle between transform.forward and target delta
        float angleDiff = Vector3.Angle(up, dir);

        // get its cross product, which is the axis of rotation to
        // get from one vector to the other
        Vector3 cross = Vector3.Cross(up, dir);

        // apply torque along that axis according to the magnitude of the angle.
        return cross.z * angleDiff * force;
    }
}

/// <summary>
/// Handles all boid behaviours
/// </summary>
public static class BoidBehaviour
{
    /// <summary>
    /// Cohesion property of boids
    /// </summary>
    /// <returns></returns>
    public static Vector2 Cohesion(float cohesion, Vector2 average_pos, GameObject boid)
    {
        // If we have no average position, we do not adjust anything cuz we have no neighbours
        if (average_pos == Vector2.zero)
        {
            return average_pos;
        }

        // Get the direction
        Vector2 adjustment = average_pos - (Vector2)boid.transform.position;

        // Return the adjustment
        return adjustment.normalized * cohesion;
    }

    /// <summary>
    /// Separation property of boids
    /// </summary>
    /// <returns></returns>
    public static Vector2 Separation(float separation, Vector2 sep_vec, GameObject boid)
    {
        return sep_vec.normalized * separation;
    }


    /// <summary>
    /// Allignment property of boids
    /// </summary>
    /// <returns></returns>
    public static Vector2 Allignment(float allignment, Vector2 average_heading, GameObject boid)
    {
        return average_heading.normalized * allignment;
    }

    /// <summary>
    /// Adjusts the velocity to match the average
    /// </summary>
    /// <param name="matching"></param>
    /// <param name="average_velocity"></param>
    /// <param name="boid"></param>
    public static Vector2 Matching(float matching, Vector2 average_velocity, GameObject boid)
    {
        return (average_velocity - boid.GetComponent<Rigidbody2D>().velocity).normalized * matching;
        return Vector2.zero;
    }


    /// <summary>
    /// Combination of all defined properties of boids
    /// </summary>
    /// <returns></returns>
    public static Vector2 Compound(float cohesion, float separation, float allignment, Vector2 average_position, Vector2 average_heading, Vector3 avoidance_dir, GameObject boid)
    {
        return Cohesion(cohesion, average_position, boid) + Allignment(allignment, average_heading, boid) + Separation(separation, avoidance_dir, boid);
    }
}
