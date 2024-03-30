using System.Collections.Generic;
using UnityEngine;

public class PolygonMeshCreator : MonoBehaviour
{
    [SerializeField] private Color color;
    public void InitMeshCreater(List<RulerPoints> rulerPoints, out GameObject meshObject)
    {
        List<Transform> points = new List<Transform>();
        for (int i = 0; i < rulerPoints.Count; i++)
        {
            points.Add(rulerPoints[i].pointA);
        }

        CreateMesh(points, out meshObject);
    }

    private void CreateMesh(List<Transform> points, out GameObject _meshObject)
    {
        GameObject meshObject = CreatePolygonMesh(points);
        AdjustMeshPivot(meshObject);
        _meshObject = meshObject;
    }

    private GameObject CreatePolygonMesh(List<Transform> points)
    {
        GameObject meshObject = new GameObject("PolygonMeshObject");

        MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();

        Vector3[] vertices = new Vector3[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            vertices[i] = points[i].position;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        
        List<int> triangles = new List<int>();
        for (int i = 1; i < points.Count - 1; i++)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1);
        }

        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;

        meshRenderer.material = new Material(Shader.Find("UI/UnlitTransparent"));
        meshRenderer.material.SetColor("_Color", color);

        return meshObject;
    }
    
    private void CreatePolygonMesh(List<Vector3> points, MeshFilter meshFilter)
    {
        Vector3[] vertices = new Vector3[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            vertices[i] = points[i];
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;

        List<int> triangles = new List<int>();
        for (int i = 1; i < points.Count - 1; i++)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1);
        }

        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;
    }
    
    private void AdjustMeshPivot(GameObject meshObject)
    {
        Mesh mesh = meshObject.GetComponent<MeshFilter>().mesh;
        Vector3 center = mesh.bounds.center;

        Vector3[] adjustedVertices = new Vector3[mesh.vertices.Length];
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            adjustedVertices[i] = mesh.vertices[i] - center;
        }

        mesh.vertices = adjustedVertices;
        mesh.RecalculateBounds();

        meshObject.transform.position = center;
    }
}