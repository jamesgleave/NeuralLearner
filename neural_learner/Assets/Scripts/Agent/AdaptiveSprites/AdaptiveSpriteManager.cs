using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdaptiveSpriteManager : MonoBehaviour
{

    public List<AdaptiveSprite> phenotypes;
    public Texture2D[] textures;
    public static Dictionary<string, Sprite> searchable_phenotype_components;

    public void Start(){
        searchable_phenotype_components = new Dictionary<string, Sprite>();
        searchable_phenotype_components["null"] = null;
        foreach(Texture2D texture in textures){
                foreach(Sprite s in Resources.LoadAll<Sprite>(texture.name)){
                searchable_phenotype_components[s.name] = s;
            }
        }
    }
}
