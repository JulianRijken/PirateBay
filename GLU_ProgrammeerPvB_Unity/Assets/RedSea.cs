using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class RedSea : MonoBehaviour
{
    [SerializeField] private float _damage;
    [SerializeField] private float _damageInterval;
    [SerializeField] private float _redSeaDistance;
    [SerializeField] private float _smoothDistance;
    [SerializeField] private AnimationCurve _damageOverWeight;
    
    [SerializeField] private Vector3 _mapCenter;
    [SerializeField] private PlayerShipController _player;
    [SerializeField] private PostProcessVolume _volume;

    private float _damageTimer = 0f;





    private void Update()
    {

        var distance = Vector3.Distance(_mapCenter, _player.transform.position);
        var weight = Mathf.Clamp01(Mathf.Abs(_redSeaDistance - distance) / _smoothDistance);
        if (distance < _redSeaDistance)
            weight = 0f;
        
        _volume.weight = weight;

        if (distance > _redSeaDistance)
        {
            if (_damageTimer > _damageInterval)
            {
                _player.OnHealthChange(-_damage * _damageOverWeight.Evaluate(weight));
                _damageTimer = 0f;
            }
            else
            {
                _damageTimer += Time.deltaTime;
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(_mapCenter,_redSeaDistance);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_mapCenter,_redSeaDistance + _smoothDistance);
    }
#endif
}
