using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeShaderFGD : MonoBehaviour
{
    [Header("The FDG Node")]
    public GameObject node;

    [Space()]
    [Header("Generation Parameters")]
    public int num_nodes;
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
    public List<ForceDirectedNode> nodes;


    void Start()
    {
        nodes = new List<ForceDirectedNode>();
        for (int i = 0; i < num_nodes; i++)
        {
            ForceDirectedNode node = Instantiate(this.node, Random.insideUnitSphere * 0.01f, Quaternion.identity).GetComponent<ForceDirectedNode>();
            node.Setup(i);
            nodes.Add(node);
        }
        foreach (ForceDirectedNode node1 in nodes)
        {
            for (int i = 0; i < n_neighbour_attempts * Random.value * Random.value; i++)
            {
                int rand_index = Random.Range(0, num_nodes);
                ForceDirectedNode node3 = nodes[rand_index];
                if (node1 != node3 && !node1.IsConnectedTo(node3.id))
                {
                    node1.AddNeighbour(node3);
                }
            }

            // node1.GetRB().mass = 3 + node1.spring_joints.Count;
            float c = Mathf.Max(1, Mathf.Log10(node1.spring_joints.Count));
        }

        // Set up our values for our buffer
        cs.SetInt("n", num_nodes);
        node_positions = new Vector3[num_nodes];
        node_forces = new Vector3[num_nodes];
        position_buffer = new ComputeBuffer(num_nodes, sizeof(float) * 3);
        force_buffer = new ComputeBuffer(num_nodes, sizeof(float) * 3);
        cs.SetBuffer(0, "Positions", position_buffer);
        cs.SetBuffer(0, "Forces", force_buffer);
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
        int num_threads = Mathf.Max(num_nodes / 32, num_nodes);
        cs.Dispatch(0, num_threads, 1, 1);

        // Get the data back!
        force_buffer.GetData(node_forces);

        // Set up the compute buffer...
        for (int i = 0; i < nodes.Count; i++)
        {
            // Send the force the the node update
            nodes[i].UpdateNode(node_forces[i], gravity);
        }
    }

    public void OnDestroy()
    {
        // Get rid of em
        position_buffer.Dispose();
        force_buffer.Dispose();
    }
}
