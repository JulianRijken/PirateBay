using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{

    [SerializeField] private CannonBall _cannonBall;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private ParticleSystem _fireEffect;
    [SerializeField] private float _firePower;
    [SerializeField] private float _xAngleOffsetMin;
    [SerializeField] private float _xAngleOffsetMax;
    [SerializeField] private float _yAngleOffsetMin;
    [SerializeField] private float _yAngleOffsetMax;

    [SerializeField] private float _maxRandomFireDelay;
    
    
    public void Fire()
    {
        StopCoroutine(FireEnumerator());
        StartCoroutine(FireEnumerator());
        
        IEnumerator FireEnumerator()
        {
            yield return new WaitForSeconds(Random.value * _maxRandomFireDelay);
            
            // Spawn cannon ball
            var spawnedCannonBall = Instantiate(_cannonBall, _firePoint.position, _firePoint.rotation);
        
            // Apply force to cannon ball
            var fireDirection = _firePoint.forward;
            fireDirection = Quaternion.AngleAxis(Random.Range(-_xAngleOffsetMin,-_xAngleOffsetMax), _firePoint.right) * fireDirection;
            fireDirection = Quaternion.AngleAxis(Random.Range(-_yAngleOffsetMin,-_yAngleOffsetMax), _firePoint.up) * fireDirection;
            spawnedCannonBall.Rigidbody.AddForce(fireDirection * _firePower, ForceMode.VelocityChange);
        
            // Spawn effect
            _fireEffect.Play(true);
        }
    }
}