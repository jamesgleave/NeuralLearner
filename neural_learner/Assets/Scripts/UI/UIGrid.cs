using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGrid : Graphic
{
    // The thickenss of the grid
    public float margin = 10;

    public Vector2Int gridsize = new Vector2Int(1, 1);
    float width, height, cell_width, cell_height;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        // Clear cache
        vh.Clear();

        // define width and height
        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        // Update cell size
        cell_width = width / gridsize.x;
        cell_height = height / gridsize.y;

        // Iterate over the grid
        int count = 0;
        for (int y = 0; y < gridsize.y; y++)
        {
            for (int x = 0; x < gridsize.x; x++)
            {
                DrawCell(x, y, count, vh);
                count++;
            }
        }
    }

    private void DrawCell(int x, int y, int index, VertexHelper vh)
    {
        float xpos = cell_width * x;
        float ypos = cell_height * y;

        // Set corners
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        // 0,0
        vertex.position = new Vector3(xpos, ypos);
        vh.AddVert(vertex);
        //0,height
        vertex.position = new Vector3(xpos, ypos + cell_height);
        vh.AddVert(vertex);
        //width,height
        vertex.position = new Vector3(xpos + cell_width, ypos + cell_height);
        vh.AddVert(vertex);
        //height,0
        vertex.position = new Vector3(xpos + cell_width, ypos);
        vh.AddVert(vertex);

        // Square it for easy increase/decrease in size
        float thickness = margin * margin;

        // Add the corners (we want a square which is two triangles)
        float ws = thickness * thickness;
        float distsqr = ws / thickness;
        float distance = Mathf.Sqrt(distsqr);

        vertex.position = new Vector3(xpos + distance, ypos + distance);
        vh.AddVert(vertex);

        vertex.position = new Vector3(xpos + distance, ypos + (cell_height - distance));
        vh.AddVert(vertex);

        vertex.position = new Vector3(xpos + (cell_width - distance), ypos + (cell_height - distance));
        vh.AddVert(vertex);

        vertex.position = new Vector3(xpos + (cell_width - distance), ypos + distance);
        vh.AddVert(vertex);

        int offset = index * 8;

        // Connect each edge
        vh.AddTriangle(offset + 0, offset + 1, offset + 5);
        vh.AddTriangle(offset + 5, offset + 4, offset + 0);

        vh.AddTriangle(offset + 1, offset + 2, offset + 6);
        vh.AddTriangle(offset + 6, offset + 5, offset + 1);

        vh.AddTriangle(offset + 2, offset + 3, offset + 7);
        vh.AddTriangle(offset + 7, offset + 6, offset + 2);

        vh.AddTriangle(offset + 3, offset + 0, offset + 4);
        vh.AddTriangle(offset + 4, offset + 7, offset + 3);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            gridsize.x += 1;
            gridsize.y += 1;
            SetAllDirty();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            gridsize.x -= 1;
            gridsize.y -= 1;
            SetAllDirty();
        }
    }
}
