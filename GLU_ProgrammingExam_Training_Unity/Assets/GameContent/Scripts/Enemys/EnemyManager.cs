using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{

    [SerializeField] private bool _enabled = true;
    [SerializeField] private AnimationCurve _spawnIntervalCruve;
    [SerializeField] private AnimationCurve _maxEnemysCruve;
    [SerializeField] private SpawnOption[] _enemySpawnOptions;
    [SerializeField] private Vector2 _spawnRadiusRange;
    [SerializeField] private float _maxDistanceFromNavMesh;
    [SerializeField] private int _spawnMask;

    #if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private Enemy testEnemy;
    #endif
    
    private float _gameTime;
    
    private List<Enemy> _enemies = new List<Enemy>();

    private void Awake()
    {
        GameManager.OnGameStart += KillEnemys;
        GameManager.OnGameRestart += KillEnemys;
    }

    private void KillEnemys()
    {
        foreach (var enemy in _enemies)
        {
            enemy.Die();
        }
    }

    private void Start()
    {
        #if UNITY_EDITOR
        if (testEnemy)
        {
            Instantiate(testEnemy, Vector3.zero, Quaternion.identity);
            return;
        }
        #endif
        
        StartCoroutine(EnemySpawnInterval());
    }

    private void Update()
    {
        if(GameManager.IsState(GameManager.GameStateEnum.Playing))
            _gameTime += Time.deltaTime;
    }
    
    [System.Serializable]
    private struct SpawnOption
    {
        public Enemy enemyPrefeb;
        public int SpawnChance;
    }

    private IEnumerator EnemySpawnInterval()
    {
        while (true)
        {
            yield return new WaitUntil(() => GameManager.IsState(GameManager.GameStateEnum.Playing));
            
            // Wait for spawn rate
            var waitTime = _spawnIntervalCruve.Evaluate(_gameTime);
            yield return new WaitForSeconds(waitTime);

            var maxEnemys = _maxEnemysCruve.Evaluate(_gameTime);
            
            if (_enabled && _enemies.Count < maxEnemys)
            {
                if (!SpawnEnemy())
                {
                    Debug.LogWarning("Enemy Can't be spawned");
                }
            }
        }
    }

    private bool SpawnEnemy()
    {
        // Get random position
        var randomOffset = Random.insideUnitCircle *  Random.Range(_spawnRadiusRange.x, _spawnRadiusRange.y);

        var player = GameManager.ActivePlayerController;
        if (!player) return false;
        
        var playerPosition = player.Location;
        var randomPoint = playerPosition + new Vector3(randomOffset.x,0,randomOffset.y);

        Debug.DrawLine(playerPosition,randomPoint,Color.blue, 3f);

        if (!NavMesh.SamplePosition(randomPoint, out var hit, _maxDistanceFromNavMesh, 1))
            return false;
        
        var spawnLocation = hit.position;
        Debug.DrawLine(playerPosition,spawnLocation,Color.green, 5f);
        
        // Spawn Enemy
        var enemy = Instantiate(GetRandomEnemy(), spawnLocation, Quaternion.identity);
        enemy.OnRemoved += OnEnemyRemoved;
        _enemies.Add(enemy);
        Debug.Log($"Enemy Spawned: {enemy.name}");

        return true;
    }

    private Enemy GetRandomEnemy()
    {
        // Get the total weight
        var totalWeight = 0f;        
        for (var i = 0; i < _enemySpawnOptions.Length; i++)
        {
            totalWeight += _enemySpawnOptions[i].SpawnChance;
        }
        
        Debug.Log(totalWeight);
        
        // Get a random weight based on the total
        var randomWeight = Random.value * totalWeight;
        
        Debug.Log(randomWeight);

        // Count up the wight until arriving at selected enemy
        var countedWeight= 0f;
        for (var i = 0; i < _enemySpawnOptions.Length; i++)
        {
            countedWeight += _enemySpawnOptions[i].SpawnChance;
            if (randomWeight < countedWeight)
                return _enemySpawnOptions[i].enemyPrefeb;
        }

        return null;
    }

    private void OnEnemyRemoved(Enemy enemyremoved)
    {
        _enemies.Remove(enemyremoved);
    }

    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var pos = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, 0));
        Gizmos.DrawSphere(pos,1f);
    }
    #endif
}
