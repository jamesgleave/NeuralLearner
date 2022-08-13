using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EASSTester : MonoBehaviour
{
    public EASSBody base_body, effected_body;

    public Genes genes;

    // Update is called once per frame
    void Update()
    {
        if(effected_body.Ready()){effected_body.Build(genes, new Color(genes.colour_r, genes.colour_b, genes.colour_g), 0, 0, 0);}
    }
}
