using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _maxSpeed;

    private Attributes _attributes;
    private NavMeshAgent _agent;

    public delegate void OnRemovedEvent(Enemy enemyRemoved);
    public event OnRemovedEvent OnRemoved;
    
    
    private void Awake()
    {
        _attributes = GetComponent<Attributes>();
        _agent = GetComponent<NavMeshAgent>();

        _attributes.OnDeath += OnDeath;
    }

    private void OnDeath(GameObject instigator)
    {
        OnRemoved?.Invoke(this);
        Destroy(gameObject);
    }

    private void Update()
    {
        var destination = GameManager.PlayerController.transform.position;
        _agent.SetDestination(destination);
    }
}
