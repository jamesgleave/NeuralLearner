using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILineRenderer : Graphic
{
    public UIGrid grid;
    public Vector2Int gridsize;
    public List<Vector2> points;

    float width, height, unit_width, unit_height;
    public float thickness = 10f;
    public float resolution = 1f;


    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        //points.Clear();
        //for (float i = -gridsize.x; i < gridsize.x; i += resolution)
        //{
        //    points.Add(new Vector2(i, Mathf.Sin(i)));
        //}

        width = rectTransform.rect.width;
        height = rectTransform.rect.height;

        unit_width = width / (float)gridsize.x;
        unit_height = height / (float)gridsize.y;

        // Return if we dont have enough points to draw anythign
        if (points.Count < 2)
        {
            return;
        }

        // Look at each point and draw the vert for each point
        float angle = 0;
        for (int i = 0; i < points.Count; i++)
        {
            if (i < points.Count - 1)
            {
                angle = GetAngle(points[i], points[i + 1]) + 45f;
            }

            DrawVerts(points[i], vh, angle);
        }

        for (int i = 0; i < points.Count - 1; i++)
        {
            int index = i * 2;
            vh.AddTriangle(index, index + 1, index + 3);
            vh.AddTriangle(index + 3, index + 2, index + 0);
        }
    }

    public float GetAngle(Vector2 me, Vector2 target)
    {
        return (float)(Mathf.Atan2(target.y - me.y, target.x - me.x) * (180 / Mathf.PI));
    }

    void DrawVerts(Vector2 p, VertexHelper vh, float angle)
    {
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(-thickness / 2, 0);
        vertex.position += new Vector3(unit_width * p.x, unit_height * p.y);
        vh.AddVert(vertex);

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 0);
        vertex.position += new Vector3(unit_width * p.x, unit_height * p.y);
        vh.AddVert(vertex);
    }

    private void Update()
    {
        if (grid != null)
        {
            if (gridsize != grid.gridsize)
            {
                gridsize = grid.gridsize;
                SetVerticesDirty();
            }
        }
    }

    private void OnMouseOver()
    {
        print("Hello!");
    }
}
