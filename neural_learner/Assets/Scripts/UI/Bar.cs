using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    public Text title;
    public Text value;
    public RectTransform bar;

    public Vector2 desired_size;

    public void Update()
    {
        if (Vector2.Distance(desired_size, bar.sizeDelta) > 0.01f)
        {
            bar.sizeDelta += (desired_size - bar.sizeDelta) * Time.deltaTime;
        }
    }

    public void SetBarSize(float size, float width)
    {
        desired_size = new Vector3(size, width); ;
    }

    public float GetBarLen()
    {
        return bar.sizeDelta.x;
    }
}
