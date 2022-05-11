using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserController3D : MonoBehaviour
{
    /*
	EXTENDED FLYCAM
		Desi Quintans (CowfaceGames.com), 17 August 2012.
		Based on FlyThrough.js by Slin (http://wiki.unity3d.com/index.php/FlyThrough), 17 May 2011.
 
	LICENSE
		Free as in speech, and free as in beer.
 
	FEATURES
		WASD/Arrows:    Movement
		          Q:    Climb
		          E:    Drop
                      Shift:    Move faster
                    Control:    Move slower
                        End:    Toggle cursor locking to screen (you can also press Ctrl+P to toggle play mode on and off).
	*/

    public float sensitivity = 5;
    public float aim_assists = 5;
    public float actual_sensitivity;

    public float climbSpeed = 4;
    public float normalMoveSpeed = 10;
    public float slowMoveFactor = 0.25f;
    public float fastMoveFactor = 3;
    public GameObject selected;

    [SerializeField]
    private float rotate_horizontal = 0.0f;
    [SerializeField]
    private float rotate_vertical = 0.0f;

    void Start()
    {
        Screen.lockCursor = true;
    }

    void Update()
    {
        // Control the camera
        if (Input.GetKeyDown(KeyCode.End) || Input.GetKeyDown(KeyCode.Return))
        {
            Screen.lockCursor = (Screen.lockCursor == false) ? true : false;
        }

        if (!Screen.lockCursor)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            // Lets check if we selected anything
            var p1 = transform.position;
            RaycastHit hit;
            float distanceToObstacle = 0;

            // Spherecast!
            if (Physics.SphereCast(p1, 0.35f, transform.forward, out hit, 100) && hit.collider.gameObject != selected && selected == null)
            {
                distanceToObstacle = hit.distance;
                Select(hit.collider.gameObject);
            }
            else
            {
                Deselect();
            }
        }
        else
        {
            // Lets check if we selected anything
            var p1 = transform.position;
            RaycastHit hit;
            float distanceToObstacle = 0;

            // Even if we are not clickling, the neurons are "sticky" for your mouse
            if (Physics.SphereCast(p1, 0.35f, transform.forward, out hit, 100) && hit.collider.gameObject != selected && selected == null)
            {
                actual_sensitivity = aim_assists;
            }
            else
            {
                actual_sensitivity = sensitivity;
            }
        }

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            transform.position += transform.forward * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
            transform.position += transform.right * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            transform.position += transform.forward * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
            transform.position += transform.right * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
        }
        else
        {
            transform.position += transform.forward * normalMoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
            transform.position += transform.right * normalMoveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
        }


        if (Input.GetKey(KeyCode.Q)) { transform.position += transform.up * climbSpeed * Time.deltaTime; }
        if (Input.GetKey(KeyCode.E)) { transform.position -= transform.up * climbSpeed * Time.deltaTime; }


        if (selected != null)
        {
            // Look at selected
            transform.LookAt(selected.transform.position);
        }
        else
        {
            rotate_horizontal = -Input.GetAxis("Mouse X");
            rotate_vertical = -Input.GetAxis("Mouse Y");
            transform.Rotate(-Vector3.up * rotate_horizontal * actual_sensitivity * Time.deltaTime);
            transform.Rotate(Vector3.right * rotate_vertical * actual_sensitivity * Time.deltaTime);
        }
    }

    public void Select(GameObject new_selected)
    {
        this.selected = new_selected;
        this.selected.GetComponent<ForceDirectedNode>().Select();
    }

    public void Deselect()
    {
        // Make the selected reference null after we call deselect
        if (this.selected != null)
        {
            this.selected.GetComponent<ForceDirectedNode>().Deselect();
            this.selected = null;
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawLine(Camera.main.ScreenToWorldPoint(Input.mousePosition), transform.forward * 100);
    }
}



