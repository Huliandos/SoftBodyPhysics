using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedVertices
{
    GameObject gameObject;
    Vector3 vertex;

    List<int> submeshIndices = new List<int>();

    public SharedVertices(GameObject go, Vector3 vert) {
        gameObject = go;
        vertex = vert;
    }

    public void AddIndex(int submeshIndex) {
        submeshIndices.Add(submeshIndex);
    }
    public List<int> GetSubmeshIndices()
    {
        return submeshIndices;
    }

    public Vector3 GetVertex() {
        return vertex;
    }

    public GameObject GetGameobject()
    {
        return gameObject;
    }

}
