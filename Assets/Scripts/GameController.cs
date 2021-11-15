using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    GameObject[] cameras, panels, texts;

    int activeCamera = 0;

    // Start is called before the first frame update
    void Start()
    {
        cameras[activeCamera].SetActive(true);
        panels[activeCamera].SetActive(true);
        texts[activeCamera].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            cameras[activeCamera].SetActive(false);
            panels[activeCamera].SetActive(false);
            texts[activeCamera].SetActive(false);

            if (activeCamera == 0) activeCamera = cameras.Length - 1;
            else activeCamera--;

            cameras[activeCamera].SetActive(true);
            panels[activeCamera].SetActive(true);
            texts[activeCamera].SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            cameras[activeCamera].SetActive(false);
            panels[activeCamera].SetActive(false);
            texts[activeCamera].SetActive(false);

            activeCamera++;
            activeCamera %= cameras.Length;

            cameras[activeCamera].SetActive(true);
            panels[activeCamera].SetActive(true);
            texts[activeCamera].SetActive(true);
        }
    }
}
