using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{

    [SerializeField] private Tile[] _tilePrefabs;
    [SerializeField] private float _worldSpeed;
    [SerializeField] private float _destroyPoint;
    [SerializeField] [Min(3)] private int _tileCount;

    private List<Tile> _activeTiles;
    private Tile _lastSpawnedTile;


    private void Start()
    {
        _activeTiles = new List<Tile>();
        
        // Spawns standard tiles
        for (int i = 0; i < _tileCount; i++)
        {
            SpawnNewTile();
        }
    }

    private void Update()
    {
        HandleTiles();
    }

    private void HandleTiles()
    {
        var tilesToRemove = new List<Tile>();

        foreach (var tile in _activeTiles)
        {
            tile.transform.position += Vector3.back * (Time.deltaTime * _worldSpeed);

            if (tile.transform.position.z < transform.position.z + _destroyPoint)
            {
                tilesToRemove.Add(tile);
            }
        }

        foreach (var tileToRemove in tilesToRemove)
        {
            _activeTiles.Remove(tileToRemove);
            Destroy(tileToRemove.gameObject);
            SpawnNewTile();
        }
    }
    

    private void SpawnNewTile()
    {
        var randomTilePrefab = _tilePrefabs[Random.Range(0, _tilePrefabs.Length)];
        var spawnedTile = Instantiate(randomTilePrefab,transform.position,Quaternion.identity);
        

        var spawnPosition =  (_lastSpawnedTile ? _lastSpawnedTile.EndPoint.position : transform.position).z + Mathf.Abs(spawnedTile.EndPoint.localPosition.z);
        var originalPosition = spawnedTile.transform.position;
        spawnedTile.transform.position = new Vector3(originalPosition.x,originalPosition.y,spawnPosition);

        _lastSpawnedTile = spawnedTile;
        _activeTiles.Add(spawnedTile);
    }
}
