using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public void Subscribe(Monster monster)
    {
        _listeners.Add(monster);
    }

    public void CreateSoundWave(Vector2 center, float radius)
    {
        foreach(Monster m in _listeners)
        {
            m.DetectSound(center, radius);
        }
    }

    // All monsters which listen to any sound of the player / objects launched by the player
    private List<Monster> _listeners;
}
