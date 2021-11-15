using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPhysicsObjects : MonoBehaviour
{
    [SerializeField]
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) rb.AddForce(-transform.right * 5, ForceMode.Impulse);
        else if (Input.GetKeyDown(KeyCode.UpArrow)) rb.AddForce(transform.up * 5, ForceMode.Impulse);
        else if (Input.GetKeyDown(KeyCode.RightArrow)) rb.AddForce(transform.right * 5, ForceMode.Impulse);
        else if (Input.GetKeyDown(KeyCode.DownArrow)) rb.AddForce(-transform.up * 5, ForceMode.Impulse);
    }

    public void setRb(Rigidbody rb) {
        this.rb = rb;
    }
}
