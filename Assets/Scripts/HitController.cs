using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This class is responsible for providing the various handles to make
 * adjustments on a target GameObject. */

public class HitController : MonoBehaviour
{
    public float xSensitivity = 1.0f;
    public float ySensitivity = 1.0f;

    public float clickTimeout = 0.2f;
    private float initialClickTime;

    private TurnManager turnManager;

    public Cue cueToInstantiate;
    private Cue cue;
    public Cue GetCue() => cue;

    private GameObject cueBall;

    public float baseForce = 3000f;
    public float aimLineMaxLength = 5f;

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

    private AudioSource audioSource;
    private AudioClip cueStrike;

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

    private GameObject camArm;

    private void SetAimLine()
    {
        float lineLength = (CurrentForceTravel / forceMaxMouseTravel)
            * aimLineMaxLength;

        Vector3 lineStart = cueBall.transform.position +
            cue.transform.forward * 0.7f;

        Vector3 lineEnd = lineStart + cue.transform.forward * lineLength;

        lineRenderer.SetPositions(new Vector3[] { lineStart, lineEnd });

        lineRenderer.gameObject.SetActive(true);
    }

    private void Start()
    {
        cueBall = GameObject.FindGameObjectWithTag("CueBall");

        turnManager = GameObject.FindGameObjectWithTag("TurnManager")
            .GetComponent<TurnManager>();

        cue = Instantiate(cueToInstantiate);

        camArm = GameObject.FindGameObjectWithTag("CameraArm");

        SetCueToTurnStart();
        camArm.transform.position = cue.cameraSocket.position;

        lineRenderer.gameObject.SetActive(false);
        //InstantiateTrajectoryIndicators();

        audioSource = GameObject.FindGameObjectWithTag("CueBall")
            .GetComponent<AudioSource>();
        cueStrike = Resources.Load<AudioClip>("Audio/cueStrike");
    }

    public void SetCueToTurnStart()
    {
        cue.transform.position = cueBall.transform.position;
        Vector3 cueRotation = cueBall.transform.rotation.eulerAngles;
        cueRotation.x = 0.0f;
        cueRotation.z = 0.0f;
        cue.transform.eulerAngles = cueRotation;

        cue.SetModelPositionBetweenMinMax(0);
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
        if (turnManager.CurrentStage == TurnManager.TurnStage.AwaitingHit)
        {
            HandleRotationInput();
            HandleForceInput();
        }

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

    private bool isAdjustingPitch = false;

    private void HandleForceInput()
    {
        if (Input.GetMouseButtonDown(0) && !isAdjustingPitch)
        {
            isTrackingForce = true;
        }

        if (isTrackingForce && Input.GetMouseButton(0))
        {
            CurrentForceTravel -= Input.GetAxis("Mouse Y");

            if (GetForceMagnitude() > 0) SetAimLine();
            else lineRenderer.gameObject.SetActive(false);
        }

        if (isTrackingForce && Input.GetMouseButtonUp(0))
        {
            if (GetForceMagnitude() > 0)
            {
                Hit();
            }

            isTrackingForce = false;
            CurrentForceTravel = 0f;
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
            //ResetPitch();
        }

        // If the user is holding the right click, we adjust pitch...
        if (Input.GetMouseButton(1) && !isResetingPitch)
        {
            isAdjustingPitch = true;
            float yInput = Input.GetAxis("Mouse Y");
            if (yInput > 0 || yInput < 0)
            {
                float yRotation = yInput * ySensitivity;
                cue.transform.Rotate(yRotation, 0, 0);
                camArm.transform.position = cue.cameraSocket.position;
            }
        }
        // Otherwise we rotate...
        else
        {
            float xInput = Input.GetAxis("Mouse X");
            if (xInput > 0 || xInput < 0)
            {
                //cue.transform.Rotate(Vector3.up, -xInput * xSensitivity);
                cue.transform.RotateAround(cueBall.transform.position, Vector3.up, -xInput * xSensitivity);
                camArm.transform.position = cue.cameraSocket.position;
            }
        }

        // If the user has released RMB we need to stop adjusting pitch
        if (Input.GetMouseButtonUp(1) && isAdjustingPitch)
        {
            isAdjustingPitch = false;
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
        audioSource.PlayOneShot(cueStrike, 1);

        Vector3 forceVector = cue.transform.forward * GetForceMagnitude();

        cueBall.GetComponent<Rigidbody>()
            .AddForce(forceVector);

        cue.SetModelPositionBetweenMinMax(0);

        lineRenderer.gameObject.SetActive(false);

        StartCoroutine(SetTurnManagerTimeout());

    }

    private IEnumerator SetTurnManagerTimeout()
    {
        cue.gameObject.SetActive(false); // Hide the cue.

        /* Wait a moment, then start to check if we can end the turn because
         * no objects are moving. */
        yield return new WaitForSeconds(1f);
        turnManager._currentStage = TurnManager.TurnStage.AwaitingTurnCompletion;
        //turnManager.shouldCheckForMovement = true;
    }
}
