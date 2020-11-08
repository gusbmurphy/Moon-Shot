﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    public static float yAxisMass = 0.1f;
    public float attractionDistance = 5f;

    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        AttractBodies();
        // if (transform.position.y < 0 || transform.position.y > 0) ApplyYAxisAttraction();
    }

    private void AttractBodies()
    {
        CelestialBody[] bodies = FindObjectsOfType<CelestialBody>();
        foreach (CelestialBody body in bodies)
        {
            if (body != this && (body.transform.position - transform.position).magnitude < attractionDistance)
            {
                Attract(body.GetComponent<Rigidbody>());
            }
        }
    }

    private void ApplyYAxisAttraction()
    {
        Vector3 direction = transform.position.y < 0 ? Vector3.up : Vector3.down;
        float distance = Mathf.Abs(transform.position.y);
        float forceMagnitude = (rb.mass * yAxisMass) / Mathf.Pow(distance, 2);
        Vector3 force = direction * forceMagnitude;

        rb.AddForce(force);
    }

    // Thank you to Brackey's for this great video on implementing gravitational pull: https://www.youtube.com/watch?v=Ouu3D_VHx9o
    private void Attract(Rigidbody rbToAttract)
    {
        Vector3 direction = rb.position - rbToAttract.position;
        float distance = direction.magnitude;
        float forceMagnitude = (rb.mass * rbToAttract.mass) / Mathf.Pow(distance, 2);
        Vector3 force = direction.normalized * forceMagnitude;

        rbToAttract.AddForce(force);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attractionDistance);
    }
}
