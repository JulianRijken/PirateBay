using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Manages the missions for the game
/// </summary>
public class MissionManager : MonoBehaviour
{
    
    [SerializeField] private EnemyShipBase _enemyBigPrefab;
    [SerializeField] private EnemyShipBase _enemySmallPrefab;
    [SerializeField] private ExplosiveBarrel _explosiveBarrelPrefab;
    [SerializeField] private Pickup[] _pickupPrefab;
    
    [SerializeField] private int _pickupCount;
    [SerializeField] private int _enemyBigCount;
    [SerializeField] private int _enemySmallCount;
    [SerializeField] private int _explosiveBarrelCount;
    
    [SerializeField] private SpawnManager _spawnManager;
    
    [SerializeField] private Compass _compass;

    private float _gameTimer;
    private float _missionLength;
    private int _shipsKilled;

    private Vector3[] _spawnOptions;
    
    private List<IslandBase> _missionIslands;
    private OutpostIsland _startOutpost;
    
    
    private bool _inMission;
    private int _targetIslandIndex;

    
    public Action<OutpostIsland, bool> OnMissionFinished;
    public Action<float> OnTimerChange;
    
    public static bool InMission => Instance._inMission;
    public static int ShipsKilled => Instance._shipsKilled;
    public static float TimeInMission => Instance._missionLength - Instance._gameTimer;
    
    public static MissionManager Instance { get; private set; }
    

    /// <summary>
    /// Subscribes tho set events.
    /// Creates an Instance.
    /// Gets all spawn points.
    /// Creates the mission islands list.
    /// </summary>
    private void Awake()
    {
        Instance = this;
        _spawnOptions = _spawnManager.GetAvailableSpawnPoints();

        GameManager.Player.OnShipSunken += OnPlayerShipSunk;
        EnemyShipBase.OnShipSunkGlobal += OnEnemyShipSunk;

        _missionIslands = new List<IslandBase>();
    }

    /// <summary>
    /// Updates the timer when the mission is in progress
    /// </summary>
    private void Update()
    {
        if (_inMission)
        {
            _gameTimer = Mathf.Max(0, _gameTimer - Time.deltaTime);
            OnTimerChange?.Invoke(_gameTimer);
            
            if (_gameTimer <= 0f)
            {
                OnTimerOver();
            }
        }
    }

    
    /// <summary>
    /// Ends the mission when the timer is over
    /// </summary>
    private void OnTimerOver()
    {
        EndMission(false);
    }

    /// <summary>
    /// Ends the mission when the player sunk
    /// </summary>
    private void OnPlayerShipSunk()
    {
        // Game lost :(
        EndMission(false);
    }
    
    
    /// <summary>
    /// Counts up the ships killed
    /// </summary>
    private void OnEnemyShipSunk()
    {
        _shipsKilled++;
    }
    
    /// <summary>
    /// If all target islands are visited this function ends the mission or sets the target mission to the next island
    /// </summary>
    private void OnTargetIslandVisited()
    {
        if (!_inMission)
            return;

        // Unsubscribe the old island
        _missionIslands[_targetIslandIndex].OnPlayerVisitIsland -= OnTargetIslandVisited;

        // Is player at last island?
        if (_targetIslandIndex == _missionIslands.Count - 1)
        {
            // Game won!
            EndMission(true);
        }
        else
        {
            Debug.Log("Moving To Next Island");
            
            // Update index
            _targetIslandIndex++;
            _missionIslands[_targetIslandIndex].OnPlayerVisitIsland += OnTargetIslandVisited;
            _missionIslands[_targetIslandIndex].CheckPoint.SetCheckpointActive(true);
            
            // Update player treasure
            GameManager.Player.SetTreasures(_targetIslandIndex);
            
            // Update compass
            _compass.Target = _missionIslands[_targetIslandIndex].CheckPoint.transform.position;
        }
    }
    
    
    /// <summary>
    /// Starts the mission for the game
    /// </summary>
    /// <param name="startOutpost"></param>
    /// <param name="treasureIslands"></param>
    /// <param name="missionLengthInMinutes"></param>
    public void StartMission(OutpostIsland startOutpost, int treasureIslands, int missionLengthInMinutes)
    {
        // Clear list
        _missionIslands.Clear();
        
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
        
        // Update island checkpoints
        foreach (var missionIsland in _missionIslands)
            missionIsland.CheckPoint.SetCheckpointActive(false);
        
        _missionIslands[_targetIslandIndex].CheckPoint.SetCheckpointActive(true);

        // Enable compass
        _compass.gameObject.SetActive(true);
        _compass.Target = _missionIslands[_targetIslandIndex].CheckPoint.transform.position;
        
        // Spawn all objects in game
        SpawnObjects();
        

        // Position player at start outpost
        if (startOutpost.PlayerStart)
        {
            // Position Player
            var playerTransform = GameManager.Player.transform;
            playerTransform.position = startOutpost.PlayerStart.position;
            playerTransform.rotation = startOutpost.PlayerStart.rotation;
            
            // Make sure the player ship is not sunken
            GameManager.Player.UnSinkShip();
        }
        else
        {
            Debug.LogWarning(startOutpost.name + " has no player start");
        }
        
        // Reset player treasure
        GameManager.Player.SetTreasures(0);
        
        _shipsKilled = 0;
        
        _startOutpost = startOutpost;
        
        _missionLength = missionLengthInMinutes * 60f;
        _gameTimer = _missionLength;
        
        _inMission = true;
        
        
        void SpawnObjects()
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
        
            for (var i = 0; i < _explosiveBarrelCount; i++)
            {
                if(spawnPoints.Count == 0)
                    return;
            
                var randomIndex = Random.Range(0, spawnPoints.Count);
                var spawnPoint = spawnPoints[randomIndex];
                spawnPoints.RemoveAt(randomIndex);

                Instantiate(_explosiveBarrelPrefab, spawnPoint, Quaternion.Euler(0, Random.Range(0f, 360f), 0));
            }
        }
    }
    
    /// <summary>
    /// Ends the mission for the game
    /// </summary>
    /// <param name="gameWon"></param>
    private void EndMission(bool gameWon)
    {
        Debug.Log("Mission End");
        _inMission = false;   
        
        // Remove checkpoint for when the player never reaches it
        _missionIslands[_targetIslandIndex].CheckPoint.SetCheckpointActive(false);
        
        // Remove all enemies
        var enemyShips = FindObjectsOfType<EnemyShipBase>();
        foreach (var enemy in enemyShips)
        {
            Destroy(enemy.gameObject);
        }
        
        var pickups = FindObjectsOfType<Pickup>();
        foreach (var pickup in pickups)
        {
            Destroy(pickup.gameObject);
        }
        
        var explosiveBarrels = FindObjectsOfType<ExplosiveBarrel>();
        foreach (var explosiveBarrel in explosiveBarrels)
        {
            Destroy(explosiveBarrel.gameObject);
        }
        
        _compass.gameObject.SetActive(false);

        var targetOutpost = (OutpostIsland)_missionIslands.Last();
        OnMissionFinished?.Invoke(targetOutpost,gameWon);
        
    }

    /// <summary>
    /// Returns the alpha of the mission (0f is the start outpost island - 1f is the end outpost island)
    /// </summary>
    public float GetMissionAlpha()
    {
        if (!_inMission)
            return 0f;
        
        var addedDistance = Mathf.Abs(1 - GetAlphaToTargetIsland()) / _missionIslands.Count;
        var alpha = Mathf.Clamp01((float)_targetIslandIndex / _missionIslands.Count + addedDistance);

        return alpha;

        float GetAlphaToTargetIsland()
        {
            var targetIslandAlpha = 0f;

            if (_inMission)
            {
                var lastIsland = _targetIslandIndex > 0 ? _missionIslands[_targetIslandIndex - 1] : _startOutpost;

                var targetIsland = _missionIslands[_targetIslandIndex];

                var distanceBetweenIslands = Vector2.Distance(lastIsland.TargetLocation, targetIsland.TargetLocation);

                var vector3Position = GameManager.Player.transform.position;
                var playerPosition = new Vector2(vector3Position.x, vector3Position.z);
                var distanceBetweenPlayerAndTargetIsland = Vector2.Distance(playerPosition, targetIsland.TargetLocation);

                if (distanceBetweenPlayerAndTargetIsland > distanceBetweenIslands)
                    return 1f;
            
            
                targetIslandAlpha = Mathf.Max(0f,distanceBetweenPlayerAndTargetIsland / distanceBetweenIslands);
            }
        
            return targetIslandAlpha;
        }
    }
}
