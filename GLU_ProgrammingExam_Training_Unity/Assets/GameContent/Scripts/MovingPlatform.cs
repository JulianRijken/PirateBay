using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Vector3[] _platformPositions;
    private float _platformMoveSpeed = 1f;
    private float _positionsAlpha;
    private bool _direction;
    
    private void Start()
    {
        transform.position = _platformPositions[0];
    }

    private void Update()
    {
        if (_direction)
        {
            _positionsAlpha += _platformMoveSpeed * Time.deltaTime;
            if (_positionsAlpha >= 1)
            {
                _direction = false;
                _positionsAlpha = 1;
            }
        }
        else
        {
            _positionsAlpha -= _platformMoveSpeed * Time.deltaTime;
            if (_positionsAlpha <= 0)
            {
                _direction = true;
                _positionsAlpha = 0;
            }
        }
        
        transform.position = Vector3.Lerp(_platformPositions[0], _platformPositions[1], _positionsAlpha);
    }
}
