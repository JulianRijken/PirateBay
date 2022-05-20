using System;
using UnityEngine;

/// <summary>
/// The checkpoint triggers and event when the player enters it and handles all the effects by itself
/// </summary>
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

    /// <summary>
    /// Sets the checkpoint active 
    /// </summary>
    /// <param name="active"></param>
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
    
    /// <summary>
    /// Check if the collider is the player and calls the events
    /// </summary>
    /// <param name="other"></param>
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
