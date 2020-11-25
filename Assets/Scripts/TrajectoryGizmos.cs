using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryGizmos : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + rb.velocity);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + rb.angularVelocity);
    }
}
