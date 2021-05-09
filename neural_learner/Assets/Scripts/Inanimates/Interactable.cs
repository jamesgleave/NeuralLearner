using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
///   <para>Holds the ID values for interactables</para>
/// </summary>
public enum ID
{
    FoodPellet = 1000,
    Meat = 999,
    Wall = 0,
    Wobbit = 1,
    WobbitEgg = 2,

}

/// <summary>
///   <para>The base class for all interactable items</para>
/// </summary>
public class Interactable : MonoBehaviour
{
    // The rigidbody attatched to the object
    protected Rigidbody2D rb;

    // The collider attached to this item
    protected Collider2D col;

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

}
