using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{

    [SerializeField] private CharacterController _controller;
    [SerializeField] private Animator _animator;
    [SerializeField] private Vector3 _velocity;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private float _smoothSpeed;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _controller = GetComponentInParent<CharacterController>();
        _playerController = GetComponentInParent<PlayerController>();

        _playerController.OnDash += OnDash;
    }

    private void OnDash()
    {
        //Debug.Log("Dash");

        _animator.SetTrigger("Dash");
        
    }

    private void Update()
    {
        //var target =  _playerModelTransform.InverseTransformDirection(_movementInput);
        
        //Debug.DrawRay(transform.position + (Vector3.up * 2), _movementInput, Color.green);
        //Debug.DrawRay(transform.position + (Vector3.up * 2), target, Color.red);

        //_animatorSmooth = Vector3.Lerp(_animatorSmooth, target, Time.deltaTime * 10f);
        
        //_animator.SetFloat("MovementX",_animatorSmooth.x);
        //_animator.SetFloat("MovementZ",_animatorSmooth.z);
        
        
        
        var targetVelocity = transform.InverseTransformDirection(_controller.velocity / _playerController.MaxSpeed);

        //_velocity = Vector3.MoveTowards(_velocity, targetVelocity, Time.deltaTime / _smoothSpeed);
        _velocity = Vector3.Lerp(_velocity, targetVelocity, Time.deltaTime * _smoothSpeed);

        
        _animator.SetFloat("VelocityX",_velocity.x);
        _animator.SetFloat("VelocityZ",_velocity.z);
    }
}
