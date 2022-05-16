using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static Action OnStartButtonPressed;
    public static Action OnRetryGameButtonPressed;
    public static Action OnExitToMainMenuButtonPressed;
    
    [SerializeField] private GameObject _mainMenuScreen;
    [SerializeField] private GameObject _endScreen;
    [SerializeField] private GameObject _GUI;
    
    [SerializeField] private GameObject _controlsScreen;
    

    private void Awake()
    {
        GameManager.OnGameStart += OnGameStart;
    }

    private void SetScreenActive(GameObject activeScreen)
    {
        _mainMenuScreen.SetActive(false);
        _GUI.SetActive(false);
        _endScreen.SetActive(false);
        
        activeScreen.SetActive(true);
    }

    private void OnGameStart()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        SetScreenActive(_GUI);
    }


    public void PressStartButton()
    {
        OnStartButtonPressed?.Invoke();
    }

    public void PressRetryButton()
    {
        OnRetryGameButtonPressed?.Invoke();
    }
    
    public void PressExitToMainMenuButton()
    {
        OnExitToMainMenuButtonPressed?.Invoke();
    }


    
    

    public void PressControlsButton()
    {
        _controlsScreen.SetActive(true);
    }

    public void PressHideControlsButton()
    {
        _controlsScreen.SetActive(false);
    }

}