using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model;
using Parse;


public class testing : MonoBehaviour
{
    public float cooldown = 0;


    public void Start()
    {
        transform.position = new Vector3(Random.Range(-35, 35), 1, Random.Range(-35, 35));
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public void Update()
    {
        cooldown -= Time.deltaTime;
        if (cooldown > 0)
        {
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<Rigidbody>().useGravity = false;
        }
        else
        {
            GetComponent<MeshRenderer>().enabled = true;
            GetComponent<BoxCollider>().enabled = true;
            GetComponent<Rigidbody>().useGravity = true;
        }

        if (transform.position.y < -5)
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            transform.position = new Vector3(Random.Range(-35, 35), 1f, Random.Range(-35, 35));
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("agent") && cooldown < 0)
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            transform.position = new Vector3(Random.Range(-35, 35), 1f, Random.Range(-35, 35));
            collision.gameObject.GetComponent<BasicAgentController>().Birth();
            cooldown = Random.value * 350;
        }
    }
}
