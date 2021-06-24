using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    [Header("All Body Components To Chose From")]
    public List<GameObject> body_components;

    [Header("All Head Components To Chose From")]
    public List<GameObject> head_components;

    public void SetSprite(int head_id, int body_id, BaseAgent agent, Color c)
    {
        // Get the game objects
        GameObject head = head_components[head_id];
        GameObject body = body_components[body_id];

        // If we do not have the values assigned then return
        if (agent.body == null)
            return;

        // Setup the body
        // Give the body the correct body sprite
        agent.body.GetComponent<SpriteRenderer>().sprite = body.GetComponent<SpriteRenderer>().sprite;
        agent.body.GetComponent<SpriteRenderer>().color = c;

        // Setup the agent's collider
        agent.body.GetComponent<PolygonCollider2D>().points = body.GetComponent<PolygonCollider2D>().points;


        // Setup the head
        // Give the head the correct body sprite
        agent.head.GetComponent<SpriteRenderer>().sprite = head.GetComponent<SpriteRenderer>().sprite;
        agent.head.GetComponent<SpriteRenderer>().color = c;

        // Setup the agent's collider
        agent.head.GetComponent<PolygonCollider2D>().points = head.GetComponent<PolygonCollider2D>().points;
    }

    public void SetBiChromaticAgent(BaseAgent agent, Color body_colour, Color head_colour)
    {
        agent.body.GetComponent<SpriteRenderer>().color = body_colour;
        agent.head.GetComponent<SpriteRenderer>().color = head_colour;
    }

    public void SetRandomComponents(Genes genes)
    {
        // Get indices
        int index_body = Random.Range(0, body_components.Count);
        int index_head = Random.Range(0, head_components.Count);

        // Setup indices
        genes.spritemap["body"] = index_body;
        genes.spritemap["head"] = index_head;
    }
}
