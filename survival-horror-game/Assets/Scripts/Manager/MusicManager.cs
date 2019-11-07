using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores all musics - manages music player (sound, play, pause) 
/// </summary>
public class MusicManager : MonoBehaviour
{
    private void Start()
    {
        _currentMusic = gameObject.AddComponent<AudioSource>();
        _delay = 5;
        // Plays first music of the manager when starting
        UpdateMusic(0);
        PlayLoop();
    }

    /// <summary>
    /// Update the clip of the current music
    /// </summary>
    /// <param name="index">Index of the music in the public parameter</param>
    public void UpdateMusic(int index)
    {
        _currentMusic.clip = musics[index];
    }

    /// <summary>
    /// Call this function to play the music in loop, with default delay / loopDelay
    /// </summary>
    public void PlayLoop()
    {
        PlayLoop(0, _delay);
    }

    /// <summary>
    /// Call this function to play the music in loop, with default loopDelay
    /// </summary>
    /// <param name="delay">Time to wait before playing music</param>
    public void PlayLoop(float delay)
    {
        PlayLoop(delay, _delay);
    }

    /// <summary>
    /// Call this function to play the music in loop
    /// </summary>
    /// <param name="delay">Time to wait before playing music</param>
    /// <param name="loopDelay">Time to wait after the music ends to restart</param>
    public void PlayLoop(float delay, float loopDelay)
    {
        _currentMusic.PlayDelayed(delay);
        _delay = loopDelay;
        StartCoroutine(WaitForPlaying(_currentMusic.clip.length + _delay));
    }

    /// <summary>
    /// Play the current music once
    /// </summary>
    public void PlayOnce()
    {
        PlayOnce(0);
    }

    /// <summary>
    /// Plays the music once. Starts after the given delay
    /// </summary>
    public void PlayOnce(float delay)
    {
        _currentMusic.PlayDelayed(delay);
    }

    /// <summary>
    /// Change the current volume of the music
    /// </summary>
    /// <param name="value">Volume of the music - between 0.0 and 1.0</param>
    public void ChangeVolume(float value)
    {
        if(value > 1) { value = 1; }
        else if(value < 0) { value = 0; }

        _volume = value;
        _currentMusic.volume = value;
    }

    /// <summary>
    /// Pause / Continue the music
    /// </summary>
    /// <param name="pause">True to pause, false to continue playing</param>
    public void Pause(bool pause)
    {
        if(pause)
        {
            _currentMusic.Pause();
            StopCoroutine("WaitForPlaying");
        }
        else
        {
            _currentMusic.UnPause();
            StartCoroutine(WaitForPlaying(_timeStaying));
        }
    }

    /// <summary>
    /// Waits and restart the music clip
    /// </summary>
    /// <param name="musicLength">Lenght of the wait</param>
    private IEnumerator WaitForPlaying(float musicLength)
    {
        _timeStaying = musicLength;
        while(_timeStaying > 0)
        {
            yield return new WaitForSeconds(1);
            _timeStaying -= 1;
            Debug.Log(_timeStaying);
        }
        PlayLoop();
    }

    /// <summary>
    /// Stops the music
    /// </summary>
    public void StopMusic()
    {
        if(_currentMusic.isPlaying)
        {
            _currentMusic.Stop();
        }
    }

    // Storage of all the game musics
    public List<AudioClip> musics;
    // Storage of the currently played music
    private AudioSource _currentMusic;
    // Music clips volume
    private float _volume;
    // Delay between the music played
    private float _delay;
    // Time staying until the end of the clip (recorded if pause is called)
    private float _timeStaying;
}
