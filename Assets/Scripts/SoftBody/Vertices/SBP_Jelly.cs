using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_Jelly : MonoBehaviour
{
    [SerializeField]
    float intensity = 1f, mass = 1f, stiffness = 1f, damping = .75f;

    //Mesh ogMesh, meshClone;
    Vector3[] restVertices, deformedVertices;
    MeshRenderer renderer;
    JellyVertex[] jv;

    // Start is called before the first frame update
    void Start()
    {
        //ogMesh = GetComponent<MeshFilter>().sharedMesh;   //sharedMesh
        //meshClone = Instantiate(ogMesh);
        //GetComponent<MeshFilter>().sharedMesh = meshClone;    //sharedMesh
        restVertices = GetComponent<MeshFilter>().mesh.vertices;
        deformedVertices = GetComponent<MeshFilter>().mesh.vertices;

        renderer = GetComponent<MeshRenderer>();

        //jv = new JellyVertex[meshClone.vertices.Length];
        //for (int i = 0; i < meshClone.vertices.Length; i++)
        //    jv[i] = new JellyVertex(i, transform.TransformPoint(meshClone.vertices[i]));

        jv = new JellyVertex[restVertices.Length];
        for (int i = 0; i < restVertices.Length; i++)
            jv[i] = new JellyVertex(i, transform.TransformPoint(restVertices[i]));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //vertices = ogMesh.vertices;

        for (int i = 0; i < jv.Length; i++) {
            //Vector3 target = transform.TransformPoint(vertices[jv[i].id]);
            Vector3 target = transform.TransformPoint(restVertices[jv[i].id]);
            float intensity = (1 - (renderer.bounds.max.y - target.y) / renderer.bounds.size.y) * this.intensity;
            jv[i].Shake(target, mass, stiffness, damping); 
            target = transform.InverseTransformPoint(jv[i].position);
            //vertices[jv[i].id] = Vector3.Lerp(vertices[jv[i].id], target, intensity);
            deformedVertices[jv[i].id] = Vector3.Lerp(deformedVertices[jv[i].id], target, intensity);
        }

        //meshClone.vertices = vertices;
        GetComponent<MeshFilter>().mesh.vertices = deformedVertices;
    }

    public class JellyVertex {
        public int id;
        public Vector3 position;
        public Vector3 velocity, force;

        public JellyVertex(int id, Vector3 position) {
            this.id = id;
            this.position = position;
        }

        public void Shake(Vector3 target, float mass, float stiffness, float damping) {
            force = (target - position) * stiffness;
            velocity = (velocity + force / mass) * damping;
            position += velocity;
            if ((velocity + force + force / mass).magnitude < .001f)
                position = target;
        }
    }
}
