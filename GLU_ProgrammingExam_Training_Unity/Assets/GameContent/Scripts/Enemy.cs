using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    [SerializeField] private float _damage;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _attackDistance;
    [SerializeField] private Transform _model;
    [SerializeField] private float _angularSpeedForMoving;
    [SerializeField] private float _angularSpeedForStandingStill;
    [SerializeField] private float _attackAngle;

    private EnemyState _state;
    private Quaternion _targetRotation;
    
    private Attributes _attributes;
    private NavMeshAgent _agent;

    public delegate void OnRemovedEvent(Enemy enemyRemoved);
    public event OnRemovedEvent OnRemoved;
    
    
    private void Awake()
    {
        _attributes = GetComponent<Attributes>();
        _agent = GetComponent<NavMeshAgent>();

        _attributes.OnDeath += OnDeath;
    }

    private void OnDeath(GameObject instigator)
    {
        OnRemoved?.Invoke(this);
        Destroy(gameObject);
    }

    private void Update()
    {
        UpdateEnemyState();
        RotateEnemy();
    }

    private enum EnemyState
    {
        Moving,
        Rotating,
        Attacking
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
            _agent.speed = 0f;

            var angle = Vector3.Angle(playerDirection.XZ(), _model.forward.XZ());
            _state = angle < _attackAngle ? EnemyState.Attacking : EnemyState.Rotating;
        }
        else
        {
            _state = EnemyState.Moving;
            _agent.speed = _moveSpeed;
        }

            
        _agent.SetDestination(playerPosition);
    }

    private void RotateEnemy()
    {
        // Get target rotation
        var playerDirection = GameManager.PlayerController.transform.position - _model.position;
        var dir = _state == EnemyState.Moving ? _agent.velocity : playerDirection;

        if(dir.XZ().magnitude > 0f)
            _targetRotation = Quaternion.LookRotation(dir.XZ(), Vector3.up);

        
        // Get angular speed based on state
        var angularSpeed = _state == EnemyState.Moving ? _angularSpeedForMoving : _angularSpeedForStandingStill;

        
        // Set rotation 
        _model.localRotation = Quaternion.Slerp(_model.localRotation, _targetRotation,angularSpeed * Time.deltaTime);
    }
}
