using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Mele : Enemy
{
    [SerializeField] private float _speed;
    [SerializeField] private float _damage;
    [SerializeField] private float _attackAngle;
    [SerializeField] private float _attackDistance;
    [SerializeField] private float _rotationSpeedWhileMoving;
    [SerializeField] private float _rotationSpeedWhileStandingStill;

    protected State _state;
    
    protected enum State
    {
        Moving,
        Rotating,
        Attacking
    }
    
    protected override void Update()
    {
        UpdateEnemyState();
        base.Update();
        Debug.Log(_state);
    }

    private void Attack()
    {
        //Physics.BoxCast()
    }
    
    
    private void UpdateEnemyState()
    {
        var playerPosition = GameManager.PlayerController.transform.position;
        var playerDirection = playerPosition - transform.position;
        
        if (playerDirection.magnitude < _attackDistance)
        {
            RotateSpeed = _rotationSpeedWhileStandingStill;
            TargetRotation = RotateTowardsOptions.Player;
            MoveSpeed = 0f;

            var angle = Vector3.Angle(playerDirection.XZ(), Model.forward.XZ());
            _state = angle < _attackAngle ? State.Attacking : State.Rotating;
        }
        else
        {
            _state = State.Moving;
            RotateSpeed = _rotationSpeedWhileMoving;
            TargetRotation = RotateTowardsOptions.Velocity;
            MoveSpeed = _speed;
        }
        
        TargetLocation = playerPosition;
    }
}
