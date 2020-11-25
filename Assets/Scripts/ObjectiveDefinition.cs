using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveDefinition : MonoBehaviour
{
    public GameObject goal;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject == goal)
        {
            print("We've done it!");
        }
    }
}
