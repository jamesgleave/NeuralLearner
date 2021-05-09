using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserControllerEvoSim : MonoBehaviour
{
    public float drag_speed = 2;
    public GameObject selected;
    private Vector3 dragOrigin;

    // Help from: https://forum.unity.com/threads/click-drag-camera-movement.39513/
    // Thx :)
    void Update()
    {

        // Check to see if we transition a scene
        if (Input.GetKeyDown(KeyCode.Return) && selected != null)
        {
            GetComponent<SimulationSceneManager>().ToAncestory(selected, selected.GetComponent<BaseAgent>(), selected.GetComponent<BaseAgent>().manager);
        }

        if (selected != null)
        {
            // Follow the selected thing!
            transform.position = Vector3.Lerp(transform.position, new Vector3(selected.transform.position.x, selected.transform.position.y, transform.position.z), (drag_speed / 5) / Vector2.Distance(transform.position, selected.transform.position));
        }

        if (Input.GetMouseButtonDown(0))
        {
            CheckSelect();
        }

        // Zoom in and out with Mouse Wheel
        transform.Translate(0, 0, Input.GetAxis("Mouse ScrollWheel") * drag_speed, Space.Self);

        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(0)) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = new Vector3(pos.x * drag_speed, pos.y * drag_speed, 0);
        transform.Translate(move, Space.World);
    }

    bool CheckSelect()
    {
        var mousePos = Input.mousePosition;
        mousePos.z = -transform.position.z; // select distance = 10 units from the camera
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePos), transform.forward);
        if (hit.collider != null)
        {
            selected = hit.collider.gameObject;
        }
        else
        {
            selected = null;
        }

        return selected != null;
    }

    private void OnDrawGizmos()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        Gizmos.DrawRay(mousePos2D, Vector2.zero);
    }
}
