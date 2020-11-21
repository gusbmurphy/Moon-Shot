using System;
using UnityEngine;

/* This class is responsible for providing the various handles to make
 * adjustments on a target GameObject. */

public class AdjustmentController : MonoBehaviour
{
    public GameObject target;

    public GameObject indicatorToInstantiate;
    private GameObject indicator;
    public TranslationAdjustmentHandle handleToInstantiate;
    private TranslationAdjustmentHandle handle;

    public float baseForce = 3000f;

    /* This bool represents whether or not the user has "locked" the indicator
     * to the target by clicking on it. */
    private bool indicatorLocked = false;

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (!indicatorLocked) HandleUnlockedIndicatorInput();
        else
        {
            /* If the indicator has been locked, then we can instantiate the
             * handle. */
            if (!handle) handle = Instantiate(
                    handleToInstantiate,
                    indicator.transform.position +
                    indicator.transform.forward * -0.5f,
                    indicator.transform.rotation * Quaternion.Euler(0, 180f, 0)
                );

            // If the user clicks the main indicator now, we hit the ball.
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.gameObject == indicator)
                    {
                        Hit();
                    }
                }
            }
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

            if (!indicator) indicator = Instantiate(indicatorToInstantiate);
            indicator.transform.position = targetHit.point;
            indicator.transform.LookAt(target.transform.position);

            /* Check to see if the user has clicked, meaning we lock the 
             * indicator. */
            if (Input.GetMouseButtonDown(0))
            {
                indicatorLocked = true;
            }
        }
        else
        {
            if (indicator != null) GameObject.Destroy(indicator.gameObject);
        }
    }

    private void Hit()
    {
        Vector3 forceVector =
            (target.transform.position - indicator.transform.position) *
            (handle.CurrentAdjustment * baseForce);
        target.GetComponent<Rigidbody>().AddForce(forceVector);
    }
}
