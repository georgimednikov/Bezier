using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bezier : MonoBehaviour
{
    public GameObject p1, p2, p3, p4;
    public GameObject movePoint;
    LineRenderer lineRenderer;

    [Range(2, 100)]
    public int nPoints = 100;
    Vector3[] pos;

    [Range(0, 1)]
    public float t;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, p1.transform.position);
        lineRenderer.SetPosition(1, p1.transform.position);
    }

    public void Update()
    {
        pos = new Vector3[(int)(t * nPoints) + 1];

        for (int i = 0; i < pos.Length; i++)
        {
            pos[i] = getP((float)i / nPoints);
        }
        lineRenderer.positionCount = pos.Length;
        lineRenderer.SetPositions(pos);
    }

    public Vector3 getP(float t)
    {
        return p1.transform.position * ( -Mathf.Pow(t, 3) + 3 * Mathf.Pow(t, 2) - 3 * t + 1) + 
            p2.transform.position * ( 3 * Mathf.Pow(t, 3) - 6 * Mathf.Pow(t, 2) + 3 * t) +
            p3.transform.position * ( -3 * Mathf.Pow(t, 3) + 3 * Mathf.Pow(t, 2)) +
            p4.transform.position * Mathf.Pow(t, 3);
    }
}
