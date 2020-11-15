using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class StrikeMarker : MonoBehaviour
{
    [SerializeField] private GameObject forceIndicator;
    public bool allowForceAdjustment = false;
    public float adjustmentRange = 2f;

    private bool dragging = false;
    private Camera camera;
    private Vector3 initialMousePosition;
    private Vector3 currentMousePosition;
    private Vector3 minimumAdjustmentPosition;
    private Vector3 maximumAdjustmentPosition;
    private Vector3 initialForceIndicatorPosition;
    private Plane adjustmentPlane;
    private Plane forwardPlane;
    private Vector3 hitPoint;
    private Ray ray;
    private float currentAdjustment = 0f;

    public void IndicateForce(float force)
    {
        forceIndicator.transform.localScale = new Vector3(forceIndicator.transform.localScale.x, forceIndicator.transform.localScale.y, forceIndicator.transform.localScale.z * force);
    }

    private void Start()
    {
        print("Start!");
        camera = Camera.main;
    }

    private void Awake()
    {
        print("Awake!");
    }

    private void Update()
    {
        if (allowForceAdjustment)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                minimumAdjustmentPosition = forceIndicator.transform.position;
                maximumAdjustmentPosition = forceIndicator.transform.position + -forceIndicator.transform.forward * adjustmentRange;
                ray = camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform == forceIndicator.transform)
                    {
                        dragging = true;
                        initialForceIndicatorPosition = forceIndicator.transform.position;
                        adjustmentPlane = new Plane(forceIndicator.transform.up, forceIndicator.transform.position);
                        forwardPlane = new Plane(-forceIndicator.transform.forward, forceIndicator.transform.position);

                        float enter;

                        if (adjustmentPlane.Raycast(ray, out enter))
                        {
                            initialMousePosition = ray.GetPoint(enter);
                        }
                    }
                }
            }

            if (Input.GetKey(KeyCode.Mouse0) && dragging)
            {
                float enter;
                ray = camera.ScreenPointToRay(Input.mousePosition);

                if (adjustmentPlane.Raycast(ray, out enter))
                {
                    if (ray.GetPoint(enter) != currentMousePosition)
                    {
                        currentMousePosition = ray.GetPoint(enter);

                        float adjustmentMagnitude = (initialMousePosition - currentMousePosition).magnitude;
                        if (!forwardPlane.GetSide(currentMousePosition)) adjustmentMagnitude = -adjustmentMagnitude;

                        currentAdjustment += adjustmentMagnitude;
                        currentAdjustment = Mathf.Max(0f, currentAdjustment);
                        currentAdjustment = Mathf.Min(1f, currentAdjustment);

                        forceIndicator.transform.position = Vector3.Lerp(minimumAdjustmentPosition, maximumAdjustmentPosition, currentAdjustment);
                        //forceIndicator.transform.position = initialForceIndicatorPosition + -forceIndicator.transform.forward * adjustmentMagnitude;
                    }
                }
            }

            if (Input.GetKeyUp(KeyCode.Mouse0)) dragging = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (dragging)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(initialMousePosition, 0.25f);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(currentMousePosition, 0.25f);
            Handles.Label(currentMousePosition, "Difference magnitude: " + (initialMousePosition - currentMousePosition).magnitude);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(minimumAdjustmentPosition, 0.5f);
        Gizmos.DrawWireSphere(maximumAdjustmentPosition, 0.5f);
    }
}
