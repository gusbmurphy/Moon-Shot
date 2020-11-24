using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This class is responsible for providing the various handles to make
 * adjustments on a target GameObject. */

public class AdjustmentController : MonoBehaviour
{
    public TurnManager turnManager;

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

    public GameObject trajectoryIndicatorToPool;
    public int numOfTrajectoryIndicatorsToPool = 5;
    private List<GameObject> trajectoryIndicatorPool = new List<GameObject>();

    /* This bool represents whether or not the user has "locked" the indicator
     * to the target by clicking on it. */
    private bool indicatorLocked = false;

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;

        InstantiateIndicatorAndHandles();

        //InstantiateTrajectoryIndicators();
    }

    private void InstantiateIndicatorAndHandles()
    {
        indicator = Instantiate(indicatorToInstantiate);
        indicator.SetActive(false);

        forceHandle = Instantiate(
                forceHandleToInstantiate,
                indicator.transform.position +
                indicator.transform.forward * -0.5f,
                indicator.transform.rotation * Quaternion.Euler(0, 180f, 0)
            );
        forceHandle.transform.SetParent(indicator.transform);
        forceHandle.gameObject.SetActive(false);

        xRotationHandle = Instantiate(
                xRotationHandleToInstantiate,
                indicator.transform.position +
                indicator.transform.right * 0.5f,
                indicator.transform.rotation
            );
        xRotationHandle.objectToRotate = indicator;
        xRotationHandle.objectToRotateAround = indicator;
        xRotationHandle.transform.SetParent(indicator.transform);
        xRotationHandle.gameObject.SetActive(false);

        yRotationHandle = Instantiate(
                yRotationHandleToInstantiate,
                indicator.transform.position +
                indicator.transform.up * 0.5f,
                indicator.transform.rotation
            );
        yRotationHandle.objectToRotate = indicator;
        yRotationHandle.objectToRotateAround = indicator;
        yRotationHandle.transform.SetParent(indicator.transform);
        yRotationHandle.gameObject.SetActive(false);

        goButton = Instantiate(
                goButtonToInstantiate,
                indicator.transform.position +
                indicator.transform.up * 1f,
                indicator.transform.rotation
            );
        goButton.transform.SetParent(indicator.transform);
        goButton.gameObject.SetActive(false);
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
        if (!indicatorLocked) HandleUnlockedIndicatorInput();
        else
        {
            /* If the indicator has been locked, then we can activate the
             * handles. */
            forceHandle.gameObject.SetActive(true);
            xRotationHandle.gameObject.SetActive(true);
            yRotationHandle.gameObject.SetActive(true);
            goButton.SetActive(true);

            //CreateHandles();

            // If the user clicks the main indicator now, we hit the ball.
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.gameObject == goButton)
                    {
                        Hit();
                    }
                }
            }
        }
    }

    private void CreateHandles()
    {
        if (!forceHandle)
        {
            forceHandle = Instantiate(
                forceHandleToInstantiate,
                indicator.transform.position +
                indicator.transform.forward * -0.5f,
                indicator.transform.rotation * Quaternion.Euler(0, 180f, 0)
            );
            forceHandle.transform.SetParent(indicator.transform);
        }

        if (!xRotationHandle)
        {
            xRotationHandle = Instantiate(
                xRotationHandleToInstantiate,
                indicator.transform.position +
                indicator.transform.right * 0.5f,
                indicator.transform.rotation
            );
            xRotationHandle.objectToRotate = indicator;
            xRotationHandle.objectToRotateAround = indicator;
            xRotationHandle.transform.SetParent(indicator.transform);
        }

        if (!yRotationHandle)
        {
            yRotationHandle = Instantiate(
                yRotationHandleToInstantiate,
                indicator.transform.position +
                indicator.transform.up * 0.5f,
                indicator.transform.rotation
            );
            yRotationHandle.objectToRotate = indicator;
            yRotationHandle.objectToRotateAround = indicator;
            yRotationHandle.transform.SetParent(indicator.transform);
        }

        if (!goButton)
        {
            goButton = Instantiate(
                goButtonToInstantiate,
                indicator.transform.position +
                indicator.transform.up * 1f,
                indicator.transform.rotation
            );
        }
    }

    private void HandleUnlockedIndicatorInput()
    {
        // Check if the mouse is over the target.
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        if (Array.Exists(hits, hit => hit.collider.gameObject == target))
        {
            RaycastHit targetHit =
                Array.Find(hits, hit => hit.collider.gameObject == target);

            indicator.transform.position = targetHit.point;
            indicator.transform.LookAt(target.transform.position);
            indicator.SetActive(true);

            /* Check to see if the user has clicked, meaning we lock the 
             * indicator. */
            if (Input.GetMouseButtonDown(0))
            {
                indicatorLocked = true;
            }
        }
        else
        {
            indicator.SetActive(false);
        }
    }

    private void ShowTrajectory(Vector3 force)
    {
        throw new NotImplementedException();
    }

    private void Hit()
    {
        Vector3 forceVector =
            (indicator.transform.position - forceHandle.transform.position) *
            (forceHandle.CurrentAdjustment * baseForce);

        target.GetComponent<Rigidbody>()
            .AddForceAtPosition(forceVector, indicator.transform.position);

        HideHandles();

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
