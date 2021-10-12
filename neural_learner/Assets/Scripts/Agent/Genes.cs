using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


[System.Serializable]
public class Genes
{
    [Header("Mutation Rates")]
    /// <summary>
    ///   <para>The amount by (+ or -) a mutation rate can mutate by upon birth </para>
    /// </summary>
    [Range(0.0f, 1.0f)]
    public float base_mutation_rate;

    /// <summary>
    ///   <para>The amount by (+ or -) an attribute can mutate by upon birth </para>
    /// </summary>
    [Range(0.0f, 1.0f)]
    public float attribute_mutation_rate;

    [Header("Probablities")]
    /// <summary>
    ///   <para>The probability that the colour will mutate</para>
    /// </summary>
    [Range(0.0f, 1.0f)]
    public float colour_mutation_prob;

    /// <summary>
    ///   <para>The probablity that an attribute of the brain will add or drop a neuron</para>
    /// </summary>
    [Range(0.0f, 1.0f)]
    public float neuro_mutation_prob;

    /// <summary>
    ///   <para>The probability that a weight will be disabled</para>
    /// </summary>
    [Range(0.0f, 1.0f)]
    public float dropout_prob;

    /// <summary>
    ///   <para>The probablity that a weight in the brain will mutate</para>
    /// </summary>
    [Range(0.0f, 1.0f)]
    public float weight_mutation_prob;

    /// <summary>
    ///   <para>The probablity that a bias in the brain will mutate</para>
    /// </summary>
    [Range(0.0f, 1.0f)]
    public float bias_mutation_prob;

    [Header("Attributes")]
    /// Attributes \\\
    /// <summary>
    ///   <para>The agents speed at which it can move and rotate</para>
    /// </summary>
    public float speed;

    /// <summary>
    ///   <para>The primary diet of the agent. 0 means herbivorous, 1 means carnivorous, and 0.5 means omnivorous</para>
    /// </summary>
    public float diet;

    /// <summary>
    ///   <para>The attack damage of the agent</para>
    /// </summary>
    public float attack;

    /// <summary>
    ///   <para>The defense of the agent</para>
    /// </summary>
    public float defense;

    /// <summary>
    ///   <para>The vitality of the agent, responsible for its starting hp</para>
    /// </summary>
    public float vitality;

    /// <summary>
    ///   <para>The size of the agent</para>
    /// </summary>
    public float size;

    /// <summary>
    ///   <para>The size of the overlap sphere generated for perception</para>
    /// </summary>
    public float perception;

    /// <summary>
    ///   <para>The update frequency of the agent. This is how fast it can process new input</para>
    /// </summary>
    public float clockrate;

    /// <summary>
    ///   <para>The agent's time spent in its egg</para>
    /// </summary>
    public float gestation_time;

    /// <summary>
    ///   <para>The agent's time to reach maturity</para>
    /// </summary>
    public float maturity_time;

    /// <summary>
    ///   <para>The agent's colour</para>
    /// </summary>
    public float colour_r, colour_g, colour_b;

    // The name of the creature
    public string genus, species;

    /// <summary>
    ///   <para>The agents genetic drift from the species it originates from.</para>
    /// </summary>
    public float genetic_drift;

    /// <summary>
    ///   <para>The code for the genes</para>
    /// </summary>
    public Vector3 code;

    /// <summary>
    ///   <para>The body hashmap which stores the indices for the agent's body, head, etc</para>
    /// </summary>
    public Dictionary<string, int> spritemap = new Dictionary<string, int>();

    /// <summary>
    /// The list of genes (gl standing for gene list)
    /// </summary>
    private List<float> gl = new List<float>();

    [Header("Behavioural:")]
    /// <summary>
    /// The cohesion factor for boids
    /// </summary>
    public float cohesion_factor;

    /// <summary>
    /// The separation factor for boids
    /// </summary>
    public float separation_factor;

    /// <summary>
    /// The allignment factor for boids
    /// </summary>
    public float allignment_factor;

    /// <summary>
    /// The velocity matching factor for boids
    /// </summary>
    public float matching_factor;

    /// <summary>
    ///   <para>The agent's genes</para>
    /// </summary>
    public Genes(
        float base_mutation_rate,
        float colour_mutation_prob,
        float attribute_mutation_rate,
        float neuro_mutation_prob,
        float weight_mutation_prob,
        float bias_mutation_prob,
        float dropout_prob,
        float speed,
        float diet,
        float attack,
        float defense,
        float vitality,
        float size,
        float perception,
        float clockrate,
        float gestation_time,
        float maturity_time,
        float colour_r,
        float colour_g,
        float colour_b,
        float cohesion_factor,
        float separation_factor,
        float allignment_factor,
        float matching_factor
        )
    {
        this.base_mutation_rate = base_mutation_rate;
        this.colour_mutation_prob = colour_mutation_prob;
        this.attribute_mutation_rate = attribute_mutation_rate;
        this.neuro_mutation_prob = neuro_mutation_prob;
        this.weight_mutation_prob = weight_mutation_prob;
        this.bias_mutation_prob = bias_mutation_prob;
        this.dropout_prob = dropout_prob;
        this.speed = speed;
        this.diet = diet;
        this.attack = attack;
        this.defense = defense;
        this.vitality = vitality;
        this.size = size;
        this.perception = perception;
        this.clockrate = clockrate;
        this.gestation_time = gestation_time;
        this.maturity_time = maturity_time;
        this.colour_r = colour_r;
        this.colour_g = colour_g;
        this.colour_b = colour_b;
        this.cohesion_factor = cohesion_factor;
        this.separation_factor = separation_factor;
        this.allignment_factor = allignment_factor;
        this.matching_factor = matching_factor;
        spritemap = new Dictionary<string, int>();
        ClampMutationRates();
    }

    /// <summary>
    ///   <para>Returns a clone of a Genes object</para>
    /// </summary>
    public Genes Clone()
    {
        Genes g = new Genes(
         base_mutation_rate,
         colour_mutation_prob,
         attribute_mutation_rate,
         neuro_mutation_prob,
         weight_mutation_prob,
         bias_mutation_prob,
         dropout_prob,
         speed,
         diet,
         attack,
         defense,
         vitality,
         size,
         perception,
         clockrate,
         gestation_time,
         maturity_time,
         colour_r,
         colour_g,
         colour_b,
         cohesion_factor,
         separation_factor,
         allignment_factor,
         matching_factor
        );

        g.species = species;
        g.genus = genus;

        // Copy over the sprite map values
        g.spritemap["body"] = spritemap["body"];
        g.spritemap["head"] = spritemap["head"];

        // Clamp the rates
        ClampMutationRates();

        return g;
    }

    /// <summary>
    ///   <para>Returns an average between two genes (sexual reproduction kinda)</para>
    /// </summary>
    public Genes Cross(Genes other)
    {
        // Take the average between the two genes
        Genes new_genes = (other.Clone() + this) / 2;

        // Take one of the body parts from each creature
        new_genes.spritemap["body"] = other.spritemap["body"];
        new_genes.spritemap["head"] = spritemap["head"];

        // Clamp rates
        ClampMutationRates();

        // Return the new genes
        return new_genes;
    }

    /// <summary>
    ///   <para>Returns a random genes object (for testing)</para>
    /// </summary>
    public static Genes GetBaseGenes()
    {
        return new Genes(
         base_mutation_rate: 0.5f + Random.value / 10,
         colour_mutation_prob: 0.1f + Random.value / 10,
         attribute_mutation_rate: 0.1f + Random.value / 10,
         neuro_mutation_prob: 0.1f + Random.value / 100,
         weight_mutation_prob: 0.5f + Random.value / 10,
         bias_mutation_prob: 0.5f + Random.value / 10,
         dropout_prob: 0.15f + Random.value / 10,
         speed: 0.5f + Random.value / 10,
         diet: 0.4f + Random.value / 10,
         attack: 0.5f + Random.value / 10,
         defense: 0.5f + Random.value / 10,
         vitality: 0.5f + Random.value / 10,
         size: 0.4f + Random.value / 10,
         perception: 0.5f + Random.value / 10,
         clockrate: 0.5f + Random.value / 10,
         gestation_time: 0.5f + Random.value / 10,
         maturity_time: 0.5f + Random.value / 10,
         colour_r: Mathf.Clamp(Random.value, 0.0f, 1f),
         colour_g: Mathf.Clamp(Random.value, 0.0f, 1f),
         colour_b: Mathf.Clamp(Random.value, 0.0f, 1f),
         cohesion_factor: 0,
         separation_factor: 0,
         allignment_factor: 0,
         matching_factor: 0
        );
    }

    public void Mutate()
    {
        TryMutateColour();
        TryMutateAttributes();
        TryMutateMutationRates();
        TryMutateBehaviour();
        ClampMutationRates();
    }

    private void TryMutateBehaviour()
    {
        if (GetProb(base_mutation_rate))
        {
            allignment_factor += Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
            allignment_factor = Mathf.Clamp(allignment_factor, 0, 1);
        }

        if (GetProb(base_mutation_rate))
        {
            separation_factor += Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
            separation_factor = Mathf.Clamp(separation_factor, 0, 1);
        }

        if (GetProb(base_mutation_rate))
        {
            cohesion_factor += Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
            cohesion_factor = Mathf.Clamp(cohesion_factor, 0, 1);
        }

        if (GetProb(base_mutation_rate))
        {
            matching_factor += Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
            matching_factor = Mathf.Clamp(matching_factor, 0, 1);
        }
    }

    private void TryMutateColour()
    {
        if (GetProb(colour_mutation_prob))
        {
            colour_r += Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
            colour_r = Mathf.Clamp01(colour_r);
        }

        if (GetProb(colour_mutation_prob))
        {
            colour_g += Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
            colour_g = Mathf.Clamp01(colour_g);
        }

        if (GetProb(colour_mutation_prob))
        {
            colour_b += Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
            colour_b = Mathf.Clamp01(colour_b);
        }
    }

    private void TryMutateMutationRates()
    {

        if (GetProb(Mathf.Pow(base_mutation_rate, 2)))
        {
            base_mutation_rate += Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
        }

        if (GetProb(Mathf.Pow(base_mutation_rate, 2)))
        {
            colour_mutation_prob += Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
        }

        if (GetProb(Mathf.Pow(base_mutation_rate, 2)))
        {
            attribute_mutation_rate += Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
        }


        if (GetProb(Mathf.Pow(base_mutation_rate, 2)))
        {
            neuro_mutation_prob += Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
        }


        if (GetProb(Mathf.Pow(base_mutation_rate, 2)))
        {
            weight_mutation_prob += Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
        }


        if (GetProb(Mathf.Pow(base_mutation_rate, 2)))
        {
            bias_mutation_prob += Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
        }


        if (GetProb(Mathf.Pow(base_mutation_rate, 2)))
        {
            dropout_prob += Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
        }
    }

    private void ClampMutationRates()
    {
        // If we have reached 0 or 1 on a mutation rate, we "radicallize it"
        if (base_mutation_rate <= 0.01 || base_mutation_rate >= 1)
        {
            base_mutation_rate = Random.Range(0, 1f);
        }

        // If we have reached 0 or 1 on a mutation rate, we "radicallize it"
        if (colour_mutation_prob < 0.01f || colour_mutation_prob >= 1)
        {
            colour_mutation_prob = Random.Range(0, 1f);
        }

        // If we have reached 0 or 1 on a mutation rate, we "radicallize it"
        if (attribute_mutation_rate < 0.01f || attribute_mutation_rate >= 1)
        {
            attribute_mutation_rate = Random.Range(0, 1f);
        }

        // If we have reached 0 or 1 on a mutation rate, we "radicallize it"
        if (neuro_mutation_prob < 0.01f || neuro_mutation_prob >= 1)
        {
            neuro_mutation_prob = Random.Range(0, 1f);
        }

        // If we have reached 0 or 1 on a mutation rate, we "radicallize it"
        if (weight_mutation_prob < 0.01f || weight_mutation_prob >= 1)
        {
            weight_mutation_prob = Random.Range(0, 1f);
        }

        // If we have reached 0 or 1 on a mutation rate, we "radicallize it"
        if (bias_mutation_prob < 0.01f || bias_mutation_prob >= 1)
        {
            bias_mutation_prob = Random.Range(0, 1f);
        }

        // If we have reached 0 or 1 on a mutation rate, we "radicallize it"
        if (dropout_prob < 0.01f || dropout_prob >= 1)
        {
            dropout_prob = Random.Range(0, 1f);
        }

        base_mutation_rate = Mathf.Clamp(base_mutation_rate, 0.1f, 1f);
        colour_mutation_prob = Mathf.Clamp01(colour_mutation_prob);
        attribute_mutation_rate = Mathf.Clamp01(attribute_mutation_rate);
        neuro_mutation_prob = Mathf.Clamp01(neuro_mutation_prob);
        weight_mutation_prob = Mathf.Clamp01(weight_mutation_prob);
        bias_mutation_prob = Mathf.Clamp01(bias_mutation_prob);
        dropout_prob = Mathf.Clamp01(dropout_prob);
    }

    private void TryMutateAttributes()
    {

        if (GetProb(base_mutation_rate))
        {
            speed += Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
            speed = Mathf.Max(speed, 0.01f);
        }

        if (GetProb(base_mutation_rate))
        {
            diet += Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
            diet = Mathf.Clamp(diet, 0.01f, 1);
        }

        if (GetProb(base_mutation_rate))
        {
            attack += Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
            attack = Mathf.Max(attack, 0.01f);
        }

        if (GetProb(base_mutation_rate))
        {
            defense += Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
            defense = Mathf.Max(defense, 0.01f);
        }

        if (GetProb(base_mutation_rate))
        {

            vitality += Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
            vitality = Mathf.Max(vitality, 0.01f);
        }

        if (GetProb(base_mutation_rate))
        {
            size += Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
            size = Mathf.Max(size, 0.1f);
        }

        if (GetProb(base_mutation_rate))
        {
            perception += Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
            perception = Mathf.Max(perception, 0.01f);
        }

        if (GetProb(base_mutation_rate))
        {
            clockrate += Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
            clockrate = Mathf.Max(clockrate, 0.01f);
        }

        if (GetProb(base_mutation_rate))
        {
            gestation_time += Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
            gestation_time = Mathf.Max(gestation_time, 0.1f);
        }

        if (GetProb(base_mutation_rate))
        {
            maturity_time += Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
            maturity_time = Mathf.Max(maturity_time, 0.1f);
        }

        // There is a small change that a body component will mutate
        if (GetProb(base_mutation_rate * attribute_mutation_rate * colour_mutation_prob))
        {
            if (GetProb(1 / 3))
            {
                int head_components = GameObject.FindGameObjectWithTag("manager").GetComponent<SpriteManager>().head_components.Count;
                spritemap["head"] = Random.Range(0, head_components);
            }
            else
            {
                int body_components = GameObject.FindGameObjectWithTag("manager").GetComponent<SpriteManager>().body_components.Count;
                spritemap["body"] = Random.Range(0, body_components);
            }

        }
    }

    /// <summary>
    ///   <para>Returns true if value% of the time</para>
    /// </summary>
    private bool GetProb(float value)
    {
        return value > Random.value;
    }

    public List<float> GetGeneList()
    {
        // Clear the list
        gl.Clear();

        // Add..
        // Attributes
        gl.Add(base_mutation_rate);
        gl.Add(colour_mutation_prob);
        gl.Add(attribute_mutation_rate);
        gl.Add(neuro_mutation_prob);
        gl.Add(weight_mutation_prob);
        gl.Add(bias_mutation_prob);
        gl.Add(dropout_prob);
        gl.Add(speed);
        gl.Add(diet);
        gl.Add(attack);
        gl.Add(defense);
        gl.Add(vitality);
        gl.Add(size);
        gl.Add(perception);
        gl.Add(clockrate);
        gl.Add(gestation_time);
        gl.Add(maturity_time);
        gl.Add(colour_r);
        gl.Add(colour_g);
        gl.Add(colour_b);

        // Behavioural
        //gl.Add(cohesion_factor);
        //gl.Add(allignment_factor);
        //gl.Add(separation_factor);
        //gl.Add(matching_factor);
        return gl;
    }

    /// <summary>
    ///   <para>Returns true if the genes g belong to the species these genes belong to</para>
    /// </summary>
    public bool IsSameSpecies(Genes g, float thresh)
    {
        float dist = CalculateGeneticDrift(g);
        return thresh > dist;
    }

    /// <summary>
    ///   <para>Returns the distance between drifting genes</para>
    /// </summary>
    public float CalculateGeneticDrift(Genes g)
    {
        // Calculate the euclidian distance between these genes and the passed genes g
        float dist = 0;
        List<float> g1 = GetGeneList();
        List<float> g2 = g.GetGeneList();
        for (int i = 0; i < g1.Count; i++)
        {
            dist += Mathf.Pow(g1[i] - g2[i], 2);
        }

        // The colours should make a significant difference in speciation
        // We want it to make enough of a difference so we use the function: f(c) = abs(e^(2c)) - 1 and with all 3 colours we have g(r, g, b) = f(r) + f(g) + f(b)
        dist += Mathf.Exp(2 * Mathf.Abs(colour_r - g.colour_r)) + Mathf.Exp(2 * Mathf.Abs(colour_g - g.colour_g)) + Mathf.Exp(2 * Mathf.Abs(colour_b - g.colour_b)) - 3;

        // Add the body parts
        dist *= spritemap["head"] == g.spritemap["head"] ? 1 : 3.0f;
        dist *= spritemap["body"] == g.spritemap["body"] ? 1 : 2.0f;

        return dist;
    }

    /// <summary>
    ///   <para>Set the genetic code of the gene based on its name</para>
    /// </summary>
    public void SetGeneticCode()
    {
        code = GetGeneticCode();
    }

    /// <summary>
    ///   <para>Returns a unique 3 digit code that represents the genome</para>
    /// </summary>
    private Vector3 GetGeneticCode()
    {
        // Setup our values
        float x = 0, y = 0, z = 0;
        float temp = 0;
        char[] x_bar = (genus + " " + species).ToCharArray();

        // Create code sequence
        for (int i = 0; i < x_bar.Length - 1; i++)
        {
            // Find sum
            temp += (((int)x_bar[i] + (int)x_bar[i + 1]) * 1 / 13f);

            // Add to components 
            x += temp;
            y += temp * x;
            z += temp * y;

            // Mod it
            x %= 7;
            y %= 5;
            z %= 3;
        }

        // Return the code in vector form
        return new Vector3(x, y, z).normalized;
    }

    public void SaveGenes(string save_path)
    {
        string gene_data = "Genes:";
        // Add each gene value
        foreach (float val in GetGeneList())
        {
            gene_data += val.ToString() + ",";
        }

        // Add the species
        gene_data += genus + "," + species + ",";

        // Add spritemap
        gene_data += spritemap["body"] + "," + spritemap["head"];

        // Write to file
        File.WriteAllText(save_path, gene_data);
    }

    public static Genes LoadFrom(string save_path)
    {
        // Read the lines
        string[] lines = System.IO.File.ReadAllLines(save_path);

        float
         base_mutation_rate = 0,
         colour_mutation_prob = 0,
         attribute_mutation_rate = 0,
         neuro_mutation_prob = 0,
         weight_mutation_prob = 0,
         bias_mutation_prob = 0,
         dropout_prob = 0,
         speed = 0,
         diet = 0,
         attack = 0,
         defense = 0,
         vitality = 0,
         size = 0,
         perception = 0,
         clockrate = 0,
         gestation_time = 0,
         maturity_time = 0,
         colour_r = 0,
         colour_g = 0,
         colour_b = 0,
         cohesion = 0,
         allignment = 0,
         separation = 0,
         matching = 0;

        // The name
        string species = null;
        string genus = null;

        // Copy over the sprite map values
        int sm_body = 0, sm_head = 0;

        // Display the file contents by using a foreach loop.
        int i = 0;
        foreach (string unsplit in lines)
        {
            if (unsplit.Contains("Genes:"))
            {
                // Get each value and assign our values
                string line = unsplit.Split(':')[1];
                var values = line.Split(',');

                // Each of the gene values
                base_mutation_rate = float.Parse(values[i++]);
                colour_mutation_prob = float.Parse(values[i++]);
                attribute_mutation_rate = float.Parse(values[i++]);
                neuro_mutation_prob = float.Parse(values[i++]);
                weight_mutation_prob = float.Parse(values[i++]);
                bias_mutation_prob = float.Parse(values[i++]);
                dropout_prob = float.Parse(values[i++]);
                speed = float.Parse(values[i++]);
                diet = float.Parse(values[i++]);
                attack = float.Parse(values[i++]);
                defense = float.Parse(values[i++]);
                vitality = float.Parse(values[i++]);
                size = float.Parse(values[i++]);
                perception = float.Parse(values[i++]);
                clockrate = float.Parse(values[i++]);
                gestation_time = float.Parse(values[i++]);
                maturity_time = float.Parse(values[i++]);
                colour_r = float.Parse(values[i++]);
                colour_g = float.Parse(values[i++]);
                colour_b = float.Parse(values[i++]);
                //cohesion = float.Parse(values[i++]);
                //allignment = float.Parse(values[i++]);
                //separation = float.Parse(values[i++]);
                //matching = float.Parse(values[i++]);

                // The name
                genus = values[i++];
                species = values[i++];

                // Get the spritemap
                sm_body = int.Parse(values[i++]);
                sm_head = int.Parse(values[i++]);
            }
        }
        Genes loaded = new Genes(
         // Attributes
         base_mutation_rate,
         colour_mutation_prob,
         attribute_mutation_rate,
         neuro_mutation_prob,
         weight_mutation_prob,
         bias_mutation_prob,
         dropout_prob,
         speed,
         diet,
         attack,
         defense,
         vitality,
         size,
         perception,
         clockrate,
         gestation_time,
         maturity_time,
         colour_r,
         colour_g,
         colour_b,

         // Behavioural Genes (Boids, etc)
         cohesion_factor: cohesion, separation_factor: separation, allignment_factor: allignment, matching_factor: matching
         );


        loaded.species = species;
        loaded.genus = genus;

        // Copy over the sprite map values
        loaded.spritemap["body"] = sm_body;
        loaded.spritemap["head"] = sm_head;

        return loaded;
    }


    /// <summary>
    ///   <para>Override the + operator</para>
    /// </summary>
    public static Genes operator +(Genes g, Genes g2)
    {
        // Create a clone of g
        Genes g1 = g.Clone();

        // Add all attributes
        g1.base_mutation_rate += g2.base_mutation_rate;
        g1.colour_mutation_prob += g2.colour_mutation_prob;
        g1.attribute_mutation_rate += g2.attribute_mutation_rate;
        g1.neuro_mutation_prob += g2.neuro_mutation_prob;
        g1.weight_mutation_prob += g2.weight_mutation_prob;
        g1.bias_mutation_prob += g2.bias_mutation_prob;
        g1.dropout_prob += g2.dropout_prob;
        g1.speed += g2.speed;
        g1.diet += g2.diet;
        g1.attack += g2.attack;
        g1.defense += g2.defense;
        g1.vitality += g2.vitality;
        g1.size += g2.size;
        g1.perception += g2.perception;
        g1.clockrate += g2.clockrate;
        g1.gestation_time += g2.gestation_time;
        g1.maturity_time += g2.maturity_time;
        g1.colour_r += g2.colour_r;
        g1.colour_g += g2.colour_g;
        g1.colour_b += g2.colour_b;
        g1.genetic_drift += g2.genetic_drift;

        g1.cohesion_factor += g2.cohesion_factor;
        g1.allignment_factor += g2.allignment_factor;
        g1.separation_factor += g2.separation_factor;
        g1.matching_factor += g2.matching_factor;


        // Return the clone
        return g1;
    }

    /// <summary>
    ///   <para>Override the + operator</para>
    /// </summary>
    public static Genes operator -(Genes g, Genes g2)
    {
        // Create a clone of g
        Genes g1 = g.Clone();

        // Add all attributes
        g1.base_mutation_rate -= g2.base_mutation_rate;
        g1.colour_mutation_prob -= g2.colour_mutation_prob;
        g1.attribute_mutation_rate -= g2.attribute_mutation_rate;
        g1.neuro_mutation_prob -= g2.neuro_mutation_prob;
        g1.weight_mutation_prob -= g2.weight_mutation_prob;
        g1.bias_mutation_prob -= g2.bias_mutation_prob;
        g1.dropout_prob -= g2.dropout_prob;
        g1.speed -= g2.speed;
        g1.diet -= g2.diet;
        g1.attack -= g2.attack;
        g1.defense -= g2.defense;
        g1.vitality -= g2.vitality;
        g1.size -= g2.size;
        g1.perception -= g2.perception;
        g1.clockrate -= g2.clockrate;
        g1.gestation_time -= g2.gestation_time;
        g1.maturity_time -= g2.maturity_time;
        g1.colour_r -= g2.colour_r;
        g1.colour_g -= g2.colour_g;
        g1.colour_b -= g2.colour_b;
        g1.genetic_drift -= g2.genetic_drift;

        g1.cohesion_factor -= g2.cohesion_factor;
        g1.allignment_factor -= g2.allignment_factor;
        g1.separation_factor -= g2.separation_factor;
        g1.matching_factor -= g2.matching_factor;

        // Return the clone
        return g1;
    }

    /// <summary>
    ///   <para>Override the / operator</para>
    /// </summary>
    public static Genes operator /(Genes g, float denom)
    {
        // Create a clone of g
        Genes g1 = g.Clone();

        // Divide all attributes by the denom
        g1.base_mutation_rate /= denom;
        g1.base_mutation_rate = Mathf.Clamp01(g1.base_mutation_rate);

        g1.colour_mutation_prob /= denom;
        g1.colour_mutation_prob = Mathf.Clamp01(g1.colour_mutation_prob);

        g1.attribute_mutation_rate /= denom;
        g1.attribute_mutation_rate = Mathf.Clamp01(g1.attribute_mutation_rate);

        g1.neuro_mutation_prob /= denom;
        g1.neuro_mutation_prob = Mathf.Clamp01(g1.neuro_mutation_prob);

        g1.weight_mutation_prob /= denom;
        g1.weight_mutation_prob = Mathf.Clamp01(g1.weight_mutation_prob);

        g1.bias_mutation_prob /= denom;
        g1.bias_mutation_prob = Mathf.Clamp01(g1.bias_mutation_prob);

        g1.dropout_prob /= denom;
        g1.dropout_prob = Mathf.Clamp01(g1.dropout_prob);

        g1.speed /= denom;
        g1.speed = Mathf.Clamp01(g1.speed);

        g1.diet /= denom;
        g1.diet = Mathf.Clamp01(g1.diet);

        g1.attack /= denom;
        g1.attack = Mathf.Clamp01(g1.attack);

        g1.defense /= denom;
        g1.defense = Mathf.Clamp01(g1.defense);

        g1.vitality /= denom;
        g1.vitality = Mathf.Clamp01(g1.vitality);

        g1.size /= denom;
        g1.size = Mathf.Clamp01(g1.size);

        g1.perception /= denom;
        g1.perception = Mathf.Clamp01(g1.perception);

        g1.clockrate /= denom;
        g1.clockrate = Mathf.Clamp01(g1.clockrate);

        g1.gestation_time /= denom;
        g1.gestation_time = Mathf.Clamp01(g1.gestation_time);

        g1.maturity_time /= denom;
        g1.maturity_time = Mathf.Clamp01(g1.maturity_time);

        g1.colour_r /= denom;
        g1.colour_r = Mathf.Clamp01(g1.colour_r);

        g1.colour_g /= denom;
        g1.colour_g = Mathf.Clamp01(g1.colour_g);

        g1.colour_b /= denom;
        g1.colour_b = Mathf.Clamp01(g1.colour_b);

        g1.genetic_drift /= denom;
        g1.genetic_drift = Mathf.Clamp01(g1.genetic_drift);

        g1.cohesion_factor /= denom;
        g1.cohesion_factor = Mathf.Clamp(g1.cohesion_factor, 0, 1);

        g1.allignment_factor /= denom;
        g1.allignment_factor = Mathf.Clamp(g1.allignment_factor, 0, 1);

        g1.separation_factor /= denom;
        g1.separation_factor = Mathf.Clamp(g1.separation_factor, 0, 1);

        g1.matching_factor /= denom;
        g1.matching_factor = Mathf.Clamp(g1.matching_factor, 0, 1);


        // Return the clone
        return g1;
    }
}
