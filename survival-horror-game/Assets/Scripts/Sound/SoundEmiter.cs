using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmiter : MonoBehaviour
{
    private void Start()
    {
        _soundOrigin = gameObject.GetComponent<AudioSource>();
        // Adds an audiosource on the gameobject when starting the game if it isn't already there
        if (!_soundOrigin)
        {
            _soundOrigin = gameObject.AddComponent<AudioSource>();
        }
        // Auto-register to the sound manager!
        _soundManager = FindObjectOfType<SoundManager>();
        if (!_soundManager)
        {
            Debug.LogError("Error: Sound Manager not found");
        }
        else
            _soundManager.Register(this);
        // Initialize the current sound to the first element of the list
        _currentPlayingSound = SoundType.Footsteps;
        _soundOrigin.clip = _soundManager.GetClipByType(_currentPlayingSound);
        ConfigureAudioSource();
    }

    private void ConfigureAudioSource()
    {
        _soundOrigin.spatialBlend = 1;
        _soundOrigin.reverbZoneMix = 1.1f;
        _soundOrigin.dopplerLevel = 5;
        _soundOrigin.rolloffMode = AudioRolloffMode.Linear;
        _soundOrigin.minDistance = 0;
        _soundOrigin.maxDistance = 5;
    }

    /// <summary>
    /// Play the audio clip referenced by the given sound type
    /// </summary>
    public void PlayEffect(SoundType sound, float intensity = 5)
    {
        if (_soundOrigin.isPlaying)
        {
            if (_currentPlayingSound != sound)
            {
                _currentPlayingSound = sound;
                _soundOrigin.Stop();
                AudioClip clip = _soundManager.GetClipByType(sound);
                if (clip != null) _soundOrigin.clip = clip;
                _soundOrigin.maxDistance = intensity;
            }
        }
        else
        {
            if(_currentPlayingSound != sound)
            {
                AudioClip clip = _soundManager.GetClipByType(sound);
                if (clip != null) _soundOrigin.clip = clip;
            }
            _soundOrigin.maxDistance = intensity;
            _soundOrigin.Play();
        }
    }

    /// <summary>
    /// Stops the sound if it's playing
    /// </summary>
    public void StopEffect()
    {
        if (_soundOrigin.isPlaying)
        {
            _soundOrigin.Stop();
        }
    }

    /// <summary>
    /// Pauses the sound.
    /// </summary>
    /// <param name="pause">True to pause, false to unpause</param>
    public void PauseEffect(bool pause)
    {
        if (pause)
            _soundOrigin.Pause();
        else
            _soundOrigin.UnPause();
    }

    /// <summary>
    /// Updates the volume of the audiosource with the given value.
    /// </summary>
    public void ChangeVolume(float value)
    {
        _soundOrigin.volume = value;
    }

    /// <summary>
    /// Interface for AlertListeners from sound manager
    /// </summary>
    public void EmitSoundWave(float intensity, int floor, float duration)
    {
        _soundManager.AlertListeners(new Noise(this, intensity, floor, _noiseEmited, duration));
    }

    /// <summary>
    /// Sets the type of noise emited by the associated game object
    /// </summary>
    /// <param name="noise">Associated GameObject type</param>
    public void SetNoiseEmited(NoiseType noise)
    {
        _noiseEmited = noise;
    }

    /// <summary>
    /// Plays the custom clip
    /// </summary>
    public void PlayCustomClip(AudioClip clip, float intensity = 5)
    {
        _currentPlayingSound = SoundType.Custom;
        _soundOrigin.clip = clip;
        _soundOrigin.maxDistance = intensity;
        _soundOrigin.Play();
    }

    public bool IsPlaying()
    {
        return _soundOrigin.isPlaying;
    }
    
    // Audiosource on which the class has to play the clip
    private AudioSource _soundOrigin;
    // Stores the playing clip type
    private SoundType _currentPlayingSound;
    // Sound Manager, to directly emit sound shockwaves and to update the volume
    private SoundManager _soundManager;
    // Noise emited by the associated game object
    private NoiseType _noiseEmited;
}