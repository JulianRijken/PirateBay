using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{

    [SerializeField] private AnimationCurve _spawnIntervalCruve;
    [SerializeField] private AnimationCurve _maxEnemysCruve;
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private Vector2 _spawnRadiusRange;
    [SerializeField] private float _maxDistanceFromNavMesh;
    [SerializeField] private int _spawnMask;
    
    private float _gameTime;
    private bool _enabled = true;
    
    private List<Enemy> _enemies = new List<Enemy>();

    private void Start()
    {
        StartCoroutine(EnemySpawnInterval());
    }

    private void Update()
    {
        if(_enabled)
            _gameTime += Time.deltaTime;
    }

    private IEnumerator EnemySpawnInterval()
    {
        while (true)
        {
            yield return new WaitUntil(() => _enabled);
            
            // Wait for spawn rate
            var waitTime = _spawnIntervalCruve.Evaluate(_gameTime);
            yield return new WaitForSeconds(waitTime);

            var maxEnemys = _maxEnemysCruve.Evaluate(_gameTime);
            if (_enemies.Count < maxEnemys)
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
        var playerPosition = GameManager.PlayerController.transform.position;
        var randomPoint = playerPosition + Random.onUnitSphere *  Random.Range(_spawnRadiusRange.x, _spawnRadiusRange.y);

        if (!NavMesh.SamplePosition(randomPoint, out var hit, _maxDistanceFromNavMesh, 1))
            return false;
        
        var spawnLocation = hit.position;
        
        
        // Spawn Enemy
        var enemy = Instantiate(_enemyPrefab, spawnLocation, Quaternion.identity);
        enemy.OnRemoved += OnEnemyRemoved;
        _enemies.Add(enemy);

        return true;
    }

    private void OnEnemyRemoved(Enemy enemyremoved)
    {
        _enemies.Remove(enemyremoved);
    }

    private void OnDrawGizmos()
    {
        var pos = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, 0));
        Gizmos.DrawSphere(pos,1f);
    }
}
