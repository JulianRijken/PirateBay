using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _maxSpeed;

    private NavMeshAgent _agent;

    
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }
    
    private void Update()
    {
        var destination = GameManager.PlayerController.transform.position;
        _agent.SetDestination(destination);
    }
}
