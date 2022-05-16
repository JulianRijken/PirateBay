using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CheckPoint : MonoBehaviour
{
    [SerializeField] private ParticleSystem _checkpointEffect;
    [SerializeField] private bool _autoDeactivate;

    private bool _active = false;
    public Action OnCheckPointEnter;


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
        
        if(other.CompareTag("Player"))
            return;
        
        OnCheckPointEnter?.Invoke();
        
        if(_autoDeactivate)
            SetCheckpointActive(false);
    }
}
