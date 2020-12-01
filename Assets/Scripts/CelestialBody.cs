using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    private float yAxisMass = 50f;
    private float yAxisAttractionThreshold = 0.25f;
    private float yAxisAttractionMaxForce = 5f;
    private Rigidbody rb;
    private float movementThreshhold = 0.01f;

    public float attractionDistance = 5f;

    private List<CelestialBody> touchingBodies = new List<CelestialBody>();
    private Vector3 positionLastFrame;

    
    private AudioSource clackSource;
    public AudioClip[] clacks;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        positionLastFrame = transform.position;
        clackSource = GetComponent<AudioSource>();
        clacks = Resources.LoadAll<AudioClip>("Audio/Clacks");
    }

    private void FixedUpdate()
    {
        AttractBodies();
        if (transform.position.y < -yAxisAttractionThreshold || transform.position.y > yAxisAttractionThreshold) ApplyYAxisAttraction();

        if (rb.velocity.magnitude < 0.01f)
        {
            rb.velocity = new Vector3();
        }

        if (rb.angularVelocity.magnitude < 0.01f)
        {
            rb.angularVelocity = new Vector3();
        }
    }

    private void LateUpdate()
    {
        positionLastFrame = transform.position;
    }

    private void AttractBodies()
    {
        CelestialBody[] bodies = FindObjectsOfType<CelestialBody>();
        foreach (CelestialBody body in bodies)
        {
            if (body != this && (body.transform.position - transform.position).magnitude < attractionDistance)
            {
                if (!touchingBodies.Contains(body)) Attract(body.GetComponent<Rigidbody>());
            }
        }
    }

    private void ApplyYAxisAttraction()
    {
        Vector3 direction = transform.position.y < 0 ? Vector3.up : Vector3.down;
        float distance = Mathf.Abs(transform.position.y);
        float forceMagnitude = (rb.mass * yAxisMass) / Mathf.Pow(distance, 2);
        forceMagnitude = Mathf.Min(forceMagnitude, yAxisAttractionMaxForce);
        Vector3 force = direction * forceMagnitude;

        rb.AddForce(force);
    }

    // Thank you to Brackey's for this great video on implementing gravitational pull: https://www.youtube.com/watch?v=Ouu3D_VHx9o
    private void Attract(Rigidbody rbToAttract)
    {
        Vector3 direction = rb.position - rbToAttract.position;
        float distance = direction.magnitude;
        float forceMagnitude = (rb.mass * rbToAttract.mass) * distance;
        Vector3 force = direction.normalized * forceMagnitude;

        rbToAttract.AddForce(force);
    }

    private void OnCollisionEnter(Collision collision)
    {
        float speed = rb.velocity.magnitude;
        if(speed > 15.0f) {
            speed = 15.0f;
        }

        clackSource.PlayOneShot(clacks[Random.Range(0,clacks.Length)], (speed/15.0f));
        Debug.Log("Velocity = " + rb.velocity.magnitude.ToString());

        CelestialBody otherBody = collision.gameObject.GetComponent<CelestialBody>();
        if (otherBody != null)
        {
            print("Adding " + otherBody.gameObject.name + " to " + gameObject.name + "s touching list");
            touchingBodies.Add(otherBody);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        CelestialBody otherBody = collision.gameObject.GetComponent<CelestialBody>();
        if (otherBody != null)
        {
            print("Removing " + otherBody.gameObject.name + " from " + gameObject.name + "s touching list");
            touchingBodies.Remove(otherBody);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attractionDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + rb.velocity);
    }

    public bool IsMoving()
    {
        return (transform.position - positionLastFrame).magnitude < movementThreshhold;
    }
}
