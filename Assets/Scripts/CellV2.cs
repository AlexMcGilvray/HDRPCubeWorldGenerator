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
    public int[] RenderVertexIndices{get;} = new int[4];
    public int[] LogicalVertexIndices{get;} = new int[4];
    public Vector3 Normal{get;}
    public int[] RenderUVIndices {get;} = new int[4];
}

public class CellV2 : MonoBehaviour
{
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

    public void MakeCell(float size, float height, float animTime = 1f)
    {
        _height = height;
        // populate logical vertex structure
        


        // make each face & setup faceInfos logical struct
        
    }

    public void MakeFace(CellV2FaceDirection direction,float size)
    {

    }

    void Update()
    {

    }
    // logical model data
    private Vector3[] _logicalVertices = new Vector3[8];
    private Dictionary<CellV2FaceDirection, FaceInfo> _faceInfos =
        new Dictionary<CellV2FaceDirection, FaceInfo>();
    private float _height;
    // render model data
    private Vector3[] _renderVertices = new Vector3[20]; // 5 faces with 4 vertices each
    private Vector3[] _renderNormals = new Vector3[20];
    private Vector2[] _renderUVs = new Vector2[20];
    private int[] _renderIndices = new int[30]; // 5 faces * (2 triangles * 3 indices)
    private MeshFilter _meshFilter;
    private Mesh _mesh;
    private MeshRenderer _meshRenderer;
}
