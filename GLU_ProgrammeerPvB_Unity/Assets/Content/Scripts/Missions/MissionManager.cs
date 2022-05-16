using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MissionManager : MonoBehaviour
{
    
    [SerializeField] private EnemyShipBase _enemyBigPrefab;
    [SerializeField] private EnemyShipBase _enemySmallPrefab;
    [SerializeField] private Pickup[] _pickupPrefab;
    
    [SerializeField] private int _pickupCount;
    [SerializeField] private int _enemyBigCount;
    [SerializeField] private int _enemySmallCount;
    
    [SerializeField] private SpawnManager _spawnManager;
    
    
    private Vector3[] _spawnOptions;
    
    private List<IslandBase> _missionIslands;
    
    private bool _inMission;
    private int _targetIslandIndex;

    public Action<IslandBase> OnMissionFinished;

    public static bool InMission => Instance._inMission;
    public static MissionManager Instance { get; private set; }


    private void Awake()
    {
        Instance = this;
        _spawnOptions = _spawnManager.GetAvailableSpawnPoints();
    }


    public void StartMission(OutpostIsland startOutpost, int treasureIslands)
    {
        Debug.Log("Starting Mission");
        
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
        _missionIslands[_targetIslandIndex].CheckPoint.SetCheckpointActive(true);


        SpawnObjects();
        

        // Position player at start outpost
        if (startOutpost.PlayerStart)
        {
            var playerTransform = GameManager.Player.transform;
            playerTransform.position = startOutpost.PlayerStart.position;
            playerTransform.rotation = startOutpost.PlayerStart.rotation;
        }
        else
        {
            Debug.LogWarning(startOutpost.name + " has no player start");
        }
        
        // Transition camera
        
        _inMission = true;
    }

    private void SpawnObjects()
    {
        // Spawn all enemies & pickups
        var spawnPoints = _spawnOptions.ToList();
        
        for (var i = 0; i < _enemyBigCount; i++)
        {
            if(spawnPoints.Count == 0)
                return;
            
            var randomIndex = Random.Range(0, spawnPoints.Count);
            var spawnPoint = spawnPoints[randomIndex];
            spawnPoints.RemoveAt(randomIndex);

            Instantiate(_enemyBigPrefab, spawnPoint, Quaternion.Euler(0, Random.Range(0f, 360f), 0));
        }
        
        for (var i = 0; i < _enemySmallCount; i++)
        {
            if(spawnPoints.Count == 0)
                return;
            
            var randomIndex = Random.Range(0, spawnPoints.Count);
            var spawnPoint = spawnPoints[randomIndex];
            spawnPoints.RemoveAt(randomIndex);

            Instantiate(_enemySmallPrefab, spawnPoint, Quaternion.Euler(0, Random.Range(0f, 360f), 0));
        }

        for (var i = 0; i < _pickupCount; i++)
        {
            if(spawnPoints.Count == 0)
                return;
            
            var randomIndex = Random.Range(0, spawnPoints.Count);
            var spawnPoint = spawnPoints[randomIndex];
            spawnPoints.RemoveAt(randomIndex);

            Instantiate(_pickupPrefab[Random.Range(0,_pickupPrefab.Length)], spawnPoint, Quaternion.Euler(0, Random.Range(0f, 360f), 0));
        }
    }

    private void OnTargetIslandVisited()
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
            Debug.Log("Moving To Next Island");
            
            _targetIslandIndex++;
            _missionIslands[_targetIslandIndex].OnPlayerVisitIsland += OnTargetIslandVisited;
            _missionIslands[_targetIslandIndex].CheckPoint.SetCheckpointActive(true);
        }
    }

    private void EndMission()
    {
        Debug.Log("Mission End");
        
        // Remove all enemies
        var enemys = FindObjectsOfType<EnemyShipBase>();
        foreach (var enemy in enemys)
        {
            Destroy(enemy.gameObject);
        }
        
        var pickups = FindObjectsOfType<Pickup>();
        foreach (var pickup in pickups)
        {
            Destroy(pickup.gameObject);
        }
        
        OnMissionFinished?.Invoke(_missionIslands[_targetIslandIndex]);
        _inMission = false;
    }

    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(_inMission)
            Gizmos.DrawLine(GameManager.Player.transform.position,_missionIslands[_targetIslandIndex].CheckPoint.transform.position);
    }
#endif
}
