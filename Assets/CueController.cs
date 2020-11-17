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
    public GameObject cueBall;
    public StrikeMarker strikePointMarker;
    public float dragForceSensitivity = 0.1f;
    public float baseForce = 3000f;
    private StrikeMarker markerInstance = null;
    private ControlStage controlStage = ControlStage.SetPosition;

    private new Camera camera;
    private Ray ray;
    private RaycastHit hit;
    private bool isStriking = false;
    private Vector3 pointToStrike;
    private Vector3 forceVector;

    void Start()
    {
        camera = GetComponentInChildren<Camera>();
    }

    private bool MouseHasMoved()
    {
        return Input.GetAxis("Mouse X") < 0 || Input.GetAxis("Mouse X") > 0 || Input.GetAxis("Mouse Y") < 0 || Input.GetAxis("Mouse Y") > 0;
    }

    private void Strike(float force)
    {
        print("Force: " + force);
        Rigidbody cueBallRb = cueBall.GetComponentInChildren<Rigidbody>();
        cueBallRb.AddForceAtPosition(forceVector * force, pointToStrike);
        isStriking = true;
    }

    void Update()
    {
        switch (controlStage)
        {
            case ControlStage.SetPosition:
                HandlePositionInput();
                break;
            case ControlStage.SetForce:
                break;
            case ControlStage.Ready:
                HandleForceInput();
                break;
        }
    }

    private void HandlePositionInput()
    {
        if (MouseHasMoved() && !isStriking)
        {
            ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            if (Array.Exists(hits, hit => hit.collider.gameObject == cueBall))
            {
                hit = Array.Find(hits, hit => hit.collider.gameObject == cueBall);
                pointToStrike = hit.point;

                if (!markerInstance)
                {
                    markerInstance = Instantiate<StrikeMarker>(strikePointMarker);
                    markerInstance.transform.position = pointToStrike;
                    markerInstance.CompleteSetup += HandleForceSetupCompletion;
                }
                else
                {
                    markerInstance.transform.position = pointToStrike;
                }

                forceVector = (cueBall.transform.position - markerInstance.transform.position);
                markerInstance.transform.LookAt(cueBall.transform.position);
            }
            else
            {
                if (markerInstance != null) GameObject.Destroy(markerInstance.gameObject);
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && markerInstance != null)
        {
            controlStage = ControlStage.SetForce;
            markerInstance.PositionIsSet();
        }
    }

    private void HandleForceSetupCompletion()
    {
        print("Dear lord.");
        controlStage = ControlStage.Ready;
    }

    private void HandleForceInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Strike(baseForce * markerInstance.CurrentAdjustment);
        }
    }
}
