using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Uses the cannon class and automatically fires at a given target using it's velocity
/// </summary>
public class AutoCannon : Cannon
{
    [SerializeField] private float _fireForce;
    [SerializeField] private Vector2 _fireInterval;
    [SerializeField] private float _damage;
    [SerializeField] private float _range;
    [SerializeField] private float _projectionDistance;
    [SerializeField] private float _maxAngle;
    [SerializeField] private float _maxRandomFireDelay;
    [SerializeField] private Transform _model;
    
    private Rigidbody _targetRigidbody;
    private Transform _target;

    private Vector3 _projectedTargetPosition;

    private bool _canFire;
    private float _targetDistance;
    private Vector3 _projectedTargetDirection;
    
    /// <summary>
    /// Sets the target and the target rigidbody to the player and starts the fire loop
    /// </summary>
    private void Start()
    {
        _target = GameManager.Player.transform;
        _targetRigidbody = GameManager.Player.Rigidbody;

        if (!_target|| !_targetRigidbody)
            Debug.LogError("Auto cannon has no target");
        
        
        StartCoroutine(FireLoop());
    }

    /// <summary>
    /// Updates the cannon angle and calculates the projected target
    /// </summary>
    private void Update()
    {

        // Check angle
        var angle = Vector3.Angle(transform.forward, _target.position - _firePoint.position);
        if (angle > _maxAngle)
        {
            _canFire = false;
            return;
        }
            
        // Check range
        _targetDistance = Vector3.Distance(_firePoint.position,_target.position);
        _canFire = _targetDistance < _range;
        
        
        _projectedTargetPosition = _target.position + _targetRigidbody.velocity * _projectionDistance;
        _projectedTargetDirection = _projectedTargetPosition - _firePoint.position;
    }


    /// <summary>
    /// Fires the cannon at a set interval and rotates the cannon towards the target
    /// </summary>
    /// <returns></returns>
    private IEnumerator FireLoop()
    {
        while (true)
        {
            yield return new WaitUntil(() => _canFire);

            var targetRotationEuler = Quaternion.LookRotation(_projectedTargetDirection).eulerAngles;
            targetRotationEuler.x = 0f;
            targetRotationEuler.z = 0f;
            
            var targetRotation = Quaternion.Euler(targetRotationEuler);
            _firePoint.rotation = targetRotation;
            _model.rotation = targetRotation;
            
            Fire(_damage,_targetDistance * _fireForce,_maxRandomFireDelay);
            yield return new WaitForSeconds(Random.Range(_fireInterval.x, _fireInterval.y));
        }
    }
}
