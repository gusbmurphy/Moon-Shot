using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonSpinner : MonoBehaviour
{
    public float degreesPerSecond = 3f;

    void Update()
    {
        transform.Rotate(0, degreesPerSecond * Time.deltaTime, 0);
    }
}
