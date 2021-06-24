using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Range(0.0f, 2f)]
    public float timescale = 1;

    [Range(0.02f, 0.1f)]
    public float fixed_timestep = 1;

    public bool matching_physics_step;

    // Update is called once per frame
    void Update()
    {

        Time.timeScale = timescale;
        Time.fixedDeltaTime = fixed_timestep;

        if (matching_physics_step)
        {
            fixed_timestep = Time.deltaTime;
        }
    }
}
