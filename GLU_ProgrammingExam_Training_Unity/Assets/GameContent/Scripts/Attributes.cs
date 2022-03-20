using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attributes : MonoBehaviour
{
    
    public delegate void OnHealthChangeEvent(float newHealth,float clampedDelta, float delta, float maxHealth);
    public event OnHealthChangeEvent OnHealthChange;
    


    private float _health;
    [SerializeField] private float _maxHealth;

    private void Awake()
    {
        _health = _maxHealth;
    }

    public void ChangeHealth(float delta)
    {
        float newHealth = _health + delta;
        newHealth = Mathf.Clamp(newHealth, 0f, _maxHealth);

        float clampedDelta = newHealth - _health;
        
        _health = newHealth;
        OnHealthChange?.Invoke(newHealth, delta,clampedDelta, _maxHealth);
        Debug.Log(gameObject.name + " Health Changed: " + newHealth);
    }
}
