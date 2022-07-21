using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public GameObject leg;
    public float timeBetweenLegs;
    public Camera camera;
    float lastLegTime = 0;

    public Vector3 move { get; private set; }

    Rigidbody rb;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //Se recoge el vector de movimiento para que las patas la usen.
        move = (camera.transform.right * Input.GetAxis("Horizontal")+ camera.transform.forward * Input.GetAxis("Vertical") + camera.transform.up * Input.GetAxis("Upwards")).normalized;

        if (move != Vector3.zero)
        {
            lastLegTime += Time.deltaTime;

            if (lastLegTime >= timeBetweenLegs)
            {
                Instantiate(leg, transform);
                lastLegTime = 0;
            }
        }
    }
}
