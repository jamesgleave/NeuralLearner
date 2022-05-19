using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AncestoryDisplayNodeFDG : ForceDirectedNode
{


    [Header("Ancestory Settings")]
    [Space]
    [Space]

    /// <summary>
    ///   <para>The node associated with this display node</para>
    /// </summary>
    public AncestorNode node;

    /// <summary>
    /// The display associaded with the agent
    /// </summary>
    public DisplayWobbit display_wobbit;

    /// <summary>
    ///   <para>The 'parent' of this node</para>
    /// </summary>
    public AncestorDisplayNode parent;

    /// <summary>
    /// The parent's name
    /// </summary>
    public string parent_name;

    // The line drawer for this node
    public LineDrawer artist;
    public Material artist_material;

    [Space]

    /// <summary>
    ///   <para>The full name of the agent.</para>
    /// </summary>
    public string fullname;

    /// <summary>
    ///   <para>How many agents of this species are there?</para>
    /// </summary>
    public float population_size;

    /// <summary>
    /// The time when the ancestor node was created
    /// </summary>
    public float node_time;

    /// <summary>
    /// True if this node is a proper population
    /// </summary>
    public bool proper;

    // Start is called before the first frame update
    public new void Start()
    {
        // Call the base start function for ForceDirectedNode
        rb = GetComponent<Rigidbody>();
    }

    public void Setup(AncestorNode n)
    {
        // Grab the time created
        node_time = AncestorDisplayFDG.instance.manager.population[n.genus + " " + n.species].time_created;

        // Assign basic identifiers
        this.node = n;

        // The line drawer
        artist = Instantiate<LineDrawer>(artist, transform);

        // Now setup the line width
        GetComponentInChildren<LineDrawer>().Setup();

        // Setup the display wobbit
        display_wobbit = GetComponent<DisplayWobbit>();
        display_wobbit.sprite_manager = StateManager.manager.GetComponent<SpriteManager>();

        // If there is no parent, we have no line to this point... so we do not deal with the incomming size
        if (parent == null)
        {
            GetComponentInChildren<LineDrawer>().SetFeatures(n.original_genes.size / 10, n.original_genes.size / 10);
            display_wobbit.Setup(n.original_genes);
        }
        else
        {
            GetComponentInChildren<LineDrawer>().SetFeatures(n.parent.original_genes.size / 10, n.original_genes.size / 10);
            display_wobbit.Setup(n.parent.original_genes);
        }

        // Check if extinct!
        AncestorDisplayFDG.instance.manager.population[n.genus + " " + n.species].CheckExtinct();
        display_wobbit.SetCross(AncestorDisplayFDG.instance.manager.population[n.genus + " " + n.species].extinct);

        // Get the population size
        population_size = AncestorDisplayFDG.instance.manager.population[n.genus + " " + n.species].size;
        print(n.FullName() + ": " + population_size);

        // If the species is extinct, change the colour of the line to red
        if (AncestorDisplayFDG.instance.manager.population[n.genus + " " + n.species].extinct)
        {
            artist.GetComponent<LineRenderer>().material.color = Color.red;
        }
    }
}
