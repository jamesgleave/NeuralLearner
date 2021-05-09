using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AncestorDisplay : MonoBehaviour
{
    [Header("Storage & Objects")]
    public AncestorDisplayNode node;
    public AncestorManager manager;
    public LineDrawer line_drawer;

    // The list containing every node
    public Dictionary<string, AncestorDisplayNode> nodes = new Dictionary<string, AncestorDisplayNode>();
    public List<AncestorDisplayNode> nodes_sorted = new List<AncestorDisplayNode>();

    [Space]
    [Header("Settings")]
    public Vector2 spacing;

    public enum ArrangementMethod
    {
        fiberoptic = 0,
        drift = 1,
        circlular = 2,
        circular_drift = 3,
        tree = 4,
        parabolic = 5
    }
    public ArrangementMethod arrangement = ArrangementMethod.fiberoptic;

    // Start is called before the first frame update
    void Start()
    {
        manager = StateManager.ancestor_manager;
        TraverseTree(genus: StateManager.selected_agent.genus);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GetComponent<SimulationSceneManager>().ToSim();
        }
    }

    void TraverseTree(string genus)
    {
        // Start at this genus
        CreateNode(0, 0, manager.genus_tree[genus], null);
        Search(manager.genus_tree[genus], spacing.y);

        // Sort the list based on depth
        nodes_sorted = nodes_sorted.OrderBy(o => o.depth).ToList();

        foreach (var n in nodes_sorted)
        {
            print(n.parent + "-> " + n.name);
        }

        // Assign each node to their parent
        foreach (AncestorDisplayNode n in nodes_sorted)
        {
            // Set a key (I do it like this out of laziness)
            string key = n.fullname;

            // The root node does not have a parent
            if (nodes[key].node.parent != null)
            {
                // Find the name of the parent
                nodes[key].parent_name = nodes[key].node.parent.FullName();

                // Now set up the node's parent
                nodes[key].parent = nodes[nodes[key].parent_name];

                // Set the transform of its parent to its parent transform (kinda confused)
                nodes[key].transform.parent = nodes[key].parent.transform;

                // Now that we have found the node and their parent, we can setup their position
                nodes[key].CalculateDesiredPosition();

                print(key + " parent -> " + nodes[key].parent.name);
            }
        }
    }


    void TraverseTree()
    {
        // Search and create the nodes
        foreach (string genus_name in manager.genus_tree.Keys)
        {
            CreateNode(0, 0, manager.genus_tree[genus_name], null);
            Search(manager.genus_tree[genus_name], spacing.y);
        }

        // Sort the list based on depth
        nodes_sorted = nodes_sorted.OrderBy(o => o.depth).ToList();

        // Assign each node to their parent
        foreach (AncestorDisplayNode n in nodes_sorted)
        {
            // Set a key (I do it like this out of laziness)
            string key = n.fullname;

            // The root node does not have a parent
            if (nodes[key].node.parent != null)
            {
                // Find the name of the parent
                nodes[key].parent_name = nodes[key].node.parent.FullName();

                // Now set up the node's parent
                nodes[key].parent = nodes[nodes[key].parent_name];

                // Now that we have found the node and their parent, we can setup their position
                nodes[key].CalculateDesiredPosition();
            }
        }
    }

    void Search(AncestorNode node, float depth = 0)
    {
        float position = 0;
        foreach (string child in node.children.Keys)
        {
            if (node.children.Count > 0)
            {
                // Search moar XD
                Search(node.children[child], depth + 1);

                // Create the node -> instantiate it lol
                CreateNode(position, depth, node.children[child], node);

                // Increment the position
                position += 1;
            }
        }
    }

    void CreateNode(float position, float depth, AncestorNode n, AncestorNode parent)
    {
        // Create a node
        AncestorDisplayNode a;
        if (parent != null && nodes.ContainsKey(parent.FullName()))
        {
            a = Instantiate(this.node, new Vector3(0, 0, 0), transform.rotation, nodes[parent.FullName()].transform);
        }
        else
        {
            a = Instantiate(this.node, new Vector3(0, 0, 0), transform.rotation, transform);
        }

        // Add it to the dict and the list 
        nodes[n.FullName()] = a;
        nodes_sorted.Add(a);

        // Setup the node
        a.Setup(n, this, position, depth);
        a.name = a.node.FullName();
    }
}
