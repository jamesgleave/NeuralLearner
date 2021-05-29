﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BaseAgent : Interactable
{
    [Header("Agent Components")]
    public GameObject body;
    public GameObject head;

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
    [Range(0.0f, 100f)]
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

    /// <summary>
    ///   <para>Whether the agent wants to attack something or not</para>
    /// </summary>
    public bool wants_to_attack;

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

    /// <summary>
    ///   <para>The value that control the speed at which the agent can move</para>
    /// </summary>
    public float internal_clock;

    [Space()]
    [Header("Movement:")]
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

    /// <summary>
    ///   <para>The kenetic energy of the agent</para>
    /// </summary>
    public float kenetic_energy;

    [Space()]
    [Header("Health:")]
    /// <summary>
    ///   <para>The health of the agent</para>
    /// </summary>
    public float health;
    public float max_health;

    [Space()]
    [Header("Energy:")]
    /// <summary>
    ///   <para>The total energy stored in the agent. If this dips too low, the agent looses health.</para>
    /// </summary>
    public float energy;
    public float max_energy;

    [Space()]
    public Vector3 max_size;

    [Space()]
    [Header("Combat:")]
    /// <summary>
    ///   <para>The attack of the agent</para>
    /// </summary>
    public float attack;

    /// <summary>
    ///   <para>The defense of the agent</para>
    /// </summary>
    public float defense;

    [Space()]
    [Header("Food:")]
    /// <summary>
    ///   <para>How hungry the agent is. This will slowly increace. If at 0, health will start depleating.</para>
    /// </summary>
    public float hunger;

    /// <summary>
    ///   <para>The rate at which this agent can consume energy (eat) in terms of energy per second</para>
    /// </summary>
    public float consumption_rate;

    [Space()]
    [Header("Age:")]
    /// <summary>
    ///   <para>How long the agent has been alive for.</para>
    /// </summary>
    public float age;

    /// <summary>
    ///   <para>The age the agent must be to be capable of reproduction</para>
    /// </summary>
    public float maturity_age;

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
    [Header("Eggs:")]
    /// <summary>
    ///   <para>How long an egg takes to be produced</para>
    /// </summary>
    public float egg_formation_time;
    /// <summary>
    ///   <para>The number of eggs ready to be laid</para>
    /// </summary>
    public int num_eggs_ready;
    /// <summary>
    ///   <para>The number of eggs have been laid</para>
    /// </summary>
    public int eggs_layed;
    /// <summary>
    ///   <para>The time needed between egg laying (same as gestation time)</para>
    /// </summary>
    public float egg_formation_cooldown = 0;
    /// <summary>
    ///   <para>The gestation time (time spent in egg before hatch)</para>
    /// </summary>
    public float egg_gestation_time;

    [Space()]
    [Header("Metabolism:")]
    /// <summary>
    ///   <para>This is a result of their genetic traits. It is how quickly they consume energy. Their energy depletes metabolism points per second.</para>
    /// </summary>
    public float metabolism;
    /// <summary>
    ///   <para>The actual cost with all factors included.</para>
    /// </summary>
    public float true_metabolic_cost;

    [Space()]
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
        None,
        Static
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
    public float grabbed_force;
    public float strength;

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

        // Setup the base interactable
        base.Setup(id);


        // Give the rigid body the mass
        rb.mass = (genes.size * genes.size) * base_mass + base_mass;

        // Setup the sprite for the agent
        manager.SetSprite(this, genes.colour_r, genes.colour_g, genes.colour_b);

        // Set the senses up
        senses = GetComponent<Senses>();
        senses.Setup(genes.perception * base_perception, id, this.transform);

        // Setup the age stuff
        lifespan = (genes.vitality + 1) * base_lifespan * genes.size;
        //maturity_age = (lifespan * (genes.maturity_time) / 2) / Mathf.Max((genes.gestation_time * base_gestation_time), 1);
        maturity_age = lifespan * Mathf.Clamp((genes.maturity_time + 1) / (genes.gestation_time + 1) - 0.5f, 0.1f, 0.8f);

        // The time the agent needs to make an egg and hatch (reproductive cycle is twice this amount of time plus the maturity age)
        egg_gestation_time = Mathf.Max(Mathf.Log(genes.gestation_time * genes.size * energy / 2) * base_gestation_time, base_gestation_time);

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

        // Setup the joint for grabbing!
        GetComponent<RelativeJoint2D>().maxForce = (1 + genes.vitality) * 5;
        GetComponent<RelativeJoint2D>().maxTorque = (1 + genes.vitality) * 5;
    }

    public virtual void Setup(int id, Genes g, Manager m)
    {
        manager = m;
        Setup(id, g);
    }

    protected virtual void Update()
    {

        // Update the internal clock
        internal_clock -= genes.clockrate;

        // Reset the true metabolism cost
        true_metabolic_cost = 0;

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
            LearningControl();
        }

        if (control != Control.Static)
        {
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

            // Calculate the KE
            kenetic_energy = 0.5f * Mathf.Pow(speed, 2f) * rb.mass;

            if (Input.GetKey(KeyCode.K))
            {
                Die();
            }

            // If we do not want to grab, we should get rid of anything we are trying to grab
            // If something is too far we drop it 
            if (wants_to_grab == true && grabbed != null)
            {
                Vector2 c1 = grabbed.GetCol().ClosestPoint(transform.position);
                Vector2 c2 = col.ClosestPoint(c1);
                grabbed_force = Vector2.Distance(c1, c2);

                // This value is like the agent's strength, it is how well they can hold onto things TODO Make the max force and torque equal to the strength!
                strength = ((1 + genes.vitality) * transform.localScale.sqrMagnitude) / 100;

                if (grabbed_force > strength)
                {
                    ReleaseGrab();
                }
            }

            // Or if we want to just drop it
            if (wants_to_grab == false)
            {
                ReleaseGrab();
            }

            // Debugging
            if (Input.GetKey(KeyCode.P))
            {
                ((EvolutionaryNeuralLearner)brain).Mutate();
            }
        }
    }

    protected void UpdateAge()
    {
        // Here we increment the time the agent has been alive! As well as update the maturity status
        // Once we are of maturity age, regenerate the collider
        if (age > maturity_age && is_mature == false)
        {
            is_mature = true;
            GenerateCollider();
        }
        else
        {
            // Since there is a signifianct overhead while calculating the collider geometry, we want to only do this when absolutely needed...
            // To determine when to update the geometry, we need to figure out how much the geometry has changed.
            // I do this by updating the collider in increments determined by the ratio of the transform scale and the max size scale.
            bool regenerate = (int)(transform.localScale.x / max_size.x * 100) % 5f == 0;

            // Here, we regenerate the geometry if we want to regenerate it with the bool we defined above or if there is no shape (has not been created yet)
            if (regenerate && is_mature == false || col.shapeCount == 0)
                GenerateCollider();
        }

        // Increment age
        age += Time.deltaTime;
        egg_formation_cooldown += Time.deltaTime;

        // The creatures only reach full size when they are mature
        if (!is_mature && maturity_age > 0)
        {
            // Scale up the agent as it ages
            transform.localScale = Vector3.Lerp(max_size * 0.1f, max_size, (age / maturity_age));

            // Lerp the consumption rate
            consumption_rate = Mathf.Lerp(base_consumption_rate / Mathf.Exp(-genes.size) * 0.3f, base_consumption_rate / Mathf.Exp(-genes.size), age / maturity_age);

            // Lerp the metabolism (babies dont need as much food as the adults)
            metabolism = Mathf.Lerp((Mathf.Pow(genes.size, 2) * genes.speed + genes.perception) * 0.3f, (Mathf.Pow(genes.size, 2) * genes.speed + genes.perception), age / maturity_age);

            // Lerp the health
            //health = Mathf.Lerp(max_health * 0.1f, max_health, age / maturity_age);
            max_health = Mathf.Lerp(base_health * Mathf.Pow(genes.size, 2) * 0.1f, base_health * Mathf.Pow(genes.size, 2), age / maturity_age);
            health = Mathf.Clamp(health, 0, max_health);

            // Lerp the attack and defense
            defense = Mathf.Lerp(genes.defense * base_defense * 0.1f, genes.defense * base_defense, age / maturity_age);
            attack = Mathf.Lerp(genes.attack * base_attack * 0.1f, genes.attack * base_attack, age / maturity_age);

            // They consume a bit more energy when growing up!
            if (energy > metabolism * Time.deltaTime)
            {
                //energy -= metabolism * Time.deltaTime;
                //manager.RecycleEnergy(metabolism * Time.deltaTime);

                //// update the true metabolism cost
                //true_metabolic_cost += metabolism * Time.deltaTime;
            }
            else
            {
                manager.RecycleEnergy(energy);
                energy = 0;
            }
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
        // The cost of existing with the cost of moving
        //float cost = ((metabolism * Time.deltaTime / 2) / manager.metabolism_scale_rate) * (1 + kenetic_energy);
        float cost = (metabolism * Time.deltaTime) / manager.metabolism_scale_rate;
        energy -= cost;
        manager.RecycleEnergy(cost);

        // If the health of the agent is less than or equal to zero or the energy has
        // dropped to 30% its max capacity the agent dies (account for the age difference)
        if (health <= 0 || energy < (max_energy * 0.3f) / Mathf.Max(maturity_age - age, 1))
        {
            Die();
        }

        // Update the true cost
        true_metabolic_cost += cost;
        time_to_die = Mathf.Min(lifespan - age, energy / true_metabolic_cost);
    }

    protected virtual void TryHealing()
    {
        // Heal the agent over time if they have more than enough energy (healing takes energy)
        if (energy > max_energy && max_health >= health)
        {
            // Heal one point per second
            float cost = Time.deltaTime;

            // Use energy to heal
            health += cost;
            energy -= cost;

            // Recycle the energy
            manager.RecycleEnergy(cost);

            // Clamp the health
            health = Mathf.Clamp(health, 0, max_health);
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
        obs = senses.GetObservations(this);
        inf = brain.GetModel().Infer(obs);

        // Allow the agent to move 
        Move(inf[0] * movement_speed, inf[1] * movement_speed, inf[2] * rotation_speed, inf[3] * rotation_speed);

        // Set the decisions
        wants_to_breed = is_mature && inf[4] > 0.5f;
        wants_to_eat = inf[5] > 0.5f || energy < max_energy / 2;
        wants_to_grab = inf[6] > 0.5f;
        wants_to_attack = inf[7] > 0.5f;

        // TODO Give the agent the ability to decide whether it wants to just lay an egg or breed (sexual vs asexual reproduction)
        // If the agent wants to breed and has grabbed another agent, check if that agent is close enough genetically or if it has the same name (meaning it is the same species)
        if (wants_to_breed && grabbed != null && grabbed.TryGetComponent<BaseAgent>(out BaseAgent a) && (genes.CalculateGeneticDrift(a.genes) < 1 || a.GetRawName().Equals(GetRawName())))
        {
            Breed(a);
            print("Bread");
        }
        // If it does not then asexually reproduce
        else if (wants_to_breed)
        {
            LayEgg();
        }

        // If we have something grabbed and are hungry then eat it!
        if (wants_to_eat && grabbed != null)
        {
            Eat(grabbed);
        }

        // If we have an agent grabbed and we want to attack then attack it!
        if (grabbed != null && grabbed.GetID() == (int)ID.Wobbit && wants_to_attack)
        {
            Attack((BaseAgent)grabbed);
        }
    }

    protected virtual void Move(float forward, float backward, float left, float right)
    {
        // TODO handle this is update age
        float scaler = Mathf.Clamp(age / maturity_age, 0.25f, 1);

        // Find force to add for moving forward
        Vector3 forward_force = transform.up * (forward + (genes.vitality * forward));
        Vector3 backward_force = transform.up * -(backward + (genes.vitality * backward));

        // Add the force
        rb.AddForce(forward_force);
        rb.AddForce(backward_force);

        // Calculate torque to add
        float torque_right = (right + genes.vitality * right) * scaler;
        float torque_left = -(left + genes.vitality * left) * scaler;

        rb.AddTorque(torque_right);
        rb.AddTorque(torque_left);

        // Clamp the velocity
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, movement_speed * scaler);
        rb.angularVelocity = Mathf.Clamp(rb.angularVelocity, -rotation_speed, rotation_speed * scaler);

    }


    /// <summary>
    ///   <para>Sexual reproduction</para>
    /// </summary>
    public void Breed(BaseAgent partner)
    {
        // If we have not had enough time to form an egg, we return without doing anything
        if (egg_formation_cooldown < egg_gestation_time)
        {
            return;
        }

        // If the partner wants to breed, then shoot your shot it
        if (partner.wants_to_breed)
        {
            // Spawn in the egg object
            Vector2 pos = egg_location.position;
            var x = Instantiate(egg, pos, transform.rotation, manager.transform);

            // Add this agent as the parent to the egg
            x.parent = this;

            // Every agent will have its eggs id be the agents id + 1
            // Create a perfect clone of the genes and pass it on to the egg
            x.Setup(this.id + 1, max_energy * 0.5f, genes.Cross(partner.genes), manager, manager.GetAgent(id));

            // Give the egg a copy of this brain's model
            // TODO Cross over of brains
            x.brain = brain.GetModel().Copy();

            // An egg requres 50% of the agents max energy
            energy -= max_energy * 0.5f;

            // Add the egg to the manager to track
            manager.AddAgent(x);

            // Update this value 
            eggs_layed++;

            // Reset the egg formation cooldown
            egg_formation_cooldown = 0;
        }
    }

    /// <summary>
    ///   <para>Asexual reproduction</para>
    /// </summary>
    public virtual void LayEgg()
    {

        // If we have not had enough time to form an egg, we return without doing anything
        if (egg_formation_cooldown < egg_gestation_time)
        {
            return;
        }

        // Spawn in the egg object
        Vector2 pos = egg_location.position;
        var x = Instantiate(egg, pos, transform.rotation, manager.transform);

        // Add this agent as the parent to the egg
        x.parent = this;

        // Every agent will have its eggs id be the agents id + 1
        // Create a perfect clone of the genes and pass it on to the egg
        x.Setup(this.id + 1, max_energy * 0.5f, genes.Clone(), manager, manager.GetAgent(id));

        // Give the egg a copy of this brain's model
        x.brain = brain.GetModel().Copy();

        // An egg requres 50% of the agents max energy
        energy -= max_energy * 0.5f;

        // Add the egg to the manager to track
        manager.AddAgent(x);

        // Update this value 
        eggs_layed++;

        // Reset the egg formation cooldown
        egg_formation_cooldown = 0;
    }

    public virtual void Eat(Interactable other)
    {
        // The agent recieves less and less energy when they get too full
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
        other.Damage(attack * transform.localScale.x);
    }

    public virtual void Damage(float damage)
    {
        float total_damage = Mathf.Max(damage - (defense * transform.localScale.x), 0.5f);
        health -= total_damage * Time.deltaTime;
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
                m.Setup((int)ID.Meat, energy, 0.5f, transform.localScale.x / pieces, this.genes, manager);
                manager.agents.Add(m);
                break;
            }
            else
            {
                Meat m = Instantiate(meat, transform.position, transform.rotation, manager.transform);
                m.Setup((int)ID.Meat, energy_per, 0.5f, transform.localScale.x / pieces, this.genes, manager);
                manager.agents.Add(m);
                energy -= energy_per;
            }
        }

        Destroy(gameObject);
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

    public string GetRawName()
    {
        string raw = genus.ToUpper()[0] + genus.Substring(1) + " " + species.ToUpper()[0] + species.Substring(1);
        raw = raw.Split('>')[1];
        return raw;
    }

    private void OnDrawGizmos()
    {
        if (grabbed != null)
        {
            Vector2 c1 = grabbed.GetCol().ClosestPoint(transform.position);
            Vector2 c2 = col.ClosestPoint(c1);
            Gizmos.DrawLine(c1, c2);
        }
    }
}