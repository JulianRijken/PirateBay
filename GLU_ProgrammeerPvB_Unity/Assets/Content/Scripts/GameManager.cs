using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    
    [SerializeField] private PlayerShipController _player;
    [SerializeField] private CinemachineFreeLook _playerCamera;
    
    private OutpostIsland[] _outpostIslands;
    private TreasureIsland[] _treasureIslands;
    private OutpostIsland _mainMenuIsland;

    public static PlayerShipController Player => Instance._player;
    public static OutpostIsland[] OutpostIslands => Instance._outpostIslands;
    public static TreasureIsland[] TreasureIslands => Instance._treasureIslands;
    
    public static GameManager Instance { get; private set; }


    public static Action OnGameStart;

    private void Awake()
    {
        Instance = this;

        if (_player == null)
        {
            _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerShipController>();
        }
    }

    private void Start()
    {
        var cameraTarget = _player.transform;
        _playerCamera.Follow = cameraTarget;
        _playerCamera.LookAt = cameraTarget;
        
        _treasureIslands =  FindObjectsOfType<TreasureIsland>();
        _outpostIslands = FindObjectsOfType<OutpostIsland>();

        _mainMenuIsland = _outpostIslands[Random.Range(0, _outpostIslands.Length)];


        UIManager.OnStartButtonPressed += OnStartButtonPressed;
        UIManager.OnRetryGameButtonPressed += OnRetryGameButtonPressed;
        UIManager.OnExitToMainMenuButtonPressed += OnExitToMainMenuButtonPressed;
    }
    

    private void StartGame()
    {
        MissionManager.Instance.StartMission(_mainMenuIsland,0);
        MissionManager.Instance.OnMissionFinished += OnMissionFinished;
        
        _player.SetControlsEnabled(true);
        
        OnGameStart?.Invoke();
    }

    private void OnMissionFinished(IslandBase outpostIsland)
    {
        _mainMenuIsland = (OutpostIsland)outpostIsland;
        EndGame();
    }
    
    
    
    private void EndGame()
    {
        _player.SetControlsEnabled(false);
        
        Debug.Log("Game Ending");
    }


    
    
    private void OnStartButtonPressed()
    {
        if(!MissionManager.InMission)
            StartGame();
    }

    private void OnRetryGameButtonPressed()
    {
     
    }

    private void OnExitToMainMenuButtonPressed()
    {
       
    }


}