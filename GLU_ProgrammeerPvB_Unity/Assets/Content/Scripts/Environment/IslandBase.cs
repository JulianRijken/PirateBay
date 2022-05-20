using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

/// <summary>
/// Handles events and settings for all islands like the camera and checkpoint
/// </summary>
public class IslandBase : MonoBehaviour
{
    [SerializeField] protected string _islandName;
    public Action OnPlayerVisitIsland;

    [SerializeField] protected CheckPoint _checkPoint;
    [SerializeField] protected CinemachineVirtualCamera _islandCamera;
    public CheckPoint CheckPoint => _checkPoint;
    public CinemachineVirtualCamera IslandCamera => _islandCamera;

    /// <summary>
    /// Returns the target location (Vector2)
    /// </summary>
    public Vector2 TargetLocation => new(_checkPoint.transform.position.x, _checkPoint.transform.position.z);
    
    /// <summary>
    /// Subscribes to the checkpoint enter event
    /// </summary>
    protected void Awake()
    {
        if (!_checkPoint)
            _checkPoint = GetComponentInChildren<CheckPoint>();
            
        _checkPoint.OnCheckPointEnter += OnCheckPointEntered;
    }

    /// <summary>
    /// Passes the checkpoint event to the player visited event
    /// </summary>
    private void OnCheckPointEntered()
    {
        OnPlayerVisitIsland?.Invoke();
    }
}