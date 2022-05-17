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
    private bool _active;
    
    private void Awake()
    {
        GameManager.OnGameOver += OnGameOver;
        GameManager.OnGameStart += OnGameStart;
    }

    private void OnGameStart()
    {
        _active = true;
    }

    private void OnGameOver(bool gameWon)
    {
        _active = false;
        _volume.weight = 0f;
    }

    private void Update()
    {
        if(!_active)
            return;
        
        var distance = Vector3.Distance(_mapCenter, _player.transform.position);

        if (distance > _redSeaDistance)
        {
            var weight = Mathf.Clamp01(Mathf.Abs(_redSeaDistance - distance) / _smoothDistance);
            _volume.weight = weight;


            if (_damageTimer > _damageInterval)
            {
                _player.OnHealthChange(-_damage * _damageOverWeight.Evaluate(weight));
                _damageTimer = 0f;
            }
            else
            {
                _damageTimer += Time.deltaTime;
            }
            
            Debug.Log("Damage: " + weight);
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
