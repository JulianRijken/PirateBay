using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] _gameAudioOptions;
    [SerializeField] private AudioClip[] _mainMenuAudioOptions;
    [SerializeField] private AudioClip _gameWonMusic;
    [SerializeField] private AudioClip _gameOverMusic;
    [SerializeField] private AudioSource _audioSource;



    private void Awake()
    {
        GameManager.OnGameStart += OnGameStart;
        GameManager.OnGameOver += OnGameOver;
        GameManager.OnGameExitToMenu += OnGameExitToMenu;
    }

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
