using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_Deformer : MonoBehaviour
{
    Mesh meshToDeform;
    Vector3[] restVertices, deformedVertices;

    Vector3[] vertexVelocities;

    [SerializeField]
    float springConstant = 5, particleMass = 1;
    
    [SerializeField]
    [Range(0f, 1f)]
    float damping = 0.5f;

    float uniformScale;

    // Start is called before the first frame update
    void Start()
    {
        meshToDeform = GetComponent<MeshFilter>().mesh;

        restVertices = meshToDeform.vertices;
        deformedVertices = meshToDeform.vertices;

        vertexVelocities = new Vector3[meshToDeform.vertices.Length];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        uniformScale = transform.localScale.x;  //for objects, that dynamically change size in runtime but stay the same size in all dimensions

        for (int i = 0; i < deformedVertices.Length; i++) {
            Vector3 velocity = vertexVelocities[i];

            //Spring
            Vector3 direction = deformedVertices[i] - restVertices[i];
            direction *= uniformScale;               //to adjust for scale differences
            float distance = direction.magnitude;
            direction = direction.normalized; //normalize direction here to net get any weighting from its distance

            //fs = -kx
            float springForce = -springConstant * distance;

            //source https://www.hawkindynamics.com/blog/from-force-to-velocity-what-is-this-wizardry
            //Net force = force minus body weight
            //Acceleration = net force ÷ body mass
            //Velocity = acceleration × time
            //Displacement = velocity × time

            float netForce = springForce - particleMass;
            //Debug.Log(netForce);
            float acceleration = netForce / particleMass;
            velocity += direction * acceleration * Time.fixedDeltaTime;    //here we add the direction, to have a directed velocity and not an undirected one
            velocity *= 1-damping;                                   //damping procentually
            if (velocity.magnitude < .02f) velocity = Vector3.zero;                          //let velocity die down once its slow enough
            Vector3 displacement = velocity * Time.fixedDeltaTime;

            //Storing velocity
            vertexVelocities[i] = velocity;

            //moving vertices
            deformedVertices[i] += displacement;
        }

        meshToDeform.vertices = deformedVertices;
        meshToDeform.RecalculateNormals();
    }

    public void AddForce(Vector3 point, float force)
    {
        Debug.DrawLine(transform.position, point, Color.red);

        point = transform.InverseTransformPoint(point); //transform to local space

        for (int i = 0; i < deformedVertices.Length; i++) {
            Vector3 vectorToVertex = deformedVertices[i] - point;
            vectorToVertex *= uniformScale;               //to adjust for scale differences
            float attenuatedForce = force / (1f + vectorToVertex.sqrMagnitude);
            float velocity = attenuatedForce * Time.fixedDeltaTime;
            vertexVelocities[i] += vectorToVertex.normalized * velocity;
        }
    }
}
