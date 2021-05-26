using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
        float colour_b
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
         colour_b
        );

        g.species = species;
        g.genus = genus;

        // Copy over the sprite map values
        g.spritemap["body"] = spritemap["body"];
        g.spritemap["head"] = spritemap["head"];

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

        // Return the new genes
        return new_genes;
    }

    /// <summary>
    ///   <para>Returns a random genes object (for testing)</para>
    /// </summary>
    public static Genes RandomGenes()
    {
        // TODO Make the maturity time and gestation time inversely proportional
        return new Genes(
         Random.value * Random.value,
         Random.value * Random.value,
         Random.value * Random.value,
         Random.value * Random.value,
         Random.value * Random.value,
         Random.value * Random.value,
         Random.value * Random.value,
         Mathf.Clamp(Random.value, 0.1f, 1f),
         Mathf.Clamp(Random.value, 0.1f, 1f),
         Mathf.Clamp(Random.value, 0.1f, 1f),
         Mathf.Clamp(Random.value, 0.1f, 1f),
         Mathf.Clamp(Random.value, 0.1f, 1f),
         Mathf.Clamp(Random.value, 0.1f, 1f),
         Mathf.Clamp(Random.value, 0.1f, 1f),
         Mathf.Clamp(Random.value, 0.1f, 1f),
         Mathf.Clamp(Random.value, 0.1f, 1f),
         Mathf.Clamp(Random.value, 0.1f, 1f),
         Mathf.Clamp(Random.value, 0.1f, 1f),
         Mathf.Clamp(Random.value, 0.1f, 1f),
         Mathf.Clamp(Random.value, 0.1f, 1f)
        );
    }

    public void Mutate()
    {

        TryMutateColour();
        TryMutateAttributes();
        TryMutateMutationRates();

    }

    private void TryMutateColour()
    {
        if (GetProb(colour_mutation_prob))
        {
            colour_r += Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
            colour_r = Mathf.Max(colour_r, 0);
        }

        if (GetProb(colour_mutation_prob))
        {
            colour_g += Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
            colour_g = Mathf.Max(colour_g, 0);
        }

        if (GetProb(colour_mutation_prob))
        {
            colour_b += Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
            colour_b = Mathf.Max(colour_b, 0);
        }
    }

    private void TryMutateMutationRates()
    {
        if (GetProb(attribute_mutation_rate))
        {
            base_mutation_rate += Random.value * Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
        }

        if (GetProb(attribute_mutation_rate))
        {
            colour_mutation_prob += Random.value * Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
        }

        if (GetProb(attribute_mutation_rate))
        {
            attribute_mutation_rate += Random.value * Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
        }

        if (GetProb(attribute_mutation_rate))
        {
            neuro_mutation_prob += Random.value * Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
        }

        if (GetProb(attribute_mutation_rate))
        {
            weight_mutation_prob += Random.value * Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
        }

        if (GetProb(attribute_mutation_rate))
        {
            bias_mutation_prob += Random.value * Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
        }

        if (GetProb(attribute_mutation_rate))
        {
            dropout_prob += Random.value * Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
        }
    }

    private void TryMutateAttributes()
    {
        speed += Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
        speed = Mathf.Max(speed, 0);

        diet += Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
        diet = Mathf.Clamp(diet, 0, 1);

        attack += Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
        attack = Mathf.Max(attack, 0);

        defense += Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
        defense = Mathf.Max(defense, 0);

        vitality += Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
        vitality = Mathf.Max(vitality, 0);

        size += Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
        size = Mathf.Max(size, 0.1f);

        perception += Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
        perception = Mathf.Max(perception, 0);

        clockrate += Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
        clockrate = Mathf.Max(clockrate, 0);

        gestation_time += Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
        gestation_time = Mathf.Max(gestation_time, 0.1f);

        maturity_time += Random.Range(-attribute_mutation_rate, attribute_mutation_rate);
        maturity_time = Mathf.Max(maturity_time, 0.1f);

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
        List<float> gl = new List<float>();
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
        g1.colour_mutation_prob /= denom;
        g1.attribute_mutation_rate /= denom;
        g1.neuro_mutation_prob /= denom;
        g1.weight_mutation_prob /= denom;
        g1.bias_mutation_prob /= denom;
        g1.dropout_prob /= denom;
        g1.speed /= denom;
        g1.diet /= denom;
        g1.attack /= denom;
        g1.defense /= denom;
        g1.vitality /= denom;
        g1.size /= denom;
        g1.perception /= denom;
        g1.clockrate /= denom;
        g1.gestation_time /= denom;
        g1.maturity_time /= denom;
        g1.colour_r /= denom;
        g1.colour_g /= denom;
        g1.colour_b /= denom;
        g1.genetic_drift /= denom;

        // Return the clone
        return g1;
    }
}
