using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipBase : Ship
{

    [Header("Enemy AI")] 
    [SerializeField] private float _fireRange;
    [SerializeField] private Transform _target;
    [SerializeField] private float _maxTimeInRange;
    [SerializeField] private float _inactiveDistance;
    [SerializeField] private float _angleRequiredForFireClose;
    [SerializeField] private float _angleRequiredForFireFar;
    [SerializeField] private float _maximumFireDistance;

    [Header("Collision Detection")] 
    [SerializeField] private float _collisionCheckDistance;
    [SerializeField] private float _collisionCheckRadius;
    [SerializeField] private AnimationCurve _rotateSpeedOverDistanceFromCollision;
    [SerializeField] private Transform _collisionCheckPoint;
    [SerializeField] private LayerMask _avoidLayerMask;
    
    private float _timeInRange;
    

    protected override void SinkShip()
    {
        base.SinkShip();
        Destroy(gameObject, 20f);
    }


    
    protected override void Update()
    {
        base.Update();
        
        // Cancle all if shit has sunk
        if(_shipSunk)
            return;
        
        HandleAI();
    }

    private void HandleAI()
    {
        
        //=== Handle obstacle avoidance ===\\
        
        // Handle collision checking
        if (_collisionCheckPoint)
        {
            Physics.SphereCast(_collisionCheckPoint.position, _collisionCheckRadius, _collisionCheckPoint.forward,
                out var hit, _collisionCheckDistance, _avoidLayerMask);

            
            // If hit something
            if (hit.collider != null)
            {
                //var hitDirection = hit.point - transform.position;
                //var rotateDirection = Vector3.Dot(transform.right, hitDirection) > 0 ? -1 : 1;
                
                Physics.SphereCast(_collisionCheckPoint.position, _collisionCheckRadius, _collisionCheckPoint.right,
                    out var hitRight, _collisionCheckDistance,_avoidLayerMask);
                
                Physics.SphereCast(_collisionCheckPoint.position, _collisionCheckRadius, -_collisionCheckPoint.right,
                    out var hitLeft, _collisionCheckDistance, _avoidLayerMask);

                var rotateDirection = hitRight.distance > hitLeft.distance ? -1 : 1;
                    
                var distanceAlpha = hit.distance / _collisionCheckDistance;
                _movementInput.x = _rotateSpeedOverDistanceFromCollision.Evaluate(1f - distanceAlpha) * rotateDirection;
                _movementInput.y = 0.5f;
                return;
            }
        }
        
        
        

        var targetDirection = _target.position - transform.position;
        targetDirection.y = 0f;

        var distanceFromTarget = targetDirection.magnitude;
        
        
        //=== Handle player range and inactivity ===\\
        
        // If the player is to far away
        if (distanceFromTarget > _inactiveDistance)
        {
            Debug.DrawLine(transform.position, _target.position, Color.red);
            _movementInput = Vector3.zero;
            return;
        }

        
        Debug.DrawLine(transform.position, _target.position, Color.green);
        _movementInput = new Vector2(0,1);


        
        //=== Handle firing cannons ===\\

        // In range
        if (distanceFromTarget < _maximumFireDistance)
        {
            var rightFireAngle = Vector3.Angle(transform.right, targetDirection);
            var leftFireAngle = Vector3.Angle(-transform.right, targetDirection);

            //Debug.Log("Shoot: " + rightFireAngle);

            var minimumAngle = Mathf.Lerp(_angleRequiredForFireClose, _angleRequiredForFireFar, distanceFromTarget / _maximumFireDistance);
            
            // Fire cannons
            if (rightFireAngle < minimumAngle)
                TryFireCannons(Side.Right);

            if (leftFireAngle < minimumAngle)
                TryFireCannons(Side.Left);
        }




        //==== Handle running away ====\\
        
        var inRange = distanceFromTarget < _fireRange;

        _timeInRange += Time.deltaTime * (inRange ? 1f : -2f);
        
        // Running
        if (_timeInRange > _maxTimeInRange)
        {
            Debug.Log("RUN");
            var runDirection = Vector3.Dot(transform.right, targetDirection.normalized) > 0f ? 1f : -1f;

            _movementInput.x = -runDirection;
            return;
        }



        
        //=== Handle moving to the player ====\\
        
        // In fire range
        if (inRange)
        {
            var diffrence = Vector3.Dot(transform.right, _target.forward);
            
            // Move to to same rotation as the player
            _movementInput.x = diffrence;
        }
        else
        {
            var playerOnSideOfShip = Vector3.Dot(transform.right, targetDirection.normalized) > 0f ? 1 : -1;

            var shipDirection = transform.forward;
            shipDirection.y = 0f;

            var rotateSpeed = Vector3.Angle(shipDirection, targetDirection) / 180f;
            
            
            
            //Debug.Log(rangeFromTarget);
            _movementInput.x = playerOnSideOfShip * rotateSpeed;
        }

        _timeInRange = Mathf.Max(0f, _timeInRange);
        
    }
    
        
    #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_collisionCheckPoint)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(_collisionCheckPoint.position, _collisionCheckRadius);
                Gizmos.DrawWireSphere(_collisionCheckPoint.position + _collisionCheckPoint.forward * _collisionCheckDistance, _collisionCheckRadius);
            }
        }
    #endif 
}