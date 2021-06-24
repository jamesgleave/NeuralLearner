using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neuron : Interactable
{

    // All but the final layer will have child neurons
    public List<Neuron> children = new List<Neuron>();

    // The neurons neighbours
    public Dictionary<string, GameObject> neighbours = new Dictionary<string, GameObject>();

    // We want to store the weight between this node and its children.
    // The weight at weights[i] corresponds to the weight between this neuron and child[i].
    public List<float> weights;

    // The value and bias of the neuron
    public float value;
    public float bias;
    public float layer;
    public float position;
    public string activation;

    // This value controls how fast it scales
    public float scale_rate = 1.5f;
    public float max_size = 1;
    public float line_scaler = 1;
    public float min_line_width = 1;

    // We need a line renderer to create show the weights and neurons
    public List<LineDrawer> artists;
    public LineDrawer artist;

    // Store our desired position
    public Vector3 desired_position;

    // Store whether or not the neuron is moving
    public bool moving;
    public bool grabbed;

    // The internal clock (for lerping)
    public float internal_clock = 0;

    // The pen for the editor
    public LineDrawer editor_pen;
    public float editor_value = 1f;


    // Start is called before the first frame update
    void Start()
    {
        // Set up the neuron
        Setup((int)ID.Neuron);
    }

    public void SetupInsertedNode()
    {
        Start();
    }

    public void UpdateMovement(float speed, float acc = 0.3f)
    {
        // Find the distance and direction
        float dist = Vector3.Distance(desired_position, transform.localPosition);
        Vector3 dir = desired_position - transform.localPosition;

        // Calculate the desired force
        Vector3 force = dir.normalized * acc * dist;

        // Add force to neuron rb
        rb.AddForce(force, ForceMode2D.Impulse);

        // Clamp the velocity
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, speed);

        // If we are pretty much not moving then stop completely
        if (Mathf.Approximately(rb.velocity.magnitude, 0.1f))
        {
            rb.velocity = Vector2.zero;
        }
    }

    public void UpdateSize()
    {
        // Set new scale
        float new_size = Mathf.Min(scale_rate * Mathf.Exp(Mathf.Abs(value)) + max_size / 10, max_size);
        float current = transform.localScale.x;
        float step = new_size * Time.deltaTime * (new_size - current);
        if (!Mathf.Approximately(current, new_size))
        {
            transform.localScale = new Vector3(current + step, current + step, current + step);
        }
    }

    public void DrawLine(bool forced)
    {
        // If this is moving
        moving = rb.velocity.magnitude > 0.1f;

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
            if (moving || children[i].moving || forced)
            {
                // Set the thickness of the line
                if (weights.Count > 0)
                {
                    // Get the scale of the weight
                    float scale = Mathf.Abs(weights[i]) * line_scaler;

                    //Clamp the scale 
                    if (Mathf.Approximately(scale, 0))
                    {
                        scale = 0;
                    }
                    else
                    {
                        scale = Mathf.Clamp(scale, min_line_width, line_scaler);
                    }

                    // Set the features of the artist
                    artists[i].SetFeatures(scale, scale);

                    // Set the colour
                    artists[i].SetColour(weights[i]);
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

        // Like the artists lines, we adjust the colour to show the magnitude of the values
        // Scale v between 0 and 1
        v = (float)System.Math.Tanh(v) + 1;

        // Set the colour
        sprite.material.color = Color32.Lerp(artist.clow, artist.chigh, v);
    }

    public void SetBias(float b)
    {
        bias = b;
    }

    public void SetLayer(float l)
    {
        layer = l;
    }

    public void SetActivation(string a)
    {
        activation = a;
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
    }

    public void Deselect()
    {
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
        x = -layer_index * horizontal_offset;
        y = (neuron_index - layer_size / 2) * vertical_offset + even_odd_offset;
        z = 0;

        // set the desired position
        desired_position.Set(x, y, z);
    }
}
