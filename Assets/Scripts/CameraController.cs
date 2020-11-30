using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject target;

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(target.transform.position);
    }
}
