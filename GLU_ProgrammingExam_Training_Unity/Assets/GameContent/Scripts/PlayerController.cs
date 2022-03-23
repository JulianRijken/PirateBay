using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    
    [SerializeField] private PlayerCamera _playerCamera;
    [SerializeField] private Transform _playerModelTransform;
    [SerializeField] private Weapon _weapon;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _rotateSpeed;
    [SerializeField] private AnimationCurve _dashCurve;
    [SerializeField] private float _dashTime;
    [SerializeField] private float _dashSpeed;
    [SerializeField] private float _dashDelay;

    private bool _bDashingAllowed = true;
    private bool _bDashing = false;
    private Vector3 _movementInput;
    private CharacterController _characterController;

    public float MaxSpeed => _maxSpeed;


    public delegate void OnDashEvent();
    public event OnDashEvent OnDash;
    
    private void Awake()
    {
        //StartCoroutine(ZeroToOne(DoABarrelRoll, StopBarrelRoll));
        _characterController = GetComponent<CharacterController>();
    }



    private void Update()
    {
        UpdateMovement();
        UpdateRotation();
    }
    

    private void UpdateMovement()
    {
        if(_bDashing)
            return;
        
        var targetVelocity = _movementInput * (_runSpeed * Time.deltaTime);
        _characterController.Move(targetVelocity);
    }


    private void UpdateRotation()
    {
        if(!_playerCamera)
            return;

        var mouseLocation = _playerCamera.GetMouseInWorldSpace();
        mouseLocation.y = 0;

        var playerPosition = transform.position;
        playerPosition.y = 0;

        var targetDirection = mouseLocation - playerPosition;
        
        var targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
        var smoothRotation = Quaternion.Slerp(_playerModelTransform.rotation, targetRotation, Time.deltaTime * _rotateSpeed);
        _playerModelTransform.rotation = smoothRotation;
    }

    private void Dash()
    {
        if (_movementInput.magnitude == 0)
            return;

        if(!_bDashingAllowed)
            return;
        
        StartCoroutine(DashTimeline());
    }

    private IEnumerator DashTimeline()
    {
        _bDashing = true;
        _bDashingAllowed = false;
        
        OnDash?.Invoke();
        
        var dashDirection = _movementInput;
        
        
        var alpha = 0f;
        while(true)
        {
            _characterController.Move(dashDirection * (_dashCurve.Evaluate(alpha) * _dashSpeed * Time.deltaTime));
            
            if (alpha >= 1f) break;
            yield return new WaitForEndOfFrame();
            alpha = Mathf.Clamp01(alpha + (Time.deltaTime * (1.0f / _dashTime)));
        }

        _bDashing = false;
        
        yield return new WaitForSeconds(_dashDelay);

        _bDashingAllowed = true;
    }
    
    
    public void OnMovementInput(InputAction.CallbackContext context)
    {
        var inputVector = context.ReadValue<Vector2>();
        inputVector.Normalize();

        _movementInput =  new Vector3(inputVector.x,0,inputVector.y);
    }

    public void OnFireInput(InputAction.CallbackContext context)
    {
        if(!context.performed)
            return;

        _weapon.Fire();
    }

    public void OnDashInput(InputAction.CallbackContext context)
    {
        if(!context.performed)
            return;

        Dash();
    }
}
