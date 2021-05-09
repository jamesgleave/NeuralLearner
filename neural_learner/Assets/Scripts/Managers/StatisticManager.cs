using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatisticManager
{

    [Header("Energy")]
    public float total_energy;
    [Space]
    public float percent_energy_in_agent_bi_product;
    public float percent_energy_in_agents;
    public float percent_energy_in_meat;
    public float percent_energy_in_eggs;
    public float percent_energy_in_food_pellets;
    public float percent_energy_in_ether;
    public float sum_sanity_check;

    [Header("Genetics")]
    public int num_genus;
    public int num_species;
    public int num_agents;
    [Space]
    public float average_genetic_drift;
    public float
         average_base_mutation_rate,
         average_colour_mutation_prob,
         average_attribute_mutation_rate,
         average_neuro_mutation_prob,
         average_weight_mutation_prob,
         average_bias_mutation_prob,
         average_dropout_prob,
         average_speed,
         average_diet,
         average_attack,
         average_defense,
         average_vitality,
         average_size,
         average_perception,
         average_clockrate,
         average_gestation_time,
         average_maturity_time,
         average_colour_r,
         average_colour_g,
         average_colour_b;

    public Dictionary<string, List<Genes>> history = new Dictionary<string, List<Genes>>();
    public Dictionary<string, int> species_count;
    public List<string> species_names;

    public void Setup(float total)
    {
        total_energy = total;
    }

    public void SetAgentEnergy(float total, float a, float m, float e)
    {
        percent_energy_in_agents = a;
        percent_energy_in_meat = m;
        percent_energy_in_eggs = e;
        percent_energy_in_agent_bi_product = total;
    }

    public void SetOtherEnergy(float pellet, float ether)
    {
        percent_energy_in_food_pellets = pellet;
        percent_energy_in_ether = ether;
    }

    public void CalculateAverageGenes(List<BaseAgent> agents)
    {
        foreach (BaseAgent agent in agents)
        {
            average_base_mutation_rate += agent.genes.base_mutation_rate;
            average_colour_mutation_prob += agent.genes.colour_mutation_prob;
            average_attribute_mutation_rate += agent.genes.attribute_mutation_rate;
            average_neuro_mutation_prob += agent.genes.neuro_mutation_prob;
            average_weight_mutation_prob += agent.genes.weight_mutation_prob;
            average_bias_mutation_prob += agent.genes.bias_mutation_prob;
            average_dropout_prob += agent.genes.dropout_prob;
            average_speed += agent.genes.speed;
            average_diet += agent.genes.diet;
            average_attack += agent.genes.attack;
            average_defense += agent.genes.defense;
            average_vitality += agent.genes.vitality;
            average_size += agent.genes.size;
            average_perception += agent.genes.perception;
            average_clockrate += agent.genes.clockrate;
            average_gestation_time += agent.genes.gestation_time;
            average_maturity_time += agent.genes.maturity_time;
            average_colour_r += agent.genes.colour_r;
            average_colour_g += agent.genes.colour_g;
            average_colour_b += agent.genes.colour_b;
        }

        average_base_mutation_rate /= agents.Count;
        average_colour_mutation_prob /= agents.Count;
        average_attribute_mutation_rate /= agents.Count;
        average_neuro_mutation_prob /= agents.Count;
        average_weight_mutation_prob /= agents.Count;
        average_bias_mutation_prob /= agents.Count;
        average_dropout_prob /= agents.Count;
        average_speed /= agents.Count;
        average_diet /= agents.Count;
        average_attack /= agents.Count;
        average_defense /= agents.Count;
        average_vitality /= agents.Count;
        average_size /= agents.Count;
        average_perception /= agents.Count;
        average_clockrate /= agents.Count;
        average_gestation_time /= agents.Count;
        average_maturity_time /= agents.Count;
        average_colour_r /= agents.Count;
        average_colour_g /= agents.Count;
        average_colour_b /= agents.Count;

        num_agents = agents.Count;
    }

    public void Update()
    {
        sum_sanity_check = percent_energy_in_agent_bi_product + percent_energy_in_food_pellets + percent_energy_in_ether;
    }
}
