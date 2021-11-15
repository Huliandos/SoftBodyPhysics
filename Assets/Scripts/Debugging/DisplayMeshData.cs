using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayMeshData : MonoBehaviour
{
#if UNITY_EDITOR 
    public Vector3[] vertices;
    public Vector3[] normals;
    public Vector2[] uvs;
    public Vector2[] normalUvs;
    public List<int> submeshIndices;
    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh = null;
        if (GetComponent<MeshFilter>()) mesh = GetComponent<MeshFilter>().mesh;
        else if (GetComponent<SkinnedMeshRenderer>()) mesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
        else goto dontCallStart;

        vertices = mesh.vertices;
        normals = mesh.normals;
        uvs = mesh.uv;
        normalUvs = mesh.uv2;

        submeshIndices = new List<int>();
        for (int i = 0; i < mesh.subMeshCount; i++) {
            //To have a clear seperator as to where a new submesh layer starts
            submeshIndices.Add(-1);
            submeshIndices.Add(i);
            submeshIndices.Add(-1);

            for (int j = 0; j < mesh.GetTriangles(i).Length; j++)
                submeshIndices.Add(mesh.GetTriangles(i)[j]);
        }

        dontCallStart:;
    }
#endif
}
