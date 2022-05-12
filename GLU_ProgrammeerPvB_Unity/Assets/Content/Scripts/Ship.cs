using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour, IDamageable
{
    [SerializeField] private float _shipMaxHealth;
    private float _shipHealth;


    private Action OnShipSink;

    private void Awake()
    {
        _shipMaxHealth = _shipHealth;
    }

    protected void BlowWind()
    {

    }

    protected void SteerShip()
    {

    }

    protected void FireCannons(Side fireSide)
    {

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