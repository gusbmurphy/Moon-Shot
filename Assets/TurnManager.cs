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

    private Rigidbody[] rbs;

    private void Start()
    {
        Turn = 1;
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
        if (rbs == null) rbs = FindObjectsOfType<Rigidbody>();
        if (!Array.Exists<Rigidbody>(rbs, rb => !rb.IsSleeping() && rb.velocity.magnitude > 0.01f && rb.angularVelocity.magnitude > 0.01f))
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
