using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EASSBody : EASSComponent
{
    /// <summary>
    /// The y point where the mouth connects to the body.
    /// </summary>
    [Range(-1, 1f)]
    public float mouth_attachment_point;

    /// <summary>
    /// Scales the size of the mouth
    /// </summary>
    public float mouth_scaler = 1;

    /// <summary>
    /// The y position of the eye when FOV is maximized.
    /// </summary>
    [Range(-1, 1f)]
    public float  eye_attachment_point_max;

    /// <summary>
    /// The y position of the eye when FOV is minimized.
    /// </summary>
    [Range(-1, 1f)]
    public float  eye_attachment_point_min;

    /// <summary>
    /// The angle of the eye
    /// </summary>
    public float eye_attachment_point_angle;

    /// <summary>
    /// Separation at max FOV
    /// </summary>
    public float eye_separation_factor_max;

    /// <summary>
    /// Separation at min Fov
    /// </summary>
    public float eye_separation_factor_min;

    /// <summary>
    /// Where the speed components of the body are attached to on the y axis.
    /// </summary>
    [Range(-1, 1f)]
    public float speed_attachment_point;

    /// <summary>
    /// How far apart the two units will be
    /// </summary>
    public float speed_separation_factor;

    /// <summary>
    /// The angle of the units
    /// </summary>
    public float speed_angle;

    public EASSBodyComponent eye_left, eye_right, mouth, speed_left, speed_right;



    public void Build(EASSBody body, Genes genes, int eye_id, int mouth_id, int speed_id){
        // Setup the body
        mouth_attachment_point = body.mouth_attachment_point;
        mouth_scaler = body.mouth_scaler;
        eye_attachment_point_max = body.eye_attachment_point_max;
        eye_attachment_point_min = body.eye_attachment_point_min;
        eye_attachment_point_angle = body.eye_attachment_point_angle;
        eye_separation_factor_max = body.eye_separation_factor_max;
        eye_separation_factor_min = body.eye_separation_factor_min;
        speed_attachment_point = body.speed_attachment_point;
        speed_separation_factor = body.speed_separation_factor;
        speed_angle = body.speed_angle;
        sprite_renderer.sprite = body.sprite_renderer.sprite;
        this.Build(genes, new Color(genes.colour_r, genes.colour_g, genes.colour_b), eye_id, mouth_id, speed_id);       
    }

    public void Build(Genes genes, Color color, int eye_id, int mouth_id, int speed_id){
        sprite_renderer.color = color;

        // Build the components
        // Start with eyes
        if(genes.perception >= 0.05 && eye_id >= 0 && (eye_left != null || eye_right != null)){
            float eye_y = Mathf.Lerp(eye_attachment_point_min, eye_attachment_point_max, genes.field_of_view);
            float eye_x = Mathf.Lerp(eye_separation_factor_min, eye_separation_factor_max, genes.field_of_view);

            // Process the eye_id. 
            // The fist eye_id passed in is thanks to the genes and points towards a list of sprites.
            // Now we need to map the perception to a sprite with in that list.
            eye_id = EASSManager.instance.sprites_per_sheet - Mathf.RoundToInt(EASSManager.instance.sprites_per_sheet * genes.perception) + eye_id * EASSManager.instance.sprites_per_sheet;
            eye_left.Build(EASSComponentType.EASS_BODY_EYE, eye_id, color, -eye_x, eye_y, -eye_attachment_point_angle);
            eye_right.Build(EASSComponentType.EASS_BODY_EYE, eye_id, color, eye_x, eye_y, eye_attachment_point_angle);
        }else if(genes.perception < 0.05){
            eye_left.Clear();
            eye_right.Clear();
        }

        // Setup speed components
        if(genes.speed >= 0.05 && speed_id >= 0 && (speed_left != null || speed_right != null)){
            speed_id = EASSManager.instance.sprites_per_sheet - Mathf.RoundToInt(EASSManager.instance.sprites_per_sheet * genes.speed) + speed_id * EASSManager.instance.sprites_per_sheet;
            speed_left.Build(EASSComponentType.EASS_BODY_SPEED, speed_id, color, -speed_separation_factor, speed_attachment_point, speed_angle, 1);
            speed_right.Build(EASSComponentType.EASS_BODY_SPEED, speed_id, color, speed_separation_factor, speed_attachment_point, -speed_angle, 1);
        }else if(genes.speed < 0.05){
            speed_left.Clear();
            speed_right.Clear();
        }
        // Setup mouth component
        mouth_id = EASSManager.instance.sprites_per_sheet - Mathf.RoundToInt(EASSManager.instance.sprites_per_sheet * (1 - genes.attack)) + mouth_id * EASSManager.instance.sprites_per_sheet;
        mouth.Build(EASSComponentType.EASS_BODY_MOUTH, mouth_id, color, 0, mouth_attachment_point, 0, mouth_scaler);        
        
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + Vector3.up * mouth_attachment_point, 0.01f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position + Vector3.up * eye_attachment_point_max + Vector3.right * eye_separation_factor_max, 0.01f);
        Gizmos.DrawSphere(transform.position + Vector3.up * eye_attachment_point_max - Vector3.right * eye_separation_factor_max, 0.01f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position + Vector3.up * eye_attachment_point_min + Vector3.right * eye_separation_factor_min, 0.01f);
        Gizmos.DrawSphere(transform.position + Vector3.up * eye_attachment_point_min - Vector3.right * eye_separation_factor_min, 0.01f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position + Vector3.up * speed_attachment_point + Vector3.right * speed_separation_factor, 0.01f);
        Gizmos.DrawSphere(transform.position + Vector3.up * speed_attachment_point - Vector3.right * speed_separation_factor, 0.01f);
    }


}
