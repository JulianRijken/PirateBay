using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    
    [SerializeField] private Transform _playerModelTransform;
    [SerializeField] private Weapon _weapon;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _rotateSpeed;
    [SerializeField] private AnimationCurve _dashCurve;
    [SerializeField] private float _dashTime;
    [SerializeField] private float _dashSpeed;
    [SerializeField] private float _dashDelay;
    [SerializeField] private float _gravityStrength;
    
    [Header("Ground Check")]
    [SerializeField] private float _groundCheckDistance;
    [SerializeField] private float _groundedFether;
    
    private Attributes _attributes;

    private float _gravity;
    private bool _bDashingAllowed = true;
    private bool _bDashing = false;
    private Vector3 _movementInput;
    private CharacterController _characterController;

    public float MaxSpeed => _maxSpeed;
    
    [HideInInspector] public PlayerCamera AttachedCamera;

    public event Action OnDash;
    public event Action OnPlayerDeath;
    
    public Vector3 Location => _playerModelTransform != null ? _playerModelTransform.position : Vector3.zero;


    private void Awake()
    {
        //StartCoroutine(ZeroToOne(DoABarrelRoll, StopBarrelRoll));
        _characterController = GetComponent<CharacterController>();

        _attributes = GetComponent<Attributes>();

        _attributes.OnHealthZero += OnHealthZero;
    }

    private void Update()
    {
        UpdateMovement();
        UpdateRotation();
    }

    private void OnHealthZero(GameObject instigator)
    {
        OnPlayerDeath?.Invoke();
        Destroy(gameObject);
    }
    private void UpdateMovement()
    {
        if(_bDashing)
            return;

        var groundCheck = GetGroundCheck();
        
        if (groundCheck.Grounded)
        {
            _gravity = 0;
        }
        else
        {
            _gravity += _gravityStrength * Time.deltaTime;
        }
        
       

        var slopeToSteep =groundCheck.GroundAngle >= _characterController.slopeLimit; 
        var movementDirection = slopeToSteep ? _movementInput : groundCheck.GroundRotation * _movementInput;
        
        
        DebugExtension.DebugArrow(transform.position,movementDirection,Color.red);
        
        var targetMovement = movementDirection * _runSpeed;
        // Add gravity
        targetMovement.y += _gravity;
        
        _characterController.Move(targetMovement * Time.deltaTime);
    }

    private GroundCheck GetGroundCheck()
    {
        
        var sphereOrigin = _characterController.transform.position + _characterController.center;
        sphereOrigin.y += -_characterController.height / 2f + _characterController.radius;
        
        Physics.SphereCast(sphereOrigin, _characterController.radius, Vector3.down, out var hit, _groundCheckDistance);

#if UNITY_EDITOR
        // Debug sphere cast origin
        DebugExtension.DebugWireSphere(sphereOrigin,Color.white, _characterController.radius);
        
        // Debug hit point
        DebugExtension.DebugPoint(hit.point,Color.red, 0.3f);

        // Debug ground hit normal
        DebugExtension.DebugArrow(transform.position, hit.normal);
#endif
        
        // Get the slope rotation
        var slopeRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
    
#if UNITY_EDITOR
        // Debug movement direction over slope rotation
        DebugExtension.DebugArrow(transform.position, slopeRotation * _movementInput);
#endif

        var grounded = hit.distance - _characterController.skinWidth < _groundedFether;
        
        return new GroundCheck()
        {
            Hit = hit,
            GroundRotation = slopeRotation,
            GroundAngle = Vector3.Angle(hit.normal,Vector3.up),
            Grounded = grounded
        };
    }
    
    private struct GroundCheck
    {
        public RaycastHit Hit;
        public Quaternion GroundRotation;
        public float GroundAngle;
        public bool Grounded;
    }


    private void UpdateRotation()
    {
        if(!AttachedCamera)
            return;

        var mouseLocation = AttachedCamera.GetMouseInWorldSpace();
        mouseLocation.y = 0;

        var playerPosition = transform.position;
        playerPosition.y = 0;

        var targetDirection = mouseLocation - playerPosition;
        
        var targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
        var smoothRotation = Quaternion.Slerp(_playerModelTransform.localRotation, targetRotation, Time.deltaTime * _rotateSpeed);
        _playerModelTransform.localRotation = smoothRotation;
    }

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
}
