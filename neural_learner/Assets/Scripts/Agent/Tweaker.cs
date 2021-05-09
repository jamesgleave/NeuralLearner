using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tweaker : MonoBehaviour
{
    public float base_health;
    public float vitality;
    public float speed, size, perception;

    public float health;
    public float energy;
    public float metabolism;

    public float attack, defense;
    public float time_to_die;

    // Start is called before the first frame update
    void Start()
    {
        Set();
    }

    void Set()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.U))
        {
            energy -= metabolism * Time.deltaTime;

            if (energy < (Mathf.Pow(size, 1.3f) * vitality + health) * 0.3f)
            {
                health -= (1f + metabolism * energy) * Time.deltaTime;
            }

            time_to_die -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            Attack();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Set();
        }
    }

    void Attack()
    {
        health -= attack - defense;
    }
}
