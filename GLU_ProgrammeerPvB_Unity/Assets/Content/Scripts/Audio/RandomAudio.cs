using UnityEngine;

/// <summary>
/// Uses the AudioSource component to play a random sound on awake using an array of audio sources
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class RandomAudio : MonoBehaviour
{
    [SerializeField] private AudioClip[] _audioClips;
    [SerializeField] private AudioSource _audioSource;
    
    /// <summary>
    /// Changes the audio source clip and picks a random audio clip from the array
    /// </summary>
    private void Awake()
    {
        if (!_audioSource)
        {
            _audioSource = GetComponent<AudioSource>();
        }

        if (_audioSource)
        {
            _audioSource.clip = _audioClips[Random.Range(0, _audioClips.Length)];
            _audioSource.Play();
        }
    }
    
}
