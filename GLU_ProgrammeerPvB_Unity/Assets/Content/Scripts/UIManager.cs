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
    
    [SerializeField] private string _winText;
    [SerializeField] private string _loseText;
    [SerializeField] private TextMeshProUGUI _endScreenText;

    
    [SerializeField] private GameObject _mainMenuScreen;
    [SerializeField] private GameObject _endScreen;
    [SerializeField] private GameObject _GUI;
    
    [SerializeField] private GameObject _controlsScreen;
    [SerializeField] private Slider _gameSlideBar;
    
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private TextMeshProUGUI _maxHealthText;
    [SerializeField] private Slider _healthSlider;
    
    [SerializeField] private TextMeshProUGUI _timerText;
    
    [SerializeField] private PowerUpUISlider _powerupShower;
    [SerializeField] private TextMeshProUGUI _finalTimeText;
    
    [SerializeField] private AudioSource _audioSource;


    private int _islandCount;
    private int _currentIsland;
    
    private void Awake()
    {
        GameManager.OnGameStart += OnGameStart;
        GameManager.OnGameOver += OnGameOver;
        GameManager.OnGameExitToMenu += OnGameExitTomMenu;
        
        GameManager.Player.OnHealthChangeEvent += OnHealthChangeEvent;
        GameManager.Player.OnPlayerSetEffect += OnPlayerSetEffect;
        MissionManager.Instance.OnTimerChange += OnTimerChange;
        
        
    }

    private void OnPlayerSetEffect(Effect effect)
    {
        _powerupShower.Show(effect.EffectType,effect.EffectDuration);
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
        _healthText.text = Mathf.Ceil(newHealth).ToString();
        _maxHealthText.text = Mathf.Ceil(maxHealth).ToString();
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
        
        if(_powerupShower.isActiveAndEnabled)
            _powerupShower.transform.localScale = Vector3.zero;
        
        SetScreenActive(_GUI);
    }

    private void OnGameOver(bool gameWon)
    {
        Debug.Log("GAME WON: " + gameWon);
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        _endScreenText.text = gameWon ? _winText : _loseText;
        _finalTimeText.text = gameWon ? _timerText.text : "";
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

    public void PlaySound(AudioClip sound)
    {
        _audioSource.PlayOneShot(sound);
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