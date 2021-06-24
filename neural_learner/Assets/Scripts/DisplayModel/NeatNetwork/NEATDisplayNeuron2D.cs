using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NEATDisplayNeuron2D : NEATDisplayNeuron
{
    public void UpdateMovement(float speed, float acc = 0.3f)
    {
        // Find the distance and direction
        float dist = Vector3.Distance(desired_position, transform.localPosition);
        Vector3 dir = desired_position - transform.localPosition;

        // Calculate the desired force
        Vector3 force = dir.normalized * acc * dist;

        // Add force to neuron rb
        rb.AddForce(force, ForceMode2D.Impulse);

        // Clamp the velocity
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, speed);

        // If we are pretty much not moving then stop completely
        if (Mathf.Approximately(rb.velocity.magnitude, 0.0f) || rb.velocity.magnitude < 0)
        {
            rb.velocity = Vector2.zero;
            moving = false;
        }
        else
        {
            moving = true;
        }
    }

    public void CalculateDesiredPosition()
    {
        // Check if the layer size is even or odd
        float even_odd_offset = 0;
        if (depth_count % 2 == 0)
        {
            // If the layer is even then add an offset
            even_odd_offset = display.alpha * 0.5f;
        }

        float average_y = 0; float average_x = 0;
        float x, y, z;
        if (children.Count > 0 && parents.Count > 0)
        {
            foreach (var p in parents)
            {
                average_y += p.transform.localPosition.y;
                average_x += p.transform.localPosition.x;
            }

            foreach (var c in children)
            {
                average_y += c.transform.localPosition.y;
                average_x += c.transform.localPosition.x;
            }

            average_y /= parents.Count + children.Count;
            average_x /= parents.Count + children.Count;
            // Set up the basic coords
            // Set up the basic coords
            x = -neuron.GetDepth() * display.lambda;
            y = (position - depth_count / 2) * display.alpha + even_odd_offset;
            x += average_x;
            y += average_y;
            x /= 2;
            y /= 2;
            z = 0;
        }
        else
        {
            // Set up the basic coords
            x = -neuron.GetDepth() * display.lambda;
            y = (position - depth_count / 2) * display.alpha + even_odd_offset;
            z = 0;
        }

        // set the desired position
        desired_position.Set(x, y, z);
    }

    /// <summary>
    /// Reads the neuron and updates the values associated with the neuron as well as other internal values
    /// </summary>
    public override void UpdateInternalState()
    {
        base.UpdateInternalState();

        UpdateMovement(display.gamma, display.phi);
        CalculateDesiredPosition();
        DrawLine(false);
        UpdateSize();
    }
}
