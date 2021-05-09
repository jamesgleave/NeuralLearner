using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public static Manager manager;
    public static Vector3 camera_position;
    public static GameObject interactable;
    public static BaseAgent selected_agent;
    public static AncestorManager ancestor_manager;
}
