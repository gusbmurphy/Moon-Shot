﻿using System;
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

    public GameObject indicatorToInstantiate;
    private GameObject indicator;

    public TranslationAdjustmentHandle forceHandleToInstantiate;
    private TranslationAdjustmentHandle forceHandle;

    public RotationAdjustmentHandle xRotationHandleToInstantiate;
    private RotationAdjustmentHandle xRotationHandle;

    public RotationAdjustmentHandle yRotationHandleToInstantiate;
    private RotationAdjustmentHandle yRotationHandle;

    public GameObject goButtonToInstantiate;
    private GameObject goButton;

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
            target.transform.position - (indicator.transform.position - target.transform.position).normalized * aimLineLength
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

        //InstantiateIndicatorAndHandles();

        //InstantiateTrajectoryIndicators();
    }

    //private void InstantiateIndicatorAndHandles()
    //{
    //    indicator = Instantiate(indicatorToInstantiate);
    //    indicator.SetActive(false);

    //    forceHandle = Instantiate(
    //            forceHandleToInstantiate,
    //            indicator.transform.position +
    //            indicator.transform.forward * -0.5f,
    //            indicator.transform.rotation * Quaternion.Euler(0, 180f, 0)
    //        );
    //    forceHandle.transform.SetParent(indicator.transform);
    //    forceHandle.gameObject.SetActive(false);

    //    xRotationHandle = Instantiate(
    //            xRotationHandleToInstantiate,
    //            indicator.transform.position +
    //            indicator.transform.right * 0.5f,
    //            indicator.transform.rotation
    //        );
    //    xRotationHandle.objectToRotate = indicator;
    //    xRotationHandle.objectToRotateAround = target;
    //    xRotationHandle.transform.SetParent(indicator.transform);
    //    xRotationHandle.gameObject.SetActive(false);

    //    yRotationHandle = Instantiate(
    //            yRotationHandleToInstantiate,
    //            indicator.transform.position +
    //            indicator.transform.up * 0.5f,
    //            indicator.transform.rotation
    //        );
    //    yRotationHandle.objectToRotate = indicator;
    //    yRotationHandle.objectToRotateAround = target;
    //    yRotationHandle.transform.SetParent(indicator.transform);
    //    yRotationHandle.gameObject.SetActive(false);

    //    goButton = Instantiate(
    //            goButtonToInstantiate,
    //            indicator.transform.position +
    //            indicator.transform.up * 1f,
    //            indicator.transform.rotation
    //        );
    //    goButton.transform.SetParent(indicator.transform);
    //    goButton.gameObject.SetActive(false);
    //}

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

        //if (!indicatorLocked) HandleUnlockedIndicatorInput();
        //else
        //{
        //    /* If the indicator has been locked, then we can activate the
        //     * handles. */
        //    forceHandle.gameObject.SetActive(true);
        //    xRotationHandle.gameObject.SetActive(true);
        //    yRotationHandle.gameObject.SetActive(true);
        //    goButton.SetActive(true);

        //    SetAimLine(); // TODO: This shouldn't be called every frame.

        //    // If the user clicks the main indicator now, we hit the ball.
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        //        if (Physics.Raycast(ray, out RaycastHit hit))
        //        {
        //            if (hit.collider.gameObject == goButton)
        //            {
        //                Hit();
        //            }
        //        }
        //    }
        //}
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

        // If the user left clicks, hit the ball.
        if (Input.GetMouseButtonDown(0))
        {
            Hit();
        }
    }

    //private void HandleUnlockedIndicatorInput()
    //{
    //    // Check if the mouse is over the target.
    //    Ray ray = cam.ScreenPointToRay(Input.mousePosition);
    //    RaycastHit[] hits = Physics.RaycastAll(ray);

    //    if (Array.Exists(hits, hit => hit.collider.gameObject == target))
    //    {
    //        RaycastHit targetHit =
    //            Array.Find(hits, hit => hit.collider.gameObject == target);

    //        /* Move the hit position to be in the XZ-plane and on the surface of
    //         * the target. */
    //        Vector3 point = targetHit.point;
    //        point.y = target.transform.position.y;
    //        Vector3 targetToPointNormal =
    //            (point - target.transform.position).normalized;
    //        float targetRadius = target.GetComponent<SphereCollider>().radius;
    //        point = target.transform.position +
    //            (targetToPointNormal * targetRadius);

    //        indicator.transform.position = point;
    //        indicator.transform.LookAt(target.transform.position);
    //        indicator.SetActive(true);

    //        /* Check to see if the user has clicked, meaning we lock the 
    //         * indicator. */
    //        if (Input.GetMouseButtonDown(0))
    //        {
    //            indicatorLocked = true;
    //        }

    //        SetAimLine();
    //        lineRenderer.gameObject.SetActive(true);
    //    }
    //    else
    //    {
    //        //indicator.SetActive(false);
    //        lineRenderer.gameObject.SetActive(false);
    //    }
    //}

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

        //HideHandles();
        //lineRenderer.gameObject.SetActive(false);

        StartCoroutine(SetTurnManagerTimeout());
    }

    private void HideHandles()
    {
        indicator.SetActive(false);
        forceHandle.gameObject.SetActive(false);
        xRotationHandle.gameObject.SetActive(false);
        yRotationHandle.gameObject.SetActive(false);
        goButton.SetActive(false);
    }

    private IEnumerator SetTurnManagerTimeout()
    {
        turnManager.awaitingUser = false;
        yield return new WaitForSeconds(1f);
        turnManager.shouldCheckForMovement = true;
    }
}
