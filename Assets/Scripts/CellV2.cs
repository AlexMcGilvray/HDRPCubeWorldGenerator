using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    public int[] RenderVertexIndices { get; } = new int[4]; // can be used to look up vertex, normal, uv's
    public int[] LogicalVertexIndices { get; } = new int[4];
    public Vector3 Normal { get; set; }
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

    List<int> GetAllTopRenderVertices()
    {
        List<int> _topFaceVertexIndices = new List<int>();
        var faces = _faceInfos.Select(x => x.Value);

        foreach (var face in faces)
        {
            // the top vertices are the first 4 logical vertices
            for (int i = 0; i < face.LogicalVertexIndices.Length; ++i)
            {
                if (face.LogicalVertexIndices[i] < 4)
                {
                    _topFaceVertexIndices.Add(face.RenderVertexIndices[i]);
                }
            }
        }

        return _topFaceVertexIndices;
    }

    public void MakeCell(float height, float size, float animTime = 1f)
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
        int indexBase = 0;

        const int NumEntriesPerQuad = 4;
        int quadBase = 0;

        MakeFace(CellV2FaceDirection.Up, indexBase, quadBase);
        indexBase += NumIndexesPerQuad;
        quadBase += NumEntriesPerQuad;

        MakeFace(CellV2FaceDirection.North, indexBase, quadBase);
        indexBase += NumIndexesPerQuad;
        quadBase += NumEntriesPerQuad;

        MakeFace(CellV2FaceDirection.South, indexBase, quadBase);
        indexBase += NumIndexesPerQuad;
        quadBase += NumEntriesPerQuad;

        MakeFace(CellV2FaceDirection.East, indexBase, quadBase);
        indexBase += NumIndexesPerQuad;
        quadBase += NumEntriesPerQuad;

        MakeFace(CellV2FaceDirection.West, indexBase, quadBase);

        _mesh = new Mesh();

        _mesh.vertices = _renderVertices;
        _mesh.triangles = _renderIndices;

        _mesh.normals = _renderNormals;
        _mesh.uv = _renderUVs;

        _meshFilter = gameObject.GetComponent<MeshFilter>();

        if (_meshFilter == null)
        {
            _meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        _meshFilter.mesh = _mesh;


        _heightAnimationTimeCurrent = animTime;
        _heightAnimationTimeTarget = animTime;
    }

    public void MakeFace(CellV2FaceDirection direction, int indexBase, int quadBase)
    {
        //Debug.Log("quad base is " + quadBase + " and index base is " + indexBase);
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

        switch (direction)
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

                    _renderIndices[indexBase + 0] = quadBase + 3;
                    _renderIndices[indexBase + 1] = quadBase + 0;
                    _renderIndices[indexBase + 2] = quadBase + 1;
                    _renderIndices[indexBase + 3] = quadBase + 2;
                    _renderIndices[indexBase + 4] = quadBase + 3;
                    _renderIndices[indexBase + 5] = quadBase + 1;

                    Vector3 vertexNormal = Vector3.Cross(
                        _renderVertices[quadBase + 0] - _renderVertices[quadBase + 3],
                        _renderVertices[quadBase + 1] - _renderVertices[quadBase + 3]);
                    vertexNormal.Normalize();

                    _renderNormals[quadBase + 0] = vertexNormal;
                    _renderNormals[quadBase + 1] = vertexNormal;
                    _renderNormals[quadBase + 2] = vertexNormal;
                    _renderNormals[quadBase + 3] = vertexNormal;

                    faceInfo.Normal = vertexNormal;
                }

                break;

            case CellV2FaceDirection.North:
                {
                    const int localoffset1 = 0;
                    const int localoffset2 = 1;
                    const int localoffset3 = 4;
                    const int localoffset4 = 5;

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

                    _renderIndices[indexBase + 0] = quadBase + 2;
                    _renderIndices[indexBase + 1] = quadBase + 1;
                    _renderIndices[indexBase + 2] = quadBase + 0;
                    _renderIndices[indexBase + 3] = quadBase + 2;
                    _renderIndices[indexBase + 4] = quadBase + 3;
                    _renderIndices[indexBase + 5] = quadBase + 1;

                    Vector3 vertexNormal = new Vector3(0f, 0f, 1f);

                    _renderNormals[quadBase + 0] = vertexNormal;
                    _renderNormals[quadBase + 1] = vertexNormal;
                    _renderNormals[quadBase + 2] = vertexNormal;
                    _renderNormals[quadBase + 3] = vertexNormal;

                    faceInfo.Normal = vertexNormal;
                }
                break;

            case CellV2FaceDirection.South:
                {
                    const int localoffset1 = 2;
                    const int localoffset2 = 3;
                    const int localoffset3 = 6;
                    const int localoffset4 = 7;

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

                    _renderIndices[indexBase + 0] = quadBase + 2;
                    _renderIndices[indexBase + 1] = quadBase + 1;
                    _renderIndices[indexBase + 2] = quadBase + 0;
                    _renderIndices[indexBase + 3] = quadBase + 2;
                    _renderIndices[indexBase + 4] = quadBase + 3;
                    _renderIndices[indexBase + 5] = quadBase + 1;

                    Vector3 vertexNormal = new Vector3(0f, 0f, -1f);

                    _renderNormals[quadBase + 0] = vertexNormal;
                    _renderNormals[quadBase + 1] = vertexNormal;
                    _renderNormals[quadBase + 2] = vertexNormal;
                    _renderNormals[quadBase + 3] = vertexNormal;

                    faceInfo.Normal = vertexNormal;
                }
                break;

            case CellV2FaceDirection.East:
                {
                    const int localoffset1 = 1;
                    const int localoffset2 = 2;
                    const int localoffset3 = 5;
                    const int localoffset4 = 6;

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

                    _renderIndices[indexBase + 0] = quadBase + 2;
                    _renderIndices[indexBase + 1] = quadBase + 1;
                    _renderIndices[indexBase + 2] = quadBase + 0;
                    _renderIndices[indexBase + 3] = quadBase + 2;
                    _renderIndices[indexBase + 4] = quadBase + 3;
                    _renderIndices[indexBase + 5] = quadBase + 1;

                    Vector3 vertexNormal = new Vector3(1f, 0f, 0f);

                    _renderNormals[quadBase + 0] = vertexNormal;
                    _renderNormals[quadBase + 1] = vertexNormal;
                    _renderNormals[quadBase + 2] = vertexNormal;
                    _renderNormals[quadBase + 3] = vertexNormal;

                    faceInfo.Normal = vertexNormal;
                }
                break;

            case CellV2FaceDirection.West:
                {
                    const int localoffset1 = 0;
                    const int localoffset2 = 3;
                    const int localoffset3 = 4;
                    const int localoffset4 = 7;

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

                    _renderIndices[indexBase + 0] = quadBase + 2;
                    _renderIndices[indexBase + 1] = quadBase + 0;
                    _renderIndices[indexBase + 2] = quadBase + 1;
                    _renderIndices[indexBase + 3] = quadBase + 2;
                    _renderIndices[indexBase + 4] = quadBase + 1;
                    _renderIndices[indexBase + 5] = quadBase + 3;

                    Vector3 vertexNormal = new Vector3(-1f, 0f, 0f);

                    _renderNormals[quadBase + 0] = vertexNormal;
                    _renderNormals[quadBase + 1] = vertexNormal;
                    _renderNormals[quadBase + 2] = vertexNormal;
                    _renderNormals[quadBase + 3] = vertexNormal;

                    faceInfo.Normal = vertexNormal;
                }
                break;
        }

        _faceInfos.Add(direction, faceInfo);
    }

    void Update()
    {
        if (_heightAnimationTimeCurrent > 0f)
        {
            _heightAnimationTimeCurrent -= Time.deltaTime;

            _heightAnimationTimeCurrent = Mathf.Clamp(
                _heightAnimationTimeCurrent,
                0f,
                _heightAnimationTimeTarget);

            float currentHeight =
                (1 - (_heightAnimationTimeCurrent / _heightAnimationTimeTarget)) * _height;
            //Debug.Log("currentHeight  is " + currentHeight);
            //Debug.Log("height  is " + currentHeight);

            var topRenderVertices = GetAllTopRenderVertices();
            //Debug.Log("vertex count  is " + topRenderVertices.Count);
            foreach (var renderVertexIndex in GetAllTopRenderVertices())
            {
                _renderVertices[renderVertexIndex].y = currentHeight;
            }

            if (_mesh != null)
            {
                _meshFilter.mesh.vertices = _renderVertices;
            }
        }
        else
        {
            if (_mesh != null)
            {
                _meshFilter.mesh.RecalculateBounds();
            }
        }
    }

    // logical model data
    private Vector3[] _logicalVertices = new Vector3[8];
    private Dictionary<CellV2FaceDirection, FaceInfo> _faceInfos =
        new Dictionary<CellV2FaceDirection, FaceInfo>();
    private float _height;
    // render model data
    private Vector3[] _renderVertices = new Vector3[20]; // 5 faces with 4 vertices each
    public Vector3[] _renderNormals = new Vector3[20];
    private Vector2[] _renderUVs = new Vector2[20];
    private int[] _renderIndices = new int[30]; // 5 faces * (2 triangles * 3 indices)
    private MeshFilter _meshFilter;
    // we may not actuall want to store this because it appears the meshfilter copies it
    private Mesh _mesh;
    private MeshRenderer _meshRenderer;
    //animation data
    private float _heightAnimationTimeCurrent = 0f;
    private float _heightAnimationTimeTarget = 0f;
}
