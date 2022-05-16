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

    private DefaultSettings _defaultSettings;
    

    [Header("Speed Effect")] 
    [SerializeField] private SpeedSettings _speedSettings;
    
    [Header("Attack Effect")] 
    [SerializeField] private AttackSettings _attackSettings;

    [Header("Attack Effect")] 
    [SerializeField] private HealSettings _healSettings;
    
    
    protected override void Awake()
    {
        base.Awake();
        
        _controls = new Controls();
        _controls.Player.Move.performed += OnMovementInput;
        _controls.Player.Move.canceled += OnMovementInput;

        _controls.Player.Shoot.performed += OnShootInput;
        
    }
    
    protected override void Start()
    {
        base.Start();
        
        _defaultSettings = new DefaultSettings()
        {
            CannonAccuracy = _cannonAccuracy,
            AttackDamage = _attackDamage,
            MaxForwardSpeed = _maxForwardSpeed,
            FireAllowedDelay = _fireAllowedDelay
        };
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


    public void SetControlsEnabled(bool enabled)
    {
        if(enabled)
            _controls.Enable();
        else
            _controls.Disable();
    }
    
    private void SetEffect(Effect effect)
    {
        switch (effect.EffectType)
        {
            case EffectType.Speed:
                _maxForwardSpeed *= _speedSettings.SpeedMultiplier;
                _accelerationSpeed *= _speedSettings.AccelerationMultiplier;
                _turnSpeedMultiplier *= _speedSettings.TurnMultiplier;
                _cannonAccuracy = _speedSettings.CannonAccuracy;
                
                break;
            case EffectType.Attack:
                _maxForwardSpeed = _attackSettings.MaxForwardSpeed;
                _attackDamage = _attackSettings.AttackDamage;
                _fireAllowedDelay = _attackSettings.FireAllowedDelay;
                
                break;
            case EffectType.Heal:
                
                // Remove the max health based on how much the player healed 
                var healDelta = _shipMaxHealth - _shipHealth;
                _shipMaxHealth -= healDelta - healDelta * _healSettings.RegainPercentage;
                
                // Apply new health
                _shipHealth = _shipMaxHealth;

                break;
        }

        _effect = effect;
        _effectTimeLeft = effect.EffectDuration;
        _hasEffect = true;
    }

    private void RemoveEffect(Effect effect)
    {
        Debug.LogWarning("Multiply should be removed or changed because of wrong resetting of values (maybe)");

        switch (effect.EffectType)
        {
            case EffectType.Speed:
                
                _maxForwardSpeed /= _speedSettings.SpeedMultiplier;
                _accelerationSpeed /= _speedSettings.AccelerationMultiplier;
                _turnSpeedMultiplier /= _speedSettings.TurnMultiplier;
                _cannonAccuracy = _defaultSettings.CannonAccuracy;
                
                break;
            case EffectType.Attack:
                _maxForwardSpeed = _defaultSettings.MaxForwardSpeed;
                _attackDamage = _defaultSettings.AttackDamage;
                _fireAllowedDelay = _defaultSettings.FireAllowedDelay;
                Debug.Log("Reset: " + _defaultSettings.FireAllowedDelay);
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
    
    private struct DefaultSettings
    {
        public Vector2 CannonAccuracy;
        public float AttackDamage;
        public float MaxForwardSpeed;
        public float FireAllowedDelay;
    }

    [Serializable]
    private struct SpeedSettings
    {
        [Range(1f,10f)] public float SpeedMultiplier;
        [Range(1f,10f)] public float AccelerationMultiplier;
        [Range(1f,10f)] public float TurnMultiplier;
        public Vector2 CannonAccuracy;
    }
    
    [Serializable]
    private struct AttackSettings
    {
        public float AttackDamage;
        public float MaxForwardSpeed;
        public float FireAllowedDelay;
    }
    
    [Serializable]
    private struct HealSettings
    {
        public float RegainPercentage;
    }
}
