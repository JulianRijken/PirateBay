using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject Instigator;

    [SerializeField] private float _moveSpeed;
    
    private void Update()
    {
        var velocity = transform.forward * (_moveSpeed * Time.deltaTime);
        transform.position += velocity;
    }
}
