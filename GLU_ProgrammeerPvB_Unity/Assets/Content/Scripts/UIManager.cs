using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static Action OnStartButtonPressed;
    public static Action OnRetryGameButtonPressed;
    public static Action OnExitToMainMenuButtonPressed;
    
    [SerializeField] private GameObject _mainMenuScreen;
    [SerializeField] private GameObject _endScreen;
    [SerializeField] private GameObject _GUI;
    
    [SerializeField] private GameObject _controlsScreen;
    [SerializeField] private Slider _gameSlideBar;
    
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private TextMeshProUGUI _maxHealthText;
    [SerializeField] private Slider _healthSlider;
    
    [SerializeField] private TextMeshProUGUI _timerText;


    private int _islandCount;
    private int _currentIsland;
    
    private void Awake()
    {
        GameManager.OnGameStart += OnGameStart;
        GameManager.OnGameOver += OnGameOver;
        GameManager.OnGameExitToMenu += OnGameExitTomMenu;
        
        GameManager.Player.OnHealthChangeEvent += OnHealthChangeEvent;
        MissionManager.Instance.OnTimerChange += OnTimerChange;
    }

    private void OnTimerChange(float timer)
    {
        var minutes = Mathf.Floor(timer / 60);
        var seconds = Mathf.RoundToInt(timer%60);;
        
        _timerText.text = $"{minutes}m {seconds}s";
    }

    private void OnHealthChangeEvent(float newHealth, float maxHealth)
    {
        _healthSlider.value = Mathf.Clamp01(newHealth / maxHealth);
        _healthText.text = newHealth.ToString();
        _maxHealthText.text = maxHealth.ToString();
    }

    private void LateUpdate()
    {
        _gameSlideBar.value = MissionManager.Instance.GetMissionAlpha();
    }
    


    private void OnGameExitTomMenu()
    {
        SetScreenActive(_mainMenuScreen);
    }

    private void Start()
    {
        SetScreenActive(_mainMenuScreen);
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
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

    private void OnGameOver(bool gameWon)
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        SetScreenActive(_endScreen);
    }


    public void PressStartButton()
    {
        OnStartButtonPressed?.Invoke();
    }
    public void PressExitGameButton()
    {
        OnExitToMainMenuButtonPressed?.Invoke();
    }
    
    public void PressRetryButton()
    {
        OnRetryGameButtonPressed?.Invoke();
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