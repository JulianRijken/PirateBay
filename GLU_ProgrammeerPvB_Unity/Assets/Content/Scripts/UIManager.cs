using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
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
    
    [SerializeField] private TextMeshProUGUI _missionTimeText;
    
    [SerializeField] private PowerUpUISlider _powerUpShower;
    [SerializeField] private TextMeshProUGUI _timeTookText;
    [SerializeField] private Color _winColor;
    [SerializeField] private Color _loseColor;
    [SerializeField] private TextMeshProUGUI _shipsKilledText;

    [SerializeField] private AudioSource _audioSource;


    private int _islandCount;
    private int _currentIsland;
    
    public static Action OnStartButtonPressed;
    public static Action OnRetryGameButtonPressed;
    public static Action OnExitToMainMenuButtonPressed;
    
    
    private void Awake()
    {
        GameManager.OnGameStart += OnGameStart;
        GameManager.OnGameOver += OnGameOver;
        GameManager.OnGameExitToMenu += OnGameExitTomMenu;
        
        GameManager.Player.OnHealthChangeEvent += OnHealthChangeEvent;
        GameManager.Player.OnPlayerSetEffect += OnPlayerSetEffect;
        MissionManager.Instance.OnTimerChange += OnTimerChange;
    }
    
    private void Start()
    {
        SetScreenActive(_mainMenuScreen);
        _controlsScreen.SetActive(false);
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    
    private void LateUpdate()
    {
        _gameSlideBar.value = MissionManager.Instance.GetMissionAlpha();
    }


    private void OnPlayerSetEffect(Effect effect)
    {
        _powerUpShower.Show(effect.EffectType,effect.EffectDuration);
    }

    private void OnTimerChange(float timer)
    {
        _missionTimeText.text = ConvertToTime(timer);
    }

    private void OnHealthChangeEvent(float newHealth, float maxHealth)
    {
        _healthSlider.value = Mathf.Clamp01(newHealth / maxHealth);
        _healthText.text = Mathf.Ceil(newHealth).ToString();
        _maxHealthText.text = Mathf.Ceil(maxHealth).ToString();
    }
    
    
    private void OnGameExitTomMenu()
    {
        SetScreenActive(_mainMenuScreen);
    }
    
    private void OnGameStart()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        _powerUpShower.Hide();
        
        SetScreenActive(_GUI);
    }

    private void OnGameOver(bool gameWon)
    {
        Debug.Log("GAME WON: " + gameWon);
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        _endScreenText.text = gameWon ? _winText : _loseText;
        
        _timeTookText.text = ConvertToTime(MissionManager.TimeInMission);
        _timeTookText.color = gameWon ? _winColor : _loseColor;
        _shipsKilledText.text = MissionManager.ShipsKilled.ToString();
        
        SetScreenActive(_endScreen);
    }
    
    
    
    private void SetScreenActive(GameObject activeScreen)
    {
        _mainMenuScreen.SetActive(false);
        _GUI.SetActive(false);
        _endScreen.SetActive(false);
        
        activeScreen.SetActive(true);
    }

    private string ConvertToTime(float time)
    {
        var minutes = Mathf.Floor(time / 60);
        var seconds = Mathf.RoundToInt(time % 60);;
        
        return $"{minutes}m {seconds}s";
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
    
    public void PlaySound(AudioClip sound)
    {
        _audioSource.PlayOneShot(sound);
    }

}