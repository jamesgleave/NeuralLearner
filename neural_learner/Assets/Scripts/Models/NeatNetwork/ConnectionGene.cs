using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Activations;

public class ConnectionGene
{
    /// <summary>
	/// The ID of the in node
	/// </summary>
    private int in_node;

    /// <summary>
	/// The ID of the out node
	/// </summary>
    private int out_node;

    /// <summary>
	/// The weight of the connection between the in_node and out_node
	/// </summary>
    private float weight;

    /// <summary>
	/// Whether or not the connection is currently expressed
	/// </summary>
    private bool expressed;

    /// <summary>
	/// The innovation number of the connection
	/// </summary>
    private int innovation_number;

    public ConnectionGene(int in_node, int out_node, float weight, bool expressed, int innovation)
    {
        this.in_node = in_node;
        this.out_node = out_node;
        this.weight = weight;
        this.expressed = expressed;
        this.innovation_number = innovation;

    }

    /**
     * Setter Functions
     */

    /// <summary>
    /// Set the innovation number of the connection
    /// </summary>
    /// <param name="n"></param>
    public void SetInnovationNumber(int n)
    {
        innovation_number = n;
    }

    /// <summary>
    /// Set the in node of the connection
    /// </summary>
    /// <param name="n"></param>
    public void SetInNode(int n)
    {
        in_node = n;
    }

    /// <summary>
    /// Set the out node of the connection
    /// </summary>
    /// <param name="n"></param>
    public void SetOutNode(int n)
    {
        out_node = n;
    }

    /// <summary>
    /// Set the weight of the connection
    /// </summary>
    /// <param name="new_weight"></param>
    public void SetWeight(float new_weight)
    {
        weight = new_weight;
    }

    /// <summary>
    /// Set whether or not the connection is expressed
    /// </summary>
    /// <param name="status"></param>
    public void SetExpressedStatus(bool status)
    {
        expressed = status;
    }


    /**
   * Getter Functions
   */

    public int getInNode()
    {
        return in_node;
    }

    public int getOutNode()
    {
        return out_node;
    }

    public float GetWeight()
    {
        return weight;
    }

    public bool IsExpressed()
    {
        return expressed;
    }

    public int GetInnovation()
    {
        return innovation_number;
    }

    public ConnectionGene Copy()
    {
        return new ConnectionGene(in_node, out_node, weight, expressed, innovation_number);
    }

    public string GetConnectionString()
    {
        if (expressed)
        {
            return "(" + innovation_number + ") " + in_node + " -(" + weight + ")-> " + out_node;
        }
        else
        {
            return "Unexpressed: " + "(" + innovation_number + ") " + in_node + " -(" + weight + ")-> " + out_node;
        }
    }
}

