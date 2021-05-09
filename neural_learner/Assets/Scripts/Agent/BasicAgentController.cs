using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parse;
using Model;

public class BasicAgentController : EvolutionaryNeuralLearner
{

    private Rigidbody rb;
    public List<float> observations;
    public List<string> previous_actions;
    public GameObject food;
    public BasicAgentController base_agent;

    public float movement_speed;
    public float rotation_speed;

    public float clockrate = 0.1f;
    public float internal_clock = 0;
    public float lifespan = 30;
    private float birth_cooldown = 1;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        base_agent = this;
        lifespan = 30;
        transform.position = new Vector3(Random.Range(-25, 25), 0.2f, Random.Range(-25, 25));
    }

    void HeuristicControl()
    {
        if (Input.GetKey(KeyCode.W))
        {
            rb.velocity += rb.transform.forward * movement_speed * Time.fixedDeltaTime;
        }

        if (Input.GetKey(KeyCode.S))
        {
            if (rb.velocity.magnitude < 10)
            {
                rb.velocity += -rb.transform.forward * movement_speed * Time.fixedDeltaTime;
            }
        }

        if (Input.GetKey(KeyCode.D))
        {
            rb.transform.Rotate(new Vector3(0, 1, 0) * rotation_speed * Time.fixedDeltaTime);
        }

        if (Input.GetKey(KeyCode.A))
        {
            rb.transform.Rotate(new Vector3(0, -1, 0) * rotation_speed * Time.fixedDeltaTime);
        }
    }

    void DoStuff(List<float> actions)
    {
        int i = 0;
        if (rb.velocity.magnitude < 8)
        {
            rb.velocity += rb.transform.forward * movement_speed * Time.fixedDeltaTime * actions[i++];
            rb.velocity += -rb.transform.forward * movement_speed * Time.fixedDeltaTime * actions[i++];
        }
        rb.transform.Rotate(new Vector3(0, 1, 0) * rotation_speed * Time.fixedDeltaTime * actions[i++]);
        rb.transform.Rotate(new Vector3(0, -1, 0) * rotation_speed * Time.fixedDeltaTime * actions[i++]);

        i = 0;
        previous_actions.Clear();
        previous_actions.Add("Move Forward: " + ((int)(actions[i++] * 1000)).ToString());
        previous_actions.Add("Move Backward: " + ((int)(actions[i++] * 1000)).ToString());
        previous_actions.Add("Move Left: " + ((int)(actions[i++] * 1000)).ToString());
        previous_actions.Add("Move Right: " + ((int)(actions[i++] * 1000)).ToString());
    }

    // Update is called once per frame
    void Update()
    {
        if (internal_clock <= 0)
        {

            internal_clock = clockrate;

            observations.Clear();
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 10);
            GameObject closest = food;
            float best_dist = -1;
            float num_food_seen = 0;
            foreach (var hitCollider in hitColliders)
            {
                if (Vector3.Distance(transform.position, hitCollider.transform.position) < best_dist && hitCollider.tag.Equals("food") || best_dist == -1 && hitCollider.tag.Equals("food"))
                {
                    best_dist = Vector3.Distance(transform.position, hitCollider.transform.position);
                    closest = hitCollider.gameObject;
                    num_food_seen++;
                }
                else if (hitCollider.tag.Equals("food"))
                {
                    hitCollider.GetComponent<MeshRenderer>().material.color = Color.grey;
                    num_food_seen++;
                }
            }
            closest.GetComponent<MeshRenderer>().material.color = Color.green;

            observations.Add(Vector2.Dot(new Vector2(closest.transform.position.x, closest.transform.position.z), new Vector2(rb.velocity.x, rb.velocity.z)));
            observations.Add(best_dist);
            observations.Add(1 / (1 + num_food_seen));


            observations.Add(lifespan / 30);
            observations.Add(rb.velocity.magnitude / 2);
            observations.Add((transform.eulerAngles.y - 180) / 360);

            DoStuff(GetAction(observations));
            //HeuristicControl();
            if (transform.position.y < -1)
            {
                lifespan = 0;
            }

            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
        else
        {
            internal_clock -= Time.deltaTime;
            lifespan -= Time.deltaTime;
            birth_cooldown -= Time.deltaTime;
        }

        if (lifespan <= 0)
        {
            Destroy(gameObject);
            if (Random.value > 0.2)
            {
                Birth();
            }
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            Mutate();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Time.timeScale += 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Time.timeScale += -0.1f;
        }

        if (transform.position.x > 40)
        {
            transform.position = new Vector3(-38, transform.position.y, transform.position.z);
        }

        if (transform.position.x < -40)
        {
            transform.position = new Vector3(38, transform.position.y, transform.position.z);
        }

        if (transform.position.z < -40)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 38);
        }

        if (transform.position.z < -40)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -38);
        }
    }

    public void Birth()
    {
        if (birth_cooldown <= 0)
        {
            BasicAgentController agent = Instantiate<BasicAgentController>(base_agent, new Vector3(transform.position.x, transform.position.y, transform.position.z + 2), Quaternion.identity);
            agent.model_code = model.GetCode();
            agent.Setup(NeuralNet.Copy(model));
            agent.Mutate();
            agent.GetComponent<BasicAgentController>().enabled = true;
            agent.GetComponent<EvolutionaryNeuralLearner>().enabled = true;

            if (Random.value < 0.01)
            {
                foreach (var x in agent.GetComponentsInChildren<MeshRenderer>())
                {
                    x.material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                }
            }
            else
            {
                foreach (var x in agent.GetComponentsInChildren<MeshRenderer>())
                {
                    x.material.color = GetComponentInChildren<MeshRenderer>().material.color;
                }
            }
            birth_cooldown = 1;
        }
        lifespan += 5;
    }

}
