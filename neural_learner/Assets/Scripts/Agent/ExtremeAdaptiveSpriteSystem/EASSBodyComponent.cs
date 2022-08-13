using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EASSBodyComponent : EASSComponent
{
    public Vector3 attachment_offset;

    public override void Build(EASSComponentType hash_id, int id, Color color, float x, float y, float angle, float size = 1){
        transform.localScale = Vector3.one * size;
        transform.localPosition = new Vector3(RoundToNearestGrid(x + attachment_offset.x),RoundToNearestGrid(y + attachment_offset.y), 0);
        transform.localEulerAngles = new Vector3(0, 0, angle);
        sprite_renderer.color = color;

        sprite_renderer.sprite = EASSManager.GetSprite(hash_id, id);
    }

    float RoundToNearestGrid(float pos)
    {
        float difference = pos % (1/sprite_renderer.sprite.pixelsPerUnit);
        pos -= difference;
        return pos;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(attachment_offset + transform.position, 0.01f);
    }
}
