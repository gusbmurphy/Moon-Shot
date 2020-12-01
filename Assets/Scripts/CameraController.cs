using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject cueBall;

    private void Start()
    {
        cueBall = GameObject.FindGameObjectWithTag("CueBall");
    }

    void LateUpdate()
    {
        transform.LookAt(cueBall.transform.position);
    }
}
