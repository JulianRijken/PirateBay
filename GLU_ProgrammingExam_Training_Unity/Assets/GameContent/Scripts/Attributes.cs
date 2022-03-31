using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attributes : MonoBehaviour
{
    
    public delegate void OnHealthChangeEvent(float newHealth,float clampedDelta, float delta, float maxHealth, GameObject instigator);
    public event OnHealthChangeEvent OnHealthChange;

    public delegate void OnDeathEvent(GameObject instigator);
    public event OnDeathEvent OnHealthZero;
    
    
    private float _health;
    [SerializeField] private float _maxHealth;

    private void Awake()
    {
        _health = _maxHealth;
    }

    public void ApplyHealthChange(float delta, GameObject instigator)
    {
        var newHealth = _health + delta;
        newHealth = Mathf.Clamp(newHealth, 0f, _maxHealth);

        var clampedDelta = newHealth - _health;
        
        _health = newHealth;
        OnHealthChange?.Invoke(newHealth, delta,clampedDelta, _maxHealth, instigator);
        
        //Debug.Log(gameObject.name + " Health Changed: " + newHealth);

        if (_health == 0f)
        {
            OnHealthZero?.Invoke(instigator);
        }
    }
}
