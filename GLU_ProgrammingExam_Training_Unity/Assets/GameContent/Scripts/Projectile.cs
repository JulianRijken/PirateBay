using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] public GameObject Instigator;
    [SerializeField] public float MoveSpeed;
    [SerializeField] public float Damage;
    
    [SerializeField] private float _radius;
    [SerializeField] private LayerMask _collisionLayers;
    
    private bool _bHitObject = false;
    private Vector3 _lastPosition;

    private void Awake()
    {
        _lastPosition = transform.position;
    }

    private void Update()
    {
        if(_bHitObject)
            return;
        
        MoveProjectile();
        CollisionCheck();
    }

    
    private void MoveProjectile()
    {
        var velocity = transform.forward * (MoveSpeed * Time.deltaTime);
        transform.position += velocity;
    }
    
    private void CollisionCheck()
    {
        
        // Update Cast
        var direction = transform.position - _lastPosition;
        Physics.SphereCast(_lastPosition, _radius, direction, out var hit,direction.magnitude,_collisionLayers);
        
        Debug.DrawRay(_lastPosition,direction, Color.green);

        // Check for collision
        if (hit.collider)
        {
            transform.position = hit.point;
            
            // Check for attributes
            if (hit.collider.GetComponent<Attributes>() is var attributes && attributes != null)
            {
                attributes.ApplyHealthChange(-Damage, Instigator);
            }

            OnProjectileHit();
        }
        else
        {
            _lastPosition = transform.position;
        }
    }

    private void OnProjectileHit()
    {
        _bHitObject = true;
        
        // Quick code for hiding and destroying 
        Destroy(gameObject,2f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,_radius);
    }
}
