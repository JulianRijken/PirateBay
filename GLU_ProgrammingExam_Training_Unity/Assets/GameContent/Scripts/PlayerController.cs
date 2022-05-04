using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Weapon _weapon;
    [SerializeField] public Transform _characterModelTransform;
    [HideInInspector] public PlayerCamera AttachedCamera;

    [Header("Movement")]
    [SerializeField] private float _maxSpeed = 5f;
    [SerializeField] private float _runSpeed = 5f;
    [SerializeField] private float _gravityStrength = -15f;
    private float _gravity;
    private Vector3 _movementInput;


    [Header("Dashing")]
    [SerializeField] private AnimationCurve _dashCurve;
    [SerializeField] private float _dashTime = 0.2f;
    [SerializeField] private float _dashSpeed = 15f;
    [SerializeField] private float _dashDelay = 1f;
    private bool _bDashingAllowed = true;
    private bool _bDashing = false;

    
    [Header("Rotating")]
    [SerializeField] private float _rotateSpeed = 30f;

    
    [Header("Ground Check")]
    [SerializeField] private float _groundCheckDistance = 10f;
    [SerializeField] private float _groundedFether = 0.02f;
    
    private Attributes _attributes;

    private CharacterController _characterController;

    public float MaxSpeed => _maxSpeed;
    public Vector3 Location => _characterController.center;
    
    
    public event Action OnDash;
    public event Action OnPlayerDeath;
    
    
    private struct GroundInfo
    {
        public bool Grounded;
        public float GroundAngle;
        public float GroundDistance;
        public Quaternion GroundRotation;
    }
    
    

    private void Awake()
    {
        _attributes = GetComponent<Attributes>();
        _characterController = GetComponent<CharacterController>();

        _attributes.OnHealthZero += OnHealthZero;
    }

    private void Update()
    {
        var groundInfo = GroundCheck();
        HandleMovement(groundInfo);
        HandleRotation();
    }

    

    
    /// <summary>
    /// Handles the player movement 
    /// </summary>
    private void HandleMovement(GroundInfo groundInfo)
    {
        if(_bDashing)
            return;
        
        // Update Gravity
        _gravity = groundInfo.Grounded ? 0 : _gravity + _gravityStrength * Time.deltaTime;
        
        // Check slope
        var slopeToSteep = groundInfo.GroundAngle >= _characterController.slopeLimit; 
        
        // Set movement direction
        var movementDirection = slopeToSteep ? _movementInput : groundInfo.GroundRotation * _movementInput;

        // Get the target motion
        var targetMotion = movementDirection * _runSpeed + Vector3.up * _gravity;

        // Apply motion
        _characterController.Move(targetMotion * Time.deltaTime);
    }

    
    /// <summary>
    /// Handles the player rotation
    /// </summary>
    private void HandleRotation()
    {
        if(!AttachedCamera)
            return;

        var mouseLocation = AttachedCamera.GetMouseInWorldSpace();
        mouseLocation.y = 0;

        var playerPosition = transform.position;
        playerPosition.y = 0;

        var targetDirection = mouseLocation - playerPosition;
        
        var targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
        var smoothRotation = Quaternion.Slerp(_characterModelTransform.localRotation, targetRotation, Time.deltaTime * _rotateSpeed);
        _characterModelTransform.localRotation = smoothRotation;
    }

    
    /// <summary>
    /// Dashes the player forward
    /// </summary>
    private void Dash()
    {
        if (_movementInput.magnitude == 0)
            return;

        if(!_bDashingAllowed)
            return;
        
        StartCoroutine(DashTimeline());
    }
    
    private IEnumerator DashTimeline()
    {
        _bDashing = true;
        _bDashingAllowed = false;
        
        OnDash?.Invoke();
        
        var dashDirection = _movementInput;
        
        
        var alpha = 0f;
        while(true)
        {
            _characterController.Move(dashDirection * (_dashCurve.Evaluate(alpha) * _dashSpeed * Time.deltaTime));
            
            if (alpha >= 1f) break;
            yield return new WaitForEndOfFrame();
            alpha = Mathf.Clamp01(alpha + (Time.deltaTime * (1.0f / _dashTime)));
        }

        _bDashing = false;
        
        yield return new WaitForSeconds(_dashDelay);

        _bDashingAllowed = true;
    }

    
    /// <summary>
    /// Returns information about the ground like the angle, rotation and distance
    /// </summary>
    private GroundInfo GroundCheck()
    {
        var sphereOrigin = _characterController.transform.position + _characterController.center;
        sphereOrigin.y += -_characterController.height / 2f + _characterController.radius;
        
        Physics.SphereCast(sphereOrigin, _characterController.radius, Vector3.down, out var hit, _groundCheckDistance);
        
        return new GroundInfo
        {
            Grounded = hit.distance - _characterController.skinWidth < _groundedFether,
            GroundAngle = Vector3.Angle(hit.normal, Vector3.up),
            GroundRotation = Quaternion.FromToRotation(Vector3.up, hit.normal),
            GroundDistance = hit.distance - _characterController.skinWidth
        };
    }

    private void OnHealthZero(GameObject instigator)
    {
        OnPlayerDeath?.Invoke();
        Destroy(gameObject);
    }

    
    #region HandeInput
    
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
    
    #endregion
}
