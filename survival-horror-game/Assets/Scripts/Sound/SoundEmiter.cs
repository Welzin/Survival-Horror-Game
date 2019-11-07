﻿using System.Collections;
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
        _currentPlayingSound = _sounds[0].type;
        _soundOrigin.clip = _sounds[0].audioClip;
    }

    /// <summary>
    /// Play the audio clip referenced by the given sound type
    /// </summary>
    public void PlayEffect(SoundType sound)
    {
        if (_soundOrigin.isPlaying)
        {
            if (_currentPlayingSound != sound)
            {
                _currentPlayingSound = sound;
                _soundOrigin.Stop();
                AudioClip clip = GetClipByType(sound);
                if (clip != null) _soundOrigin.clip = clip;
            }
        }
        else
            _soundOrigin.Play();
    }

    /// <summary>
    /// Stops the sound if it's playing
    /// </summary>
    public void StopEffect()
    {
        if(_soundOrigin.isPlaying)
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
    /// Interface for CreateSoundWave from sound manager
    /// </summary>
    public void EmitSoundWave(float radius, int floor, ListenNoise emiter)
    {
        _soundManager.CreateSoundWave(gameObject.transform.position, radius, floor, emiter);
    }

    /// <summary>
    /// Search the audio clip related to the type.
    /// </summary>
    /// <returns>AudioClip clip if it finds something. Null otherwise.</returns>
    private AudioClip GetClipByType(SoundType type)
    {
        Sound search = _sounds.Find((Sound sound) => sound.type == type);
        if(search != null)
        {
            return search.audioClip;
        }
        return null;
    }

    // Every sounds which the gameObject associated can access to.
    [SerializeField]
    private List<Sound> _sounds = new List<Sound>();
    
    // Audiosource on which the class has to play the clip
    private AudioSource _soundOrigin;
    // Stores the playing clip type
    private SoundType _currentPlayingSound;
    // Sound Manager, to directly emit sound shockwaves and to update the volume
    private SoundManager _soundManager;
}
