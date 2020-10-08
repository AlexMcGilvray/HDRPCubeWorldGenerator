using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellV2FaceDirection
{
    Up,
    North,
    South,
    East,
    West
}

public class FaceInfo
{

}

public class CellV2 : MonoBehaviour
{
    public float Height = 1f;

    public Material worldMaterial;

    void Start()
    {
        _meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (_meshRenderer == null)
        {
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }

        if (worldMaterial == null)
        {
            _meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
        }
        else
        {
            _meshRenderer.sharedMaterial = worldMaterial;
        }

        _meshFilter = gameObject.GetComponent<MeshFilter>();
        if (_meshFilter == null)
        {
            _meshFilter = gameObject.AddComponent<MeshFilter>();
        }
    }

    void Update()
    {

    }
    // logical model data
    Vector3[] _logicalVertices = new Vector3[8];

    Dictionary<CellV2FaceDirection, FaceInfo> _faceInfos =
        new Dictionary<CellV2FaceDirection, FaceInfo>();
    // render model data
    Vector3[] _renderVertices = new Vector3[20]; // 5 faces with 4 vertices each
    Vector3[] _renderNormals = new Vector3[20];
    Vector2[] _renderUVs = new Vector2[20];
    int[] _renderIndices = new int[30]; // 5 faces * (2 triangles * 3 indices)

    MeshFilter _meshFilter;

    Mesh _mesh;

    MeshRenderer _meshRenderer;

}
