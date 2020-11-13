using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CueController : MonoBehaviour
{
    // public GameObject cue;
    public GameObject cueBall;
    public float strikeForce = 10f;
    public GameObject strikePointMarker;
    private GameObject markerInstance = null;
    // public float stopZoneOffset = 0.25f;

    private new Camera camera;
    private Ray ray;
    private RaycastHit hit;
    private bool isStriking = false;
    private GameObject stopZone;
    private Vector3 pointToStrike;
    private Vector3 forceVector;

    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponentInChildren<Camera>();
    }

    private bool MouseHasMoved()
    {
        return Input.GetAxis("Mouse X") < 0 || Input.GetAxis("Mouse X") > 0 || Input.GetAxis("Mouse Y") < 0 || Input.GetAxis("Mouse Y") > 0;
    }

    private void Strike()
    {
        Rigidbody cueBallRb = cueBall.GetComponentInChildren<Rigidbody>();
        cueBallRb.AddForceAtPosition(forceVector, pointToStrike);
        //cueRb.AddForce(cue.transform.forward * strikeForce);
    }

    // Update is called once per frame
    void Update()
    {
        if (MouseHasMoved() && !isStriking)
        {
            ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == cueBall.transform)
                {
                    pointToStrike = hit.point;

                    //// "Lock" the point to strike into the XZ-plane
                    //pointToStrike.y = 0f;
                    //if((pointToStrike - cueBall.transform.position).magnitude < cueBall.GetComponent<SphereCollider>().radius)
                    //{
                    //    pointToStrike = (pointToStrike - cueBall.transform.position).normalized * cueBall.GetComponent<SphereCollider>().radius;
                    //}

                    if (!markerInstance)
                    {
                        markerInstance = GameObject.Instantiate(strikePointMarker, pointToStrike, Quaternion.identity);
                    } else
                    {
                        markerInstance.transform.position = pointToStrike;
                    }

                    forceVector = (pointToStrike - cueBall.transform.position) * -strikeForce;
                    markerInstance.transform.LookAt(cueBall.transform.position);
                    //cue.transform.position = hit.point;
                    //cue.transform.LookAt(cueBall.transform.position);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && !isStriking)
        {
            if (hit.transform == cueBall.transform)
            {
                stopZone = new GameObject();
                SphereCollider stopZoneCollider = stopZone.AddComponent<SphereCollider>();
                stopZoneCollider.isTrigger = true;
                stopZone.transform.position = cueBall.transform.position /*+ (cue.transform.forward * stopZoneOffset)*/;
                stopZone.transform.localScale = cueBall.transform.localScale;
                stopZone.tag = "StopZone";
                Strike();
                isStriking = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == stopZone.GetComponent<Collider>())
        {
            print("In the stop zone.");
        }
    }
}
