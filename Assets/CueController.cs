using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlStage
{
    SetPosition,
    SetForce,
    Ready,
    Done
}

public class CueController : MonoBehaviour
{
    // public GameObject cue;
    public GameObject cueBall;
    public float strikeForce = 10f;
    public StrikeMarker strikePointMarker;
    private StrikeMarker markerInstance = null;
    private ControlStage controlStage = ControlStage.SetPosition;
    // public float stopZoneOffset = 0.25f;

    private new Camera camera;
    private Ray ray;
    private RaycastHit hit;
    private bool isStriking = false;
    private GameObject stopZone;
    private Vector3 pointToStrike;
    private Vector3 forceVector;
    private Vector3 initialMousePosition;
    private bool dragging = false;

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
        isStriking = true;
        //cueRb.AddForce(cue.transform.forward * strikeForce);
    }

    // Update is called once per frame
    void Update()
    {
        switch (controlStage)
        {
            case ControlStage.SetPosition:
                HandlePositionInput();
                break;
            case ControlStage.SetForce:
                HandleForceInput();
                break;
        }
    }

    private void HandlePositionInput()
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
                        //markerInstance = GameObject.Instantiate(strikePointMarker, pointToStrike, Quaternion.identity);
                        markerInstance = GameObject.Instantiate<StrikeMarker>(strikePointMarker);
                        markerInstance.transform.position = pointToStrike;
                    }
                    else
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

        if (Input.GetKeyDown(KeyCode.Mouse0) && markerInstance != null)
        {
            controlStage = ControlStage.SetForce;
        }
    }

    private void HandleForceInput()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (!dragging)
            {
                print("Hello?");
                ray = camera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform == markerInstance.transform)
                    {
                        print("Here we are");
                        dragging = true;
                        initialMousePosition = Input.mousePosition;
                        print(initialMousePosition);
                    }
                }
            }
            else
            {
                print("What's up.");
            }
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            markerInstance.IndicateForce((Input.mousePosition - initialMousePosition).magnitude);
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            dragging = false;
        }
        //if (Input.GetKeyDown(KeyCode.Mouse0) && !isStriking)
        //{
        //    if (hit.transform == cueBall.transform)
        //    {
        //        stopZone = new GameObject();
        //        SphereCollider stopZoneCollider = stopZone.AddComponent<SphereCollider>();
        //        stopZoneCollider.isTrigger = true;
        //        stopZone.transform.position = cueBall.transform.position /*+ (cue.transform.forward * stopZoneOffset)*/;
        //        stopZone.transform.localScale = cueBall.transform.localScale;
        //        stopZone.tag = "StopZone";
        //        Strike();
        //    }
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == stopZone.GetComponent<Collider>())
        {
            print("In the stop zone.");
        }
    }
}
