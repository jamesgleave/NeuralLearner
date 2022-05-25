using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Egg : Edible
{
    /// <summary>
    ///   <para>The scale of the egg</para>
    /// </summary>
    public float size;

    public float gestation_time;
    public float total_gestation_time;

    /// <summary>
    ///   <para>The manager associated with this meat</para>
    /// </summary>
    public Manager manager;

    /// <summary>
    ///   <para>The prefab.</para>
    /// </summary>
    public BaseAgent agent;

    /// <summary>
    ///   <para>He who smelt it... I mean layed it.</para>
    /// </summary>
    public BaseAgent parent;

    /// <summary>
    ///   <para>The node belonging to the parent</para>
    /// </summary>
    public AncestorNode parent_node;

    /// <summary>
    ///   <para>The generation of the egg</para>
    /// </summary>
    public int generation;

    /// <summary>
    ///   <para>The model that controls the agent</para>
    /// </summary>
    public NEATNetwork brain;
    public Genome model_genome;

    /// <summary>
    /// If the egg was created by the manager instead of another agent
    /// </summary>
    public bool initial_population;

    public Genes genes = null;

    /// <summary>
    /// True if the egg was produced using sexual reproduction, false otherwise.
    /// </summary>
    private bool sexually_produced;
    private BaseAgent p1, p2;

    public float time_spent_building = 0;

    public float energy_density;

    public void Setup(int id, float e, Genes genes, Manager m, BaseAgent a, Brain parent_brain)
    {

        // Set genes and mutate them
        this.genes = genes;
        this.genes.Mutate();

        // Set the agent
        this.agent = a;

        // Set the values
        manager = m;
        energy = e;
        size = energy / manager.egg_energy_density;
        transform.localScale = new Vector3(size, size, size);

        // Calculate the gestation time
        gestation_time = Mathf.Max(Mathf.Log(genes.gestation_time * size * e) * parent.base_gestation_time, parent.base_gestation_time);
        total_gestation_time = gestation_time;

        // Setup the parent node & generation
        parent_node = parent.node;
        generation = parent.generation + 1;

        // Give the egg a brain!
        brain = (NEATNetwork)parent_brain.GetModel();
        model_genome = brain.CopyGenome();

        base.Setup(id);

        // Lightly set the color of the egg to that of its parent
        this.sprite.color = new Color(genes.colour_r + 0.5f, genes.colour_g + 0.5f, genes.colour_b + 0.5f);

    }

    public void Setup(int id, float e, Genes genes, Manager m, BaseAgent a, BaseAgent parent_1, BaseAgent parent_2)
    {

        // Set genes and mutate them
        this.genes = genes;
        this.genes.Mutate();

        // Set the agent
        this.agent = a;

        // Set the values
        manager = m;
        energy = e;
        size = energy / manager.egg_energy_density;
        transform.localScale = new Vector3(size, size, size);

        // Calculate the gestation time
        gestation_time = Mathf.Max(Mathf.Log(genes.gestation_time * size * e) * parent.base_gestation_time, parent.base_gestation_time);
        total_gestation_time = gestation_time;

        // Setup the parent node & generation
        parent_node = parent.node;
        generation = parent.generation + 1;

        // Give the egg a brain!
        brain = (NEATNetwork)parent_1.brain.GetModel();
        model_genome = ((NEATNetwork)parent_1.brain.GetModel()).CopyGenome();

        // Setup the parents
        sexually_produced = true;
        p1 = parent_1;
        p2 = parent_2;

        // TODO implement crossover (it already exists in the genome!)
        // Setup as an interactable
        base.Setup(id);

        // Lightly set the color of the egg to that of its parent
        this.sprite.color = new Color(genes.colour_r + 0.5f, genes.colour_g + 0.5f, genes.colour_b + 0.5f);
    }

    public void Setup(Manager m, int id)
    {
        // Set genes and mutate them
        if(m.dynamic_mutation_rates){
            this.genes = m.random_initial_genes ? Genes.GetRandomGenes() : Genes.GetBaseGenes();
        }else{
            this.genes = Genes.GetBaseGenesSetMutationRate();
        }

        // Set the agent
        this.agent = m.agent;

        // Set the values
        manager = m;
        energy = (Mathf.Pow(genes.size, 2) * agent.base_health);
        size = energy/  manager.egg_energy_density;
        transform.localScale = new Vector3(size, size, size);

        // Calculate the gestation time
        gestation_time = Mathf.Max(Mathf.Log(genes.gestation_time * size * energy) * agent.base_gestation_time, agent.base_gestation_time) * Random.Range(0.5f, 2f);
        total_gestation_time = gestation_time;

        // Generate a name
        string identifier = "<" + Random.Range(1000, 10000).ToString("X") + ">";
        string full_name = identifier + NameGenerator.GenerateFullName();
        genes.genus = full_name.Split(' ')[0];
        genes.species = full_name.Split(' ')[1];

        // Setup the parent node & generation
        parent_node = new AncestorNode(parent: null, genes: genes, name: full_name); ;
        generation = 0;

        // To track the ancestory, we use an ancestor manager object which is a tree like structure using nodes
        manager.anc_manager.AddGenus(parent_node);

        // Add the egg to the manager
        manager.AddAgent(this);

        // Set this as the initial population
        initial_population = true;

        // Give the egg a brain!
        // agent.brain.Setup();
        brain = (NEATNetwork)agent.brain.GetModel();

        // Setup as an interactable
        base.Setup(id);

        // Lightly set the color of the egg to that of its parent
        this.sprite.color = new Color(genes.colour_r + 0.5f, genes.colour_g + 0.5f, genes.colour_b + 0.5f);
    }

    public void Update()
    {
        // If it has been eaten, get rid of the object
        if (energy == 0)
        {
            // Remove from this list of all agents
            manager.agents.Remove(this);
            manager.GetComponent<EntityPoolManager>().Destroy(this);
        }

        gestation_time -= Time.deltaTime;
        if (gestation_time <= 0)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            //BaseAgent a = Instantiate(agent, transform.localPosition, transform.rotation, manager.transform);
            BaseAgent a = manager.GetComponent<EntityPoolManager>().InstantiateAgent(agent, transform.localPosition, transform.rotation, manager.transform);
            a.transform.Rotate(new Vector3(0, 0, Random.Range(0, 180f)));

            // If created by manager, we want to randomize is body
            if (initial_population)
            {
                // Set the genes' spritemap (establish the look of the agent randomly)
                manager.GetComponent<SpriteManager>().SetRandomComponents(genes);

                // If it is the initial population, we dont have any genetic drift
                genes.genetic_drift = 0;
            }
            else
            {
                genes.genetic_drift = manager.anc_manager.GetAverageGenes(genes.genus + " " + genes.species).CalculateGeneticDrift(genes);
            }

            // Add the agent object to the manager's list
            manager.AddAgent(a);

            // To track the ancestory, we use an ancestor manager object which is a tree like structure using nodes
            a.node = parent_node;

            // Increment the generation
            a.generation = generation;


            // TODO This is a temporary step to create new agents in new species to test its funcitonality!
            if (genes.genetic_drift > 5f && parent_node != null)
            {
                string t = "<" + Random.Range(100000, 1000000).ToString() + ">";
                string new_name = NameGenerator.GenerateFullName() + t;
                genes.species = new_name.Split(' ')[1];
                genes.genetic_drift = 0;
                a.generation = 0;

                AncestorNode new_node = new AncestorNode(parent_node, genes.Clone(), genes.genus + " " + genes.species);
                a.node = new_node;

                // Add a new node to the familial tree
                parent_node.AddChild(new_node);
            }

            // Setup the agent
            a.Setup(id - 1, genes, manager);
            a.energy = energy;
            a.GetRB().velocity = this.rb.velocity;

            // Setup the brain
            SetupBrain(a);

            // Setup using parents
            HandleReproductionMode(a);

            // Remove the egg from the manager
            manager.agents.Remove(this);

            // Update the population in the ancestor manager
            manager.anc_manager.UpdatePopulation(a);

            time_spent_building = watch.ElapsedMilliseconds / 1000f;

            // Kill the egg!
            manager.GetComponent<EntityPoolManager>().Destroy(this);

            
        }
energy_density = GetEnergyDensity();
        if(Input.GetKeyDown(KeyCode.H))
        {
            gestation_time = 0;
        }
    }

    /// <summary>
    /// If the agent was created using sexual reproduciton, the parents will determine the colouration
    /// </summary>
    /// <param name="a"></param>
    public void HandleReproductionMode(BaseAgent a)
    {
        if (sexually_produced)
        {
            Color c1 = new Color(p1.genes.colour_r, p1.genes.colour_g, p1.genes.colour_b);
            Color c2 = new Color(p2.genes.colour_r, p1.genes.colour_g, p1.genes.colour_b);
            manager.SetSprite(a, c1, c2);
        }
    }

    private void SetupBrain(BaseAgent a)
    {

        // Give the brain
        if (initial_population)
        {
            // If initial population, give a new brain
            a.brain.Setup();
        }
        else
        {
            // Else, use the brain set by the parent
            ((EvolutionaryNEATLearner)a.brain).SetupGenome(model_genome);

        }
    }

    public override float Eat()
    {
        // Temp store the energy so it can be returned
        float temp_energy = energy;

        // Set the energy of this egg to zero since it has been eaten
        energy = 0;

        return temp_energy;
    }
}
