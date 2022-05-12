using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MissionManager : MonoBehaviour
{
    
    private EnemyShipBase[] _enemyPrefabs;
    private EnemyShipBase[] _enemys;
    
    private List<IslandBase> _missionIslands;


    private bool _inMission;
    private int _targetIslandIndex;

    public Action<IslandBase> OnMissionFinished;

    public static MissionManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }


    public void StartMission(OutpostIsland startOutpost, int treasureIslands)
    {

        // Clear list
        _missionIslands = new List<IslandBase>();
        
        // Fill list with random treasure islands
        var treasureIslandOptions = GameManager.TreasureIslands.ToList();
        for (var i = 0; i < treasureIslands; i++)
        {
            var randomIslandFromList = treasureIslandOptions[Random.Range(0, treasureIslandOptions.Count)];
            _missionIslands.Add(randomIslandFromList);
            treasureIslandOptions.Remove(randomIslandFromList);
        }

        // Add a random outpost as the last island
        var outpostIslandOptions = GameManager.OutpostIslands.ToList();
        var randomOutpost = outpostIslandOptions[Random.Range(0, outpostIslandOptions.Count)];
        _missionIslands.Add(randomOutpost);

        
        // Set the target island to the first treasure island
        _targetIslandIndex = 0;
        _missionIslands[_targetIslandIndex].OnPlayerVisitIsland += OnTargetIslandVisited;



        // Spawn all enemies


        // Position player at start outpost

        // Transition camera

        _inMission = true;
    }

    public void OnTargetIslandVisited()
    {
        if (!_inMission)
            return;

        // Unsubscribe the old island
        _missionIslands[_targetIslandIndex].OnPlayerVisitIsland -= OnTargetIslandVisited;

        
        // Is player at last island?
        if (_targetIslandIndex == _missionIslands.Count - 1)
        {
            EndMission();
        }
        else
        {
            _targetIslandIndex++;
            _missionIslands[_targetIslandIndex].OnPlayerVisitIsland += OnTargetIslandVisited;
        }
    }

    private void EndMission()
    {
        OnMissionFinished?.Invoke(_missionIslands[_targetIslandIndex]);
        _inMission = false;
        
        // Remove all enemies
    }
}
