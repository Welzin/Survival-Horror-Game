using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private void Start()
    {
        _listeners = new List<Monster>();
    }
    public void Subscribe(Monster monster)
    {
        _listeners.Add(monster);
    }

    public void CreateSoundWave(Vector2 center, float radius)
    {
        GameObject go = CreateGO(center, radius);
        if(_listeners.Count != 0)
        {
            foreach (Monster m in _listeners)
            {
                m.DetectSound();
            }
        }
        // Delete sound wave
        Destroy(go);
    }

    private GameObject CreateGO(Vector2 center, float radius)
    {
        GameObject go = new GameObject();
        go.transform.position = center;
        CircleCollider2D sound = go.AddComponent<CircleCollider2D>(); 
        sound.radius = radius;
        sound.isTrigger = true;
        // Sound layer
        go.layer = 12;
        go.name = "Soundwave";
        return go;
    }

    // All monsters which listen to any sound of the player / objects launched by the player
    private List<Monster> _listeners;
}