using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayWobbit : MonoBehaviour
{
    // 
    public int head_id, body_id;
    public SpriteManager sprite_manager;
    public GameObject head, body, cross;

    public void Setup(Genes g)
    {
        head_id = g.spritemap["head"];
        body_id = g.spritemap["body"];

        try
        {
            head.GetComponent<Image>().sprite = sprite_manager.head_components[head_id].GetComponent<SpriteRenderer>().sprite;
            head.GetComponent<Image>().color = new Color(g.colour_r, g.colour_g, g.colour_b);

            body.GetComponent<Image>().sprite = sprite_manager.body_components[body_id].GetComponent<SpriteRenderer>().sprite;
            body.GetComponent<Image>().color = new Color(g.colour_r, g.colour_g, g.colour_b);
        }
        catch
        {
            head.GetComponent<SpriteRenderer>().sprite = sprite_manager.head_components[head_id].GetComponent<SpriteRenderer>().sprite;
            head.GetComponent<SpriteRenderer>().color = new Color(g.colour_r, g.colour_g, g.colour_b);

            body.GetComponent<SpriteRenderer>().sprite = sprite_manager.body_components[body_id].GetComponent<SpriteRenderer>().sprite;
            body.GetComponent<SpriteRenderer>().color = new Color(g.colour_r, g.colour_g, g.colour_b);
            transform.localScale *= (1 + g.size);
        }

    }

    public void Update()
    {
        head.transform.LookAt(Camera.main.transform.position);
        body.transform.LookAt(Camera.main.transform.position);

        if (cross != null)
        {
            cross.transform.LookAt(Camera.main.transform.position);
        }
    }

    public void SetCross(bool state)
    {
        cross.SetActive(state);
    }
}
