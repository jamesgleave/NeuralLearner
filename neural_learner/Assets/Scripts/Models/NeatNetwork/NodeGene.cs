using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGene
{
    /// <summary>
	/// The type of this node Gene
	/// </summary>
    private NodeGeneType type;

    /// <summary>
	/// The ID of the connection
	/// </summary>
    private int id;

    private int input_distance;

    public NodeGene(NodeGeneType type, int id)
    {
        this.type = type;
        this.id = id;
    }

    /// <summary>
	/// Get the ID of the connection
	/// </summary>
	/// <returns></returns>
    public int GetID()
    {
        return id;
    }

    /// <summary>
	/// Get the type of this connection
	/// </summary>
	/// <returns></returns>
    public NodeGeneType GetNodeType()
    {
        return type;
    }

    /// <summary>
    /// Returns true if the node is a hidden node
    /// </summary>
    /// <returns></returns>
    public bool IsHidden()
    {
        return type == NodeGeneType.Hidden;
    }

    /// <summary>
    /// Returns true if the node is an input node
    /// </summary>
    /// <returns></returns>
    public bool IsInput()
    {
        return type == NodeGeneType.Input;
    }

    /// <summary>
    /// Returns true if the node is an ouput node
    /// </summary>
    /// <returns></returns>
    public bool IsOutput()
    {
        return type == NodeGeneType.Output;
    }

    /// <summary>
    /// Returns a memberwise clone of this node
    /// </summary>
    /// <returns></returns>
    public NodeGene Copy()
    {
        return new NodeGene(type, id);
    }


}
