using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 moveDirection;
    public float timeToRech;
    Vector3 startPos;
    float timePassed = 0;
    
    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        timePassed += Time.deltaTime;
        float t = Mathf.PingPong(timePassed/timeToRech, 1);

        transform.position = startPos + t * moveDirection;
    }
}
