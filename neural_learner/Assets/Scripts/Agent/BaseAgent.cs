using System.Collections;
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
    [Range(0.0f, 100f)]
    public float base_consumption_rate = 1;

    /// <summary>
    ///   <para>The base interval at which an agent can make a decision (in miliseconds)</para>
    /// </summary>
    [Range(1f, 1000f)]
    public float base_update_rate = 1;


    [Header("Agent Decisions")]

    /// <summary>
    ///   <para> How much the agent wants to move forward</para>
    /// </summary>
    public float wants_move_forward;

    /// <summary>
    ///   <para> How much the agent wants to rotate clockwise</para>
    /// </summary>
    public float wants_rotate_clockwise;

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

    /// <summary>
    ///   <para>Whether the agent wants to produce red pheromone</para>
    /// </summary>
    public bool wants_to_produce_red_pheromone;

    /// <summary>
    ///   <para>Whether the agent wants to produce green pheromone</para>
    /// </summary>
    public bool wants_to_produce_green_pheromone;

    /// <summary>
    ///   <para>Whether the agent wants to produce blue pheromone</para>
    /// </summary>
    public bool wants_to_produce_blue_pheromone;

    // testing
    private List<float> inf = new List<float>();


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

    /// <summary>
    ///   <para>The cost of the agent's movement</para>
    /// </summary>
    public float cost_of_movement;

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
    ///   <para>The actual cost with all factors included deducted every frame.</para>
    /// </summary>
    public float true_metabolic_cost;

    /// <summary>
    /// If true, having a large brain will require more energy
    /// </summary>
    public bool brain_cost;

    /// <summary>
    /// TODO DELETE LATER
    /// </summary>
    public Vector2 cost_comparison;

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

    [Space()]
    /// <summary>
    ///   <para>The generation of the agent. The first generation (generation=0), is the first agent of a specific species</para>
    /// </summary>
    public int generation;

    /// <summary>
    /// The number of pellets consumed by the agent in its lifetime
    /// </summary>
    public int num_pellets_eaten;

    /// <summary>
    /// The number of meats consumed by the agent in its lifetime
    /// </summary>
    public int num_meat_eaten;

    /// <summary>
    /// The number of meats consumed by the agent in its lifetime
    /// </summary>
    public int num_eggs_eaten;

    /// <summary>
    /// The number of other agents killed
    /// </summary>
    public int num_kills;


    /// <summary>
    ///   <para>A debug parameter which makes the agent very powerful</para>
    /// </summary>
    [Space]
    [Header("For Debugging Purposes")]
    public bool god;
    public float grabbed_force;
    public float strength;

    public List<float> obs;

    public float torque_for_boids;

    public virtual void Setup(int id, Genes genes)
    {

        // Set the tag
        this.tag = "agent";

        // Setup the names
        genus = genes.genus;
        species = genes.species;
        name = genus + " " + species;

        // Default the age to zero
        age = 0;
        is_mature = false;

        // The health of the agent
        health = base_health * Mathf.Pow(genes.size, 2);
        max_health = health;

        // The energy contained in the agent
        energy = Mathf.Pow(genes.size, 2) * base_health;
        max_energy = energy;

        // The cost of movement and stuff (starts small and increases as it grows
        metabolism = (Mathf.Pow(genes.size, 2) * genes.speed + genes.perception + Mathf.Pow((genes.attack * genes.defense), 2)) * 0.25f;

        // How fast can the consume food?
        consumption_rate = (base_consumption_rate / Mathf.Exp(-genes.size)) * 0.5f;

        // Find the   and defense of the agent
        // The attack and defense starts lower
        attack = genes.attack * base_attack * 0.25f;
        defense = genes.defense * base_defense * 0.25f;

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

        // Set up the agent's brain
        brain = GetComponent<Brain>();

        // Set the senses up
        senses = GetComponent<Senses>();
        senses.Setup(genes.perception * base_perception, id, this.transform);

        // Setup the age stuff
        lifespan = (genes.vitality + 1) * base_lifespan * genes.size;
        //maturity_age = (lifespan * (genes.maturity_time) / 2) / Mathf.Max((genes.gestation_time * base_gestation_time), 1);
        maturity_age = lifespan * Mathf.Clamp(genes.maturity_time - genes.gestation_time, 0.1f, 0.8f);

        // The time the agent needs to make an egg and hatch (reproductive cycle is twice this amount of time plus the maturity age)
        egg_gestation_time = Mathf.Max(Mathf.Log(genes.gestation_time * genes.size * energy / 2) * base_gestation_time, base_gestation_time);

        // Set up the max size
        max_size = Vector3.one * genes.size * 4.5f;
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

        // Setup the joint for grabbing!
        GetComponent<RelativeJoint2D>().maxForce = (1 + genes.vitality) * 5;
        GetComponent<RelativeJoint2D>().maxTorque = (1 + genes.vitality) * 5;

        // Reset number of eggs layed and other things since we reuse these objects
        eggs_layed = 0;
    }

    public virtual void Setup(int id, Genes g, Manager m)
    {
        manager = m;
        Setup(id, g);
    }

    protected virtual void Update()
    {
        // Update the internal clock
        internal_clock += Time.deltaTime;

        // Reset the true metabolism cost
        true_metabolic_cost = 0;

        if (control == Control.Heuristic)
        {
        }
        else if (control == Control.Hard)
        {
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

            if (Input.GetKey(KeyCode.L))
            {
                LayEgg();
            }

            // If we do not want to grab, we should get rid of anything we are trying to grab
            // If something is too far we drop it
            if (wants_to_grab == true && grabbed != null)
            {
                Vector2 c1 = grabbed.GetCol().ClosestPoint(transform.position);
                Vector2 c2 = head.GetComponent<Collider2D>().ClosestPoint(c1);
                grabbed_force = Vector2.Distance(c1, c2);

                // This value is like the agent's strength, it is how well they can hold onto things TODO Make the max force and torque equal to the strength!
                strength = ((1 + genes.vitality) * transform.localScale.sqrMagnitude) / 50;

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
        }
    }

    protected void UpdateAge()
    {
        // Here we increment the time the agent has been alive! As well as update the maturity status
        if (age > maturity_age && is_mature == false)
        {
            is_mature = true;
        }

        // Increment age
        age += Time.deltaTime;
        egg_formation_cooldown += Time.deltaTime;


        // The creatures only reach full size when they are mature
        if (!is_mature && maturity_age > 0)
        {
            // Scale up the agent as it ages
            transform.localScale = Vector3.Lerp(max_size * 0.25f, max_size, (age / maturity_age));

            // Lerp the consumption rate
            consumption_rate = Mathf.Lerp(base_consumption_rate / Mathf.Exp(-genes.size) * 0.3f, base_consumption_rate / Mathf.Exp(-genes.size), age / maturity_age);

            // Lerp the metabolism (babies dont need as much food as the adults)
            metabolism = Mathf.Lerp((Mathf.Pow(genes.size, 2) * genes.speed + genes.perception) * 0.25f, (Mathf.Pow(genes.size, 2) * genes.speed + genes.perception), age / maturity_age);

            // Lerp the health
            //health = Mathf.Lerp(max_health * 0.1f, max_health, age / maturity_age);
            max_health = Mathf.Lerp(base_health * Mathf.Pow(genes.size, 2) * 0.25f, base_health * Mathf.Pow(genes.size, 2), age / maturity_age);
            health = Mathf.Clamp(health, 0, max_health);

            // Lerp the attack and defense
            defense = Mathf.Lerp(genes.defense * base_defense * 0.25f, genes.defense * base_defense, age / maturity_age);
            attack = Mathf.Lerp(genes.attack * base_attack * 0.25f, genes.attack * base_attack, age / maturity_age);

        }

        // If the creature is too old they die!
        if (age > lifespan)
        {
            Die();
        }
    }

    protected virtual void ExistentialCost()
    {
        // The cost of existing with the cost of moving and the cost of having a large brain
        float brain_cost = this.brain_cost ? (brain.GetModel().GetComplexity() / manager.brain_cost_reduction_factor) : 0;
        float cost = ((metabolism + brain_cost) * Time.deltaTime) / manager.metabolism_scale_rate;
        cost_comparison.Set((metabolism + brain_cost), metabolism);
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

    public void LearningControl()
    {

        // Only update if the update time has been triggered
        if (internal_clock < (base_update_rate / 1000) * genes.clockrate)
        {
            // Do nothing right now
            // This is case, the agent's internal clock has not cycled and therefore the agent cannot make a new decision yet.
        }
        else
        {
            // Reset the agent's internal clock
            internal_clock = 0;

            // Sense the environment
            senses.Sense();

            // Move back to other side if goes to far
            if ((Vector2.Distance(transform.position, manager.transform.position) > (manager.gridsize) * 1.25f) && manager.teleport)
            {
                transform.position = -transform.position;
                transform.position = Vector3.MoveTowards(transform.position, manager.transform.position, 3f);
            }

            // Clear the list of inferences and pass in our observations
            inf.Clear();
            inf.AddRange(brain.GetAction(senses.GetObservations(this)));

            // Set the decisions
            wants_move_forward = inf[0];
            wants_rotate_clockwise = inf[1];

            wants_to_breed = is_mature && inf[2] >= 0; //is_mature && (inf[2] > 0 || energy > max_energy * 1.75f);
            wants_to_eat = inf[3] >= 0;
            wants_to_grab = inf[4] > 0;
            wants_to_attack = inf[5] > 0;
            wants_to_produce_red_pheromone = inf[6] > 0.5;
            wants_to_produce_red_pheromone = inf[7] > 0.5;
            wants_to_produce_red_pheromone = inf[8] > 0.5;

            // Pheromones
            TryProducingPheromone();

            // If the agent wants to breed and has grabbed another agent, check if that agent is close enough genetically or if it has the same name (meaning it is the same species)
            if (wants_to_breed && grabbed != null && grabbed.TryGetComponent<BaseAgent>(out BaseAgent a) && (genes.CalculateGeneticDrift(a.genes) < 1 || a.GetRawName().Equals(GetRawName())))
            {
                Breed(a);
            }
            // If it does not then asexually reproduce
            else if (wants_to_breed)
            {
                LayEgg();
            }

            // If we have an agent grabbed and we want to attack then attack it!
            if (grabbed != null && grabbed.GetID() == (int)ID.Wobbit && wants_to_attack)
            {
                Attack((BaseAgent)grabbed);
            }
        }

        // After the agent has made a decision or not, we use its inferences (new or old) to make the agent act
        // If we have something grabbed and are hungry then eat it!
        if (wants_to_eat && grabbed != null)
        {
            Eat(grabbed);
        }

        // Move the agent with its inferences
        Move(wants_move_forward * movement_speed, 0, wants_rotate_clockwise * rotation_speed, 0);
    }

    protected virtual void Move(float forward, float backward, float left, float right)
    {
        // TODO handle this is update age
        float scaler = Mathf.Clamp(age / maturity_age, 0.1f, 1);

        // Find force to add for moving forward
        Vector3 forward_force = transform.up * (forward);
        Vector3 backward_force = transform.up * -(backward);

        // Add the force
        rb.AddForce(forward_force);
        rb.AddForce(backward_force);

        // Calculate torque to add
        float torque_right = (right) * scaler;
        float torque_left = -(left) * scaler;

        rb.AddTorque(torque_right);
        rb.AddTorque(torque_left);

        // Clamp the velocity
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, movement_speed * scaler);
        rb.angularVelocity = Mathf.Clamp(rb.angularVelocity, -rotation_speed, rotation_speed * scaler);

        // TODO keep this updated
        // Testing energy consumption by moving
        cost_of_movement = ((genes.size * (Mathf.Abs(torque_left) + forward_force.magnitude) * Time.deltaTime) / manager.movement_cost_scale_rate);
        energy -= cost_of_movement;
        manager.RecycleEnergy(cost_of_movement);
        true_metabolic_cost += cost_of_movement;
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
            var x = manager.GetComponent<EntityPoolManager>().InstantiateEgg(egg, pos, transform.rotation, manager.transform);

            // Add this agent as the parent to the egg
            x.parent = this;

            // Take the minimum to give to the child, either all of the energy if there is not enough or half of the max energy
            float energy_to_child = Mathf.Min(max_energy * 0.5f, energy);

            // Every agent will have its eggs id be the agents id + 1
            // Create a perfect clone of the genes and pass it on to the egg.
            // Since this agent was made using sexual reproduction, give the partner as well
            x.Setup(this.id + 1, energy_to_child, genes.Clone(), manager, manager.GetAgent(id), this, partner);

            // Give the egg a copy of this brain's model
            // x.brain = brain.GetModel().Copy();

            // TODO Implement crossover

            // An egg requres 50% of the agents max energy or just the remaining amount of energy they have
            energy -= energy_to_child;

            // Add the egg to the manager to track
            manager.AddAgent(x);

            // Update this value
            eggs_layed++;

            // Reset the egg formation cooldown
            egg_formation_cooldown = 0;
        }
    }

    /// <summary>
    /// Tries to produce pheromones based on the agent's wants
    /// </summary>
    private void TryProducingPheromone()
    {
        if (wants_to_produce_red_pheromone)
        {
            GetComponent<PheromoneGland>().SpawnRed();
        }

        if (wants_to_produce_green_pheromone)
        {
            GetComponent<PheromoneGland>().SpawnGreen();
        }

        if (wants_to_produce_blue_pheromone)
        {
            GetComponent<PheromoneGland>().SpawnBlue();
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

        print("Ag: " + brain.GetModel().GetComplexity());

        // Spawn in the egg object
        Vector2 pos = egg_location.position;
        Egg x = manager.GetComponent<EntityPoolManager>().InstantiateEgg(egg, pos, transform.rotation, manager.transform);

        // Add this agent as the parent to the egg
        x.parent = this;

        // Take the minimum to give to the child, either all of the energy if there is not enough or half of the max energy
        float energy_to_child = Mathf.Min(max_energy * 0.5f, energy);

        // Every agent will have its eggs id be the agents id + 1
        // Create a perfect clone of the genes and pass it on to the egg
        x.Setup(this.id + 1, energy_to_child, genes.Clone(), manager, manager.GetAgent(id), brain);

        // Give the egg a copy of this brain's model
        // x.brain = brain.GetModel().Copy();

        // An egg requres 50% of the agents max energy or just the remaining amount of energy they have
        energy -= energy_to_child;

        // Add the egg to the manager to track
        manager.AddAgent(x);

        // Update this value
        eggs_layed++;

        // Reset the egg formation cooldown
        egg_formation_cooldown = 0;
    }

    /// <summary>
    /// Eats some or all of the other interactable
    /// </summary>
    /// <param name="other"></param>
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
            {
                num_pellets_eaten++;
                ReleaseGrab();
            }

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
            {
                num_meat_eaten++;
                ReleaseGrab();
            }

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
            {
                num_eggs_eaten++;
                ReleaseGrab();
            }


            // Determine how much of it is actually extracted
            float extracted_percentage = genes.diet;
            energy += extracted_percentage * obtained_energy;

            // The rest of the 'undigested' food gets recycled as waste
            manager.RecycleEnergy(obtained_energy - extracted_percentage * obtained_energy);
        }
    }

    /// <summary>
    /// Grabs the other interactable i
    /// </summary>
    /// <param name="i"></param>
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

    /// <summary>
    /// Releases a grab
    /// </summary>
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

    /// <summary>
    /// Attacks the 'other' base agent
    /// </summary>
    /// <param name="other"></param>
    public virtual void Attack(BaseAgent other)
    {
        // Damage the other agent (relative to its size)
        other.Damage(attack * transform.localScale.x);

        // Release grab and add attack force to other agent
        ReleaseGrab();
        other.GetRB().AddForce((transform.position - other.transform.position).normalized * -100 * attack);

        // Check to see if the other agent has died
        if (other.health <= 0)
        {
            num_kills++;
        }
    }

    /// <summary>
    /// Damage this agent
    /// </summary>
    /// <param name="damage"></param>
    public virtual void Damage(float damage)
    {
        // The total damage delt to the agent (again, relative to its size)
        float total_damage = Mathf.Max(damage - (defense * transform.localScale.x), 1f);
        health -= total_damage;
    }

    public virtual void Die()
    {
        // Change the name
        this.tag = "DeadAgent";

        // Remove this agent from the manager's list
        manager.agents.Remove(this);

        // Break the agent into meat pieces
        int pieces = Random.Range(1, 2);
        float energy_per = energy / pieces;
        for (int i = 0; i < pieces; i++)
        {
            if (energy - energy_per <= 0)
            {
                Meat m = Instantiate(meat, transform.position, transform.rotation, manager.transform);
                m.Setup((int)ID.Meat, energy, 0.5f, genes.size / pieces, this.genes, manager);
                manager.agents.Add(m);
                break;
            }
            else
            {
                Meat m = Instantiate(meat, transform.position, transform.rotation, manager.transform);
                m.Setup((int)ID.Meat, energy_per, 0.5f, genes.size / pieces, this.genes, manager);
                manager.agents.Add(m);
                energy -= energy_per;
            }
        }

        num_pellets_eaten = num_meat_eaten = num_eggs_eaten = num_kills = 0;
        manager.GetComponent<EntityPoolManager>().Destroy(this);
    }

    public virtual void OnCollisionStay2D(Collision2D collision)
    {
        // We handle sustained collisions using this method
        if (collision.gameObject.TryGetComponent(out Interactable i) && collision.contacts[0].otherCollider.CompareTag("Head"))
        {

            // If we want to eat then eat yum!
            if (wants_to_eat)
            {
                Eat(i);
            }

            // If we want to attack then attack!
            if (wants_to_attack && collision.gameObject.TryGetComponent(out BaseAgent a))
            {
                Attack(a);
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        // Check to see if the collision was with something we want
        if (collision.gameObject.TryGetComponent(out Interactable i))
        {
            // If it is the front, we can grab things
            if (collision.contacts[0].otherCollider.CompareTag("Head"))
            {
                // If we want to grab then grab
                if (wants_to_grab)
                {
                    Grab(i);
                }

                // If we want to attack then attack!
                if (wants_to_attack && collision.gameObject.TryGetComponent(out BaseAgent a))
                {
                    Attack(a);
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
