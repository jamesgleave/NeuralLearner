using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilityFunctions
{
    /// <summary>
    /// Destroy all game objects in given list.
    /// </summary>
    /// <param name="arr"></param>
    public static void DestroyGameObjects<T>(List<T> arr) where T : MonoBehaviour{
        foreach(T obj in arr){
            GameObject.Destroy(obj.gameObject);
        }
    }
}
