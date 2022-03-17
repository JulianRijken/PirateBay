using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    public Vector3 GetMouseInWorldSpace()
    {
        Camera mainCamera = Camera.main;

        if (mainCamera)
        {
            var cameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);
            var groundPlane = new Plane(Vector3.up, Vector3.zero);

            float rayLength;
            groundPlane.Raycast(cameraRay, out rayLength);

            return cameraRay.GetPoint(rayLength);
        }

        return Vector3.zero;
    }

}
