using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Bezier))]
public class Leg2D : MonoBehaviour
{
    public GameObject marker;
    public float timeToReach;
    public float maxLifetime;
    public float maxLongitude;
    public float angle = 60;
    public bool hitted { get; private set; }
    float timeSinceStart = 0;
    float lifetime = 0;
    bool dying = false;

    PlayerController2D controller;
    Vector3 endPos;
    Rigidbody2D rb;
    Bezier bezier;
    RaycastHit2D hit;
    Vector3 hitPoint;

    Vector3 up1, up2;

    // Start is called before the first frame update
    void Start()
    {
        rb = transform.parent.GetComponent<Rigidbody2D>();
        bezier = GetComponent<Bezier>();

        controller = transform.parent.GetComponent<PlayerController2D>();
        hitted = false;

        Vector3 dir = controller.move;
        int tries = 0;

        endPos = Vector3.one;
        while (!hitted)
        {
            float rAngle = Mathf.Atan2(controller.move.y, controller.move.x) + Mathf.Deg2Rad * Random.Range(-angle, angle);
            dir = new Vector3(Mathf.Cos(rAngle), Mathf.Sin(rAngle)).normalized;
            // Does the ray intersect any objects excluding the player layer
            Debug.DrawRay(transform.position, dir * maxLongitude, Color.white);

            hit = Physics2D.Raycast(transform.position, dir, maxLongitude);
            if (hit.collider != null)
            {
                endPos = hit.point;
                hitted = true;
                hitPoint = hit.collider.transform.InverseTransformPoint(hit.point);
            }
            else
            {
                if (tries >= 100)
                {
                    Destroy(this.gameObject);
                    break;
                }
            }
            tries++;
        }
        if (hitted)
        {

            up1 = Vector3.up * Random.Range(-2, 2f);
            up2 = Vector3.up * Random.Range(-2, 2f);

            bezier.p1 = new GameObject();
            bezier.p1.transform.position = transform.position;
            bezier.p1.transform.parent = this.gameObject.transform;

            bezier.p2 = new GameObject();
            bezier.p2.transform.position = bezier.p1.transform.position;
            bezier.p2.transform.parent = this.gameObject.transform;
            bezier.p2.transform.Translate((endPos - bezier.p1.transform.position) / 3 + up1, Space.Self);

            bezier.p3 = new GameObject();
            bezier.p3.transform.position = bezier.p1.transform.position;
            bezier.p3.transform.parent = this.gameObject.transform;
            bezier.p3.transform.Translate(2 * (endPos - bezier.p1.transform.position) / 3 + up2, Space.Self);

            bezier.p4 = new GameObject();
            bezier.p4.transform.position = endPos;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (!dying)
        {

            if (endPos != Vector3.zero)
            {
                Debug.DrawRay(transform.position, (endPos - transform.position), Color.yellow);

                bezier.p2.transform.position = bezier.p1.transform.position;
                bezier.p2.transform.Translate((endPos - bezier.p1.transform.position) / 3 + up1, Space.Self);

                bezier.p3.transform.position = bezier.p1.transform.position;
                bezier.p3.transform.Translate(2 * (endPos - bezier.p1.transform.position) / 3 + up2, Space.Self);

                endPos = hit.collider.transform.TransformPoint(hitPoint);
                bezier.p4.transform.position = endPos;

                if (timeSinceStart <= timeToReach )
                {
                    timeSinceStart += Time.deltaTime;
                    bezier.t = timeSinceStart / timeToReach;
                }
                else
                    rb.AddForce(endPos - transform.position + new Vector3(hit.normal.x * 2, hit.normal.y, 0));

                if((endPos - transform.position).magnitude > maxLongitude)
                {
                    timeSinceStart = 0;
                    dying = true;
                }

                if (controller.move != Vector3.zero && rb.velocity != Vector2.zero)
                {
                    lifetime += Time.deltaTime;
                    if (lifetime >= maxLifetime)
                    {
                        timeSinceStart = 0;
                        dying = true;
                    }
                }
            }
        }
        else
        {
            if (timeSinceStart <= timeToReach)
            {
                timeSinceStart += Time.deltaTime;
                bezier.t = 1f - timeSinceStart / timeToReach;
            }
            else
            {
                Destroy(this.gameObject);
                Destroy(bezier.p4);
            }
        }
    }
}
