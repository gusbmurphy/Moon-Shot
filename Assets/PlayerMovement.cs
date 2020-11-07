using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 0.05f;
    public float rotationSpeed = 0.05f;
    // Start is called before the first frame update
    void Start()
    {

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
    }
}
