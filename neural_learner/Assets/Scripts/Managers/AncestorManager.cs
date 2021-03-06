using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
///   <para> This class represents a node in an ancestor tree</para>
/// </summary>
[System.Serializable]
public class AncestorNode
{
    public string genus;
    public string species;
    public string full_name;
    public float genetic_drift;
    public AncestorNode parent;
    public Genes original_genes;
    public Dictionary<string, AncestorNode> children = new Dictionary<string, AncestorNode>();

    public AncestorNode(AncestorNode parent, Genes genes, string name)
    {
        full_name = name;
        genus = name.Split(' ')[0];
        species = name.Split(' ')[1];
        this.parent = parent;
        this.original_genes = genes;
    }

    public AncestorNode(Genes genes, string name)
    {
        genus = name.Split(' ')[0];
        species = name.Split(' ')[1];
        this.original_genes = genes;
    }

    public void AddChild(AncestorNode child)
    {
        // Add a child to the dictionary
        if (!children.ContainsKey(child.species))
        {
            children[child.species] = child;
            if (child.parent == null)
            {
                child.parent = this;
            }
        }
    }

    public string FullName()
    {
        return genus + " " + species;
    }

    public AncestorNode Clone()
    {
        return new AncestorNode(this.parent, this.original_genes, FullName());
    }
}


/// <summary>
///   <para>This object stores populations given a species name. This is used to track populations.</para>
/// </summary>
[System.Serializable]
public class PopulationContainer
{
    /// <summary>
    ///   <para>The population size.</para>
    /// </summary>
    public int size;

    /// <summary>
    ///   <para>The largest a population has been.</para>
    /// </summary>
    public int max_size;

    /// <summary>
    ///   <para>The name of the population</para>
    /// </summary>
    public string pop_name;

    /// <summary>
    ///   <para>Contains all individuals within a species.</para>
    /// </summary>
    public List<BaseAgent> agents;

    /// <summary>
    ///   <para>The average genes across a whole species</para>
    /// </summary>
    public Genes average;

    /// <summary>
    ///   <para>If the species is extinct</para>
    /// </summary>
    public bool extinct = false;

    /// <summary>
    /// If true, then this species has had enough agents to be considered a true new species
    /// </summary>
    public bool is_proper_population;

    /// <summary>
    /// The time created
    /// </summary>
    public float time_created;


    /// <summary>
    /// The genes of the first agent of the population
    /// </summary>
    public Genes initial_genes;


    public PopulationContainer(Genes initial_genes)
    {
        // setup the list
        time_created = Time.realtimeSinceStartup;
        agents = new List<BaseAgent>();
        average = null;

        // Store the initial genes of the population
        this.initial_genes = initial_genes;
    }

    public void CheckExtinct()
    {

        // Remove all null values (agents which have died)
        for (int i = agents.Count - 1; i >= 0; i--)
        {
            if (agents[i].CompareTag("DeadAgent") || agents[i] == null)
            {
                agents.RemoveAt(i);
            }
        }

        // Set the size
        size = agents.Count;

        // If there are no agents in the population, they are extinct!
        if (size == 0)
        {
            // Get rid of extra baggage
            extinct = true;
        }
        else
        {
            // If there is more than one agent, the species is not extinct nah not once XD
            extinct = false;
        }
    }

    public void AddAgent(BaseAgent a, string full_name)
    {

        // Check to see if species is developed
        if (max_size > 3 || a.node.children.Count != 0 || a.node.parent == null)
        {
            is_proper_population = true;
        }

        // Remove all null values (agents which have died)
        for (int i = agents.Count - 1; i >= 0; i--)
        {
            if (agents[i].isActiveAndEnabled == false || agents[i] == null)
            {
                agents.RemoveAt(i);
            }
        }

        // If we do not have an average genes value yet, calculate it from scratch
        if (average == null || true)
        {
            //// Set average to the passed agent to start recalculating the average genes
            average = a.genes;
        }
        // If we do, we can iteravely calculate it
        else
        {
            // To calculate the average without having to redefine and count the whole group is:
            // new_mean = old_mean + (x - old_mean)/n
            average = average + (a.genes - average) / (agents.Count + 1);
        }

        // Add the agent to the list
        agents.Add(a);

        // Update the population size
        size = agents.Count;

        // Update the largest population value
        max_size = (size > max_size ? size : max_size);

        // Set the name
        pop_name = full_name;
    }
}


/// <summary>
///   <para> Manages all aspects of the ancestory of the agents!</para>
/// </summary>
[System.Serializable]
public class AncestorManager
{
    /// <summary>
    ///   <para>The roots of all ancestory trees.</para>
    /// </summary>
    public Dictionary<string, AncestorNode> genus_tree = new Dictionary<string, AncestorNode>();

    /// <summary>
    ///   <para>All genus names.</para>
    /// </summary>
    public List<string> genus_names = new List<string>();

    /// <summary>
    ///   <para>All names ever created.</para>
    /// </summary>
    public List<string> names = new List<string>();

    /// <summary>
    ///   <para>Stores information about populations. The keys are the names of a species and the items are population containers which store all the info about a population.</para>
    /// </summary>
    public Dictionary<string, PopulationContainer> population = new Dictionary<string, PopulationContainer>();
    public List<string> counts = new List<string>();

    // Singleton instance
    public static AncestorManager instance;

    public AncestorManager()
    {
        instance = this;
    }

    public void AddGenus(AncestorNode child)
    {
        // Add a child to the dictionary
        if (!genus_tree.ContainsKey(child.genus))
        {
            genus_tree[child.genus] = child;
            genus_names.Add(child.genus);
            names.Add(child.genus + " " + child.species);
        }
    }

    public void UpdatePopulation(BaseAgent agent)
    {
        // If the population contains the species, add the agent to the species.
        PopulationContainer container;
        string fullname = agent.genes.genus + " " + agent.genes.species;
        if (population.ContainsKey(fullname))
        {
            // Add the agent
            container = population[fullname];
            container.AddAgent(agent, fullname);
        }
        else
        {
            // Create a new population container and add the agent
            container = new PopulationContainer(agent.genes);
            container.AddAgent(agent, fullname);
            population[fullname] = container;
        }

        // Update the max size
        container.max_size = Mathf.Max(population[fullname].max_size, population[fullname].size);
    }

    public void SetupTesting()
    {

        string name = "A A";
        AncestorNode a = new AncestorNode(null, Genes.GetBaseGenes(), name);
        AncestorNode b = new AncestorNode(a, Genes.GetBaseGenes(), "A B");
        AncestorNode c = new AncestorNode(Genes.GetBaseGenes(), "A C");
        AncestorNode d = new AncestorNode(Genes.GetBaseGenes(), "A D");
        AncestorNode e = new AncestorNode(Genes.GetBaseGenes(), "A E");
        AncestorNode f = new AncestorNode(Genes.GetBaseGenes(), "A F");
        AncestorNode g = new AncestorNode(Genes.GetBaseGenes(), "A G");
        AncestorNode h = new AncestorNode(Genes.GetBaseGenes(), "A H");

        AddGenus(a);

        a.AddChild(b);
        a.AddChild(c);
        a.AddChild(d);

        b.AddChild(e);

        c.AddChild(f);
        c.AddChild(g);

        g.AddChild(h);

        AncestorNode a2 = new AncestorNode(null, Genes.GetBaseGenes(), "X Y");
        AncestorNode c2 = new AncestorNode(a2, Genes.GetBaseGenes(), "X Z");
        AddGenus(a2);
        a2.AddChild(c2);

    }

    /// <summary>
    ///   <para>Returns the average genes of a species</para>
    /// </summary>
    public Genes GetAverageGenes(string name)
    {
        return population[name].average;
    }

    // Finds whether or not an agent has drifed far enough away from its original species to be considered a new one
    public bool DetermineRelation(Genes genes)
    {
        return true;
    }

    public bool IsSpecies(string genus, string species)
    {
        return genus_tree[genus].children.ContainsKey(species);
    }

    // Determines whether or not an individual has drifted far enough away
    public string IsNewSpecies(Genes genes)
    {
        // If the individual has drifted far enough away from its parent. We compare each member of the species to the individual,
        // and if they are closer to the individual, the become they are relabelled as the species.

        // Grab the name and average genes for the parent species
        string fullname = genes.genus + " " + genes.species;
        Genes species_average_genes = GetAverageGenes(fullname);

        if (population[fullname].size == 0)
        {
            return "";
        }

        // Look at each sibling species (comes from immediate common ancestor)
        int num_closer = 1;
        foreach (var sib in population[fullname].agents)
        {
            // if the sibling is closer genetically to the passed genes than the parent's genes then it is a part of that species
            if (sib.node.parent != null && sib.genes.CalculateGeneticDrift(genes) < sib.genes.CalculateGeneticDrift(sib.node.parent.original_genes))
            {
                num_closer++;
            }
            else if (sib.node.parent == null)
            {
                return fullname + " Is An OG Species";
            }
        }

        return ("Number closer for " + fullname + " is " + num_closer.ToString() + ". Expressed: " + (num_closer / population[fullname].size).ToString());
    }
}
