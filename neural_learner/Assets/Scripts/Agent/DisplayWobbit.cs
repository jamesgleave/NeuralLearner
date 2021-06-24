using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayWobbit : MonoBehaviour
{
    // 
    public int head_id, body_id;
    public SpriteManager sprite_manager;
    public GameObject head, body;
    public void Setup(Genes g)
    {
        head_id = g.spritemap["head"];
        body_id = g.spritemap["body"];

        head.GetComponent<Image>().sprite = sprite_manager.head_components[head_id].GetComponent<SpriteRenderer>().sprite;
        body.GetComponent<Image>().sprite = sprite_manager.body_components[body_id].GetComponent<SpriteRenderer>().sprite;
    }
}
