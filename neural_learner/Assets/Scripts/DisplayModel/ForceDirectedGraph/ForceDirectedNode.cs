using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ForceDirectedNode : Interactable
{
    [Tooltip("The magnitude of the force used to repulse two non-connected neurons")]
    public float repulsion_force;

    [Tooltip("The ID of this node")]
    public int id;
    public int n_adjacent_nodes = 0;

    public Material linemat;
    public Material selected_mat, deselected_mat;

    // The Rigidbody
    private Rigidbody rb;
    public List<SpringJoint> spring_joints = new List<SpringJoint>();
    public List<LineRenderer> line_renderers = new List<LineRenderer>();
    public Dictionary<int, ForceDirectedNode> adjacent_nodes = new Dictionary<int, ForceDirectedNode>();
    public List<int> adjacent_node_ids = new List<int>();


    // Start is called before the first frame update
    void Start()
    {
        // Set the rb
        rb = GetComponent<Rigidbody>();
    }

    public void Setup(int id)
    {
        this.id = id;
    }

    public void UpdateNode(Vector3 base_force, float gravity)
    {
        // Apply the force vector the the gameobject's rigidbody
        rb.AddForce((base_force * -repulsion_force) - (gravity * transform.position.normalized));
    }

    public void AddNeighbour(ForceDirectedNode node)
    {
        // Add a spring joint between this node and the neighbour and add it to our list
        SpringJoint sj = gameObject.AddComponent(typeof(SpringJoint)) as SpringJoint;
        sj.connectedBody = node.GetRB();
        spring_joints.Add(sj);

        // Create and add a line renderer
        var lr = new GameObject().AddComponent<LineRenderer>();
        lr.gameObject.transform.SetParent(transform, false);
        // just to be sure reset position and rotation as well
        lr.gameObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        line_renderers.Add(lr.GetComponent<LineRenderer>());

        // Setup the lines!
        lr.GetComponent<LineRenderer>().SetWidth(0.25f, 0.15f);
        lr.GetComponent<LineRenderer>().material = linemat;

        // Add the neighbour to the hashmap
        adjacent_nodes.Add(node.id, node);
        adjacent_node_ids.Add(node.id);
        n_adjacent_nodes++;
    }

    public void SetValues(float strength, float damper, float size)
    {
        // Look at each joint and edit the values
        for (int i = 0; i < spring_joints.Count; i++)
        {
            // Set the joint values
            spring_joints[i].spring = strength;
            spring_joints[i].damper = damper;

            // Check to see if the adjacent node we are looking at right now is also connected to this node (a loop)
            if (adjacent_nodes[adjacent_node_ids[i]].IsConnectedTo(id))
            {
                // Set the line drawer values
                // If the ID is higher than this node then +y else -y
                if (adjacent_node_ids[i] > this.id)
                {
                    line_renderers[i].SetPosition(0, transform.position + Vector3.up);
                    line_renderers[i].SetPosition(1, spring_joints[i].connectedBody.transform.position + Vector3.up / 2.25f);
                }
                else
                {
                    line_renderers[i].SetPosition(0, transform.position - Vector3.up);
                    line_renderers[i].SetPosition(1, spring_joints[i].connectedBody.transform.position - Vector3.up / 2.25f);
                }
            }
            else
            {
                // Set the line drawer values
                line_renderers[i].SetPosition(0, transform.position);
                line_renderers[i].SetPosition(1, spring_joints[i].connectedBody.transform.position);
            }
        }

        // Increase/Decrease the size according to the manager
        if (Mathf.Abs(transform.localScale.x - size) > 0.01f)
        {
            if (transform.localScale.x > size)
            {
                transform.localScale *= 0.99f;
            }
            else
            {
                transform.localScale *= 1.01f;
            }
        }
    }

    public bool IsConnectedTo(int id)
    {
        return this.adjacent_nodes.ContainsKey(id);
    }

    public Rigidbody GetRB()
    {
        return GetComponent<Rigidbody>();
    }

    public void Select()
    {
        GetComponent<MeshRenderer>().material = selected_mat;
    }

    public void Deselect()
    {
        GetComponent<MeshRenderer>().material = deselected_mat;
    }
}
