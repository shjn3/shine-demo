using System;
using UnityEngine;

public class InputManager
{
    public Action<Vector3> pointerDownLeftCallback = (position) => { };

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            pointerDownLeftCallback.Invoke(GetWorldMousePosition());
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100f))
            {
                if (hit.transform != null)
                {

                }
            }
        }
    }

    public Vector3 GetWorldMousePosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
