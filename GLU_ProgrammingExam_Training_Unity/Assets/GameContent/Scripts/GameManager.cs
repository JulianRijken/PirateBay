using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{

    [SerializeField] private Transform[] _playerSpawnTransforms;
    [SerializeField] private PlayerController _playerPrefab;
    [SerializeField] private PlayerCamera _playerCamera;
    private PlayerController _activePlayerController;

    public static GameManager Instance;

    public static PlayerController ActivePlayerController => Instance._activePlayerController;

    private GameStateEnum _gameState = GameStateEnum.MainMenu;

    public static GameStateEnum GameState => Instance._gameState;
    
    public static event Action OnGameStart;
    public static event Action OnGameEnd;
    public static event Action OnGameRestart;
    public static event Action OnQuitToMainMenu;
    
    
    public enum GameStateEnum
    {
        MainMenu,
        Playing,
        EndScreen
    }

    public static bool IsState(GameStateEnum state)
    {
        return Instance._gameState == state;
    }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Force game to start in main menu
        QuitToMainMenu();
        Debug.Log("Forcing Game To Menu");
    }


    /// <summary>
    /// Spawns the player and starts the game
    /// </summary>
    public void StartGame()
    {
        SpawnPlayer();
        _gameState = GameStateEnum.Playing;
        OnGameStart?.Invoke();
    }

    /// <summary>
    /// Ends the game
    /// </summary>
    public void EndGame()
    {
        _gameState = GameStateEnum.EndScreen;
        OnGameEnd?.Invoke();
    }
    
    /// <summary>
    /// Brings the player back to life and restarts the game
    /// </summary>
    public void RestartGame()
    {
        Debug.LogWarning("Maybe replace with place coming back to life");
        SpawnPlayer();
        _gameState = GameStateEnum.Playing;
        OnGameRestart?.Invoke();
    }

    /// <summary>
    /// Returns the game back to the main menu
    /// </summary>
    public void QuitToMainMenu()
    {
        _gameState = GameStateEnum.MainMenu;
        OnQuitToMainMenu?.Invoke();
    }
    
    
    
    
    
    private void SpawnPlayer()
    {
        // Check if player already exists
        var foundPlayerControllers = FindObjectsOfType<PlayerController>();
        for (var i = 0; i < foundPlayerControllers.Length; i++)
        {
            if (i == 0)
            {
                _activePlayerController = foundPlayerControllers[i];
                continue;
            }
            
            Destroy(foundPlayerControllers[i].gameObject);
        }
        
        // If no player has been found spawn one
        if (_activePlayerController == null)
        {
            _activePlayerController = Instantiate(_playerPrefab);
            _playerCamera.AttachToPlayer(_activePlayerController);
        }
        
        // Move Player
        var spawnLocation = _playerSpawnTransforms[Random.Range(0, _playerSpawnTransforms.Length)].position;
        _activePlayerController.transform.position = spawnLocation;

        _activePlayerController.OnPlayerDeath += OnPlayerDeath;
    }

    private void OnPlayerDeath()
    {
        EndGame();
    }
}
