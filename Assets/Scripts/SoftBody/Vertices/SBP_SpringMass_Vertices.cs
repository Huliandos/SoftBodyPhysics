using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_SpringMass_Vertices : MonoBehaviour
{
    [Tooltip("Submesh to make into a Softbody. A negative value will make the whole mesh a Softbody")]
    [SerializeField]
    int submeshToSoftbody;

    Mesh mesh;
    List<SharedVertices> vertexGOs = new List<SharedVertices>();

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        if((rb = gameObject.GetComponent<Rigidbody>()) == null) rb = transform.gameObject.AddComponent<Rigidbody>();

        mesh = GetComponent<MeshFilter>().mesh;

        List<int> submeshIndices = new List<int>();

        if (submeshToSoftbody < 0)
        {
            for (int i = 0; i < GetComponent<MeshFilter>().mesh.subMeshCount; i++)
            {
                submeshIndices.AddRange(GetComponent<MeshFilter>().mesh.GetTriangles(i));
            }
        }
        else {
            submeshIndices.AddRange(GetComponent<MeshFilter>().mesh.GetTriangles(submeshToSoftbody));
        }

        List<Vector3> vertices = new List<Vector3>();
        mesh.GetVertices(vertices);

        //Find all individual Vertices and create Gameobjects for those
        for (int i = 0; i < submeshIndices.Count; i++)
        {
            foreach (SharedVertices vertexGO in vertexGOs) {
                if (vertexGO.GetVertex() == vertices[submeshIndices[i]]) {
                    vertexGO.AddIndex(submeshIndices[i]);

                    goto sharedVertexFound;
                }
            }


            GameObject go = new GameObject("Vertex " + vertexGOs.Count);

            go.transform.parent = transform;
            go.transform.position = vertices[submeshIndices[i]] + transform.position;
            SphereCollider col = go.AddComponent(typeof(SphereCollider)) as SphereCollider;
            //ToDo: Set col size according to public variables here
            col.radius = .1f;   //ToDo: Subject to change. Shouldn't be hardcoded

            go.AddComponent<Rigidbody>();

            SharedVertices sharedVertices = new SharedVertices(go, vertices[submeshIndices[i]]);

            vertexGOs.Add(sharedVertices);

            sharedVertexFound:;
        }

        //connect all triangle vertices with Joints
        //for (int i = 0; i < submeshIndices.Count; i += 3) { 

        //}
        for (int i = 0; i < vertexGOs.Count; i++) {
            SpringJoint springJoint = vertexGOs[i].GetGameobject().AddComponent<SpringJoint>();

            springJoint.connectedBody = rb;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < vertexGOs.Count; i++) {
            Vector3 newPosition = vertexGOs[i].GetGameobject().transform.position - transform.position;

            for (int j = 0; j < vertexGOs[i].GetSubmeshIndices().Count; j++)
            {
                vertices[vertexGOs[i].GetSubmeshIndices()[j]] = newPosition;
            }
        }

        mesh.vertices = vertices;
    }
}
