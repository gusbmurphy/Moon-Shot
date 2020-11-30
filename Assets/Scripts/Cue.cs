using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cue : MonoBehaviour
{
    public Transform cameraSocket;
    public GameObject cueModel;
    public Transform minCuePosition;
    public Transform maxCuePosition;

    public void SetModelPositionBetweenMinMax(float f)
    {
        cueModel.transform.position =
            Vector3.Lerp(minCuePosition.position, maxCuePosition.position, f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(minCuePosition.position, 0.2f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(maxCuePosition.position, 0.2f);
    }
}
