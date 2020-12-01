using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Color defaultColor;
    public Color activeColor;
    public Color deactivatedColor;

    public GameObject restartElement;
    public GameObject lmbElement;
    public GameObject rmbElement;

    void Start()
    {
        Canvas canvas = GetComponentInChildren<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;

        SetElementsColorTo(restartElement, defaultColor);
        SetElementsColorTo(lmbElement, defaultColor);
        SetElementsColorTo(rmbElement, defaultColor);
    }

    private void SetElementsColorTo(GameObject obj, Color color)
    {
        Text[] texts = obj.GetComponentsInChildren<Text>();
        Image[] images = obj.GetComponentsInChildren<Image>();

        foreach (Text text in texts)
        {
            text.color = color;
        }

        foreach (Image image in images)
        {
            image.color = color;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SetElementsColorTo(restartElement, activeColor);
        }
        else if (Input.GetKeyUp(KeyCode.R))
        {
            SetElementsColorTo(restartElement, defaultColor);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            SetElementsColorTo(rmbElement, activeColor);
            SetElementsColorTo(lmbElement, deactivatedColor);
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            SetElementsColorTo(rmbElement, defaultColor);
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                SetElementsColorTo(lmbElement, activeColor);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SetElementsColorTo(lmbElement, activeColor);
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            SetElementsColorTo(lmbElement, defaultColor);
        }
    }
}
