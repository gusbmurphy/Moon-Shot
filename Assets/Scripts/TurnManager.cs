using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using System;


public class TurnManager : MonoBehaviour
{
    public enum TurnStage
    {
        AwaitingHit,
        AwaitingTurnCompletion,
        SettingUpNextTurn
    }

    public TurnStage _currentStage = TurnStage.AwaitingHit;
    public TurnStage CurrentStage
    {
        get { return _currentStage; }
        set
        {
            _currentStage = value;
            switch (value)
            {
                case TurnStage.SettingUpNextTurn:
                    hitController.SetCueToTurnStart();
                    shouldLerpCam = true;
                    lerpTarget = cue.cameraSocket;
                    initialCamPosition = cam.transform.position;
                    initialCamRotation = cam.transform.rotation;
                    camLerpT = 0f;
                    break;
                case TurnStage.AwaitingHit:
                    cue.gameObject.SetActive(true);
                    break;
            }
        }
    }

    public float movementThreshhold = 0.01f;
    public Text turnText;
    public Text objectivesText;
    public bool awaitingUser = true;
    public HitController hitController;
    public Text completionText;
    public Button nextLevelButton;
    public Transform camSocket;

    private Cue cue;

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
        Cursor.lockState = CursorLockMode.Locked;

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

        cue = hitController.cue;
    }

    private void FixedUpdate()
    {
        switch (CurrentStage)
        {
            case TurnStage.AwaitingTurnCompletion:
                if (!BodiesAreMoving()) EndTurn();
                break;
            case TurnStage.SettingUpNextTurn:
                LerpCamTo(lerpTarget);
                break;
        }
    }

    private float camLerpT;
    private Vector3 initialCamPosition;
    private Quaternion initialCamRotation;
    private bool shouldLerpCam = false;
    public float cameraLerpTime = 1f;

    private Transform lerpTarget;

    private void LerpCamTo(Transform target)
    {
        camLerpT += Time.deltaTime;

        float lerpCompletion = camLerpT > cameraLerpTime ? 1f :
            camLerpT < 0f ? 0f :
            camLerpT / cameraLerpTime;

        cam.transform.position = Vector3.Lerp(initialCamPosition,
            target.position, lerpCompletion);

        if (lerpCompletion >= 1f)
        {
            shouldLerpCam = false;
            CurrentStage = TurnStage.AwaitingHit;
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
            if (SceneManager.GetActiveScene().buildIndex + 1 <
                SceneManager.sceneCountInBuildSettings)
            {
                Cursor.lockState = CursorLockMode.None;
                nextLevelButton.gameObject.SetActive(true);
                completionText.gameObject.SetActive(true);
            }
            else GameFinished();
        }
    }

    private void EndTurn()
    {
        CurrentStage = TurnStage.SettingUpNextTurn;
        Turn++;
    }

    public void RestartLevel() =>
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    public void GoToNextLevel() =>
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    private void GameFinished() => throw new NotImplementedException();

    private IEnumerator CompleteLevelWithDelay()
    {
        completionText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);

        if (SceneManager.GetActiveScene().buildIndex + 1 <
            SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene
                (SceneManager.GetActiveScene().buildIndex + 1);
        }
        else GameFinished();
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
