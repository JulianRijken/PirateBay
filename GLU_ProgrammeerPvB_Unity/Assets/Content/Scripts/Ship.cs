using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Ship : MonoBehaviour, IDamageable
{
    [SerializeField] protected float _shipMaxHealth;
    
    
    [SerializeField] protected float _maxForwardSpeed = 180f;
    [SerializeField] protected float _maxBackwardsSpeed = 30f;
    [SerializeField] protected float _accelerationSpeed = 30f;
    [SerializeField] protected float _rotationSpeed = 2f;
    [SerializeField] protected float _shipWobble = 2f;
    [SerializeField] protected float _shipUnFlipAngle = 50f;
    [SerializeField] protected Vector2 _shipUnFlipForce;
    [SerializeField] protected BuoyancyEffector3D _buoyancyEffector3D;


    [Header("Cannons")] 
    [SerializeField] private float _fireDelay;
    [SerializeField] private bool _waitingForFireDelay;
    [SerializeField] private Cannon[] _frontCannons;
    [SerializeField] private Cannon[] _leftCannons;
    [SerializeField] private Cannon[] _rightCannons;
    
    protected float _moveSpeed = 0f;
    protected float _shipHealth = 0f;
    protected Vector2 _movementInput = Vector2.zero;
    
    private Rigidbody _rigidbody;


    protected Action OnShipSink;
    

    protected void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _shipMaxHealth = _shipHealth;
    }
    
    
    protected void Update()
    {
        HandleShipMoveSpeed();
    }

    protected void FixedUpdate()
    {
        FixedHandleUnFlippingShip();
        FixedHandleShipMovement();
    }

    private void HandleShipMoveSpeed()
    {
        var targetSpeed = _maxBackwardsSpeed * _movementInput.y > 0 ? _movementInput.y * _maxForwardSpeed : _movementInput.y * _maxBackwardsSpeed;
        _moveSpeed = Mathf.MoveTowards(_moveSpeed, targetSpeed, Time.deltaTime * _accelerationSpeed);
    }
    
    private void FixedHandleShipMovement()
    {
        if (_buoyancyEffector3D != null && !_buoyancyEffector3D.IsUnderWater)
            return;

        // Move ship
        var direction = transform.forward;
        direction.y = 0;
        _rigidbody.AddForce(direction * _moveSpeed, ForceMode.Acceleration);
    
        // Rotate Ship    
        _rigidbody.AddTorque(Vector3.up * (_rotationSpeed * (_moveSpeed > 0 ? 1 : -1) * _movementInput.x), ForceMode.Acceleration);
        
        // Wobble ship in corners
        _rigidbody.AddTorque(transform.forward * _shipWobble * _movementInput.x, ForceMode.Acceleration);
    }

    private void FixedHandleUnFlippingShip()
    {
        var flippedAngle = (1f - (transform.up.y + 1f) / 2f) * 360;
        var unFlipForce =  Mathf.Max(0,(flippedAngle - _shipUnFlipAngle) / (360 - _shipUnFlipAngle));
        
        if (unFlipForce > 0)
        {
            var targetRotation = Quaternion.Slerp(_rigidbody.rotation, Quaternion.identity, Time.deltaTime * Mathf.Lerp(_shipUnFlipForce.y,_shipUnFlipForce.x, unFlipForce));
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.MoveRotation(targetRotation);
        }
    }

    protected void FireCannons(Side fireSide)
    {
        if(_waitingForFireDelay)
            return;
        
        Cannon[] cannonsToFire = null;
        
        switch (fireSide)
        {
            case Side.Left:
                cannonsToFire = _leftCannons;
                break;
            case Side.Right:
                cannonsToFire = _rightCannons;
                break;
            case Side.Front:
                cannonsToFire = _frontCannons;
                break;
        }
        
        if(cannonsToFire == null)
            return;
        

        foreach (var cannon in cannonsToFire)
        {
            cannon.Fire();
        }

        IEnumerable AddFireDelay()
        {
            yield return new WaitForSeconds(_fireDelay);
        }
        
    }

    protected enum Side
    { 
        Left,
        Right,
        Front
    }

    public void OnHealthChange(float delta)
    {
        _shipHealth += delta;
        Debug.Log(delta);
    }
}