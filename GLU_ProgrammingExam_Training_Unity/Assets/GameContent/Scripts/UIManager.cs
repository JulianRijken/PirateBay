using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    //[SerializeField] private Dictionary<string, GameObject> _screens;
    [SerializeField] private GameObject _mainMenuScreen;
    [SerializeField] private GameObject _endScreen;
    [SerializeField] private GameObject _GUI;

    private void Awake()
    {
        GameManager.OnGameStart += OnGameStart;
        GameManager.OnGameEnd += OnGameEnd;
        GameManager.OnQuitToMainMenu += OnQuitToMainMenu;
        GameManager.OnGameRestart += OnGameRestart;
    }

    private void OnGameRestart()
    {
        SetScreenActive(_GUI);
    }

    private void OnQuitToMainMenu()
    {
       SetScreenActive(_mainMenuScreen);
    }

    private void OnGameEnd()
    {
        SetScreenActive(_endScreen);
    }

    private void OnGameStart()
    {
        SetScreenActive(_GUI);
    }

    
    

    private void SetScreenActive(GameObject menu)
    {
        _GUI.SetActive(false);
        _mainMenuScreen.SetActive(false);
        _endScreen.SetActive(false);

        if(menu)
            menu.SetActive(menu);
    }
    
    
    public void OnStartGameButton()
    {
        GameManager.Instance.StartGame();
    }    
    
    public void OnRestartGameButton()
    {
        GameManager.Instance.RestartGame();
    }    
    
    public void OnQuitToMainMenuButton()
    {
        GameManager.Instance.QuitToMainMenu();
    }
}
