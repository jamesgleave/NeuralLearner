using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
///   <para>Holds the ID values for interactables</para>
/// </summary>
public enum ID
{
    Wobbit = 1,
    WobbitEgg = 2,
    FoodPellet = 3,
    Meat = 4,

    red_pharomone = 6,
    blue_pharomone = 7,
    green_pharomone = 8,

    MaxID = 10,

    // For neuron display (Anything outside of the simulation has a negative ID because I said so)
    Neuron = -1
}

/// <summary>
///   <para>The base class for all interactable items</para>
/// </summary>
public class Interactable : MonoBehaviour
{
    // The rigidbody attatched to the object
    protected Rigidbody2D rb;

    // The collider attached to this item
    public Collider2D col;

    // The sprite renderer
    protected SpriteRenderer sprite;

    /// <summary>
    ///   <para>The id of the interactable. Should be a value between 0 and 1000.</para>
    /// </summary>
    protected int id;

    /// <summary>
    ///   <para>Sets up the interactable by assinging its rigidbody, collider, and id code. This should always be called when instantiating an interactable</para>
    /// </summary>
    public void Setup(int id)
    {
        // Set the rigidbody, collider, and sprite renderer
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        if (col == null)
        {
            col = GetComponentsInChildren<Collider2D>()[1];
        }

        sprite = GetComponent<SpriteRenderer>();

        // Setup the id
        this.id = id;
    }

    /// <summary>
    ///   <para>Returns the int id of the interactable.</para>
    /// </summary>
    public int GetID()
    {
        return id;
    }

    public Collider2D GetCol()
    {
        return col;
    }

    public Rigidbody2D GetRB()
    {
        return rb;
    }

    public void GenerateCollider()
    {
        // Only generate if it is of the type composite collider
        if (col.GetType() == typeof(CompositeCollider2D))
        {
            ((CompositeCollider2D)col).generationType = CompositeCollider2D.GenerationType.Manual;
            ((CompositeCollider2D)col).GenerateGeometry();
        }
    }

}
