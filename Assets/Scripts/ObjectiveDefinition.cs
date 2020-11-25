using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectiveDefinition : MonoBehaviour
{
    public GameObject goal;

    private bool _isCompleted = false;
    public bool IsCompleted => _isCompleted;

    public UnityEvent completed;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject == goal)
        {
            _isCompleted = true;
            completed.Invoke();
        }
    }
}
