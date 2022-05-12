using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CloudRotate : MonoBehaviour
{
    [SerializeField] private float _speed;
    
    private void Update()
    {
        transform.Rotate(Vector3.up * _speed);
    }
}
