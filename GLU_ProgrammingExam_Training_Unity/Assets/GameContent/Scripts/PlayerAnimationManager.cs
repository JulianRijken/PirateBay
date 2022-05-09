using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationManager : MonoBehaviour
{

    [SerializeField] private float _smoothSpeed;
    [SerializeField] private CharacterController _controller;
    [SerializeField] private PlayerController _playerController;

    [Header("IK")] 
    [SerializeField] private float _castHeight;
    [SerializeField] private Vector3 _footSize;
    
    [SerializeField] private Transform _lf_Bone;
    [SerializeField] private Transform _lf_KTarget;
    [SerializeField] private Vector3 _lf_PositionOffset;
    [SerializeField] private Vector3 _lf_RotationOffset;
    
    // [SerializeField] private Transform _rightFootIKBone;
    // [SerializeField] private Transform _rightFootIKTarget;
    
    private Vector3 _velocity;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _controller = GetComponentInParent<CharacterController>();
        _playerController = GetComponentInParent<PlayerController>();

        _playerController.OnDash += OnDash;
    }

    private void OnDash()
    {
        _animator.SetTrigger("Dash");
    }

    private void Update()
    {
        var targetVelocity = transform.InverseTransformDirection(_controller.velocity / _playerController.MaxSpeed);

        //_velocity = Vector3.MoveTowards(_velocity, targetVelocity, Time.deltaTime / _smoothSpeed);
        _velocity = Vector3.Lerp(_velocity, targetVelocity, Time.deltaTime / _smoothSpeed);

        _animator.SetFloat("VelocityX",_velocity.x);
        _animator.SetFloat("VelocityZ",_velocity.z);

        //HandleFootIK();
    }

    private void HandleFootIK()
    {

        var rayOrigin = _lf_Bone.transform.position;
        rayOrigin.y = transform.position.y + _castHeight;

        if (Physics.BoxCast(rayOrigin, _footSize, Vector3.down, out var hit))
        {
            
            
            Debug.DrawLine(rayOrigin, hit.point, Color.blue);




            var targetPosition = _lf_Bone.position;
            //targetPosition.y = hit.point.y + _lf_PositionOffset.y;
            _lf_KTarget.position =  targetPosition;

            
            Debug.Log(hit.normal);
            
            if(!float.IsNaN(hit.normal.x))
                _lf_KTarget.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * Quaternion.Euler(_lf_RotationOffset);
        
            // Quaternion.Euler(_lf_RotationOffset)

            //_LeftFootBone.transform.position
            
            
        }
        
        
        


    }
}
