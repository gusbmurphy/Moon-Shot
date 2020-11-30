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
    public Button nextLevelButton;
    public Transform camSocket;

    private ObjectiveDefinition[] objectives;
    private Camera cam;

    private GameObject cueBall;

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
        nextLevelButton.gameObject.SetActive(false);

        cam = Camera.main;
        cueBall = GameObject.FindGameObjectWithTag("CueBall");
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
            if (SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings)
            {
                nextLevelButton.gameObject.SetActive(true);
                completionText.gameObject.SetActive(true);
            }
            else GameFinished();
        }
    }

    private IEnumerator CompleteLevelWithDelay()
    {
        completionText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        if (SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        else
            GameFinished();
    }

    public void GoToNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void GameFinished()
    {
        throw new NotImplementedException();
    }

    private void FixedUpdate()
    {
        if (shouldCheckForMovement && !BodiesAreMoving()) EndTurn();

        if (!awaitingUser)
        {
            if (!shouldLerpCam) MoveCameraToObserve();
            else LerpCamTo(camSocket);
        }
    }

    private float camLerpT;
    private Vector3 initialCamPosition;
    private Quaternion initialCamRotation;
    private bool shouldLerpCam = false;
    public float cameraLerpTime = 1f;

    private void MoveCameraToObserve()
    {
        shouldLerpCam = true;
        camLerpT = 0.0f;
        initialCamPosition = cam.transform.position;
        initialCamRotation = cam.transform.rotation;
    }

    private void LerpCamTo(Transform target)
    {
        camLerpT += Time.deltaTime;

        float lerpCompletion = camLerpT > cameraLerpTime ? 1f :
            camLerpT < 0f ? 0f :
            camLerpT / cameraLerpTime;

        cam.transform.position = Vector3.Lerp(initialCamPosition,
            target.position, lerpCompletion);
        cam.transform.LookAt(cueBall.transform.position);

        if (lerpCompletion > 1f)
        {
            shouldLerpCam = false;
        }
    }

    private void EndTurn()
    {
        shouldCheckForMovement = false;
        awaitingUser = true;
        Turn++;
        //adjController.indicatorLocked = false;
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
