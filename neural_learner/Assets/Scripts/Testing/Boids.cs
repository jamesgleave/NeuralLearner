using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boids : MonoBehaviour
{
    public List<GameObject> obs = new List<GameObject>();
    public Vector2 boid_vec;
    public Vector2 matching_vec;

    public BoidController controller;

    private Rigidbody2D rb;
    private float speed_bias;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        transform.position = Random.insideUnitCircle * 25;
        speed_bias = 1;
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.up * controller.rad / 1.3f + transform.position, controller.rad);
        foreach (var x in cols)
        {
            if (x.gameObject != gameObject)
            {
                obs.Add(x.gameObject);
            }
        }

        BoidWrap moves = BehaviouralNeuron.Boid(controller.co, controller.sep, controller.al, controller.avoidance, controller.matching, obs, gameObject);
        rb.AddTorque(moves.torque);
        rb.velocity += moves.velocity;
        matching_vec = moves.velocity;
        obs.Clear();

        rb.velocity = transform.up * controller.speed * speed_bias;
        rb.angularVelocity = Mathf.Clamp(rb.angularVelocity, 0, controller.speed);

        if (transform.position.magnitude > 100)
        {
            transform.position = -transform.position + transform.up * 10;
        }


    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.up * controller.rad / 1.3f + transform.position, controller.rad);
    }
}
