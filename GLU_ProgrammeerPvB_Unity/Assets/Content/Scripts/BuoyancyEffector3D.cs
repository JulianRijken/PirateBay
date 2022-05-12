using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BuoyancyEffector3D : MonoBehaviour
{
    
    [SerializeField] private Transform[] _floatingPoints;
    [SerializeField] private float _waterHeight = 0f;
    [SerializeField] private float _floatingPower;
    
    [SerializeField] private float _underWaterDrag = 5f;
    [SerializeField] private float _underWaterAngularDrag = 2f;
    
    private float _normalAngularDrag;
    private float _normalDrag;
    
    private bool _underwater;
    private Rigidbody _rigidbody;

    public bool IsUnderWater => _underwater;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _normalAngularDrag = _rigidbody.angularDrag;
        _normalDrag = _rigidbody.drag;
    }

    private void FixedUpdate()
    {
        HandleBuoyancy();
    }
    
    
    private void HandleBuoyancy()
    {
        foreach (var floatingPoint in _floatingPoints)
        {
            var waterDifference = floatingPoint.position.y - _waterHeight;
            
            // If floating point is under water
            if (waterDifference < 0)
            {
                
                _rigidbody.AddForceAtPosition(Vector3.up * _floatingPower * Mathf.Abs(waterDifference), floatingPoint.position, ForceMode.Force);

                if (_underwater) 
                    continue;
                
                SetUnderWater(true);
                return;
            }
        }
        
        SetUnderWater(false);
    }



    private void SetUnderWater(bool underwater)
    {
        _underwater = underwater;

        if (underwater)
        {
            _rigidbody.drag = _underWaterDrag;
            _rigidbody.angularDrag = _underWaterAngularDrag;
        }
        else
        {
            _rigidbody.drag = _normalDrag;
            _rigidbody.angularDrag = _normalAngularDrag;
        }
    }
}
