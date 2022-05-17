using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class IslandBase : MonoBehaviour
{
    [SerializeField] protected string _islandName;
    public Action OnPlayerVisitIsland;

    [SerializeField] protected CheckPoint _checkPoint;
    [SerializeField] protected CinemachineVirtualCamera _islandCamera;
    public CheckPoint CheckPoint => _checkPoint;
    public CinemachineVirtualCamera IslandCamera => _islandCamera;

    public Vector2 TargetLocation => new(_checkPoint.transform.position.x, _checkPoint.transform.position.z);
    
    protected void Awake()
    {
        if (!_checkPoint)
            _checkPoint = GetComponentInChildren<CheckPoint>();
            
        _checkPoint.OnCheckPointEnter += OnCheckPointEntered;
    }

    private void OnCheckPointEntered()
    {
        OnPlayerVisitIsland?.Invoke();
    }
}