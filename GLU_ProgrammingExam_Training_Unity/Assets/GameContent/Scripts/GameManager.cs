using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{

    [SerializeField] private Transform[] _playerSpawnTransforms;
    [SerializeField] private PlayerController _playerPrefab;
    private PlayerController _playerController;
    
    private void Start()
    {
        StartGame();
    }
    

    /// <summary>
    /// Spawns the player and starts the game
    /// </summary>
    private void StartGame()
    {
        // Check if player already exists
        PlayerController[] foundPlayerControllers = FindObjectsOfType<PlayerController>();
        for (int i = 0; i < foundPlayerControllers.Length; i++)
        {
            if (i == 0)
            {
                _playerController = foundPlayerControllers[i];
                continue;
            }
            
            Destroy(foundPlayerControllers[i].gameObject);
        }
        
        // If no player has been found spawn one
        if (_playerController == null)
        {
            _playerController = Instantiate(_playerPrefab);
        }
        
        // Move Player
        Vector3 spawnLocation = _playerSpawnTransforms[Random.Range(0, _playerSpawnTransforms.Length)].position;
        _playerController.transform.position = spawnLocation;
        Debug.Log("Player Spawned");
    }
}
