using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeformerInputManager : MonoBehaviour
{
    [SerializeField]
    float force = 10f;

    [SerializeField]
    float forceOffset = .1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0)) {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)) {
                SBP_Deformer deformer = hit.collider.GetComponent<SBP_Deformer>();
                if (deformer) {
                    Vector3 point = hit.point;
                    point += hit.normal * forceOffset;
                    deformer.AddForce(point, force);
                }
            }
        }
    }
}
