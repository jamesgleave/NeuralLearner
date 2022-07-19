using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NEATMutationConfig : MonoBehaviour
{
    public static NEATMutationConfig config;

    [Header("Nodes")]
    [Range(0f, 1f)]
    public float add_node_prob;
    [Range(0f, 1f)]
    public float delete_node_prob;

    [Space]
    public bool allow_input_node_mutation;
    public bool allow_output_node_mutation;

    [Range(0f, 1f)]
    public float node_activation_mutation_prob;
    
    [Range(0f, 1f)]
    public float node_calculation_method_mutation_prob;

    [Header("Connections")]
    [Range(0f, 1f)]
    public float add_connection_prob;

    [Range(0f, 1f)]
    public float delete_connection_prob;

    [Space]
    [Range(0f, 1f)]
    public float connection_mutation_prob;

    [Range(0f, 1f)]
    public float connection_replacement_prob;

    void Start(){
        config = this;
    }
}
