using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AncestorDisplayFDG : MonoBehaviour
{
    [Header("The FDG Node")]
    public GameObject node;

    [Space()]
    [Header("Generation Parameters")]
    public float neighbour_prob;
    public int n_neighbour_attempts;

    [Space()]
    [Header("Node Parameters")]
    public float gravity;
    public float base_spring;

    [Space()]
    [Header("Other")]
    public ComputeShader cs;
    ComputeBuffer position_buffer;
    ComputeBuffer force_buffer;

    public Vector3[] node_positions;
    public Vector3[] node_forces;
    public List<AncestoryDisplayNodeFDG> nodes;
    public Dictionary<string, AncestoryDisplayNodeFDG> fast_access_nodes;

    /// <summary>
    /// Singleton instance
    /// </summary>
    public static AncestorDisplayFDG instance;
    public AncestorManager manager;


    void Start()
    {
        // Set our singleton instance
        instance = this;

        // Create a list and dict of our nodes
        nodes = new List<AncestoryDisplayNodeFDG>();
        fast_access_nodes = new Dictionary<string, AncestoryDisplayNodeFDG>();

        // Set the manager
        manager = StateManager.ancestor_manager;

        // Get the selected wobbit
        AncestorNode selected = manager.genus_tree[StateManager.selected_agent.genus];

        // Set up all of the nodes
        AddNodes(selected, 0);

        // Set up our values for our buffer
        cs.SetInt("n", nodes.Count);
        node_positions = new Vector3[nodes.Count];
        node_forces = new Vector3[nodes.Count];
        position_buffer = new ComputeBuffer(nodes.Count, sizeof(float) * 3);
        force_buffer = new ComputeBuffer(nodes.Count, sizeof(float) * 3);
        cs.SetBuffer(0, "Positions", position_buffer);
        cs.SetBuffer(0, "Forces", force_buffer);
    }

    void AddNodes(AncestorNode root, int id)
    {
        // Create the display node
        AncestoryDisplayNodeFDG display_node = Instantiate(this.node, Random.insideUnitSphere * 0.01f, Quaternion.identity).GetComponent<AncestoryDisplayNodeFDG>();

        // Set up our new node
        display_node.Setup(root);
        display_node.id = id;

        // Add the selected node to our list and dict
        nodes.Add(display_node);
        fast_access_nodes[root.full_name] = display_node;

        // Add a connection between this and its parent
        if (root.parent != null)
        {
            display_node.AddNeighbour(fast_access_nodes[root.parent.full_name]);
        }

        // Loop through each child and add them
        foreach (AncestorNode child in root.children.Values)
        {
            id++;
            AddNodes(child, id);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Set up the compute buffer...
        for (int i = 0; i < nodes.Count; i++)
        {
            node_positions[i] = nodes[i].transform.position;
            nodes[i].SetValues(base_spring, 0.1f, nodes[i].adjacent_nodes.Count / 3.4f * 100);
        }

        // Create a position and force buffer
        position_buffer.SetData(node_positions);

        // Run the thing
        int num_threads = Mathf.Max(nodes.Count / 32, nodes.Count);
        cs.Dispatch(0, num_threads, 1, 1);

        // Get the data back!
        force_buffer.GetData(node_forces);

        // Set up the compute buffer...
        for (int i = 0; i < nodes.Count; i++)
        {
            // Send the force the the node update
            nodes[i].UpdateNode(node_forces[i], gravity);
        }

        // Check if we go back to the sim
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GetComponent<SimulationSceneManager>().ToSim();
        }
    }

    public void OnDestroy()
    {
        // Get rid of em
        position_buffer.Dispose();
        force_buffer.Dispose();
    }
}
