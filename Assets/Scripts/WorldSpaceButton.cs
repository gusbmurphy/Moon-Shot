using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WorldSpaceButton : MonoBehaviour
{
    public UnityEvent OnClick;

    private Collider collider;
    private bool isClicked = false;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponentInChildren<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider == collider) isClicked = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider == collider)
                {
                    if (isClicked) OnClick.Invoke();
                }
                else if (isClicked)
                {
                    isClicked = false;
                }
            }
        }
    }
}
