using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootBallOnClick : MonoBehaviour
{
    [SerializeField]
    GameObject ball;

    bool mouse0Clicked;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !mouse0Clicked)
        {
            Camera cam = transform.GetComponent<Camera>();

            Ray ray = cam.ViewportPointToRay(cam.ScreenToViewportPoint(Input.mousePosition));

            GameObject ball = Instantiate(this.ball, transform.position, Quaternion.identity);

            ball.GetComponent<Rigidbody>().AddForce(ray.direction * 12.5f, ForceMode.Impulse);

            mouse0Clicked = true;
        }
        else if (Input.GetMouseButtonUp(0) && mouse0Clicked){
            mouse0Clicked = false;
        }
    }
}
