using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectiveDefinition : MonoBehaviour
{
    public GameObject goal;

    private bool _isCompleted = false;
    public bool IsCompleted
    {
        get { return _isCompleted; }
        set
        {
            _isCompleted = value;
            if (_isCompleted)
            {
                DisappearIntoGoal();
                completed.Invoke();
            }
        }
    }

    public UnityEvent completed;

    private bool shouldLerpToGoal = false;
    private Vector3 lerpStartPos;
    private float t = 0.0f;
    private float lerpTime = 1.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == goal) IsCompleted = true;
    }

    private void DisappearIntoGoal()
    {
        // Stop all current velocity
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.velocity = new Vector3();

        shouldLerpToGoal = true;
        lerpStartPos = transform.position;
    }

    private void FixedUpdate()
    {
        if (shouldLerpToGoal)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(
                lerpStartPos,
                goal.transform.position,
                t / lerpTime);
            if (t / lerpTime >= 1f) gameObject.SetActive(false);
        }
    }
}
