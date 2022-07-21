using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    public float speed;
    public GameObject leg;
    public float timeBetweenLegs;
    float lastLegTime = 0;

    public Vector3 move { get; private set; }

    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        //RaycastHit hit;
        //if (Physics.Raycast(transform.position, Vector3.down, out hit, 2))
        //{
        //    transform.position = hit.point + Vector3.up * 2;
        //}

        if (move != Vector3.zero)
        {
            //rb.AddForce(move.normalized * speed);

            lastLegTime += Time.deltaTime;

            if (lastLegTime >= timeBetweenLegs)
            {
                Instantiate(leg, transform);
                lastLegTime = 0;
            }
        }
    }
}
