using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Cannon : MonoBehaviour
{

    [SerializeField] private Transform _firePoint;
    [SerializeField] private ParticleSystem _fireEffect;
    
    [Header("Custom Settings")]
    [SerializeField] protected CannonSettings _cannonSettings;

    public void Fire()
    {
        
        if (!_cannonSettings)
        {
            Debug.LogError("Cannon Settings are not assigned");
            return;
        }
        
        StopCoroutine(FireEnumerator());
        StartCoroutine(FireEnumerator());
        
        IEnumerator FireEnumerator()
        {
            yield return new WaitForSeconds(Random.value * _cannonSettings._maxRandomFireDelay);
            
            // Spawn cannon ball
            var spawnedCannonBall = Instantiate(_cannonSettings._cannonBall, _firePoint.position, _firePoint.rotation);

            if(_cannonSettings._useCannonDamage)
                spawnedCannonBall.Damage = _cannonSettings._damage;
            
            // Apply force to cannon ball
            var fireDirection = _firePoint.forward;
            fireDirection = Quaternion.AngleAxis(Random.Range(-_cannonSettings._xAngleOffsetMin,-_cannonSettings._xAngleOffsetMax) / _cannonSettings._accuracyMultiplier, _firePoint.right) * fireDirection;
            fireDirection = Quaternion.AngleAxis(Random.Range(-_cannonSettings._yAngleOffsetMin,-_cannonSettings._yAngleOffsetMax) / _cannonSettings._accuracyMultiplier, _firePoint.up) * fireDirection;
            spawnedCannonBall.Rigidbody.AddForce(fireDirection * _cannonSettings._fireSpeedPower, ForceMode.VelocityChange);
        
            // Spawn effect
            _fireEffect.Play(true);
        }
    }
}