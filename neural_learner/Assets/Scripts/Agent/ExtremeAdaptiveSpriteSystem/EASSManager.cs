using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EASSManager : MonoBehaviour
{
    public static EASSManager instance;

    /// <summary>
    /// The number of sprites on a sprite sheet.
    /// This needs to be consistent with the number of sprites in the sprite sheet.
    /// Used to index the sprites in the sprite list (i * sprites_per_sheet + sub_id).
    /// </summary>
    public int sprites_per_sheet = 10;

    public Sprite emptySprite;
    public Sprite[] eye_sprites;
    public Sprite[] body_sprites;
    public Sprite[] mouth_sprites;
    public Sprite[] speed_sprites;
    protected Dictionary<EASSComponentType, Sprite[]> sprite_dict;


    public void Start()
    {
        instance = this;
        
        // Initialize the sprite dictionary
        sprite_dict = new Dictionary<EASSComponentType, Sprite[]>();
        sprite_dict.Add(EASSComponentType.EASS_BODY_EYE, eye_sprites);
        sprite_dict.Add(EASSComponentType.EASS_BODY_BODY, body_sprites);
        sprite_dict.Add(EASSComponentType.EASS_BODY_MOUTH, mouth_sprites);
        sprite_dict.Add(EASSComponentType.EASS_BODY_SPEED, speed_sprites);

    }

    public void BuildSprite(BaseAgent agent){

    }

    public void BuildSprite(Genes genes){

    }

    public static Sprite GetSprite(EASSComponentType hash_id, int sub_id){
        return instance.sprite_dict[hash_id][sub_id];
    }
}
