using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
    [SerializeField] private Transform _compassDirection;

    public Vector3 Target;
    
    private Transform _playerShip;

    private void Start()
    {
        _playerShip = GameManager.Player.transform;
    }

    private void Update()
    {
        if(!_playerShip)
            return;

        Debug.DrawLine(_playerShip.transform.position,Target, Color.green);
        
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

        var targetDirection = Target - _playerShip.transform.position;

        
        var targetRotation = Quaternion.LookRotation(targetDirection).eulerAngles;
        targetRotation.x = 0f;
        targetRotation.z = 0f;
        _compassDirection.rotation = Quaternion.Euler(targetRotation);
    }
}