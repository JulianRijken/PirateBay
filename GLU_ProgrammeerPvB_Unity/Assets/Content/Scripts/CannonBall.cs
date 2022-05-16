using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CannonBall : MonoBehaviour
{
    [SerializeField] public float Damage = 4f;
    [SerializeField] private GameObject _explodeEffect;
    [SerializeField] private GameObject _waterEffect;
    [SerializeField] private ParticleSystem _smokeTrail;

    private bool _hit = false;
    
    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }
    

    public Rigidbody Rigidbody { get; private set; }

    private void Update()
    {
        if (transform.position.y < 0) 
            OnHit(true);

        if (_hit)
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, Time.deltaTime * 0.5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnHit(false, collision);
    }

    private void OnHit(bool water, Collision collision = null)
    {
        if(_hit)
            return;


        if (collision != null)
        {
            var damageableInterface = collision.gameObject.GetComponent<IDamageable>();
            damageableInterface?.OnHealthChange(-Damage);
        }

        // Effects
        _hit = true;
        Destroy(Instantiate(water ? _waterEffect : _explodeEffect, transform.position, transform.rotation), 5f);
        Destroy(gameObject, 5f);
        Rigidbody.drag = 5f;
        _smokeTrail.Stop();
    }
}