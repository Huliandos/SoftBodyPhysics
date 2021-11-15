using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetObjects : MonoBehaviour
{
    [SerializeField]
    bool instantiateFromPrefab = false;

    [SerializeField]
    GameObject[] GOs;

    [SerializeField]
    GameObject[] Prefabs;

    [SerializeField]
    Vector3[] resetPos;

    [SerializeField]
    ControlPhysicsObjects controlPhysicsObjects;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            for (int i=0; i<GOs.Length; i++)
            {
                Transform parent = GOs[i].transform.parent;

                GameObject instantiatedGo;
                if (instantiateFromPrefab) instantiatedGo = Instantiate(Prefabs[i], resetPos[i], Quaternion.identity);
                else instantiatedGo = Instantiate(GOs[i], resetPos[i], Quaternion.identity);
                Destroy(GOs[i]);

                GOs[i] = instantiatedGo;
                instantiatedGo.transform.parent = parent;

                if (controlPhysicsObjects) controlPhysicsObjects.setRb(instantiatedGo.GetComponent<Rigidbody>());
            }
        }
    }
}
