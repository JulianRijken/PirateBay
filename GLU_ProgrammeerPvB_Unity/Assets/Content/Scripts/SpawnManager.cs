using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private Vector2 _size;
    [SerializeField] private int _pointsPerSide;
    [SerializeField] private float _checkRadius;
    [SerializeField] private LayerMask _layerMask;

    public Vector3[] GetAvailableSpawnPoints()
    {
        var availableSpawnPoints = new List<Vector3>();

        for (var x = 0; x < _pointsPerSide; x++)
        {
            for (var y = 0; y < _pointsPerSide; y++)
            {
                var position = new Vector3((x * _size.x / (_pointsPerSide - 1)) - _size.x / 2f, 0f, (y * _size.y / (_pointsPerSide - 1)) - _size.y / 2f);

                var hitColliders = Physics.OverlapSphere(position, _checkRadius, _layerMask);

                if(hitColliders.Length > 0) 
                    continue;
                
                availableSpawnPoints.Add(position);
            }
        }

        return availableSpawnPoints.ToArray();
    }

#if UNITY_EDITOR
    

    [SerializeField] private bool _visualize;

    private void OnDrawGizmos()
    {
        if(!_visualize)
            return;
        
        Gizmos.DrawWireCube(transform.position,new Vector3(_size.x,0,_size.y));

        for (var x = 0; x < _pointsPerSide; x++)
        {
            for (var y = 0; y < _pointsPerSide; y++)
            {
                var position = new Vector3((x * _size.x / (_pointsPerSide - 1)) - _size.x / 2f, 0f, (y * _size.y / (_pointsPerSide - 1)) - _size.y / 2f);

                var hitColliders = Physics.OverlapSphere(position, _checkRadius, _layerMask);

                Gizmos.color = hitColliders.Length > 0 ? Color.red : Color.green;
                Gizmos.DrawWireSphere(position,_checkRadius);
            }
        }
    }
#endif
}
