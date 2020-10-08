using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Material worldMaterial;

    MeshFilter meshFilter;

    Mesh mesh;

    MeshRenderer meshRenderer;

    Vector3[] vertices = new Vector3[8];
    Vector3[] normals = new Vector3[8];
    Vector2[] uv = new Vector2[8];
    int[] tris = new int[30];


    public float Height{get;set;}
    // we animate from a flat plane to its height. These variables are for tracking height 
    // information.
    float animTimeTarget;
    float animTimeCurrent;

    const float AnimHeightSpeed = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }

        if (worldMaterial == null)
        {
            meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
        }
        else
        {
            meshRenderer.sharedMaterial = worldMaterial;
        }

        meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }
    }

        //     vertices[0].x = -side;
        // vertices[0].y = 0;
        // vertices[0].z = side;

        // vertices[1].x = side;
        // vertices[1].y = 0;
        // vertices[1].z = side;

        // vertices[2].x = side;
        // vertices[2].y = 0;
        // vertices[2].z = -side;

        // vertices[3].x = -side;
        // vertices[3].y = 0;
        // vertices[3].z = -side;

        // // top
        // currentIndexBase = NumIndexesPerQuad * 0;
        // tris[currentIndexBase + 0] = 3;
        // tris[currentIndexBase + 1] = 0;
        // tris[currentIndexBase + 2] = 1;
        // tris[currentIndexBase + 3] = 2;
        // tris[currentIndexBase + 4] = 3;
        // tris[currentIndexBase + 5] = 1;

        // Vector3 vertexNormal = Vector3.Cross(vertices[0] - vertices[3], vertices[1] - vertices[3]);
        // vertexNormal.Normalize();

        // currentNormalBase = NumNormalsPerQuad * 0;
        // normals[currentNormalBase + 0] = vertexNormal;
        // normals[currentNormalBase + 1] = vertexNormal;
        // normals[currentNormalBase + 2] = vertexNormal;
        // normals[currentNormalBase + 3] = vertexNormal;

        // uv[0] = new Vector2(0, 0);
        // uv[1] = new Vector2(1, 0);
        // uv[2] = new Vector2(0, 1);
        // uv[3] = new Vector2(1, 1);

        // vertices[4].x = -side;
        // vertices[4].y = 0;
        // vertices[4].z = side;

        // vertices[5].x = side;
        // vertices[5].y = 0;
        // vertices[5].z = side;

        // vertices[6].x = side;
        // vertices[6].y = 0;
        // vertices[6].z = -side;

        // vertices[7].x = -side;
        // vertices[7].y = 0;
        // vertices[7].z = -side;



    public void MakeCube(float height, float side, float animTime = 1)
    {
        const int NumIndexesPerQuad = 6;
        int currentIndexBase = 0;

        const int NumNormalsPerQuad = 4;
        int currentNormalBase = 0;

        Height = height;
        animTimeTarget = animTime;
        animTimeCurrent = 0;

        // vertices[0].x = -side;
        // vertices[0].y = 0;
        // vertices[0].z = side;

        // vertices[1].x = side;
        // vertices[1].y = 0;
        // vertices[1].z = side;

        // vertices[2].x = side;
        // vertices[2].y = 0;
        // vertices[2].z = -side;

        // vertices[3].x = -side;
        // vertices[3].y = 0;
        // vertices[3].z = -side;

        // vertices[4].x = -side;
        // vertices[4].y = 0;
        // vertices[4].z = side;

        // vertices[5].x = side;
        // vertices[5].y = 0;
        // vertices[5].z = side;

        // vertices[6].x = side;
        // vertices[6].y = 0;
        // vertices[6].z = -side;

        // vertices[7].x = -side;
        // vertices[7].y = 0;
        // vertices[7].z = -side;

        vertices[0].x = -side;
        vertices[0].y = 0;
        vertices[0].z = side;

        vertices[1].x = side;
        vertices[1].y = 0;
        vertices[1].z = side;

        vertices[2].x = side;
        vertices[2].y = 0;
        vertices[2].z = -side;

        vertices[3].x = -side;
        vertices[3].y = 0;
        vertices[3].z = -side;

        // top
        currentIndexBase = NumIndexesPerQuad * 0;
        tris[currentIndexBase + 0] = 3;
        tris[currentIndexBase + 1] = 0;
        tris[currentIndexBase + 2] = 1;
        tris[currentIndexBase + 3] = 2;
        tris[currentIndexBase + 4] = 3;
        tris[currentIndexBase + 5] = 1;

        Vector3 vertexNormal = Vector3.Cross(vertices[0] - vertices[3], vertices[1] - vertices[3]);
        vertexNormal.Normalize();

        currentNormalBase = NumNormalsPerQuad * 0;
        normals[currentNormalBase + 0] = vertexNormal;
        normals[currentNormalBase + 1] = vertexNormal;
        normals[currentNormalBase + 2] = vertexNormal;
        normals[currentNormalBase + 3] = vertexNormal;

        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);

        vertices[4].x = -side;
        vertices[4].y = 0;
        vertices[4].z = side;

        vertices[5].x = side;
        vertices[5].y = 0;
        vertices[5].z = side;

        vertices[6].x = side;
        vertices[6].y = 0;
        vertices[6].z = -side;

        vertices[7].x = -side;
        vertices[7].y = 0;
        vertices[7].z = -side;









        // north
        currentIndexBase = NumIndexesPerQuad * 1;
        tris[currentIndexBase + 0] = 1;
        tris[currentIndexBase + 1] = 0;
        tris[currentIndexBase + 2] = 4;
        tris[currentIndexBase + 3] = 1;
        tris[currentIndexBase + 4] = 4;
        tris[currentIndexBase + 5] = 5;

        // south
        currentIndexBase = NumIndexesPerQuad * 2;
        tris[currentIndexBase + 0] = 3;
        tris[currentIndexBase + 1] = 2;
        tris[currentIndexBase + 2] = 6;
        tris[currentIndexBase + 3] = 3;
        tris[currentIndexBase + 4] = 6;
        tris[currentIndexBase + 5] = 7;

        // east
        currentIndexBase = NumIndexesPerQuad * 3;
        tris[currentIndexBase + 0] = 2;
        tris[currentIndexBase + 1] = 1;
        tris[currentIndexBase + 2] = 5;
        tris[currentIndexBase + 3] = 2;
        tris[currentIndexBase + 4] = 5;
        tris[currentIndexBase + 5] = 6;

        // west
        currentIndexBase = NumIndexesPerQuad * 4;
        tris[currentIndexBase + 0] = 3;
        tris[currentIndexBase + 1] = 4;
        tris[currentIndexBase + 2] = 0;
        tris[currentIndexBase + 3] = 4;
        tris[currentIndexBase + 4] = 3;
        tris[currentIndexBase + 5] = 7;

        normals[0] = -Vector3.forward;
        normals[1] = -Vector3.forward;
        normals[2] = -Vector3.forward;
        normals[3] = -Vector3.forward;

        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);

        mesh = new Mesh();

        mesh.vertices = vertices;
        mesh.triangles = tris;

        mesh.normals = normals;
        mesh.uv = uv;

        meshFilter = gameObject.GetComponent<MeshFilter>();
        
        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        meshFilter.mesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {
        if (animTimeCurrent <= animTimeTarget)
        {
            animTimeCurrent += Time.deltaTime;
            animTimeCurrent = Mathf.Clamp(animTimeCurrent,0,animTimeTarget);

            float currentHeight = (animTimeCurrent / animTimeTarget) * Height;

            vertices[0].y = currentHeight;
            vertices[1].y = currentHeight;
            vertices[2].y = currentHeight;
            vertices[3].y = currentHeight;

            if (mesh != null)
            {
                mesh.vertices = vertices;
            }
        }
        
    }
}