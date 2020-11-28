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
            if (_isCompleted) completed.Invoke();
        }
    }

    public UnityEvent completed;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == goal) IsCompleted = true;
    }
}
