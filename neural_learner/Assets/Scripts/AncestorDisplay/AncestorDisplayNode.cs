using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AncestorDisplayNode : MonoBehaviour
{
    /// <summary>
    ///   <para>The node associated with this display node</para>
    /// </summary>
    public AncestorNode node;

    /// <summary>
    ///   <para>The sprite attatched to the agent</para>
    /// </summary>
    public GameObject sprite;

    /// <summary>
    ///   <para>The 'parent' of this node</para>
    /// </summary>
    public AncestorDisplayNode parent;
    public string parent_name;

    // The line drawer for this node
    public LineDrawer artist;

    /// <summary>
    ///   <para>The display for callbacks</para>
    /// </summary>
    [Space]
    public AncestorDisplay display;

    /// <summary>
    ///   <para>The full name of the agent.</para>
    /// </summary>
    public string fullname;

    /// <summary>
    ///   <para>The layer of the ancestor. This is kind of saying how far separated the agent associated with this node is from the origional agent.</para>
    /// </summary>
    [Space]
    public float depth;

    /// <summary>
    ///   <para>The position in the list of siblings of the ancestor. This value is arbitrary for calculating the target position for animation.</para>
    /// </summary>
    public float position;

    /// <summary>
    ///   <para>The number of children this node has</para>
    /// </summary>
    public float num_children;

    /// <summary>
    ///   <para>The number of siblings this node has</para>
    /// </summary>
    public float num_siblings;

    /// <summary>
    ///   <para>The position where this display node belongs</para>
    /// </summary>
    [Space]
    public Vector3 target_positon;

    /// <summary>
    ///   <para>How many agents of this species are there?</para>
    /// </summary>
    public float population_size;

    /// <summary>
    ///   <para>Setup the node with an ancestor</para>
    /// </summary>
    public void Setup(AncestorNode n, AncestorDisplay disp, float p, float d)
    {
        // Assign basic identifiers
        fullname = n.FullName();
        this.display = disp;
        this.position = p;
        this.node = n;
        depth = d;

        // Grab the number of children
        num_children = n.children.Count;

        // The line drawer
        artist = Instantiate<LineDrawer>(artist, transform);

        // Set up the way that the sprite looks!
        // Colour:
        GetComponentInChildren<SpriteRenderer>().color = new Color(n.original_genes.colour_r, n.original_genes.colour_g, n.original_genes.colour_b);
        // Size:
        sprite.transform.localScale *= n.original_genes.size;

        // Now setup the line width
        GetComponentInChildren<LineDrawer>().Setup();

        // If there is no parent, we have no line to this point... so we do not deal with the incomming size
        if (parent == null)
        {
            GetComponentInChildren<LineDrawer>().SetFeatures(n.original_genes.size / 25, n.original_genes.size / 25);
        }
        else
        {
            GetComponentInChildren<LineDrawer>().SetFeatures(n.parent.original_genes.size / 25, n.original_genes.size / 25);
        }

        // Get the population size
        population_size = display.manager.population[n.FullName()].size;
        // If the species is extinct, change the colour of the line to red
        if (display.manager.population[n.FullName()].extinct)
        {
            artist.GetComponent<LineRenderer>().material.color = Color.red;
        }

    }

    public void CalculateDesiredPosition()
    {
        switch (display.arrangement)
        {
            case AncestorDisplay.ArrangementMethod.fiberoptic:
                if (node.parent != null)
                {
                    num_siblings = node.parent.children.Count;
                    float target_x = display.spacing.x * Mathf.Cos(360 / position) * (-1 * (num_siblings - position / display.spacing.x) % 2);
                    float target_y = display.spacing.x * Mathf.Sin(360 / position);
                    target_positon = new Vector3(target_x, target_y + display.spacing.y, 0);
                }
                else
                {
                    target_positon = new Vector3(0, 0, 0);
                }
                break;

            case AncestorDisplay.ArrangementMethod.drift:
                if (node.parent != null)
                {
                    num_siblings = node.parent.children.Count;
                    float target_x = display.spacing.x * Mathf.Cos(360 / position) * (-1 * (num_siblings - position / display.spacing.x) % 2) * (1 + node.genetic_drift);
                    float target_y = display.spacing.x * Mathf.Sin(360 / position) * (-1 * (num_siblings - position / display.spacing.x) % 2) * (1 + node.genetic_drift);
                    target_positon = new Vector3(target_x, target_y + display.spacing.y, 0);
                }
                else
                {
                    target_positon = new Vector3(0, 0, 0);
                }
                break;

            case AncestorDisplay.ArrangementMethod.circlular:
                if (node.parent != null)
                {
                    num_siblings = node.parent.children.Count;
                    float target_x = display.spacing.x * Mathf.Cos(360 / position - display.spacing.x / 2);
                    float target_y = display.spacing.x * Mathf.Sin(360 / position) - display.spacing.x / 2;
                    target_positon = new Vector3(target_x, target_y + display.spacing.y, 0);
                }
                else
                {
                    target_positon = new Vector3(0, 0, 0);
                }
                break;
            case AncestorDisplay.ArrangementMethod.tree:
                if (node.parent != null && parent != null)
                {
                    num_siblings = node.parent.children.Count;
                    target_positon = new Vector3(position * display.spacing.x - (num_siblings / 2 * display.spacing.x), Mathf.Abs(transform.localPosition.x) * 0.1f * display.spacing.y + depth, 0);
                }
                else
                {
                    target_positon = new Vector3(0, 0, 0);
                }
                break;

            case AncestorDisplay.ArrangementMethod.parabolic:
                if (node.parent != null)
                {
                    num_siblings = node.parent.children.Count;
                    Vector3 p3d = SampleParabola(-(transform.right * num_siblings * display.spacing.x), transform.right * num_siblings * display.spacing.x, display.spacing.y, (position + 1) / num_siblings, transform.up);
                    target_positon = p3d;

                    // Set the rotation
                }
                else
                {
                    target_positon = new Vector3(0, 0, 0);
                }
                break;
        }
    }

    public void Update()
    {
        // Move towards the desired positon
        float distance = Vector3.Distance(transform.localPosition, target_positon);
        if (distance > 0.001f)
        {
            float modifier = 1 + Mathf.Log(1 + distance);
            transform.localPosition = Vector3.Lerp(transform.localPosition, target_positon, 0.01f * modifier);
        }

        // Check if parent and artist is null
        if (parent != null)
        {
            // Draw line to parent
            artist.GetComponent<LineDrawer>().Draw(parent.transform.position, transform.position, "tree");
        }
        else
        {
            // If the parent is null, we should not draw any lines!
            artist.GetComponent<LineDrawer>().Clear();
        }

        CalculateDesiredPosition();
    }

    Vector3 SampleParabola(Vector3 start, Vector3 end, float height, float t, Vector3 out_direction)
    {
        float parabolicT = t * 2 - 1;
        //start and end are not level, gets more complicated
        Vector3 travel_direction = end - start;
        Vector3 level_direction = end - new Vector3(start.x, end.y, start.z);
        Vector3 right = Vector3.Cross(travel_direction, level_direction);
        Vector3 up = out_direction;
        Vector3 result = start + t * travel_direction;
        result += ((-parabolicT * parabolicT + 1) * height) * up.normalized;
        return result;
    }

    private void OnDrawGizmos()
    {

    }
}
