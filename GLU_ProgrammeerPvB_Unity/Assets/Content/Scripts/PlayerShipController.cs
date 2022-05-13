using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShipController : Ship, ICanPickup
{
    
    private Controls _controls;

    private float _effectTimeLeft;
    private bool _hasEffect = false;
    private Effect _effect;

    
    // Speed effect
    [Header("Speed Effect")] 
    [SerializeField] [Range(1f,10f)] private float _speedEffectSpeedMultiplier;
    [SerializeField] [Range(1f,10f)] private float _speedEffectAccelerationMultiplier;
    [SerializeField] [Range(1f,10f)] private float _speedEffectTurnMultiplier;
    [SerializeField] private CannonSettings _speedEffectCannonSettings;
    private CannonSettings _defaultCannonSettings;
    
    // Attack effect
    
    // Heal effect
    
    protected override void Awake()
    {
        base.Awake();
        
        _controls = new Controls();
        _controls.Player.Move.performed += OnMovementInput;
        _controls.Player.Move.canceled += OnMovementInput;

        _controls.Player.Shoot.performed += OnShootInput;

        _defaultCannonSettings = _cannonSettings;
    }
    
    protected override void Start()
    {
        base.Start();
        
        _controls.Player.Enable();
    }

    protected override void Update()
    {
        base.Update();

        if (_hasEffect)
        {
            _effectTimeLeft = Mathf.Max(0f, _effectTimeLeft - Time.deltaTime);
            if (_effectTimeLeft == 0f)
            {
                RemoveEffect(_effect);
            }
        }
    }

    
    private void SetEffect(Effect effect)
    {
        switch (effect.EffectType)
        {
            case EffectType.Speed:
                _maxForwardSpeed *= _speedEffectSpeedMultiplier;
                _accelerationSpeed *= _speedEffectAccelerationMultiplier;
                _turnSpeedMultiplier *= _speedEffectTurnMultiplier;
                _cannonSettings = _speedEffectCannonSettings;


                break;
            case EffectType.Attack:
                
                break;
            case EffectType.Heal:
                
                break;
        }
        
        _effectTimeLeft = effect.EffectDuration;
        _hasEffect = true;
    }

    private void RemoveEffect(Effect effect)
    {
        Debug.LogWarning("Should be removed or changed for obvious reasons");

        switch (effect.EffectType)
        {
            case EffectType.Speed:
                
                _maxForwardSpeed /= _speedEffectSpeedMultiplier;
                _accelerationSpeed /= _speedEffectAccelerationMultiplier;
                _turnSpeedMultiplier /= _speedEffectTurnMultiplier;
                _cannonSettings = _defaultCannonSettings;
                
                break;
            case EffectType.Attack:
                
                break;
            case EffectType.Heal:
                
                break;
            
        }
        
        
        Debug.Log("Effect Removed");
        _hasEffect = false;
    }
    
    

    public void OnPickup(Effect effect)
    {
        Debug.Log($"Player picked up: {effect}");
        SetEffect(effect);
    }

    
    
    
    
    
    public bool CanPickup()
    {
        return !_hasEffect;
    }


    
    
    private void OnMovementInput(InputAction.CallbackContext context)
    {
        _movementInput = context.ReadValue<Vector2>();
    }

    private void OnShootInput(InputAction.CallbackContext context)
    {
        if(!context.performed)
            return;
        
        var side = context.ReadValue<float>();
        
        TryFireCannons(side > 0f ? Side.Right : Side.Left);
    }

}
