using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Cannon : MonoBehaviour
{

    [SerializeField] protected Transform _firePoint;
    [SerializeField] protected ParticleSystem _fireEffect;
    [SerializeField] protected CannonBall _cannonBall;
    
    
    [SerializeField] [Range(-90f,90f)] protected float _xAngleOffsetMin;
    [SerializeField] [Range(-90f,90f)] protected float _xAngleOffsetMax;
    [SerializeField] [Range(-90f,90f)] protected float _yAngleOffsetMin;
    [SerializeField] [Range(-90f,90f)] protected float _yAngleOffsetMax;


    public void Fire(float attackDamage, float force = 50f, float maxRandomFireDelay = 0f)
    {
        Fire(attackDamage, Vector2.one, force, maxRandomFireDelay);
    }
    
    public void Fire(float attackDamage, Vector2 accuracyMultiplier, float force = 50f, float maxRandomFireDelay = 0f)
    {
        StopCoroutine(FireEnumerator());
        StartCoroutine(FireEnumerator());
        
        IEnumerator FireEnumerator()
        {
            yield return new WaitForSeconds(Random.value * maxRandomFireDelay);
            
            // Spawn cannon ball
            var spawnedCannonBall = Instantiate(_cannonBall, _firePoint.position, _firePoint.rotation);

           
            spawnedCannonBall.Damage = attackDamage;
            
            // Apply force to cannon ball
            var fireDirection = _firePoint.forward;
            fireDirection = Quaternion.AngleAxis(Random.Range(-_xAngleOffsetMin * accuracyMultiplier.x,-_xAngleOffsetMax * accuracyMultiplier.x), _firePoint.right) * fireDirection;
            fireDirection = Quaternion.AngleAxis(Random.Range(-_yAngleOffsetMin * accuracyMultiplier.y,-_yAngleOffsetMax * accuracyMultiplier.y), _firePoint.up) * fireDirection;
            spawnedCannonBall.Rigidbody.AddForce(fireDirection * force, ForceMode.VelocityChange);
        
            // Spawn effect
            _fireEffect.Play(true);
        }
    }
}