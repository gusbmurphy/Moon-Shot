using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 0.05f;
    public float rotationSpeedHorizontal = 1f;
    public float rotationSpeedVertical = 1f;

    private Transform cameraTransform;

    private float yaw;
    private float pitch;
    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = GetComponentInChildren<Camera>().transform;
        yaw = transform.eulerAngles.y;
        pitch = cameraTransform.eulerAngles.x;
    }

    // Update is called once per frame
    void Update()
    {
        CheckForInput();
    }

    private void CheckForInput()
    {
        if (Input.GetKey(KeyCode.W)) transform.Translate(Vector3.forward * movementSpeed);
        if (Input.GetKey(KeyCode.S)) transform.Translate(Vector3.back * movementSpeed);
        if (Input.GetKey(KeyCode.A)) transform.Translate(Vector3.left * movementSpeed);
        if (Input.GetKey(KeyCode.D)) transform.Translate(Vector3.right * movementSpeed);

        yaw += rotationSpeedHorizontal * Input.GetAxis("Mouse X");
        pitch -= rotationSpeedVertical * Input.GetAxis("Mouse Y");

        transform.eulerAngles = new Vector3(0.0f, yaw, 0.0f);
        cameraTransform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }
}
