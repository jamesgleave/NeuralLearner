using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatisticManager
{
    /// <summary>
    /// The manager of the simulation
    /// </summary>
    public Manager manager;

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
         field_of_view,
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
    
    /// <summary>
    /// Percent energy in ether over time. Appended to every manager.delta_history_update_time seconds.
    /// </summary>
    public List<float> energy_in_ether_history = new List<float>();
     /// <summary>
    /// Percent energy in agents over time. Appended to every manager.delta_history_update_time seconds.
    /// </summary>
    public List<float> energy_in_agents_history = new List<float>();    
    /// <summary>
    /// Percent energy in pellets over time. Appended to every manager.delta_history_update_time seconds.
    /// </summary>
    public List<float> energy_in_pellets_history = new List<float>();
    /// <summary>
    /// Number of agents over time.
    /// </summary>
    public List<int> num_pellets_history = new List<int>();
        /// <summary>
    /// Number of pellets over time.
    /// </summary>
    public List<int> num_agents_history = new List<int>();

    /// <summary>
    /// Our list of histories. The order goes: energy in ether, agents, and pellets, then number of agents alive, number of active pellets, 
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, List<int>> all_histories = new  Dictionary<string, List<int>>();

    public void Setup(float total)
    {
        /// Set the total energy for a reference
        total_energy = total;

        // On setup, start the coroutine which updates the history
        manager.StartCoroutine(UpdateHistory());
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
            field_of_view += agent.genes.field_of_view;
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
        field_of_view /= agents.Count;
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

    /// <summary>
    /// Appends current state of simulation to the history
    /// </summary>
    public void AppendToHistory(){
        // Append to the histories
        energy_in_ether_history.Add(percent_energy_in_ether);
        energy_in_agents_history.Add(percent_energy_in_agents);
        energy_in_pellets_history.Add(percent_energy_in_food_pellets);

        num_pellets_history.Add(manager.num_active_food);
        num_agents_history.Add(num_agents);

        // Check if the the histories are too long (they are all the same length so just need to check the one)
        if(energy_in_ether_history.Count >= manager.history_length_threshold){
            // If its too long, we bin it to half its length
            for(int i = 0; i < energy_in_ether_history.Count - 1; i+=2){
                // Take the mean between the ith and i+1th element
                energy_in_ether_history[i] = Mathf.Clamp01((energy_in_ether_history[i] + energy_in_ether_history[i + 1]) / 2f);
                energy_in_agents_history[i] = Mathf.Clamp01((energy_in_agents_history[i] + energy_in_agents_history[i + 1]) / 2f);
                energy_in_pellets_history[i] = Mathf.Clamp01((energy_in_pellets_history[i] + energy_in_pellets_history[i + 1]) / 2f);
                num_pellets_history[i] = (num_pellets_history[i] + num_pellets_history[i + 1]) / 2;
                num_pellets_history[i] = (num_pellets_history[i] + num_pellets_history[i + 1]) / 2;

                // set i+1th element to NAN/inValue to remove after
                energy_in_ether_history[i + 1] = float.NaN;
                energy_in_agents_history[i + 1] = float.NaN;
                energy_in_pellets_history[i + 1] = float.NaN;

                num_pellets_history[i + 1] = int.MinValue;
                num_agents_history[i + 1] = int.MinValue;

                
            }

            // Strip the NaN/MinValues values
            energy_in_ether_history.RemoveAll(x => float.IsNaN(x));
            energy_in_agents_history.RemoveAll(x => float.IsNaN(x));
            energy_in_pellets_history.RemoveAll(x => float.IsNaN(x));

            num_agents_history.RemoveAll(x => x == int.MinValue);
            num_pellets_history.RemoveAll(x => x == int.MinValue);
        }
    }

    public IEnumerator UpdateHistory(){
        while(true){
            yield return new WaitForSeconds(manager.delta_history_update_time);
            AppendToHistory();
        }
    }
}
