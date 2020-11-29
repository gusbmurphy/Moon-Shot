using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This class is responsible for providing the various handles to make
 * adjustments on a target GameObject. */

public class AdjustmentController : MonoBehaviour
{
    public float xSensitivity = 1.0f;

    public TurnManager turnManager;

    public Cue cue;

    public GameObject target;

    public float baseForce = 3000f;
    public float aimLineLength = 5f;

    public GameObject trajectoryIndicatorToPool;
    public int numOfTrajectoryIndicatorsToPool = 5;
    private List<GameObject> trajectoryIndicatorPool = new List<GameObject>();

    public LineRenderer lineRenderer;

    /* This bool represents whether or not the user has "locked" the indicator
     * to the target by clicking on it. */
    public bool indicatorLocked = false;

    private Camera cam;

    private void SetAimLine()
    {
        lineRenderer.SetPositions(new Vector3[] {
            target.transform.position,
            target.transform.position - (cue.transform.position - target.transform.position).normalized * aimLineLength
            });
    }

    private void Start()
    {
        cam = Camera.main;

        lineRenderer.gameObject.SetActive(false);

        cue.transform.position = target.transform.position - target.transform.forward * target.GetComponent<SphereCollider>().radius;
        cue.transform.LookAt(target.transform.position);
        cam.transform.position = cue.cameraSocket.position;
        cam.transform.LookAt(target.transform.position);

        //InstantiateTrajectoryIndicators();
    }

    private void InstantiateTrajectoryIndicators()
    {
        for (int i = 0; i < numOfTrajectoryIndicatorsToPool; i++)
        {
            GameObject newIndicator = Instantiate(trajectoryIndicatorToPool);
            newIndicator.SetActive(false);
            trajectoryIndicatorPool.Add(newIndicator);
        }
    }

    private void Update()
    {
        HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        // If the user is holding the right click, we adjust pitch...
        if (Input.GetMouseButton(1))
        {
            float yInput = Input.GetAxis("Mouse Y");
            if (yInput > 0 || yInput < 0)
            {
                cue.transform.RotateAround(target.transform.position, cue.transform.right, yInput);
                cam.transform.position = cue.cameraSocket.position;
                cam.transform.LookAt(target.transform.position);
            }
        }
        // Otherwise we rotate...
        else
        {
            float xInput = Input.GetAxis("Mouse X");
            if (xInput > 0 || xInput < 0)
            {
                cue.transform.RotateAround(target.transform.position, Vector3.up, -xInput * xSensitivity);
                cam.transform.position = cue.cameraSocket.position;
                cam.transform.LookAt(target.transform.position);
            }
        }

        SetAimLine();

        // If the user left clicks, hit the ball.
        if (Input.GetMouseButtonDown(0))
        {
            Hit();
        }
    }

    private void ShowTrajectory(Vector3 force)
    {
        throw new NotImplementedException();
    }

    private void Hit()
    {
        Vector3 forceVector =
            (target.transform.position - cue.transform.position).normalized *
            baseForce;

        target.GetComponent<Rigidbody>()
            .AddForce(forceVector);

        lineRenderer.gameObject.SetActive(false);

        StartCoroutine(SetTurnManagerTimeout());
    }

    private IEnumerator SetTurnManagerTimeout()
    {
        turnManager.awaitingUser = false;
        yield return new WaitForSeconds(1f);
        turnManager.shouldCheckForMovement = true;
    }
}
