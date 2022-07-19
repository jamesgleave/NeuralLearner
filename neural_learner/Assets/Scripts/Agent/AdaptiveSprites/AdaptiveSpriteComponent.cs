using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AdaptiveSpriteComponent
{
    public List<string> component_names;
    public SpriteRenderer sprite_renderer;
    public string attribute_name;

    /// <summary>
    /// If true, then the zero value will be turning off the sprite renderer
    /// </summary>
    public bool nullable;


    public void SetAttributes(float attribute, Color color){
        
        // Create the index and set the sprite color
        int index = Mathf.FloorToInt(component_names.Count * attribute);
        sprite_renderer.color = color;

        // If nullable is true, then the lowest value will result in no sprite
        if(nullable){
            if(index == 0){
                sprite_renderer.sprite = null;
            }else{

                sprite_renderer.sprite = AdaptiveSpriteManager.searchable_phenotype_components[component_names[index]];
            }
        }else{
            index = Mathf.Clamp(Mathf.CeilToInt(component_names.Count * attribute) - 1, 0 , component_names.Count - 1);
            sprite_renderer.sprite = AdaptiveSpriteManager.searchable_phenotype_components[component_names[index]];
        }
    }
}
