using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private void Awake()
    {
        _listeners = new List<Listener>();
        _soundEmiters = new List<SoundEmiter>();
    }
    public void Subscribe(Listener listener)
    {
        _listeners.Add(listener);
    }
    public void Register(SoundEmiter se)
    {
        _soundEmiters.Add(se);
        // Update emiter's volume with current volume.
        se.ChangeVolume(_volume);
    }

    public void CreateSoundWave(Vector2 center, float radius, int floor, ListenNoise emiter)
    {
        GameObject go = CreateGO(center, radius);
        if(_listeners.Count != 0)
        {
            foreach (Listener listener in _listeners)
            {
                if (listener.ListensTo(emiter)) { listener.DetectSound(floor); }
            }
        }
        // Delete sound wave
        Destroy(go);
    }

    public void UpdateSoundVolume(float value)
    {
        _volume = value;
        foreach(SoundEmiter emiter in _soundEmiters)
        {
            emiter.ChangeVolume(_volume);
        }
    }

    private GameObject CreateGO(Vector2 center, float radius)
    {
        GameObject go = new GameObject();
        go.transform.position = center;
        CircleCollider2D sound = go.AddComponent<CircleCollider2D>(); 
        sound.radius = radius;
        sound.isTrigger = true;
        // Sound layer
        go.layer = LayerMask.NameToLayer("Sound");
        go.name = "Soundwave";
        return go;
    }

    // Every gameObject which listen to any sound of the environment / player / monster
    private List<Listener> _listeners;
    // All elements which can emit soundwaves
    private List<SoundEmiter> _soundEmiters;
    // Record the current volume to change the volume of an emiter when it registers to the Manager
    private float _volume = 1f;
}