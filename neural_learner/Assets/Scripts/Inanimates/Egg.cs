using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : Interactable
{
    /// <summary>
    ///   <para>The energy containted in the egg</para>
    /// </summary>
    public float energy;

    /// <summary>
    ///   <para>The scale of the egg</para>
    /// </summary>
    public float size;

    public float gestation_time;

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
    public Model.BaseModel brain = null;

    protected Genes genes = null;

    public void Setup(int id, float e, Genes genes, Manager m, BaseAgent a)
    {

        // Set genes and mutate them
        this.genes = genes;
        this.genes.Mutate();

        // Set the agent
        this.agent = a;

        // Set the values
        manager = m;
        energy = e;
        size = genes.size * 3;
        transform.localScale = new Vector3(size, size, size);
        gestation_time = Mathf.Max(Mathf.Log(genes.gestation_time * size * e) * parent.base_gestation_time, parent.base_gestation_time);

        // Setup the parent node & generation
        parent_node = parent.node;
        generation = parent.generation + 1;

        base.Setup(id);
    }

    public void Update()
    {
        // If it has been eaten, get rid of the object
        if (energy == 0)
        {
            // Remove from this list of all agents
            manager.agents.Remove(this);

            Destroy(gameObject);
        }

        gestation_time -= Time.deltaTime;
        if (gestation_time <= 0)
        {
            // TODO create a funciton for the manager that takes the ID and  returns the agent (given we have multiple types of agents)
            BaseAgent a = Instantiate(agent, transform.localPosition, transform.rotation, manager.transform);

            // Add the agent object to the manager's list
            manager.AddAgent(a);

            // To track the ancestory, we use an ancestor manager object which is a tree like structure using nodes
            a.node = parent_node;
            // Set the genetic drift of the agent by checking againts its parent's node. The parent's node contains the genes of the original.
            genes.genetic_drift = manager.anc_manager.GetAverageGenes(genes.genus + " " + genes.species).CalculateGeneticDrift(genes);
            // Increment the generation
            a.generation = generation;


            // TODO This is a temporary step to create new agents in new species to test its funcitonality!
            if (genes.genetic_drift > 1f && parent_node != null)
            {
                print(manager.anc_manager.IsNewSpecies(genes));
                string new_name = NameGenerator.GenerateFullName();
                if (manager.anc_manager.population.ContainsKey(new_name) || manager.anc_manager.IsSpecies(a.node.genus, new_name.Split(' ')[1]))
                {
                    print("No! Bad! Name Taken! XD Appending random number for now...");
                    new_name += Random.Range(0, 100).ToString();
                }

                genes.species = new_name.Split(' ')[1];
                genes.genetic_drift = 0;
                a.generation = 0;

                AncestorNode new_node = new AncestorNode(parent_node, genes.Clone(), genes.genus + " " + genes.species);
                a.node = new_node;

                // Add a new node to the familial tree
                parent_node.AddChild(new_node);
            }
            else if (genes.genetic_drift > 1f)
            {
                print("There was an error with: " + a.genus + " " + a.species);
                print(parent);
                print("");
            }

            // Setup the agent
            a.Setup(id - 1, genes, manager);
            a.energy = energy;

            // Give the brain
            a.brain.Setup(brain);

            // Remove the egg from the manager
            manager.agents.Remove(this);

            // Update the population in the ancestor manager
            manager.anc_manager.UpdatePopulation(a);

            // Kill the egg!
            Destroy(gameObject);
        }
    }

    public float Eat()
    {
        // Temp store the energy so it can be returned
        float temp_energy = energy;

        // Set the energy of this egg to zero since it has been eaten
        energy = 0;

        return temp_energy;
    }

}
