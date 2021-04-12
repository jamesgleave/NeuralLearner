using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neuron : MonoBehaviour
{

    // All but the final layer will have child neurons
    public List<Neuron> children = new List<Neuron>();

    // The neurons neighbours
    public Dictionary<string, GameObject> neighbours = new Dictionary<string, GameObject>();

    public Rigidbody rb;

    // We want to store the weight between this node and its children.
    // The weight at weights[i] corresponds to the weight between this neuron and child[i].
    public List<float> weights;

    // The value and bias of the neuron
    public float value;
    public float bias;
    public float layer;
    public float position;

    // This value controls how fast it scales
    public float scale_rate = 1.5f;
    public float max_size = 1;
    public float line_scaler = 1;

    // We need a line renderer to create show the weights and neurons
    public List<LineDrawer> artists;
    public LineDrawer artist;

    // Store our desired position
    public Vector3 desired_position;

    // Store whether or not the neuron is moving
    public bool moving;

    // The materials
    public Material neuron_mat;
    public Material selected_mat;

    // Start is called before the first frame update
    void Start()
    {
        // Get the rigidbody and create the list of artists to draw the edges
        rb = GetComponent<Rigidbody>();
    }

    public void SetupInsertedNode()
    {
        Start();
    }

    public void UpdateMovement(float speed)
    {
        // We clamp the distance so we arnt wiggling
        float dist = Vector3.Distance(desired_position, transform.localPosition);
        if (dist > 0.001f)
        {
            Vector3 relative = desired_position - transform.localPosition;
            Vector3 new_vel = relative.normalized * speed * (float)System.Math.Tanh(dist);
            rb.velocity = Vector3.Lerp(rb.velocity, new_vel, 0.05f);
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    public void UpdateSize()
    {
        // Set new scale
        float new_size = 1 / (1 + Mathf.Pow(scale_rate, -value)) * max_size / 2 + max_size * 0.5f;
        transform.localScale = new Vector3(new_size, new_size, new_size);
    }

    public void DrawLine()
    {
        // If this is moving
        moving = rb.velocity.magnitude > 0;

        // First, check if we are missing any artists or if we have too many
        // Remove/Add values until they match
        while (artists.Count != children.Count)
        {
            if (artists.Count < children.Count)
            {
                artists.Add(Instantiate(artist, parent: transform));
                artists[artists.Count - 1].Setup();
            }
            else if (artists.Count > children.Count)
            {
                Destroy(artists[artists.Count - 1].gameObject);
                artists.RemoveAt(artists.Count - 1);
            }
        }

        // Draw a line for each child
        for (int i = 0; i < children.Count; i++)
        {
            // Draw only when moving or when child is moving
            if (moving || children[i].moving)
            {
                // Set the thickness of the line
                if (weights.Count > 0)
                {
                    artists[i].SetFeatures(Mathf.Abs(weights[i]) * line_scaler, Mathf.Abs(weights[i]) * line_scaler);
                }
                artists[i].Draw(transform.position, children[i].transform.position);
            }
        }
    }

    public void UpdateValue(float new_value)
    {
        // Update the value at this neuron 
        value = new_value;
    }

    public void SetWeights(List<float> w)
    {
        // Sets the weights to the passed w list
        weights = w;
    }

    public void SetValue(float v)
    {
        // Set the value of the neuron
        value = v;
    }

    public void SetBias(float b)
    {
        bias = b;
    }

    public void SetLayer(float l)
    {
        layer = l;
    }

    public void SetPosition(float pos)
    {
        position = pos;
    }

    public void AddChild(Neuron child)
    {
        // Add Child to the list
        children.Add(child);
    }

    public Dictionary<string, float> GetInfo()
    {
        Dictionary<string, float> info = new Dictionary<string, float>();
        info["layer"] = layer;
        info["bias"] = bias;
        info["value"] = value;
        info["position"] = position;
        return info;
    }

    public void Select()
    {
        GetComponent<MeshRenderer>().material = selected_mat;
    }

    public void Deselect()
    {
        GetComponent<MeshRenderer>().material = neuron_mat;
    }

    public void CalculateDesiredPosition(int layer_index, int neuron_index, int layer_size, float vertical_offset, float horizontal_offset)
    {
        // Each neuron has a repulsive force.
        // The force will keep each neuron in a certain position.
        // Every neuron is only effected by their neighbours

        // Check if the layer size is even or odd
        float even_odd_offset = 0;
        if (layer_size % 2 == 0)
        {
            // If the layer is even then add an offset
            even_odd_offset = vertical_offset * 0.5f;
        }

        // Set up the basic coords
        float x, y, z;
        x = 0;
        y = (neuron_index - layer_size / 2) * vertical_offset + even_odd_offset;
        z = layer_index * horizontal_offset;

        // Loop through each key and find the position that is in the middle of each of the neighbours positions plus an offset
        foreach (string key in neighbours.Keys)
        {
            if (neighbours[key] != null)
            {
                GameObject neighbour = neighbours[key];
                Vector3 np = neighbour.transform.localPosition;

                // If the neighbour is an upper neighbour then set the desired position the be below it
                if (key == "upper")
                {

                }
                // If the neighbour is a lower neighbour then set the desired position the be above it
                else if (key == "lower")
                {

                }
            }
        }

        // set the desired position
        desired_position.Set(x, y, z);
    }
}
