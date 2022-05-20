using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The compass points towards a certain target from the player position
/// </summary>
public class Compass : MonoBehaviour
{
    [SerializeField] private Transform _compassDirection;

    public Vector3 Target;
    
    private Transform _playerShip;

    private void Start()
    {
        _playerShip = GameManager.Player.transform;
    }

    /// <summary>
    /// Rotates the compass towards the target rotation
    /// </summary>
    private void LateUpdate()
    {
        if(!_playerShip)
            return;

        
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

        var targetDirection = Target - _playerShip.transform.position;

        
        var targetRotation = Quaternion.LookRotation(targetDirection).eulerAngles;
        targetRotation.x = 0f;
        targetRotation.z = 0f;
        _compassDirection.rotation = Quaternion.Euler(targetRotation);
    }
}