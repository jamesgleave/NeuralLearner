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

    /// <summary>
    /// The string encoding of the activation function
    /// </summary>
    /// <param name="type"></param>
    /// <param name="id"></param>
    private string activation;

    public NodeGene(NodeGeneType type, int id, string activation = "Tanh")
    {
        this.type = type;
        this.id = id;

        // Set as sigmoid if it is an output node
        if (IsOutput())
        {
            activation = "Tanh";
        }
        this.activation = activation;

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
    /// Increment a nodes ID value by one
    /// </summary>
    public void IncrementID()
    {
        id++;
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
        return new NodeGene(type, id, activation);
    }

    public string GetActivationString()
    {
        return activation;
    }

    public void SetActivationString(string activ)
    {
        activation = activ;
    }


}
