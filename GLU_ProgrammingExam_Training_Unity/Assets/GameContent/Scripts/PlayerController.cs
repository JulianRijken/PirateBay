using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    [SerializeField] private PlayerCamera _playerCamera;
    [SerializeField] private Transform _playerModelTransform;
    [SerializeField] private Weapon _weapon;
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _rotateSpeed;
    [SerializeField] private AnimationCurve _dashCurve;
    [SerializeField] private float _dashTime;
    [SerializeField] private float _dashSpeed;

    private bool _bDashing;
    private Vector3 _movementInput;
    private CharacterController _characterController;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private Vector3 _animatorSmooth;
    
    private void Update()
    {
        UpdateMovement();
        UpdateRotation();
        
        
        var target =  _playerModelTransform.InverseTransformDirection(_movementInput);
        
        Debug.DrawRay(transform.position + (Vector3.up * 2), _movementInput, Color.green);
        Debug.DrawRay(transform.position + (Vector3.up * 2), target, Color.red);

        _animatorSmooth = Vector3.Lerp(_animatorSmooth, target, Time.deltaTime * 10f);
        
        _animator.SetFloat("MovementX",_animatorSmooth.x);
        _animator.SetFloat("MovementZ",_animatorSmooth.z);

    }

    private void UpdateMovement()
    {
        if(_bDashing)
            return;
        
        Vector3 targetVelocity = _movementInput * (_walkSpeed * Time.deltaTime);
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
        if(_bDashing)
            return;
        
        StartCoroutine(DashTimeline());
    }

    private IEnumerator DashTimeline()
    {
        _bDashing = true;
        
        var dashDirection = _movementInput;
        
        var alpha = 0f;
        for (; ; )
        {
            _characterController.Move(dashDirection * (_dashCurve.Evaluate(alpha) * _dashSpeed * Time.deltaTime));
            
            if (alpha >= 1f) break;
            yield return new WaitForEndOfFrame();
            alpha = Mathf.Clamp01(alpha + (Time.deltaTime * (1.0f / _dashTime)));
        }

        _bDashing = false;

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
