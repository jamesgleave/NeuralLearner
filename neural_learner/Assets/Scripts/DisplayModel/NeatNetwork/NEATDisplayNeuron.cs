using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NEATDisplayNeuron : Interactable
{

    // All but the final layer will have child neurons
    public List<NEATDisplayNeuron> children = new List<NEATDisplayNeuron>();
    public List<NEATDisplayNeuron> parents = new List<NEATDisplayNeuron>();

    public NEATNetwork neat_network;

    public DisplayEvoNEAT display;

    // We want to store the weight between this node and its children.
    // The weight at weights[i] corresponds to the weight between this neuron and child[i].
    public List<float> weights;

    // The value and bias of the neuron
    public float value;
    public float bias;
    public int layer; // Layer and depth are the same here
    public int position;
    public int depth_count;
    public string activation;

    // This value controls how fast it scales
    public float scale_rate;
    public float max_size;
    public float line_scaler;
    public float min_line_width;

    /// <summary>
    /// The NEAT Neuron object associated with this display neuron 
    /// </summary>
    public NEATNeuron neuron;

    // We need a line renderer to create show the weights and neurons
    public List<LineDrawer> artists;
    public LineDrawer artist;

    // Store our desired position
    public Vector3 desired_position;

    // Store whether or not the neuron is moving
    public bool moving = true;
    public bool grabbed;

    // The internal clock (for lerping)
    public float internal_clock = 0;

    // The pen for the editor
    public LineDrawer editor_pen;
    public float editor_value = 1f;

    /// <summary>
    /// The name of the input/output neuron
    /// </summary>
    public string io_name;

    /// <summary>
    /// True if the weights have been faded out
    /// </summary>
    public bool weights_faded = false;

    // Start is called before the first frame update
    void Start()
    {
        // Set up the neuron
        Setup((int)ID.Neuron);
    }

    public void Setup(NEATNeuron n, DisplayEvoNEAT display)
    {
        // Set the neuron to the instance n
        neuron = n;

        // Set the display
        this.display = display;

        // Setup all values
        value = n.GetOutput();
        layer = n.GetDepth();
        activation = n.GetActivation().name;

        // Setup from values in display
        scale_rate = display.theta;
        max_size = display.beta;
        line_scaler = display.sigma;
        min_line_width = display.sigma_prime;

        // Set up the neuron as an interactable
        Setup((int)ID.Neuron);
        OnSetup();
    }

    /// <summary>
    /// Just a function to be overwritten. Occurs at the end of setup.
    /// </summary>
    public virtual void OnSetup()
    {

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

                // Draw blue lines for recurrent connections
                if (children[i].neuron.GetInputs()[neuron.GetNeuronID()].GetInputType() == NEATInputContainer.NEATInputType.Recurrent)
                {
                    artists[i].clow = Color.magenta;
                    artists[i].chigh = Color.cyan;
                    artists[i].SetColour(weights[i]);
                    artists[i].DrawRecurrentConnection(transform.position, children[i].transform.position, 1.5f);
                }
                else
                {
                    artists[i].Draw(transform.position, children[i].transform.position);
                }

            }
        }
    }

    /// <summary>
    /// Reads the neuron and updates the values associated with the neuron as well as other internal values
    /// </summary>
    public virtual void UpdateInternalState()
    {
        // Update all values
        value = neuron.GetOutput();
        layer = neuron.GetDepth();
        activation = neuron.GetActivation().name;

        // Update from values in display
        scale_rate = display.theta;
        max_size = display.beta;
        line_scaler = display.sigma;
        min_line_width = display.sigma_prime;
        depth_count = display.depth_counter[layer];
    }

    /// <summary>
    /// Add a child to the list of children
    /// </summary>
    /// <param name="c"></param>
    public void AddChild(NEATDisplayNeuron c)
    {
        // Add this neuron as a parent to the child
        c.parents.Add(this);
        // Add the child
        children.Add(c);
        // Create a new line drawer for that child
        artists.Add(Instantiate(artist, transform));
        // Add a weight to the list
        weights.Add(neuron.GetSpecifiedWeight(c.neuron.GetNeuronID()));
    }

    /// <summary>
    /// Select a neuron and change the colour
    /// </summary>
    public void Select()
    {
        GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);
    }

    /// <summary>
    /// Deselect a neuron and change the colour
    /// </summary>
    public void Deselect()
    {
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
    }

    public void FadeOutWeights()
    {
        foreach (var a in artists)
        {
            a.Fade(false);
        }
    }

    public void FadeInWeights()
    {
        foreach (var a in artists)
        {
            a.Fade(true);
        }
    }

    /// <summary>
    /// Set the name of the neuron
    /// </summary>
    /// <param name="n"></param>
    public void SetIOName(string n)
    {
        io_name = n;
    }

    public List<LineDrawer> GetAllConnectedArtists(int calls = 0)
    {
        // Create new list for all connected neurons
        List<LineDrawer> connected = new List<LineDrawer>();

        // Get the index of the line drawer that draws each connection from the parent to this neuron
        foreach (var p in parents)
        {
            if (calls > display.GetAllNeurons().Count)
            {
                break;
            }

            int index = p.children.IndexOf(this);
            if (connected.Contains(p.artists[index]) == false)
            {
                connected.Add(p.artists[index]);
                connected.AddRange(p.GetAllConnectedArtists(calls++));
            }

        }

        return connected;
    }
}
