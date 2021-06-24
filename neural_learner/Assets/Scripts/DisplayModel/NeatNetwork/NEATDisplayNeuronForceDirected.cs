using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Neuron that produces shapes using a force directed procedure
/// </summary>
public class NEATDisplayNeuronForceDirected : NEATDisplayNeuron
{
    [Header("The Force Strength:")]
    /// <summary>
    /// The strength of the attraction/repultion force
    /// </summary>
    public float force_strength;

    [Header("The Length Of Each Link:")]
    /// <summary>
    /// The strength of links
    /// </summary>
    public float link_length;

    [Header("Each node this neuron is not attatched to:")]
    /// <summary>
    /// The strength of links
    /// </summary>
    public List<NEATDisplayNeuron> repelling;
    public float repulse_radius;
    public float repulse_force;
    private bool repelling_setup = false;

    public float gravity_scale;

    public void Attraction()
    {
        // Loop through each child node
        foreach (NEATDisplayNeuron child in children)
        {
            Vector3 force_dir = child.transform.position - transform.position;
            float dist_sqr = force_dir.sqrMagnitude;

            // If the squared distance is greater than the link length, add impulse
            if (dist_sqr > link_length * link_length)
            {
                // set up the direction of movement
                float dist_sqr_norm = dist_sqr / (link_length * link_length);
                Vector3 target_rb_impulse = -force_dir.normalized * force_strength * dist_sqr_norm;
                Vector3 source_rb_impulse = force_dir.normalized * force_strength * dist_sqr_norm;

                // Add force to child and this
                child.GetRB().AddForce(target_rb_impulse * Mathf.Abs(neuron.GetOutput()), ForceMode2D.Impulse);
                rb.AddForce(source_rb_impulse * Mathf.Abs(neuron.GetOutput()), ForceMode2D.Impulse);
            }
        }
    }

    public void Repulse()
    {
        // test which node in within forceSphere.
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(this.transform.position, repulse_radius);

        // only apply force to nodes within forceSphere, with Falloff towards the boundary of the Sphere and no force if outside Sphere.
        foreach (Collider2D hitCollider in hitColliders)
        {
            Rigidbody2D hitRb = hitCollider.attachedRigidbody;

            if (hitRb != null && hitRb != rb)
            {
                Vector3 direction = hitCollider.transform.position - this.transform.position;
                float distSqr = direction.sqrMagnitude;

                // Normalize the distance from forceSphere Center to node into 0..1
                float impulseExpoFalloffByDist = Mathf.Clamp(1 - (distSqr / repulse_radius), 0, 1);

                // apply normalized distance
                hitRb.AddForce(direction.normalized * repulse_force * impulseExpoFalloffByDist * Mathf.Abs(neuron.GetOutput()));
            }
        }
    }

    public void Gravity()
    {
        // Apply global gravity pulling node towards center of universe
        Vector3 dirToCenter = -this.transform.position;
        Vector3 impulse = dirToCenter.normalized * rb.mass * gravity_scale;
        rb.AddForce(impulse);
    }

    public void Update()
    {
        TrySettingUpRepelling();
        Repulse();
        Attraction();
        Gravity();
    }

    public void TrySettingUpRepelling()
    {
        // Set up the repelling list
        if (repelling_setup == false)
        {
            // Setup all nodes that this is not attached to
            repelling = new List<NEATDisplayNeuron>();
            repelling.AddRange(display.input_neurons);
            repelling.AddRange(display.hidden_neurons);
            repelling.AddRange(display.output_neurons);
            // Remove each child from the list
            //foreach (var c in children)
            //{
            //    var sj = gameObject.AddComponent<SpringJoint2D>();
            //    sj.connectedBody = c.GetRB();
            //    sj.enableCollision = true;
            //}
            repelling_setup = true;
        }
    }

    public override void OnSetup()
    {
        // Give initial force to move them all apart
        var big_bang_force = Random.insideUnitCircle * Random.Range(-500, 500);
        print(big_bang_force);
        rb.AddForce(big_bang_force, ForceMode2D.Impulse);
    }

    public override void UpdateInternalState()
    {
        base.UpdateInternalState();
        DrawLine(false);
        UpdateSize();
    }
}
