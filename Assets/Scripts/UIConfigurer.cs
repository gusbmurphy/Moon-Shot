using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIConfigurer : MonoBehaviour
{
    /* Currently, all this script is for is to ensure that the UI canvas's
     * camera is correctly set. */
    void Start()
    {
        Canvas canvas = GetComponentInChildren<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;
    }
}
