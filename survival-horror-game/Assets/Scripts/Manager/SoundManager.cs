using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    /// <summary>
    /// Needs to be on awake because the lists need to be initialized when the game begins to avoid all nullreference errors
    /// </summary>
    private void Awake()
    {
        _listeners = new List<Listener>();
        _soundEmiters = new List<SoundEmiter>();
    }

    private void OnLevelWasLoaded()
    {
        _listeners = new List<Listener>();
        _soundEmiters = new List<SoundEmiter>();
    }

    /// <summary>
    /// Adds a listener to the list of objects to be notified when there is a sound
    /// </summary>
    public void Subscribe(Listener listener)
    {
        _listeners.Add(listener);
    }

    /// <summary>
    /// Registers a SoundEmiter to the potential changements of volume
    /// </summary>
    public void Register(SoundEmiter se)
    {
        _soundEmiters.Add(se);
        // Update emiter's volume with current volume.
        se.ChangeVolume(_volume);
    }

    public void AlertListeners(Noise noise)
    {
        if(_listeners.Count != 0)
        {
            foreach (Listener listener in _listeners)
            {
                if (listener.ListensTo(noise.emiterType)) { listener.DetectSound(noise); }
            }
        }
    }

    /// <summary>
    /// Call this function to update the volume of the sound effects
    /// </summary>
    /// <param name="value">New sound volume</param>
    public void UpdateSoundVolume(float value)
    {
        if (value > 1) value = 1;
        else if (value < 0) value = 0;

        _volume = value;
        foreach(SoundEmiter emiter in _soundEmiters)
        {
            emiter.ChangeVolume(_volume);
        }
    }

    /// <summary>
    /// Search the audio clip related to the type.
    /// </summary>
    /// <returns>AudioClip clip if it finds something. Null otherwise.</returns>
    public AudioClip GetClipByType(SoundType type)
    {
        Sound search = _sounds.Find((Sound sound) => sound.type == type);
        if (search != null)
        {
            return search.audioClip;
        }
        return null;
    }

    // Every sounds which the gameObject associated can access to.
    [SerializeField]
    private List<Sound> _sounds = new List<Sound>();
    // Every gameObject which listen to any sound of the environment / player / monster
    private List<Listener> _listeners;
    // All elements which can emit soundwaves
    private List<SoundEmiter> _soundEmiters;
    // Record the current volume to change the volume of an emiter when it registers to the Manager
    private float _volume = 1f;
}