using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    [Min(1)]
    [SerializeField] private int xSize = 10;
    [Min(1)]
    [SerializeField] private int zSize = 10;
    [SerializeField] private float cellSize = 1f;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uv;
    private Vector3[] normals;

    private void Awake()
    {
        mesh = new Mesh
        {
            name = "GeneratedMesh"
        };

        var meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;

        GenerateMesh();
    }

    private void GenerateMesh()
    {
        CreateShape();
        UpdateMesh();
    }

    private void CreateShape()
    {
        // Vertices
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        uv = new Vector2[vertices.Length];
        normals = new Vector3[vertices.Length];

        var i = 0;
        for (var z = 0; z <= zSize; z++)
        {
            for (var x = 0; x <= xSize; x++)
            {
                var pos = new Vector3(x * cellSize, 0f, z * cellSize);
                vertices[i] = pos;
                uv[i] = new Vector2((float)x / xSize, (float)z / zSize);
                normals[i] = Vector3.up;
                i++;
            }
        }

        // Triangles
        triangles = new int[xSize * zSize * 6];
        var vert = 0;
        var tris = 0;

        for (var z = 0; z < zSize; z++)
        {
            for (var x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;

                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }

            vert++;
        }
    }

    private void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.normals = normals;

        // If you want Unity to compute normals instead of using the flat ones:
        // mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
