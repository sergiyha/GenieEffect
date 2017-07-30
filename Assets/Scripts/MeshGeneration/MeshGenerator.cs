using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.VR;
using UnityEngine;

public class MeshGenerator
{
    public static MeshData GenerateMeshSquare(int width, int height)
    {
        float topLeftX = (width - 1) / -2f;
        float topLeftY = (height - 1) / 2f;

        MeshData meshData = new MeshData(width, height);
        int vertexIndex = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                meshData.Vertecies[vertexIndex] = new Vector3(topLeftX + x, topLeftY - y, 0);
                meshData.Uvs[vertexIndex] = new Vector2(x / ((float)width - 1), y / ((float)height - 1));
                if (x < width - 1 && y < height - 1)
                {
                    meshData.AddTriangles(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                    meshData.AddTriangles(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                }
                vertexIndex++;
            }

        }
        return meshData;
    }

}

public class MeshData
{
    public Vector3[] Vertecies;
    public int[] Triangles;
    public Vector2[] Uvs;

    private int _triangleIndex;

    public MeshData(int meshWidth, int meshHeight)
    {
        Vertecies = new Vector3[meshHeight * meshWidth];
        Triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
        Uvs = new Vector2[meshHeight * meshWidth];
    }

    public void AddTriangles(int a, int b, int c)
    {
        Triangles[_triangleIndex] = a;
        Triangles[_triangleIndex + 1] = b;
        Triangles[_triangleIndex + 2] = c;
        _triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = Vertecies;
        mesh.triangles = Triangles;
        mesh.uv = Uvs;
        mesh.RecalculateBounds();

        return mesh;
    }
}
