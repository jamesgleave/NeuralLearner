using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edible : Interactable
{

    /// <summary>
    ///   <para>The energy contained in the edible object</para>
    /// </summary>
    public float energy;

    /// <summary>
    /// How quickly this can be turned into energy for an agent
    /// </summary>
    public float digestion_rate;

    public virtual float Eat(){
        return 0;
    }

    public virtual float Eat(float consumption_rate){
        return 0;
    }

    public float GetEnergyDensity(){
        return energy / GetVolume();
    }

    public float GetVolume(){
        return Mathf.Max(transform.localScale.x, float.Epsilon);
    }
}
