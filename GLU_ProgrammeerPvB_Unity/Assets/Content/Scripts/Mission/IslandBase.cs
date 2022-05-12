using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandBase : MonoBehaviour
{
    [SerializeField] protected string _islandName;
    public Action OnPlayerVisitIsland;
    
    //[SerializeField] protected IslandType _islandType;

    // protected enum IslandType
    // {
    //     Outpost,
    //     Treasure, 
    //     Fort
    // }
}

