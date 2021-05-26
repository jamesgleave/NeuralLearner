using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Display : MonoBehaviour
{
    [Header(" Vertical Layer Seperation ")]
    public float alpha = 3;

    [Header("Horizontal Neuron Seperation ")]
    public float lambda = 3;

    [Header("Max Neuron Size")]
    public float beta = 0.76f;

    [Header("Neuron Scale Rate")]
    public float theta = 6.9f;

    [Header("Max Weight Width")]
    public float sigma = 0.31f;

    [Header("Min Weight Width")]
    public float sigma_prime = 0.1f;

    [Header("Neuron Acceleration")]
    public float phi = 1;

    [Header("Neuron Movement Speed")]
    public float gamma = 30f;

    [Header("The Visualizer Has Been Activated (Click To Reset)")]
    public bool activated = false;

    [Header("The update rate of the display")]
    public float update_rate = 1f;

    public virtual void Activate()
    {
        activated = true;
    }
}
