using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class StrikeMarker : MonoBehaviour
{
    [SerializeField] private GameObject forceIndicator;
    public bool allowForceAdjustment = false;
    public float adjustmentRange = 2f;
    public float adjustmentSensitivity = 0.1f;

    private bool dragging = false;
    private Camera mainCamera;
    private Vector3 initialMousePosition;
    private Vector3 currentMousePosition;
    private Vector3 minimumAdjustmentPosition;
    private Vector3 maximumAdjustmentPosition;
    private Plane adjustmentPlane;
    private Plane forwardPlane;
    private Ray ray;
    private float _currentAdjustment = 0f;
    private LineRenderer lineRenderer;

    public event Action CompleteSetup;

    public float CurrentAdjustment
    {
        get { return _currentAdjustment; }
        set 
        {
            _currentAdjustment = Mathf.Max(0f, value);
            _currentAdjustment = Mathf.Min(1f, value);
        }
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void IndicateForce(float force)
    {
        forceIndicator.transform.localScale = new Vector3(
            forceIndicator.transform.localScale.x,
            forceIndicator.transform.localScale.y, 
            forceIndicator.transform.localScale.z * force
            );
    }

    public void PositionIsSet()
    {
        allowForceAdjustment = true;
        minimumAdjustmentPosition = forceIndicator.transform.position;
        maximumAdjustmentPosition = forceIndicator.transform.position + -forceIndicator.transform.forward * adjustmentRange;
        SetupLineRenderer();
    }

    private void SetupLineRenderer()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Standard"));
        lineRenderer.positionCount = 2;
        lineRenderer.SetPositions(new Vector3[2] { minimumAdjustmentPosition, maximumAdjustmentPosition } );
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.25f;

        GradientColorKey[] colorKeys = new GradientColorKey[2];
        colorKeys[0].color = Color.cyan;
        colorKeys[0].time = 0.0f;
        colorKeys[1].color = Color.red;
        colorKeys[1].time = 1.0f;

        Gradient gradient = new Gradient();
        gradient.SetKeys(colorKeys,
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) });

        lineRenderer.colorGradient = gradient;
    }

    private void Update()
    {
        if (allowForceAdjustment)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform == forceIndicator.transform)
                    {
                        dragging = true;
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
                ray = mainCamera.ScreenPointToRay(Input.mousePosition);

                if (adjustmentPlane.Raycast(ray, out enter))
                {
                    if (ray.GetPoint(enter) != currentMousePosition)
                    {
                        currentMousePosition = ray.GetPoint(enter);

                        float adjustmentMagnitude = (initialMousePosition - currentMousePosition).magnitude;
                        if (!forwardPlane.GetSide(currentMousePosition)) adjustmentMagnitude = -adjustmentMagnitude;

                        CurrentAdjustment += adjustmentMagnitude * adjustmentSensitivity;
                        print("adjustment in strikemarker: " + CurrentAdjustment);
                        forceIndicator.transform.position = Vector3.Lerp(minimumAdjustmentPosition, maximumAdjustmentPosition, CurrentAdjustment);
                    }
                }
            }

            if (Input.GetKeyUp(KeyCode.Mouse0) && dragging)
            {
                dragging = false;
                CompleteSetup();
            }
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
