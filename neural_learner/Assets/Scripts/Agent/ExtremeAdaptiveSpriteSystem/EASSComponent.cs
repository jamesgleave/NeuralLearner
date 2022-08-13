using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EASSComponent : MonoBehaviour
{
    [SerializeField]
    protected SpriteRenderer sprite_renderer;

    public void Start(){
        sprite_renderer = GetComponent<SpriteRenderer>();
    }

    public bool Ready(){
        return sprite_renderer != null;
    }

    public virtual void Build(EASSComponentType hash_id, int id, Color color, float x, float y, float angle, float size = 1){
        transform.localScale = Vector3.one * size;
        transform.localPosition = new Vector3(x, y, 0);
        transform.localEulerAngles = new Vector3(0, 0, angle);
        sprite_renderer.color = color;
    }

    public void Flip(){
        sprite_renderer.flipX = !sprite_renderer.flipX;
    }

    public void Clear(){
        sprite_renderer.sprite = EASSManager.instance.emptySprite;
    }
}

public enum EASSComponentType{
    EASS_BODY_HEAD,
    EASS_BODY_BODY,
    EASS_BODY_EYE,
    EASS_BODY_MOUTH,
    EASS_BODY_NOSE,
    EASS_BODY_EAR,
    EASS_BODY_SPEED,
}
