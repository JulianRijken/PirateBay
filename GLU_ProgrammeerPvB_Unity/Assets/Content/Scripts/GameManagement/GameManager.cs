using System;
using Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Manages and Handles the game and is used to string the game together. The game manager handles:
/// Info about the player,
/// Info about the islands in the map,
/// When the game starts, stops and exits,
/// </summary>
public class GameManager : MonoBehaviour
{
    
    [SerializeField] private PlayerShipController _player;
    [SerializeField] private CinemachineFreeLook _playerCamera;
    [SerializeField] private CinemachineBrain _mainCamera;

    private OutpostIsland[] _outpostIslands;
    private TreasureIsland[] _treasureIslands;
    private OutpostIsland _mainMenuIsland;

    public static PlayerShipController Player => Instance._player;
    public static OutpostIsland[] OutpostIslands => Instance._outpostIslands;
    public static TreasureIsland[] TreasureIslands => Instance._treasureIslands;
    
    public static GameManager Instance { get; private set; }


    public static Action OnGameStart;
    public static Action OnGameExitToMenu;
    public static Action<bool> OnGameOver;

    /// <summary>
    /// Gets all types of islands.
    /// Gets the player.
    /// Gets a random menu island
    /// Subscribes to the UI button events
    /// </summary>
    private void Awake()
    {
        Instance = this;

        if (_player == null)
        {
            _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerShipController>();
        }
        
        _treasureIslands =  FindObjectsOfType<TreasureIsland>();
        _outpostIslands = FindObjectsOfType<OutpostIsland>();

        _mainMenuIsland = _outpostIslands[Random.Range(0, _outpostIslands.Length)];

        
        UIManager.OnStartButtonPressed += OnStartButtonPressed;
        UIManager.OnRetryGameButtonPressed += OnRetryGameButtonPressed;
        UIManager.OnExitToMainMenuButtonPressed += OnExitToMainMenuButtonPressed;
    }

    private void Start()
    {
        var cameraTarget = _player.transform;
        _playerCamera.Follow = cameraTarget;
        _playerCamera.LookAt = cameraTarget;
        
        _mainMenuIsland.IslandCamera.Priority = 1;
    }
    

    /// <summary>
    /// Starts the game
    /// </summary>
    private void StartGame()
    {
        Debug.Log(_mainMenuIsland + " ISLAND");
        
        MissionManager.Instance.StartMission(_mainMenuIsland,2, 5);
        MissionManager.Instance.OnMissionFinished += OnMissionFinished;

        _player.SetControlsEnabled(true);

        _mainMenuIsland.IslandCamera.Priority = 0;
        _playerCamera.Priority = 1;
        OnGameStart?.Invoke();
    }
    
   /// <summary>
   /// Ends the game
   /// </summary>
   /// <param name="isGameWon"></param>
    private void GameOver(bool isGameWon)
    {
        _player.SetControlsEnabled(false);
        _playerCamera.Priority = 0;
        _mainMenuIsland.IslandCamera.Priority = 1;
        OnGameOver?.Invoke(isGameWon);
    }

    /// <summary>
    /// Exits the game back to the menu
    /// </summary>
    private void ExitToMenu()
    {
        OnGameExitToMenu?.Invoke();
    }
    
    
    private void OnMissionFinished(OutpostIsland outpostIsland, bool gameWon)
    {
        _mainMenuIsland = outpostIsland;
        GameOver(gameWon);
    }
    
    
    private void OnStartButtonPressed()
    {
        if(!MissionManager.InMission)
            StartGame();
    }

    private void OnRetryGameButtonPressed()
    {
        Debug.LogWarning("Might not work");
        StartGame();
    }

    private void OnExitToMainMenuButtonPressed()
    {
        ExitToMenu();
    }


}