using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikeMarker : MonoBehaviour
{
    [SerializeField] private GameObject forceIndicator;

    public void IndicateForce(float force)
    {
        print("Indicating force: " + force);
    }
}
