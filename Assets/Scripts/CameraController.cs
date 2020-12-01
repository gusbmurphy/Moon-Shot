using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject cueBall;
    private TurnManager turnManager;

    private Camera cam;

    public float hSensitivity = 1f;
    public float vSensitivity = 1f;

    private float pitch;
    private float yaw;

    private void Start()
    {
        cueBall = GameObject.FindGameObjectWithTag("CueBall");
        turnManager = GameObject.FindGameObjectWithTag("TurnManager")
            .GetComponent<TurnManager>();
        cam = GetComponentInChildren<Camera>();
    }

    void LateUpdate()
    {
        switch (turnManager.CurrentStage)
        {
            case TurnManager.TurnStage.AwaitingTurnCompletion:
                //HandleUserLook();
                break;
            default:
                cam.transform.LookAt(cueBall.transform.position);
                yaw = transform.eulerAngles.y;
                pitch = cam.transform.eulerAngles.x;
                break;
        }

    }

    private void HandleUserLook()
    {
        yaw += hSensitivity * Input.GetAxis("Mouse X");
        pitch -= vSensitivity * Input.GetAxis("Mouse Y");

        transform.eulerAngles = new Vector3(0.0f, yaw, 0.0f);
        cam.transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }
}
