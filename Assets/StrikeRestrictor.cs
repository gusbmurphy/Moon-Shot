using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikeRestrictor : MonoBehaviour
{
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();    
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "StopZone")
        {
            rb.velocity = Vector3.zero;
        }
    }
}
