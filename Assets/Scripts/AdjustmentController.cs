using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This class is responsible for providing the various handles to make
 * adjustments on a target GameObject. */

public class AdjustmentController : MonoBehaviour
{
    public float xSensitivity = 1.0f;
    public float ySensitivity = 1.0f;

    public float clickTimeout = 0.2f;
    private float initialClickTime;

    public TurnManager turnManager;

    public Cue cue;

    public GameObject target;

    public float baseForce = 3000f;
    public float aimLineLength = 5f;

    public GameObject trajectoryIndicatorToPool;
    public int numOfTrajectoryIndicatorsToPool = 5;
    private List<GameObject> trajectoryIndicatorPool = new List<GameObject>();

    public LineRenderer lineRenderer;

    public float pitchResetTime = 0.5f;
    private bool isResetingPitch = false;
    private float pitchResetTimer = 0.0f;
    private float pitchResetBeginningRotation;

    [Tooltip("The distance a user will have to move the mouse to reach full " +
        "force.")]
    public float forceMaxMouseTravel = 1000f;
    private float _currentForceTravel = 0.0f;
    private bool isTrackingForce = false;
    private Vector3 initialCueModelPos;
    public float cueMaxTravelDistance = 3f;

    private float CurrentForceTravel
    {
        get { return _currentForceTravel; }
        set
        {
            if (value < 0) _currentForceTravel = 0f;
            else if (value > forceMaxMouseTravel) _currentForceTravel = forceMaxMouseTravel;
            else _currentForceTravel = value;

            cue.SetModelPositionBetweenMinMax
                (_currentForceTravel / forceMaxMouseTravel);
        }
    }

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

        cue.transform.position = target.transform.position;
        cue.transform.rotation = target.transform.rotation;

        cue.SetModelPositionBetweenMinMax(0);
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
        if (!isTrackingForce) HandleRotationInput();
        HandleForceInput();

        //if (isResetingPitch)
        //{
        //    float currentRotation;
        //    pitchResetTimer += Time.deltaTime;

        //    if (pitchResetTimer < pitchResetTime)
        //    {
        //        currentRotation = Mathf.Lerp(pitchResetBeginningRotation,
        //            0.0f, pitchResetTimer / pitchResetTime);
        //    }
        //    else
        //    {
        //        currentRotation = 0.0f;
        //        isResetingPitch = false;
        //    }

        //    cue.transform.rotation = Quaternion.Euler(
        //        currentRotation,
        //        cue.transform.rotation.y,
        //        cue.transform.rotation.z
        //        );
        //}
    }

    private void HandleForceInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isTrackingForce = true;
        }

        if (isTrackingForce && Input.GetMouseButton(0))
        {
            CurrentForceTravel += Input.GetAxis("Mouse Y");
        }

        if (isTrackingForce && Input.GetMouseButtonUp(0))
        {
            Hit();
        }
    }

    private void HandleRotationInput()
    {
        // If the user presses the RMB, we set the time it happened...
        if (Input.GetMouseButtonDown(1))
            initialClickTime = Time.time;

        // If the user releases the RMB, see if it counts as a click...
        if (Input.GetMouseButtonUp(1) &&
            Time.time - initialClickTime < clickTimeout &&
            (cue.transform.rotation.x > 0 || cue.transform.rotation.x < 0))
        {
            ResetPitch();
        }

        // If the user is holding the right click, we adjust pitch...
        if (Input.GetMouseButton(1) && !isResetingPitch)
        {
            float yInput = Input.GetAxis("Mouse Y");
            if (yInput > 0 || yInput < 0)
            {
                cue.transform.Rotate(yInput * ySensitivity, 0, 0);
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
                //cue.transform.Rotate(Vector3.up, -xInput * xSensitivity);
                cue.transform.RotateAround(target.transform.position, Vector3.up, -xInput * xSensitivity);
                cam.transform.position = cue.cameraSocket.position;
                cam.transform.LookAt(target.transform.position);
            }
        }
    }

    private void ResetPitch()
    {
        throw new NotImplementedException();
        //isResetingPitch = true;
        //pitchResetBeginningRotation = cue.transform.rotation.x;
    }

    private void ShowTrajectory(Vector3 force)
    {
        throw new NotImplementedException();
    }

    private float GetForceMagnitude()
    {
        return baseForce * (CurrentForceTravel / forceMaxMouseTravel);
    }

    private void Hit()
    {
        Vector3 forceVector = cue.transform.forward * GetForceMagnitude();

        target.GetComponent<Rigidbody>()
            .AddForce(forceVector);

        cue.SetModelPositionBetweenMinMax(0);

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
