using System;
using UnityEngine;
using UnityEditor;

/* This class allows the GameObject it is attached to to be "dragged" in the
 * direction of its "forward" (Z) facing, up to a specified maximum distance. It
 * also reports the amount that it has been adjusted from its initial
 * position. */

public class TranslationAdjustmentHandle : MonoBehaviour
{
    public enum Direction { x, y, z }
    public Direction direction = Direction.z;
    public float maxAdjustmentDistance = 5f;
    public bool isAdjustable = false;

    private Vector3 maxPosition;
    private Vector3 minPosition;

    private bool isClicked = false;
    private bool isDragging = false;

    private Ray ray;
    private RaycastHit hit;

    /* The adjustmentPlane exists so that we can track the mouse's movement
     * along two dimensions. When the object is clicked, the plane is created
     * so that it goes through the objects position at that time, and is facing
     * the camera. */
    private Plane adjustmentPlane;

    /* These other two planes are used to define the bounds of where the object
     * can be positioned. */
    private Plane maxPlane;
    private Plane minPlane;

    /* These two vectors are used to store the mouse's position on the
     * adjustment plane. */
    private Vector3 currentMousePos;
    private Vector3 previousMousePos;

    public float CurrentAdjustment
    {
        get
        {
            float distFromMin = (transform.position - minPosition).magnitude;
            float maxPotentialDist = (maxPosition - minPosition).magnitude;
            return distFromMin / maxPotentialDist;
        }
    }

    private void Start()
    {
        minPosition = transform.position;

        // Create the maximum position based off of the given direction.
        switch (direction)
        {
            case Direction.x:
                maxPosition = transform.position + transform.right * maxAdjustmentDistance;
                break;
            case Direction.y:
                maxPosition = transform.position + transform.up * maxAdjustmentDistance;
                break;
            case Direction.z:
                maxPosition = transform.position + transform.forward * maxAdjustmentDistance;
                break;
            default:
                throw new Exception("Translation Adjustment Handle has invalid direction.");
        }

        minPlane = new Plane((maxPosition - minPosition), minPosition);
        maxPlane = new Plane((minPosition - maxPosition), maxPosition);
    }

    void Update()
    {
        if (isAdjustable)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform == transform)
                    {
                        isClicked = true;
                    }

                    adjustmentPlane = new Plane
                        (
                            Camera.main.transform.position - transform.position,
                            transform.position
                        );

                    // Set the previous mouse position to this initial one.
                    if (adjustmentPlane.Raycast(ray, out float enter))
                    {
                        previousMousePos = ray.GetPoint(enter);
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
                            ApplyDrag(currentMousePos - previousMousePos);

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
        else
        {
            isClicked = false;
            isDragging = false;
        }
    }

    private void ApplyDrag(Vector3 change)
    {
        /* Get a projection of the change vector onto the "rail" that we are 
         * allowing change along. */
        Vector3 projection = Vector3.Project(change, maxPosition - minPosition);
        transform.Translate(projection);

        /* Constrain to within the max and min planes. Since each of these goes
         * through one constraint and looks at the other, if the transform is
         * not on one of their positive sides, it must be beyond that
         * constraint. */
        if (!maxPlane.GetSide(transform.position))
            transform.position = maxPosition;
        else if (!minPlane.GetSide(transform.position))
            transform.position = minPosition;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(maxPosition, 0.25f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(minPosition, 0.25f);
        Gizmos.DrawLine(minPosition, maxPosition);

        if (isDragging)
        {
            Handles.Label(transform.position + new Vector3(0, 2), "Dragging...");
            Handles.Label(transform.position + new Vector3(2, 2), "currentMousePos: " + currentMousePos.ToString());

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(currentMousePos, 0.25f);
        }

        if (isClicked)
        {
            Handles.Label(transform.position, "Clicked");
        }

        Handles.Label(transform.position, "Current adjustment: " + CurrentAdjustment);
    }
}
