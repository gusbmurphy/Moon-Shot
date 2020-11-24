using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using System;

public class TurnManager : MonoBehaviour
{
    public float movementThreshhold = 0.01f;
    public Text turnText;
    public bool shouldCheckForMovement = false;
    public bool awaitingUser = true;

    private int _turn;
    private int Turn
    {
        get { return _turn; }
        set
        {
            _turn = value;
            turnText.text = _turn.ToString();
        }
    }

    private CelestialBody[] bodies;

    private void Start()
    {
        Turn = 1;
        bodies = FindObjectsOfType<CelestialBody>();
    }

    private void FixedUpdate()
    {
        if (shouldCheckForMovement)
        {
            if (!BodiesAreMoving())
            {
                shouldCheckForMovement = false;
                awaitingUser = true;
                Turn++;
            }
        }
    }

    private bool BodiesAreMoving()
    {
        bool atLeastOneIsMoving = Array.Exists<CelestialBody>(bodies, body =>
        {
            Rigidbody rb = body.GetComponent<Rigidbody>();
            return rb.velocity.magnitude > movementThreshhold ||
                rb.angularVelocity.magnitude > movementThreshhold;
        });
        return atLeastOneIsMoving;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnDrawGizmos()
    {
        Rigidbody[] gizmoRbs = FindObjectsOfType<Rigidbody>();
        Gizmos.color = Color.white;

        foreach (Rigidbody rb in gizmoRbs)
        {
            Handles.Label(rb.transform.position, "velocity magnitude: " + rb.velocity.magnitude + " " + (rb.IsSleeping() ? "asleep" : "awake"));
        }
    }
}
