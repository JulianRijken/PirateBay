using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class PlayerCamera : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCamera;
    
    private void Awake()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    public void AttachToPlayer(PlayerController playerController)
    {
        playerController.AttachedCamera = this;
        _virtualCamera.Follow = playerController.transform;
        _virtualCamera.LookAt = playerController.transform;
    }

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
