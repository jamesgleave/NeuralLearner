using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject node;
    public int num_nodes;
    public float neighbour_prob;
    public List<ForceDirectedNode> nodes;

    void Start()
    {
        nodes = new List<ForceDirectedNode>();
        for(int i = 0; i < num_nodes; i ++){
            ForceDirectedNode node = Instantiate(this.node).GetComponent<ForceDirectedNode>();
            nodes.Add(node);
        }

        foreach(ForceDirectedNode node1 in nodes)
        {
            foreach(ForceDirectedNode node2 in nodes)
            {
                // Add a random conneciton
                if(Random.value > neighbour_prob){
                    continue;
                }

                int rand_index = Random.Range(0, num_nodes);
                ForceDirectedNode node3 = nodes[rand_index];
                if(node1 != node3){
                    node1.AddNeighbour(node3);
                }
            }
        }
    }
}