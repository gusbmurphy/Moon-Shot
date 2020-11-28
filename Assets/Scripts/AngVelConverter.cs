using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngVelConverter : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 spin;
    public float spinFactor = 0.01f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();   
    }

    private void FixedUpdate()
    {
        Vector3 angVelocity = rb.angularVelocity;
        spin = -angVelocity;
        rb.velocity += spin * spinFactor;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + spin);
    }
}
