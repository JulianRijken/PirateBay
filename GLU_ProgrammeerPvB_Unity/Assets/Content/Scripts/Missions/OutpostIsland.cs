using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutpostIsland : IslandBase
{
    [SerializeField] private Transform _playerStart;
    public Transform PlayerStart => _playerStart;
}