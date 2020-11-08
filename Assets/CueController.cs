using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CueController : MonoBehaviour
{
    public GameObject cue;
    public GameObject cueBall;
    public float strikeForce = 10f;

    private new Camera camera;
    private Ray ray;
    private RaycastHit hit;
    private bool isStriking = false;
    private GameObject stopZone;

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
        Rigidbody cueRb = cue.GetComponentInChildren<Rigidbody>();
        cueRb.AddForce(cue.transform.forward * strikeForce);
    }

    // Update is called once per frame
    void Update()
    {
        if (MouseHasMoved() || !isStriking)
        {
            ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == cueBall.transform)
                {
                    cue.transform.position = hit.point;
                    cue.transform.LookAt(cueBall.transform.position);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (hit.transform == cueBall.transform)
            {
                stopZone = new GameObject();
                SphereCollider stopZoneCollider = stopZone.AddComponent<SphereCollider>();
                stopZoneCollider.isTrigger = true;
                stopZone.transform.position = cueBall.transform.position;
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
