using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentStomach : MonoBehaviour
{
    [Header("Stomach Volume")]
    [SerializeField]
    private float volume_capacity;

    [SerializeField]
    private float volume_current;

    [SerializeField]
    private float volume_by_meat;

    [SerializeField]
    private float volume_by_pellet;

    [SerializeField]
    private float volume_by_egg;

    [Header("Potential Energy")]
        [SerializeField]
    private float potential_energy_by_meat;

    [SerializeField]
    private float potential_energy_by_pellet;

    [SerializeField]
    private float potential_energy_by_egg;

    [Header("Stomach Digestion")]
    [SerializeField]
    private float digestion_rate = 1;

    [Header("Energy")]
    [SerializeField]
    private float available_energy;

    [SerializeField]
    private float net_energy_movement;

    private Manager manager;
    private BaseAgent agent;

    public void Setup(BaseAgent agent, Manager manager){
        this.manager = manager;
        this.agent = agent;
    }

    public void Consume(float energy, float energy_density, int id){

        if(float.IsNaN(energy_density) || energy_density == 0){return;}
        // Calculate the volume of the food consumed
        float volume_consumed = energy / energy_density;

        // Add the volume
        switch(id){
            case (int)ID.FoodPellet:
                volume_by_pellet += volume_consumed;
                break;
            case (int)ID.Meat:
                volume_by_meat += volume_consumed;
                break;
            case (int)ID.WobbitEgg:
                volume_by_egg += volume_consumed;
                break;
        }

        // Total volume is going to be the sum of the three above
        volume_current = volume_by_egg + volume_by_meat + volume_by_pellet;
    }

    private void Update(){
        volume_capacity = agent.transform.localScale.x * manager.stomach_volume_multiplier;
        Digest();
    }

    private void Digest(){
        float total = 0;
        digestion_rate = manager.digestion_coefficient;
        if(volume_current == 0){ net_energy_movement = -agent.true_metabolic_cost; return;}
        // Digest proportionally to the volume
        float weighted_egg_digestion = volume_by_egg / volume_current;
        float weighted_meat_digestion = volume_by_meat / volume_current;
        float weighted_pellet_digestion = volume_by_pellet / volume_current;

        // Calculate how efficient the digestion is
        float pellet_digestion_efficiency_percentage = (1 - agent.genes.diet);
        float meat_and_egg_digestion_efficiency_percentage = Mathf.Clamp(agent.genes.diet, 0.35f, 1f);

        // Calculate the total energy and volume reduction
        float energy_extracted_pellets = digestion_rate * (weighted_pellet_digestion * pellet_digestion_efficiency_percentage * Time.deltaTime / manager.food_pellet_digestion_toughness);
        float volume_reduction_pellets = energy_extracted_pellets / manager.food_pellet_energy_density;

        float energy_extracted_eggs = digestion_rate * (weighted_egg_digestion * meat_and_egg_digestion_efficiency_percentage * Time.deltaTime / manager.egg_digestion_toughness);
        float volume_reduction_eggs = energy_extracted_eggs / manager.egg_energy_density;

        float energy_extracted_meat = digestion_rate * (weighted_meat_digestion * meat_and_egg_digestion_efficiency_percentage * Time.deltaTime / manager.meat_pellet_digestion_toughness);
        float volume_reduction_meat = energy_extracted_meat / manager.meat_energy_density;

        volume_by_egg = Mathf.Max(0, volume_by_egg - volume_reduction_eggs);
        volume_by_meat = Mathf.Max(0, volume_by_meat - volume_reduction_meat);
        volume_by_pellet = Mathf.Max(0, volume_by_pellet - volume_reduction_pellets);

        volume_current = volume_by_egg + volume_by_meat + volume_by_pellet;

        // Calculate the potential energy
        potential_energy_by_pellet = volume_by_pellet * manager.food_pellet_energy_density * pellet_digestion_efficiency_percentage;
        potential_energy_by_meat = volume_by_meat * manager.meat_energy_density * meat_and_egg_digestion_efficiency_percentage;
        potential_energy_by_egg = volume_by_egg * manager.egg_energy_density * meat_and_egg_digestion_efficiency_percentage;

        // Calculate the energy extracted
        total = energy_extracted_pellets + energy_extracted_eggs + energy_extracted_meat;
        available_energy += total;

        net_energy_movement = total - agent.true_metabolic_cost;
    }

    public float GetAvailableEnergy(){
        float returned = available_energy;
        available_energy = 0;
        return returned;
    }

    public float GetFullnessPercentage(){
        return volume_current / volume_capacity;
    }

    public float GetTotalPotentialEnergy(){return potential_energy_by_egg + potential_energy_by_pellet + potential_energy_by_meat;}

    
}
