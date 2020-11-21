using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* TODO: This class and the TranslationAdjustmentHandle share a lot of
 * functionality that could probably be shared with an interface (IDraggable)
 * or something. */

public class RotationAdjustmentHandle : MonoBehaviour
{
    public enum RotationDirection { x, y }
    public RotationDirection direction = RotationDirection.x;
    public float sensitivity = 50f;

    public GameObject objectToRotate;
    public GameObject objectToRotateAround;

    private bool isDragging = false;
    private bool isClicked = false;

    private Ray ray;
    private RaycastHit hit;

    private Plane adjustmentPlane;

    /* The adjustment vector is set from the initial click of the handle, and is
     * used to determine the direction of the intended rotation. */
    private Vector3 adjustmentVector;

    private Vector3 currentMousePos;
    private Vector3 previousMousePos;

    private void Update()
    {
        // Check to see if we're clicked if we're not already dragging...
        if (Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform)
                {
                    isClicked = true;

                    adjustmentPlane = new Plane
                    (
                        Camera.main.transform.position - transform.position,
                        transform.position
                    );

                    switch (direction)
                    {
                        case (RotationDirection.x):
                            adjustmentVector = transform.right - transform.position;
                            break;
                        case (RotationDirection.y):
                            adjustmentVector = transform.up - transform.position;
                            break;
                        default:
                            throw new Exception
                                ("Invalid direction in RotationAdjustmentHandle");
                    }

                    // Set the previous mouse position to this initial one.
                    if (adjustmentPlane.Raycast(ray, out float enter))
                    {
                        previousMousePos = ray.GetPoint(enter);
                    }
                }
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (isClicked)
            {
                isDragging = true;
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                // Get the current mouse position on the adjustment plane.
                if (adjustmentPlane.Raycast(ray, out float enter))
                {
                    currentMousePos = ray.GetPoint(enter);

                    if (currentMousePos != previousMousePos)
                        ApplyRotation(currentMousePos - previousMousePos);

                    previousMousePos = currentMousePos;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isClicked = false;
            isDragging = false;
        }
    }

    private void ApplyRotation(Vector3 change)
    {
        Vector3 projection = Vector3.Project(change, adjustmentVector);

        Vector3 axis;
        float rotation;

        switch (direction)
        {
            case (RotationDirection.x):
                axis = Vector3.up;
                rotation = projection.x;
                break;
            case (RotationDirection.y):
                axis = Vector3.right;
                rotation = projection.y;
                break;
            default:
                throw new Exception
                    ("Invalid direction in RotationAdjustmentHandle");
        }

        rotation *= sensitivity;

        objectToRotate.transform.RotateAround
            (objectToRotateAround.transform.position, axis, rotation);
    }
}
