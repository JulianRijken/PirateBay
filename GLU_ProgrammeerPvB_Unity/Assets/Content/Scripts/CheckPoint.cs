using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CheckPoint : MonoBehaviour
{
    [SerializeField] private ParticleSystem _checkpointEffect;
    [SerializeField] private ParticleSystem _collectEffect;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private bool _autoDeactivate;

    private bool _active;
    public Action OnCheckPointEnter;


    
    private void Start()
    {
        _active = false;
    }

    public void SetCheckpointActive(bool active)
    {
        _active = active;
        
        if (active)
        {
            _checkpointEffect.Play();
        }
        else
        {
            _checkpointEffect.Stop();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(!_active)
            return;
        
        if(!other.CompareTag("Player"))
            return;

        if (_audioSource)
        {
            _audioSource.Play();
        }
        
        _collectEffect.Play();
        
        OnCheckPointEnter?.Invoke();
        
        if(_autoDeactivate)
            SetCheckpointActive(false);
    }
}
