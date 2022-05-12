using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private OutpostIsland[] _outpostIslands;
    public static OutpostIsland[] OutpostIslands => Instance._outpostIslands;
    
    private TreasureIsland[] _treasureIslands;
    public static TreasureIsland[] TreasureIslands => Instance._treasureIslands;
    
    public static GameManager Instance { get; private set; }


    private OutpostIsland _mainMenuIsland;

    public Action OnGameStarted;
    public Action OnGameOver;

    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void StartGame()
    {
        MissionManager.Instance.StartMission(_mainMenuIsland,2);
  /*      MissionManager.Instance.OnMissionFinished += OnMissionFinished;*/
    }

/*    private void OnMissionFinished(IslandBase outpostIsland)
    {
        _mainMenuIsland = (OutpostIsland)outpostIsland;
        EndGame();
    }*/
    
    public void EndGame()
    {
        
    }


    public void OnStartButtonPressed()
    {
      

    }

    public void OnRetryGameButtonPressed()
    {
     
    }

    public void OnExitToMainMenuButtonPressed()
    {
       
    }


}
