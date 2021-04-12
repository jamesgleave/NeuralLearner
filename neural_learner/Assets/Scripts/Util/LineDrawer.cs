using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    // We need a line renderer to create the lines
    LineRenderer line;

    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    public void Setup()
    {
        Start();
    }

    public void Draw(Vector3 point0, Vector3 point1)
    {
        Vector3 mid;

        // Find the mid point
        mid = 3 * Vector3.Normalize(point1 - point0) + point0;
        mid.Set(mid.x, point0.y, mid.z);
        DrawQuadraticBezierCurve(point0, mid, point1);
    }

    public void SetFeatures(float start, float end)
    {
        line.startWidth = start;
        line.endWidth = end;
    }

    void DrawQuadraticBezierCurve(Vector3 point0, Vector3 point1, Vector3 point2)
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 50;
        float t = 0f;
        Vector3 B = new Vector3(0, 0, 0);
        for (int i = 0; i < line.positionCount; i++)
        {
            B = (1 - t) * (1 - t) * point0 + 2 * (1 - t) * t * point1 + t * t * point2;
            line.SetPosition(i, B);
            t += (1 / (float)line.positionCount);
        }
    }
}
