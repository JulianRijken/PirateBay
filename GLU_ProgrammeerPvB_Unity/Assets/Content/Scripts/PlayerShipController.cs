using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerShipController : Ship
{
    
    private Controls _controls;

    private void Awake()
    {
        base.Awake();
        
        _controls = new Controls();
        _controls.Player.Move.performed += OnMovementInput;
        _controls.Player.Move.canceled += OnMovementInput;

        _controls.Player.Shoot.performed += OnShootInput;
    }

    
    private void Start()
    {
        _controls.Player.Enable();
    }
    
    
    
    private void OnMovementInput(InputAction.CallbackContext context)
    {
        _movementInput = context.ReadValue<Vector2>();
    }

    private void OnShootInput(InputAction.CallbackContext context)
    {
        if(!context.performed)
            return;
        
        var side = context.ReadValue<float>();
        
        FireCannons(side > 0f ? Side.Right : Side.Left);
    }
}
