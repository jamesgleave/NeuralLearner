using System.IO.IsolatedStorage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AdaptiveSprite : MonoBehaviour
{
    [Range(0f, 1f)]
    public float attack;

    [Range(0f, 1f)]
    public float speed;

    [Range(0f, 1f)]
    public float perception;

    public Dictionary<string, float> attributes = new Dictionary<string, float>();
    
    public AdaptiveSpriteComponent[] components;

    public void Update(){
        attributes["attack"] = attack;
        attributes["speed"] = speed;
        attributes["perception"] = perception;
        components[0].SetAttributes(Mathf.Sin(Time.time), Color.Lerp(Color.white, Color.red, Mathf.Sin(Time.time/10f)));
        components[1].SetAttributes(Mathf.Sin(0.25f + Time.time), Color.Lerp(Color.white, Color.green, Mathf.Sin(Time.time/10f)));
        components[2].SetAttributes(Mathf.Cos(Time.time), Color.Lerp(Color.white, Color.blue, Mathf.Sin(Time.time/10f)));

    }
}
