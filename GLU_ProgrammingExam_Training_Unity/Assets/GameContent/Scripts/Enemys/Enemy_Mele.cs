using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quaternion = System.Numerics.Quaternion;

public class Enemy_Mele : Enemy
{
    [SerializeField] private float _speed;
    [SerializeField] private float _damage;
    [SerializeField] private float _attackAngle;
    [SerializeField] private float _attackDistance;
    [SerializeField] private float _rotationSpeedWhileMoving;
    [SerializeField] private float _rotationSpeedWhileStandingStill;
    [SerializeField] private Vector3 _attackBoxExtents;
    [SerializeField] private float _attackRange;

    
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

        if(_state == State.Attacking)
            Attack();
    }

    private void Attack()
    {
        var hits = Physics.BoxCastAll(Model.position, _attackBoxExtents, Model.forward,UnityEngine.Quaternion.identity,_attackRange);


        foreach (var hit in hits)
        {
            // If the hit has a collider
            if (!hit.collider)
                continue;
            
            if (!hit.collider.tag.Equals("Player"))
                continue;
        
            var attributes = hit.collider.GetComponent<Attributes>();
            attributes.ApplyHealthChange(-_damage, gameObject);
        }
    }
    
    private void UpdateEnemyState()
    {
        var player = GameManager.ActivePlayerController;
        if(!player) return;
        
        
        var playerPosition = player.Location;
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
