using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Manages the music for the game
/// </summary>
public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] _gameAudioOptions;
    [SerializeField] private AudioClip[] _mainMenuAudioOptions;
    [SerializeField] private AudioClip _gameWonMusic;
    [SerializeField] private AudioClip _gameOverMusic;
    [SerializeField] private AudioSource _audioSource;
    
    /// <summary>
    /// Subscribes to the specific functions from the GameManager
    /// </summary>
    private void Awake()
    {
        GameManager.OnGameStart += OnGameStart;
        GameManager.OnGameOver += OnGameOver;
        GameManager.OnGameExitToMenu += OnGameExitToMenu;
    }

    /// <summary>
    /// Calls OnGameExitToMenu() by default
    /// </summary>
    private void Start()
    {
        OnGameExitToMenu();
    }


    private void OnGameOver(bool gameWon)
    {
        _audioSource.Stop();
        _audioSource.clip = null;
        _audioSource.PlayOneShot(gameWon ? _gameWonMusic : _gameOverMusic);
    }

    
    private void OnGameExitToMenu()
    {

        _audioSource.clip = _mainMenuAudioOptions[Random.Range(0, _mainMenuAudioOptions.Length)];
        _audioSource.Play();
    }

    private void OnGameStart()
    {
        _audioSource.clip = _gameAudioOptions[Random.Range(0, _gameAudioOptions.Length)];
        _audioSource.Play();
    }
}
