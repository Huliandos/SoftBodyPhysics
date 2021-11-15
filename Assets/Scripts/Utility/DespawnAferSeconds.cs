using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnAferSeconds : MonoBehaviour
{
    [SerializeField]
    float seconds;

    float spawntime;

    // Start is called before the first frame update
    void Start()
    {
        spawntime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (spawntime + seconds <= Time.time) {
            Destroy(gameObject);
        }
    }
}
