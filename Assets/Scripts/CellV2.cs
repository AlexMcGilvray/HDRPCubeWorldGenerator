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
    public int[] RenderVertexIndices{get;} = new int[4]; // can be used to look up vertex, normal, uv's
    public int[] LogicalVertexIndices{get;} = new int[4];
    public Vector3 Normal{get;set;}
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
        /// top
        _logicalVertices[0].x = -size;
        _logicalVertices[0].y = 0;
        _logicalVertices[0].z = size;

        _logicalVertices[1].x = size;
        _logicalVertices[1].y = 0;
        _logicalVertices[1].z = size;

        _logicalVertices[2].x = size;
        _logicalVertices[2].y = 0;
        _logicalVertices[2].z = -size;

        _logicalVertices[3].x = -size;
        _logicalVertices[3].y = 0;
        _logicalVertices[3].z = -size;

        /// bottom
        _logicalVertices[4].x = -size;
        _logicalVertices[4].y = 0;
        _logicalVertices[4].z = size;

        _logicalVertices[5].x = size;
        _logicalVertices[5].y = 0;
        _logicalVertices[5].z = size;

        _logicalVertices[6].x = size;
        _logicalVertices[6].y = 0;
        _logicalVertices[6].z = -size;

        _logicalVertices[7].x = -size;
        _logicalVertices[7].y = 0;
        _logicalVertices[7].z = -size;

        // make each face & setup faceInfos logical struct
        const int NumIndexesPerQuad = 6;
        int currentIndexBase = 0;

        const int NumEntriesPerQuad = 4;
        int currentQuadBase = 0;

        MakeFace(CellV2FaceDirection.Up, size, currentIndexBase, currentQuadBase);
        currentIndexBase += NumIndexesPerQuad;
        currentQuadBase += NumEntriesPerQuad;
        
        MakeFace(CellV2FaceDirection.North, size, currentIndexBase, currentQuadBase);
        currentIndexBase += NumIndexesPerQuad;
        currentQuadBase += NumEntriesPerQuad;
        
        MakeFace(CellV2FaceDirection.South, size, currentIndexBase, currentQuadBase);
        currentIndexBase += NumIndexesPerQuad;
        currentQuadBase += NumEntriesPerQuad;
        
        MakeFace(CellV2FaceDirection.East, size, currentIndexBase, currentQuadBase);
        currentIndexBase += NumIndexesPerQuad;
        currentQuadBase += NumEntriesPerQuad;
        
        MakeFace(CellV2FaceDirection.West, size, currentIndexBase, currentQuadBase);
    }

    public void MakeFace(CellV2FaceDirection direction, float size, int indexBase, int quadBase)
    {
        // add render vertices
        // add uv's
        // add normals
        // add indices
        // setup faceinfo structure

        FaceInfo faceInfo = new FaceInfo();

        faceInfo.RenderVertexIndices[0] = quadBase + 0;
        faceInfo.RenderVertexIndices[1] = quadBase + 1;
        faceInfo.RenderVertexIndices[2] = quadBase + 2;
        faceInfo.RenderVertexIndices[3] = quadBase + 3;

        switch(direction)
        {
            case CellV2FaceDirection.Up:
            {
                const int localoffset1 = 0;
                const int localoffset2 = 1;
                const int localoffset3 = 2;
                const int localoffset4 = 3;
                
                _renderVertices[quadBase + 0] = _logicalVertices[localoffset1];
                _renderVertices[quadBase + 1] = _logicalVertices[localoffset2];
                _renderVertices[quadBase + 2] = _logicalVertices[localoffset3];
                _renderVertices[quadBase + 3] = _logicalVertices[localoffset4];

                faceInfo.LogicalVertexIndices[0] = localoffset1;
                faceInfo.LogicalVertexIndices[1] = localoffset2;
                faceInfo.LogicalVertexIndices[2] = localoffset3;
                faceInfo.LogicalVertexIndices[3] = localoffset4;

                _renderUVs[quadBase + 0] = new Vector2(0, 0);
                _renderUVs[quadBase + 1] = new Vector2(1, 0);
                _renderUVs[quadBase + 2] = new Vector2(0, 1);
                _renderUVs[quadBase + 3] = new Vector2(1, 1);
            }

            break;

            case CellV2FaceDirection.North:

            

            break;

            case CellV2FaceDirection.South:

            break;

            case CellV2FaceDirection.East:

            break;

            case CellV2FaceDirection.West:

            break;
        }

        Vector3 vertexNormal = Vector3.Cross(
            _renderVertices[quadBase + 0] - _renderVertices[quadBase + 3], 
            _renderVertices[quadBase + 1] - _renderVertices[quadBase + 3]);
        vertexNormal.Normalize();

        _renderNormals[quadBase + 0] = vertexNormal;
        _renderNormals[quadBase + 1] = vertexNormal;
        _renderNormals[quadBase + 2] = vertexNormal;
        _renderNormals[quadBase + 3] = vertexNormal;

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
