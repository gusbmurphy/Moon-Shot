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
    public Text objectivesText;
    public bool shouldCheckForMovement = false;
    public bool awaitingUser = true;
    public AdjustmentController adjController;
    public Text completionText;

    private ObjectiveDefinition[] objectives;

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

        objectives = FindObjectsOfType<ObjectiveDefinition>();
        foreach (ObjectiveDefinition objective in objectives)
        {
            objective.completed.AddListener(UpdateObjectives);
        }
        UpdateObjectives();

        completionText.gameObject.SetActive(false);
    }

    private void UpdateObjectives()
    {
        String description = "";

        for (int i = 0; i < objectives.Length; i++)
        {
            ObjectiveDefinition objective = objectives[i];
            if (objective.IsCompleted) description += "X";
            description += "Get " + objective.gameObject.name +
                " to " + objective.goal.gameObject.name;
            if (i + 1 != objectives.Length) description += Environment.NewLine;
        }

        objectivesText.text = description;

        if (Array.TrueForAll<ObjectiveDefinition>(objectives,
            objective => objective.IsCompleted))
        {
            StartCoroutine(CompleteLevel());
        }
    }

    private IEnumerator CompleteLevel()
    {
        completionText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        if (SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        else
            GameFinished();
    }

    private void GameFinished()
    {
        throw new NotImplementedException();
    }

    private void FixedUpdate()
    {
        if (shouldCheckForMovement && !BodiesAreMoving()) EndTurn();
    }

    private void EndTurn()
    {
        shouldCheckForMovement = false;
        awaitingUser = true;
        Turn++;
        adjController.indicatorLocked = false;
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
