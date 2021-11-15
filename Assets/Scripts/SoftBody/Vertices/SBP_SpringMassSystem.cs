using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_SpringMassSystem : MonoBehaviour
{
    Mesh mesh;

    [Range(.0f, 1f)]
    float elasticity;               //Not implemented yet. Should set how Rigid the Material is, with 0 being fully Rigid and 1 Fully ellastic

    //Spring
    float[] springRestLength;       //Length of the Spring when it's netiher compressed or stretched
    float[] springMinimumLengths;   //Min Spring Length when fully compressed
    float[] springMaximumLengths;   //Min Spring Length when fully compressed
    float springPullbackModifier;   //Modifier on Spring Pullbackstrength

    //Mass
    Vector3[] vertices, lastVertexPositions;
    float[] vertexMasses;                       //Maybe far away vertices have less mass, and close ones more or smth like that? Maybe the other way around

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;

        vertices = mesh.vertices;
        lastVertexPositions = mesh.vertices;

        //initialize the masses
        vertexMasses = new float[vertices.Length];
        for (int i=0; i<vertexMasses.Length; i++) {
            vertexMasses[i] = 1f;   //ToDo: Set the masses according to some operational logic
        }


        Vector3 centerOfMass = Vector3.zero;
        float totalMass = 0;
        for (int i = 0; i < vertexMasses.Length; i++)
        {
            centerOfMass += vertices[i] * vertexMasses[i];
            totalMass += vertexMasses[i];
        }
        centerOfMass /= totalMass;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
