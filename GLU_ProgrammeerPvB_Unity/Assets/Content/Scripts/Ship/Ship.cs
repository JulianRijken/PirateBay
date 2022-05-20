using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Handles everything for ships withing the game like:
/// Health,
/// Movement,
/// UnFlipping,
/// Cannons,
/// Sinking,
/// Events
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Ship : MonoBehaviour, IDamageable
{

    [Header("Health")]
    [SerializeField] protected float _shipStartHealth;
    protected float _shipMaxHealth;

    [Header("Movement")]
    [SerializeField] protected float _maxForwardSpeed = 165f;
    [SerializeField] protected float _maxBackwardsSpeed = 30f;
    [SerializeField] protected float _accelerationSpeed = 30f;
    [SerializeField] protected AnimationCurve _turnSpeed;
    [SerializeField] protected float _turnSpeedMultiplier = 1f;
    [SerializeField] protected float _shipWobble = 2f;
    [SerializeField] protected float _shipUnFlipAngle = 50f;
    [SerializeField] protected Vector2 _shipUnFlipForce;
    [SerializeField] protected Transform _centerOfMass;
    [SerializeField] protected BuoyancyEffector3D _buoyancyEffector3D;
 
    [Header("Cannons")] 
    [SerializeField] protected float _fireAllowedDelay;
    [SerializeField] protected Cannon[] _frontCannons;
    [SerializeField] protected Cannon[] _leftCannons;
    [SerializeField] protected Cannon[] _rightCannons;
    [SerializeField] protected float _attackDamage;
    [SerializeField] protected Vector2 _cannonAccuracy;
    [SerializeField] protected float _cannonForce;
    [SerializeField] protected float _cannonMaxRandomFireDelay;
    
    [SerializeField] private GameObject _shipSinkExplosionPrefab;
    [SerializeField] private MeshFilter _shipMesh;
    [SerializeField] private int _amountOfExplosions;
    [SerializeField] private float _explodeEffectSpeed;
    [SerializeField] private float _shipSinkSpeed = 5f;
    [SerializeField] private float _shipSinkDelay = 2f;

    private float _defaultFloatingPower;
    
    private bool _waitingForFireDelay;
    
    protected float _moveSpeed = 0f;
    protected float _shipHealth = 0f;
    protected bool _shipSunk = false;
    protected Vector2 _movementInput = Vector2.zero;
    
    private Rigidbody _rigidbody;
    public Rigidbody Rigidbody => _rigidbody;

    public Action OnShipSunken;

    public delegate void OnHealthChangeDelegate(float newHealth, float maxHealth);

    public OnHealthChangeDelegate OnHealthChangeEvent;
    
    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    protected virtual void Start()
    {
        if (_centerOfMass)
        {
            _rigidbody.centerOfMass = _centerOfMass.localPosition;
        }

        _defaultFloatingPower = _buoyancyEffector3D.FloatingPower;

        _shipMaxHealth = _shipStartHealth;
        _shipHealth = _shipMaxHealth;
        OnHealthChangeEvent?.Invoke(_shipHealth,_shipMaxHealth);
    }

    protected virtual void Update()
    {
        HandleShipMoveSpeed();
    }

    protected virtual void FixedUpdate()
    {
        FixedHandleUnFlippingShip();
        FixedHandleShipMovement();
    }

    /// <summary>
    /// Handles the ship movement and continuously updates the move speed based on the target speed and accelerationSpeed
    /// </summary>
    private void HandleShipMoveSpeed()
    {
        var targetSpeed = _shipSunk ? 0f : _maxBackwardsSpeed * _movementInput.y > 0 ? _movementInput.y * _maxForwardSpeed : _movementInput.y * _maxBackwardsSpeed;
        _moveSpeed = Mathf.MoveTowards(_moveSpeed, targetSpeed, Time.deltaTime * _accelerationSpeed);
    }
    
    /// <summary>
    /// Handles the ship movement by adding force based on the move speed and torque based on the turn speed
    /// </summary>
    private void FixedHandleShipMovement()
    {
        if (_buoyancyEffector3D != null && !_buoyancyEffector3D.IsUnderWater)
            return;

        // Move ship
        var direction = transform.forward;
        direction.y = 0;
        _rigidbody.AddForce(direction * _moveSpeed, ForceMode.Acceleration);

        if (!_shipSunk)
        {
            // Rotate Ship    
            _rigidbody.AddTorque(Vector3.up * ((_moveSpeed >= 0 ? 1 : -1) * _turnSpeedMultiplier * _movementInput.x * _turnSpeed.Evaluate(_moveSpeed / _maxForwardSpeed)), ForceMode.Acceleration);

            // Wobble ship in corners
            _rigidbody.AddTorque(transform.forward * (_shipWobble * Mathf.Clamp(_movementInput.x, -1f,1f)), ForceMode.Acceleration);
        }
    }

    /// <summary>
    /// Handles un flipping the ship by forcing it upright using rigidbody MoveRotation
    /// </summary>
    private void FixedHandleUnFlippingShip()
    {
        if (_shipSunk)
            return;
        
        var flippedAngle = (1f - (transform.up.y + 1f) / 2f) * 360;
        var unFlipForce =  Mathf.Max(0,(flippedAngle - _shipUnFlipAngle) / (360 - _shipUnFlipAngle));
        
        if (unFlipForce > 0)
        {
            var targetRotation = Quaternion.Slerp(_rigidbody.rotation, Quaternion.identity, Time.deltaTime * Mathf.Lerp(_shipUnFlipForce.y,_shipUnFlipForce.x, unFlipForce));
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.MoveRotation(targetRotation);
        }
    }

    /// <summary>
    /// Attempts to fire the cannons based on the side
    /// </summary>
    /// <param name="fireSide"></param>
    /// <returns>If the cannons can fire based on the fire delay</returns>
    protected bool TryFireCannons(Side fireSide)
    {
        if(_shipSunk)
             return false;
        
        if(_waitingForFireDelay)
            return false;
        
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
            return false;
        

        foreach (var cannon in cannonsToFire)
        {
            cannon.Fire(_attackDamage,_cannonAccuracy,_cannonForce,_cannonMaxRandomFireDelay);
        }

        StartCoroutine(AddFireDelay());

        return true;
        
        IEnumerator AddFireDelay()
        {
            _waitingForFireDelay = true;
            yield return new WaitForSeconds(_fireAllowedDelay);
            _waitingForFireDelay = false;
        }
    }
    

    /// <summary>
    /// The side of the ship
    /// </summary>
    protected enum Side
    { 
        Left,
        Right,
        Front
    }

    /// <summary>
    /// Gets called as soon as the player health changes
    /// </summary>
    /// <param name="delta"></param>
    public virtual void OnHealthChange(float delta)
    {
        // Check if the ship has not already sunk
        if(_shipSunk)
            return;
        
        _shipHealth += delta;
        _shipHealth = Mathf.Max(0f, _shipHealth);
        
        OnHealthChangeEvent?.Invoke(_shipHealth,_shipMaxHealth);
        Debug.Log($"name: {gameObject.name}, health change: {delta}, current health: {_shipHealth}");
        
        if (_shipHealth <= 0f)
        {
            SinkShip();
        }
    }

    /// <summary>
    /// UnSinks the ship and resetting all necessary parameters
    /// </summary>
    public virtual void UnSinkShip()
    {
        _shipMaxHealth = _shipStartHealth;
        _shipHealth = _shipMaxHealth;
        OnHealthChangeEvent?.Invoke(_shipHealth,_shipMaxHealth);
        
        _buoyancyEffector3D.FloatingPower = _defaultFloatingPower;
        _shipSunk = false;
    }

    /// <summary>
    /// Sinks the ship and shows all the effects
    /// </summary>
    protected virtual void SinkShip()
    {
        Debug.Log($"Ship sunk: {gameObject.name}");

        _shipSunk = true;

        StartCoroutine(ShipSinkEffect());
        StartCoroutine(ShipExplodeEffect());

        IEnumerator ShipExplodeEffect()
        {
            if (_shipMesh.mesh.isReadable && _shipMesh)
            {
                for (var i = 0; i < _amountOfExplosions; i++)
                {
                    var vertices = _shipMesh.mesh.vertices;
                    var randomPoint = _shipMesh.transform.TransformPoint(vertices[Random.Range(0, vertices.Length)]);

                    Destroy(Instantiate(_shipSinkExplosionPrefab, randomPoint, transform.rotation), 5f);

                    yield return new WaitForSeconds(Random.value * _explodeEffectSpeed);
                }
            }
            else
            {
                Debug.LogWarning("Mesh is not readable or not assigned");
            }
        }

        IEnumerator ShipSinkEffect()
        {
            yield return new WaitForSeconds(_shipSinkDelay);

            var timer = _shipSinkSpeed;
            while (timer > 0f)
            {
                _buoyancyEffector3D.FloatingPower = _defaultFloatingPower * timer / _shipSinkSpeed;
                _rigidbody.AddTorque(transform.right * (-50f * Time.deltaTime), ForceMode.Acceleration);
                _rigidbody.AddForce(Vector3.down * (100 * Time.deltaTime), ForceMode.Acceleration);
                timer -= Time.deltaTime;
                yield return null;
            }

            OnShipSunken?.Invoke();
        }
    }

}