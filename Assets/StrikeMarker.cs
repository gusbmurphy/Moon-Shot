using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class StrikeMarker : MonoBehaviour
{
    [SerializeField] private TranslationAdjustmentHandle forceIndicator;
    private TranslationAdjustmentHandle forceIndicatorInstance;

    public event Action CompleteSetup;

    public void SetPosition()
    {
        forceIndicatorInstance = Instantiate(forceIndicator, transform.position + -transform.forward * 0.5f, transform.rotation, transform);
        //forceIndicatorInstance.transform.LookAt(transform.position);
    }
}
