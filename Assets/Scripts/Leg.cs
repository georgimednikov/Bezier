using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Bezier))]
public class Leg : MonoBehaviour
{
    public GameObject marker;
    public float timeToReach;
    public float maxLifetime;
    public float maxLongitude;
    public bool hitted { get; private set; }
    public float force = 1;
    float timeSinceStart = 0;
    float lifetime = 0;
    bool dying = false;

    PlayerController controller;
    Vector3 endPos;
    Rigidbody rb;
    Bezier bezier;
    RaycastHit hit;
    Vector3 hitPoint;

    Vector3 up1, up2;

    void Start()
    {
        rb = transform.parent.GetComponent<Rigidbody>();
        bezier = GetComponent<Bezier>();

        controller = transform.parent.GetComponent<PlayerController>();
        hitted = false;

        Vector3 dir = controller.move;
        int tries = 0;
        endPos = Vector3.one;
        // Se busca una posicion valida en el cono
        while (!hitted)
        {
            dir = (Random.insideUnitSphere + dir * maxLongitude).normalized;
            Debug.DrawRay(transform.position, dir * maxLongitude, Color.white);
            if (Physics.Raycast(transform.position, dir, out hit, maxLongitude))
            {
                endPos = hit.point;
                hitted = true;
                hitPoint = hit.collider.transform.InverseTransformPoint(hit.point);
            }
            else
            {
                // Si despues de 100 intentos no lo ha conseguido, se destruye la pata.
                if (tries >= 100)
                {
                    Destroy(this.gameObject);
                    break;
                }
            }
            tries++;
        }

        // Si hay impacto, entonces se ponen los puntos de bezier respecto a ese punto.
        if (hitted)
        {

            up1 = Vector3.up * Random.Range(-2, 2f) + Vector3.right * Random.Range(-2, 2f);
            up2 = Vector3.up * Random.Range(-2, 2f) + Vector3.right * Random.Range(-2, 2f);

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


                if (timeSinceStart <= timeToReach)
                {
                    timeSinceStart += Time.deltaTime;
                    bezier.percentageOfCurve = timeSinceStart / timeToReach;
                }
                else
                    rb.AddForce((endPos - transform.position + hit.normal * 2) * force);

                if ((endPos - transform.position).magnitude > maxLongitude)
                {
                    timeSinceStart = 0;
                    dying = true;
                }

                if (controller.move != Vector3.zero && rb.velocity != Vector3.zero)
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
                bezier.percentageOfCurve = Mathf.Max(1f - timeSinceStart / timeToReach, 0);
            }
            else
            {
                Destroy(this.gameObject);
                Destroy(bezier.p4);
            }
        }
    }
}
