using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerShipController : Ship
{
    
    private Controls _controls;
    private Vector2 _movementInput;
    
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _shipWobble;
    [SerializeField] private float _shipUnflipAngle = 50f;
    [SerializeField] private Vector2 _shipUnflipForce;
    [SerializeField] private BuoyancyEffector3D _buoyancyEffector3D;

    private Rigidbody _rigidbody;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        
        _controls = new Controls();
        _controls.Player.Move.performed += OnMovementInput;
        _controls.Player.Move.canceled += OnMovementInput;
       
    }

    
    private void Start()
    {
        _controls.Player.Enable();
    }
    
    
    private void FixedUpdate()
    {
        HandleUnFlippingShip();
        HandleShipMovement();
    }

    private void HandleShipMovement()
    {
        if (!_buoyancyEffector3D.IsUnderWater)
            return;

        // Move ship
        var direction = transform.forward * _movementInput.y;
        direction.y = 0;
        _rigidbody.AddForce(direction * _moveSpeed, ForceMode.Acceleration);
    
        // Rotate Ship    
        _rigidbody.AddTorque(Vector3.up * _rotationSpeed * _movementInput.x, ForceMode.Acceleration);
        _rigidbody.AddTorque(transform.forward * _shipWobble * _movementInput.x, ForceMode.Acceleration);
    }

    private void HandleUnFlippingShip()
    {
        var flippedAngle = (1f - (transform.up.y + 1f) / 2f) * 360;
        var unFlipForce =  Mathf.Max(0,(flippedAngle - _shipUnflipAngle) / (360 - _shipUnflipAngle));
        
        if (unFlipForce > 0)
        {
            var targetRotation = Quaternion.Slerp(_rigidbody.rotation, Quaternion.identity, Time.deltaTime * Mathf.Lerp(_shipUnflipForce.y,_shipUnflipForce.x, unFlipForce));
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.MoveRotation(targetRotation);
        }
    }
    
    
    private void OnMovementInput(InputAction.CallbackContext context)
    {
        _movementInput = context.ReadValue<Vector2>();
    }
}
