using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BaseAgent : Interactable
{
    [Header("Initial Agent Settings")]
    /// <summary>
    ///   <para>The base health of all agents</para>
    /// </summary>
    [Range(0.0f, 500.0f)]
    public float base_health = 50;

    /// <summary>
    ///   <para>The base attack of all agents</para>
    /// </summary>
    [Range(0.0f, 500.0f)]
    public float base_attack = 10;

    /// <summary>
    ///   <para>The base defense of all agents</para>
    /// </summary>
    [Range(0.0f, 500.0f)]
    public float base_defense = 10;

    /// <summary>
    ///   <para>The base speed of all agents</para>
    /// </summary>
    [Range(0.0f, 100f)]
    public float base_speed = 10;

    /// <summary>
    ///   <para>The base speed of all agents</para>
    /// </summary>
    [Range(0.0f, 10)]
    public float base_rotation_speed = 3;

    /// <summary>
    ///   <para>The base perception of all agents</para>
    /// </summary>
    [Range(0.0f, 100f)]
    public float base_perception = 5;

    /// <summary>
    ///   <para>The base gestation time (time spent in egg) of all agents</para>
    /// </summary>
    [Range(0.0f, 100f)]
    public float base_gestation_time = 3;

    /// <summary>
    ///   <para>The base lifespan for all agents</para>
    /// </summary>
    [Range(0.0f, 1000f)]
    public float base_lifespan = 60;

    /// <summary>
    ///   <para>The base mass for all agents</para>
    /// </summary>
    [Range(0.0f, 25)]
    public float base_mass = 1;

    /// <summary>
    ///   <para>The base rate at which an agent can consume food (energy/second). As an agent approaching a size of zero, the rate which they consume energy approaches this value</para>
    /// </summary>
    [Range(0.0f, 10f)]
    public float base_consumption_rate = 1;


    [Header("Agent Decisions")]
    /// <summary>
    ///   <para>Whether the agent wants to breed or not</para>
    /// </summary>
    public bool wants_to_breed;

    /// <summary>
    ///   <para>Whether the agent wants to eat or not</para>
    /// </summary>
    public bool wants_to_eat;

    /// <summary>
    ///   <para>Whether the agent wants to grab something or not</para>
    /// </summary>
    public bool wants_to_grab;

    // testing
    public List<float> inf = new List<float>();


    [Header("Aquired Agent Attributes")]
    /// <summary>
    ///   <para>The genus name</para>
    /// </summary>
    public string genus;
    /// <summary>
    ///   <para>The species name</para>
    /// </summary>
    public string species;

    /// <summary>
    ///   <para>The name of the first entity of this agents family tree</para>
    /// </summary>
    public string original_name;
    [Space()]

    /// <summary>
    ///   <para>The Genes of this agent</para>
    /// </summary>
    public Genes genes;

    /// <summary>
    ///   <para>The Manager for this simulation</para>
    /// </summary>
    public Manager manager;

    /// <summary>
    ///   <para>The meat object spawned in when this creature dies</para>
    /// </summary>
    public Meat meat;

    /// <summary>
    ///   <para>The egg of the agent</para>
    /// </summary>
    public Egg egg;
    /// <summary>
    ///   <para>The location where an egg comes from the agent</para>
    /// </summary>
    public Transform egg_location;

    /// <summary>
    ///   <para>The tool used by the agent to sense its environment</para>
    /// </summary>
    public Senses senses;

    /// <summary>
    ///   <para>The agent's brain</para>
    /// </summary>
    public Brain brain;

    /// <summary>
    ///   <para>The agent's object that it has grabbed!</para>
    /// </summary>
    public Interactable grabbed;

    [Space()]
    /// <summary>
    ///   <para>The value that control the speed at which the agent can move</para>
    /// </summary>
    public float movement_speed;
    /// <summary>
    ///   <para>The value that control the speed at which the agent can rotate</para>
    /// </summary>
    public float rotation_speed;
    /// <summary>
    ///   <para>The speed of the agent</para>
    /// </summary>
    public float speed;

    [Space()]
    /// <summary>
    ///   <para>The health of the agent</para>
    /// </summary>
    public float health;
    public float max_health;

    [Space()]
    /// <summary>
    ///   <para>The total energy stored in the agent. If this dips too low, the agent looses health.</para>
    /// </summary>
    public float energy;
    public float max_energy;

    [Space()]
    public Vector3 max_size;

    [Space()]
    /// <summary>
    ///   <para>The attack of the agent</para>
    /// </summary>
    public float attack;

    /// <summary>
    ///   <para>The defense of the agent</para>
    /// </summary>
    public float defense;

    [Space()]
    /// <summary>
    ///   <para>How hungry the agent is. This will slowly increace. If at 0, health will start depleating.</para>
    /// </summary>
    public float hunger;

    /// <summary>
    ///   <para>The rate at which this agent can consume energy (eat) in terms of energy per second</para>
    /// </summary>
    public float consumption_rate;

    [Space()]
    /// <summary>
    ///   <para>How long the agent has been alive for.</para>
    /// </summary>
    public float age;

    /// <summary>
    ///   <para>The age the agent must be to be capable of reproduction</para>
    /// </summary>
    public float maturity_age;
    public int eggs_layed;

    /// <summary>
    ///   <para>The max age of the agent.</para>
    /// </summary>
    public float lifespan;

    [Space()]
    /// <summary>
    ///   <para>This stores whether or not the agent is able to reproduce and is at max strenght.</para>
    /// </summary>
    public bool is_mature;

    [Space()]
    /// <summary>
    ///   <para>This is a result of their genetic traits. It is how quickly they consume energy. Their energy depletes metabolism points per second.</para>
    /// </summary>
    public float metabolism;

    /// <summary>
    ///   <para>The time until the agent dies given it eats nothing</para>
    /// </summary>
    public float time_to_die;

    /// <summary>
    ///   <para>The way the agent is controlled</para>
    /// </summary>
    public enum Control
    {
        Heuristic,
        Hard,
        Learning,
        None
    }
    public Control control = Control.Heuristic;

    /// <summary>
    ///   <para> The node used to track its ancestory. It contains its parent node (ancestor)!</para>
    /// </summary>
    public AncestorNode node;

    /// <summary>
    ///   <para>The generation of the agent. The first generation (generation=0), is the first agent of a specific species</para>
    /// </summary>
    public int generation;


    /// <summary>
    ///   <para>A debug parameter which makes the agent very powerful</para>
    /// </summary>
    [Space]
    [Header("For Debugging Purposes")]
    public bool god;

    public List<float> obs;

    public virtual void Setup(int id, Genes genes)
    {

        // Setup the names
        genus = genes.genus;
        species = genes.species;
        name = genus + " " + species;

        // The health of the agent
        health = base_health * Mathf.Pow(genes.size, 2);
        max_health = health;

        // The energy contained in the agent
        energy = Mathf.Pow(genes.size, 2) * base_health;
        max_energy = energy;

        // The cost of movement and stuff (starts small and increases as it grows
        metabolism = (Mathf.Pow(genes.size, 2) * genes.speed + genes.perception) * 0.3f;

        // How fast can the consume food?
        consumption_rate = (base_consumption_rate / Mathf.Exp(-genes.size)) * 0.5f;

        // Find the attack and defense of the agent
        // The attack and defense starts lower
        attack = genes.attack * base_attack * 0.1f;
        defense = genes.defense * base_defense * 0.1f;

        // Set up the speeds
        movement_speed = base_speed * genes.speed;
        rotation_speed = base_rotation_speed * genes.speed;

        // Setup the agents base stuff
        this.genes = genes;
        // Set the genetic code (3 floating point numbers)
        this.genes.SetGeneticCode();
        base.Setup(id);

        // Give the rigid body the mass
        rb.mass = (genes.size * genes.size) * base_mass + base_mass;

        // Set the colour of the agent
        sprite.color = new Color(genes.colour_r, genes.colour_g, genes.colour_b);

        // Set the senses up
        senses = GetComponent<Senses>();
        senses.Setup(genes.perception * base_perception, id, this.transform);

        // Setup the age stuff
        lifespan = (genes.vitality + 1) * base_lifespan * (genes.size / genes.speed);
        maturity_age = (lifespan * (genes.maturity_time) / 2) / Mathf.Max((genes.gestation_time * base_gestation_time), 1);

        // Set up the max size
        max_size = transform.localScale * genes.size;
        // Setup the birth size
        transform.localScale = max_size * 0.1f;

        // Check if its a god lol
        if (god)
        {
            lifespan = 1000000;
            metabolism = 0;
            movement_speed = base_speed;
            rotation_speed = base_rotation_speed;
        }

        // Finally, set up the brain!!
        brain = GetComponent<Brain>();
        brain.Setup();
        Model.NeuralNet.RandomizeWeights((Model.NeuralNet)brain.GetModel());
    }

    public virtual void Setup(int id, Genes g, Manager m)
    {
        manager = m;
        Setup(id, g);
    }

    protected virtual void Update()
    {
        if (control == Control.Heuristic)
        {
            HeuristicControl();
        }
        else if (control == Control.Hard)
        {
            HardControl();
        }
        else if (control == Control.Learning)
        {
            obs = senses.GetObservations(this);
            inf = brain.GetModel().Infer(obs);

            Move(inf[0] * movement_speed, inf[1] * movement_speed, inf[2] * rotation_speed, inf[3] * rotation_speed);
            wants_to_breed = is_mature && inf[4] > 0;
            wants_to_eat = inf[5] > 0 || energy < max_energy / 2;
            wants_to_grab = inf[6] > 0;

            if (wants_to_breed)
            {
                LayEgg();
            }

            if (wants_to_eat && grabbed != null)
            {
                Eat(grabbed);
            }

            if (grabbed != null && grabbed.GetID() == (int)ID.Wobbit && inf[7] > 0)
            {
                Attack((BaseAgent)grabbed);
            }
        }

        // Reduces energy over the agent's life 
        ExistentialCost();

        // Heal the agent if they have enough energy
        TryHealing();

        // Sense stuff!
        SenseEnv();

        // Update the age of the agent
        UpdateAge();

        // Update the speed of the agent
        speed = rb.velocity.magnitude;

        if (Input.GetKey(KeyCode.K))
        {
            Die();
        }

        // If we do not want to grab, we should get rid of anything we are trying to grab
        if (wants_to_grab == false || grabbed != null && Vector2.Distance(grabbed.transform.position, transform.position) > 3)
        {
            ReleaseGrab();
        }
    }

    protected void UpdateAge()
    {
        // Here we increment the time the agent has been alive! As well as update the maturity status
        is_mature = age > maturity_age;
        age += Time.deltaTime;

        // The creatures only reach full size when they are mature
        if (!is_mature && maturity_age > 0)
        {
            // Scale up the agent as it ages
            transform.localScale = Vector3.Lerp(max_size * 0.1f, max_size, (age / maturity_age));

            // Lerp the consumption rate
            consumption_rate = Mathf.Lerp(base_consumption_rate / Mathf.Exp(-genes.size) * 0.5f, base_consumption_rate / Mathf.Exp(-genes.size), age / maturity_age);

            // Lerp the metabolism (babies dont need as much food as the adults)
            metabolism = Mathf.Lerp((Mathf.Pow(genes.size, 2) * genes.speed + genes.perception) * 0.3f, (Mathf.Pow(genes.size, 2) * genes.speed + genes.perception), age / maturity_age);

            // Lerp the health
            health = Mathf.Lerp(max_health * 0.1f, max_health, age / maturity_age);

            // Lerp the attack and defense
            defense = Mathf.Lerp(genes.defense * base_defense * 0.1f, genes.defense * base_defense, age / maturity_age);
            attack = Mathf.Lerp(genes.attack * base_attack * 0.1f, genes.attack * base_attack, age / maturity_age);
        }

        // If the creature is too old they die!
        if (age > lifespan)
        {
            Die();
        }
    }

    protected virtual void SenseEnv()
    {
        senses.Sense();
    }

    protected virtual void ExistentialCost()
    {
        // The cost of existing
        float cost = (metabolism * Time.deltaTime / 2) / manager.metabolism_scale_rate;
        energy -= cost;
        manager.RecycleEnergy(cost);

        // If the health of the agent is less than or equal to zero or the energy has
        // dropped to 30% its max capacity the agent dies (account for the age difference)
        if (health <= 0 || energy < (max_energy * 0.3f) / Mathf.Max(maturity_age - age, 1))
        {
            Die();
        }
    }

    protected virtual void TryHealing()
    {
        // Heal the agent over time if they have more than enough energy
        if (energy > max_energy && max_health >= health)
        {
            health += Time.deltaTime;
        }
    }

    protected virtual void HeuristicControl()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Move(movement_speed * genes.speed, 0, 0, 0);
        }

        if (Input.GetKey(KeyCode.S))
        {
            Move(0, movement_speed * genes.speed, 0, 0);
        }

        if (Input.GetKey(KeyCode.D))
        {
            Move(0, 0, genes.speed * rotation_speed, 0);
        }

        if (Input.GetKey(KeyCode.A))
        {
            Move(0, 0, 0, genes.speed * rotation_speed);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            LayEgg();
        }

        wants_to_grab = Input.GetKey(KeyCode.G);
        wants_to_eat = Input.GetKey(KeyCode.E);

        if (wants_to_eat && grabbed != null)
        {
            Eat(grabbed);
        }
    }

    public void HardControl()
    {
        senses.detected = senses.detected.Where(Interactable => Interactable != null).ToList();
        if (senses.detected.Count() > 0 && grabbed == null)
        {
            var closest = senses.detected.OrderBy(x => Vector2.Distance(this.transform.position, x.transform.position)).ToList()[0];

            if (wants_to_breed)
            {
                foreach (var v in senses.detected)
                {
                    if (v.GetID() == (int)ID.Wobbit)
                    {
                        closest = v;
                        break;
                    }
                }
            }

            if (closest != null && !(closest.GetID() == (int)ID.Wobbit && ((BaseAgent)closest).genes.CalculateGeneticDrift(genes) < 1.5f) && !(closest.GetID() == (int)ID.WobbitEgg && ((Egg)closest).parent != null && ((Egg)closest).parent.species.Equals(species)))
            {
                transform.up = Vector2.Lerp(transform.up, closest.transform.position - transform.position, rotation_speed * 0.5f * Time.deltaTime);
                Move(movement_speed, 0, 0, 0);
            }
            else
            {
                if (wants_to_breed && closest.GetID() == (int)ID.Wobbit && ((BaseAgent)closest).genes.species.Equals(genes.species) && is_mature && ((BaseAgent)closest).is_mature)
                {
                    transform.up = Vector2.Lerp(transform.up, closest.transform.position - transform.position, rotation_speed * 0.5f * Time.deltaTime);
                    Move(movement_speed, 0, 0, 0);
                }
                else
                {
                    transform.up = Vector2.Lerp(transform.up, -(closest.transform.position - transform.position), rotation_speed * 0.5f * Time.deltaTime);
                    Move(movement_speed, 0, 0, 0);
                }
            }
        }
        else if (grabbed != null && wants_to_eat)
        {
            Move(0, 0, 0f, 0);
        }
        else
        {
            Move(0, 0, 1f, 0);
        }

        if (is_mature && energy > max_energy * 2f)
        {
            wants_to_breed = true;
            wants_to_eat = false;
            if (manager.anc_manager.population[genes.genus + " " + genes.species].size < 2)
            {
                LayEgg();
            }
        }
        else
        {
            wants_to_breed = false;
            wants_to_eat = true;
            wants_to_grab = true;
        }


        if (wants_to_eat && grabbed != null)
        {
            Eat(grabbed);
        }

        if (grabbed != null && grabbed.GetID() == (int)ID.Wobbit && ((BaseAgent)grabbed).genes.CalculateGeneticDrift(genes) > 1.0f)
        {
            Attack((BaseAgent)grabbed);
        }
        else if (grabbed != null && grabbed.GetID() == (int)ID.Wobbit && ((BaseAgent)grabbed).wants_to_breed && wants_to_breed)
        {
            Breed((BaseAgent)grabbed);
        }
        else if (grabbed != null && grabbed.GetID() == (int)ID.Wobbit && ((BaseAgent)grabbed).genes.CalculateGeneticDrift(genes) < 1.0f)
        {
            wants_to_grab = false;
        }
    }

    public void LearningControl()
    {
        // We first need to create the list of observations

    }

    protected virtual void Move(float forward, float backward, float left, float right)
    {
        rb.AddForce(transform.up * forward);
        rb.AddForce(transform.up * -backward);
        rb.AddTorque(right);
        rb.AddTorque(-left);

        // Clamp the velocity
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, movement_speed);
        rb.angularVelocity = Mathf.Clamp(rb.angularVelocity, -rotation_speed, rotation_speed);

    }


    /// <summary>
    ///   <para>Sexual reproduction</para>
    /// </summary>
    public void Breed(BaseAgent partner)
    {
        // If the partner wants to breed, then shoot your shot it
        if (partner.wants_to_breed && energy > max_energy)
        {
            // Spawn in the egg object
            Vector2 pos = egg_location.position;
            var x = Instantiate(egg, pos, transform.rotation, manager.transform);

            // Add this agent as the parent to the egg
            x.parent = this;

            // Every agent will have its eggs id be the agents id + 1
            // Create a perfect clone of the genes and pass it on to the egg
            x.Setup(this.id + 1, max_energy, genes.Cross(partner.genes), manager, manager.GetAgent(id));

            // Give the egg a copy of this brain's model
            x.brain = brain.GetModel().Copy();

            // An egg requires the same amount of energy as the end product
            energy -= max_energy;

            // Add the egg to the manager to track
            manager.AddAgent(x);

            // Update this value 
            eggs_layed++;
        }
    }

    /// <summary>
    ///   <para>Asexual reproduction</para>
    /// </summary>
    public virtual void LayEgg()
    {
        if (energy > max_energy)
        {
            // Spawn in the egg object
            Vector2 pos = egg_location.position;
            var x = Instantiate(egg, pos, transform.rotation, manager.transform);

            // Add this agent as the parent to the egg
            x.parent = this;

            // Every agent will have its eggs id be the agents id + 1
            // Create a perfect clone of the genes and pass it on to the egg
            x.Setup(this.id + 1, max_energy, genes.Clone(), manager, manager.GetAgent(id));

            // Give the egg a copy of this brain's model
            x.brain = brain.GetModel().Copy();

            // An egg requires the same amount of energy as the end product
            energy -= max_energy;

            // Add the egg to the manager to track
            manager.AddAgent(x);

            // Update this value 
            eggs_layed++;
        }
    }

    public virtual void Eat(Interactable other)
    {
        // The agent should not be able to overeat and reproduce if it is not of age
        if (energy >= max_energy * 2f)
        {
            return;
        }

        // The energy obtained by the meal
        float obtained_energy;

        // 1000 is the id of a FoodPellet
        if (other.GetID() == (int)ID.FoodPellet)
        {

            // Get the energy from the food pellet
            obtained_energy = other.GetComponent<FoodPellet>().Eat(consumption_rate * Time.deltaTime);

            // Release the grab
            if (other.GetComponent<FoodPellet>().eaten)
                ReleaseGrab();

            // Determine how much of it is actually extracted
            float extracted_percentage = 1f - genes.diet;
            energy += extracted_percentage * obtained_energy;

            // The rest of the 'undigested' food gets recycled as waste
            manager.RecycleEnergy(obtained_energy - extracted_percentage * obtained_energy);
        }
        else if (other.GetID() == (int)ID.Meat)
        {
            // Get the energy from the food pellet
            obtained_energy = other.GetComponent<Meat>().Eat();

            // Release the grab
            if (other.GetComponent<Meat>().energy <= 0)
                ReleaseGrab();

            // Determine how much of it is actually extracted
            float extracted_percentage = genes.diet;
            energy += extracted_percentage * obtained_energy;

            // The rest of the 'undigested' food gets recycled as waste
            manager.RecycleEnergy(obtained_energy - extracted_percentage * obtained_energy);
        }
        else if (other.GetID() == (int)ID.WobbitEgg)
        {
            // Get the energy from the food pellet
            obtained_energy = other.GetComponent<Egg>().Eat();

            // Release the grab
            if (other.GetComponent<Egg>().energy <= 0)
                ReleaseGrab();

            // Determine how much of it is actually extracted
            float extracted_percentage = genes.diet;
            energy += extracted_percentage * obtained_energy;

            // The rest of the 'undigested' food gets recycled as waste
            manager.RecycleEnergy(obtained_energy - extracted_percentage * obtained_energy);
        }
    }

    public virtual void Grab(Interactable i)
    {
        // Grab something if we havnt got one already
        if (grabbed == null)
        {
            // Grab object
            grabbed = i;
            // Activate the spring joint
            GetComponent<RelativeJoint2D>().enabled = true;
            GetComponent<RelativeJoint2D>().connectedBody = grabbed.GetRB();
        }
    }

    public virtual void ReleaseGrab()
    {
        if (grabbed != null)
        {
            grabbed = null;
            // Activate the spring joint
            GetComponent<RelativeJoint2D>().enabled = false;
            GetComponent<RelativeJoint2D>().connectedBody = null;
        }
    }


    public virtual void Attack(BaseAgent other)
    {
        other.Damage(attack * Time.deltaTime);
    }

    public virtual void Damage(float damage)
    {
        health -= Mathf.Clamp(damage - (defense * (genes.size - genes.speed)), 1, damage);
    }

    public virtual void Die()
    {

        // Remove this agent from the manager's list
        manager.agents.Remove(this);

        // Break the agent into meat pieces
        int pieces = Random.Range(1, 8);
        float energy_per = energy / pieces;
        for (int i = 0; i < pieces; i++)
        {
            if (energy - energy_per <= 0)
            {
                Meat m = Instantiate(meat, transform.position, transform.rotation, manager.transform);
                m.Setup((int)ID.Meat, energy, 1f, genes.size + transform.localScale.x, this.genes, manager);
                manager.agents.Add(m);
                break;
            }
            else
            {
                Meat m = Instantiate(meat, transform.position, transform.rotation, manager.transform);
                m.Setup((int)ID.Meat, energy_per, 1f, genes.size + transform.localScale.x, this.genes, manager);
                manager.agents.Add(m);
                energy -= energy_per;
            }
        }

        Destroy(gameObject);
    }

    public virtual void CheckVitality()
    {

    }

    public virtual void OnCollisionStay2D(Collision2D collision)
    {
        // We handle sustained collisions using this method
        if (collision.gameObject.TryGetComponent(out Interactable i) && senses.Touch(collision, transform.up).Equals("Front"))
        {
            // If we want to eat then eat yum!
            if (wants_to_eat)
            {
                Eat(i);
            }
        }

    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        // Where did we collide?
        string where;

        // Check to see if the collision was with something we want
        if (collision.gameObject.TryGetComponent(out Interactable i))
        {
            // Find out where we collided
            where = senses.Touch(collision, transform.up);

            // If it is the front, we can grab things
            if (where.Equals("Front"))
            {
                // If we want to grab then grab
                if (wants_to_grab)
                {
                    Grab(i);
                }
            }

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.up + transform.position, transform.up * 5 + transform.position);
    }
}
