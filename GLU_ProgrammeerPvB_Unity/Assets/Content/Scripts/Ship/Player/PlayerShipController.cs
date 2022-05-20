using System;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// The player ship controller handles the ship of the player.
/// Handles the input for the ship class.
/// Handles the effects and pickups.
/// </summary>
public class PlayerShipController : Ship, ICanPickup
{
    
    private Controls _controls;

    private float _effectTimeLeft;
    private bool _hasEffect = false;
    private Effect _effect;

    private DefaultSettings _defaultSettings;

    public Action<Effect> OnPlayerSetEffect;
    
    [Header("Speed Effect")] 
    [SerializeField] private SpeedSettings _speedSettings;
    
    [Header("Attack Effect")] 
    [SerializeField] private AttackSettings _attackSettings;

    [Header("Attack Effect")] 
    [SerializeField] private HealSettings _healSettings;

    [SerializeField] private GameObject[] _treasures;

    /// <summary>
    /// Subscribes all the input functions
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        
        _controls = new Controls();
        _controls.Player.Move.performed += OnMovementInput;
        _controls.Player.Move.canceled += OnMovementInput;

        _controls.Player.Shoot.performed += OnShootInput;
        _controls.Player.SinkShip.performed += OnSinkShipInput;
    }
    
    /// <summary>
    /// Sets the default settings
    /// </summary>
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

    /// <summary>
    /// Handles effect timer
    /// </summary>
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

    /// <summary>
    /// Sets the amount of treasure visible on the player ship
    /// </summary>
    /// <param name="amount"></param>
    public void SetTreasures(int amount)
    {
        var count = Mathf.Clamp(amount, 0, _treasures.Length);
        
        for (var i = 0; i < _treasures.Length; i++)
        {
            _treasures[i].SetActive(i + 1 <= count);
        }
    }

    
    /// <summary>
    /// Sets the controls Enabled or Disabled
    /// </summary>
    /// <param name="enabled"></param>
    public void SetControlsEnabled(bool enabled)
    {
        if(enabled)
            _controls.Enable();
        else
            _controls.Disable();
    }
    
    /// <summary>
    /// Sets an effect on the player
    /// </summary>
    /// <param name="effect"></param>
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
                
                // Fire event
                OnHealthChangeEvent?.Invoke(_shipHealth,_shipMaxHealth);

                break;
        }

        _effect = effect;
        _effectTimeLeft = effect.EffectDuration;
        _hasEffect = true;
        
        OnPlayerSetEffect(effect);

    }

    /// <summary>
    /// Removes the effect from the player
    /// </summary>
    /// <param name="effect"></param>
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
    
    
    /// <summary>
    /// Gets called when the player picks up an item
    /// </summary>
    /// <param name="effect"></param>
    public void OnPickup(Effect effect)
    {
        Debug.Log($"Player picked up: {effect}");
        SetEffect(effect);
    }

    
    /// <summary>
    /// Returns if the player can pickup
    /// </summary>
    /// <returns>If the player ship is ready to pickup</returns>
    public bool CanPickup()
    {
        if (_shipSunk)
            return false;
        
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
    
    private void OnSinkShipInput(InputAction.CallbackContext context)
    {
        if(_shipSunk)
            return;
        
        _shipHealth = 0f;
        OnHealthChangeEvent?.Invoke(_shipHealth,_shipMaxHealth);
        SinkShip();
    }

    /// <summary>
    /// Contains the default settings used by the effect system
    /// </summary>
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
