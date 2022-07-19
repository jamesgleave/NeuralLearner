using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    // We need a line renderer to create the lines
    LineRenderer line;
    public int max_verts = 200;
    public int num_verts;

    // The colours for high and low (we lerp between these values)
    public Color chigh;
    public Color clow;

    // For fadeout
    public bool on = true;
    public float fade_time = 1;

    // the mesh collider for the line renderer
    public PolygonCollider2D col;

    void Start()
    {
        // Get the line renderer component 
        line = GetComponent<LineRenderer>();

        // Initialize the mesh collider as disabled (less overhead)
        // col.enabled = false;
    }

    public void Setup()
    {
        Start();
    }

    public void Clear()
    {
        if (line != null)
        {
            line.positionCount = 0;
        }
    }

    public void Draw(Vector3 point0, Vector3 point1, string type = "line")
    {
        // The number of verts will reach its max value when we are at least at 100 frames per second
        num_verts = Mathf.Clamp((int)(max_verts / (100 * Time.deltaTime)), 10, max_verts);

        // Ensure we have a scale of one nomatter how large the parent gets 
        transform.lossyScale.Set(1 / transform.parent.localScale.x, 1 / transform.parent.localScale.x, 1 / transform.parent.localScale.x);

        if (type.Equals("mid"))
        {
            Vector3 mid;
            // Find the mid point
            mid = 3 * Vector3.Normalize(point1 - point0) + point0;
            mid.Set(mid.x, point0.y, mid.z);
            DrawQuadraticBezierCurve(point0, mid, point1, num_verts);
        }
        else if (type.Equals("tree"))
        {
            Vector3 branch_point = new Vector3(point0.x + point1.x / 2f, point0.y + point1.y / 4, point0.z);
            DrawQuadraticBezierCurve(point0, branch_point, point1, num_verts);
        }
        else if (type.Equals("line"))
        {
            SimpleLine(point0, point1);
        }
    }

    public void DrawRecurrentConnection(Vector3 point0, Vector3 point1, float midscaler)
    {

        // The number of verts will reach its max value when we are at least at 100 frames per second
        num_verts = Mathf.Clamp((int)(max_verts / (100 * Time.deltaTime)), 10, max_verts);

        // Ensure we have a scale of one nomatter how large the parent gets 
        transform.lossyScale.Set(1 / transform.parent.localScale.x, 1 / transform.parent.localScale.x, 1 / transform.parent.localScale.x);

        Vector3 mid;
        // Find the mid point
        mid = (point0 + point1) / 2 + Vector3.up * midscaler;

        // If the points are equal, meaning the node feeds into itself, just shift point 1 and two a bit
        if (point1 == point0)
        {
            point1.x += 0.1f;
            point0.x -= 0.1f;
        }

        DrawQuadraticBezierCurve(point0, mid, point1, num_verts);
    }

    public void SimpleLine(Vector3 v1, Vector3 v2)
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.SetPosition(0, v1);
        line.SetPosition(1, v2);
    }

    public void SetFeatures(float start, float end)
    {
        line.startWidth = start;
        line.endWidth = end;
    }

    public void SetColour(float v)
    {
        // Scale v between 0 and 1
        // v = (float)System.Math.Tanh(v) + 1;

        // Store the alpha
        float alpha = line.material.color.a;

        // Set the colour
        // line.material.color = Color.Lerp(clow, chigh, v);
        line.material.color = v > 0.0f ? chigh : clow;
        // Set the alpha
        Color c = line.material.color;
        c.a = alpha;
        line.material.color = c;
    }

    public void SetEndWidth(float end)
    {
        line.endWidth = end;
    }

    void DrawQuadraticBezierCurve(Vector3 point0, Vector3 point1, Vector3 point2, int count = 200)
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = count;
        float t = 0f;
        Vector3 B = new Vector3(0, 0, 0);
        for (int i = 0; i < line.positionCount; i++)
        {
            B = (1 - t) * (1 - t) * point0 + 2 * (1 - t) * t * point1 + t * t * point2;
            line.SetPosition(i, B);
            t += (1 / (float)line.positionCount);
        }
    }

    IEnumerator FadeIn()
    {
        on = true;
        // for (float ft = line.material.color.a; ft <= 1f; ft += fade_time * Time.deltaTime)
        // {
        //     Color c = line.material.color;
        //     c.a = ft;
        //     line.material.color = c;
        //     yield return null;
        // }
        line.enabled = on;
        return null;
    }

    IEnumerator FadeOut()
    {
        on = false;
        line.enabled = on;
        return null;
        // for (float ft = line.material.color.a; ft >= 0.1f; ft -= fade_time * Time.deltaTime)
        // {
        //     Color c = line.material.color;
        //     c.a = ft;
        //     line.material.color = c;
        //     yield return null;
        // }
    }

    public void Fade(bool fade_in)
    {
        if (fade_in)
        {
            StartCoroutine("FadeIn");
        }
        else
        {
            StartCoroutine("FadeOut");
        }
    }

    public float GetEndWidth()
    {
        return line.endWidth;
    }

    public void SetMaterial(Material mat)
    {
        line.material = mat;
    }
}
