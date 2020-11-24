using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using System;

public class TurnManager : MonoBehaviour
{
    public Text turnText;
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
        if (!awaitingUser)
        {
            CheckForEndOfTurn();
        }
    }

    private void CheckForEndOfTurn()
    {
        if (Array.Exists(bodies, body => body.GetComponent<Rigidbody>().velocity.magnitude > 0f || body.GetComponent<Rigidbody>().angularVelocity.magnitude > 0f) == false)
        {
            print("Turn complete!");
            awaitingUser = true;
            Turn++;
        }
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
