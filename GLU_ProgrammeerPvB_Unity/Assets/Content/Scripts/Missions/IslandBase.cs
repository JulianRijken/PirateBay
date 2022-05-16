using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandBase : MonoBehaviour
{
    [SerializeField] protected string _islandName;
    public Action OnPlayerVisitIsland;

    [SerializeField] protected CheckPoint _checkPoint;
    public CheckPoint CheckPoint => _checkPoint;

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