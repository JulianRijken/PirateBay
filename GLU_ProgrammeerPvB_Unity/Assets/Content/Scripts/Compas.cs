using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compas : MonoBehaviour
{
    [SerializeField] private Transform _compasDirection;

    [SerializeField] private Vector3 _target;
    [SerializeField] private Transform _playerShip;

    private void Update()
    {
        Debug.DrawLine(_playerShip.transform.position,_target, Color.green);
        
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

        var targetDirection = _target - _playerShip.transform.position;

        Debug.DrawRay(_playerShip.position,targetDirection,Color.red);
        
        var targetRotation = Quaternion.LookRotation(targetDirection).eulerAngles;
        targetRotation.x = 0f;
        targetRotation.z = 0f;
        _compasDirection.rotation = Quaternion.Euler(targetRotation);
    }
}