using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// All the noises that can be listened. 
/// </summary>
public enum ListenNoise
{
    Player,
    Monster,
    Environment,
}

/// <summary>
/// Listens to any sound. It registers itself in SoundManager.
/// </summary>
public class Listener : MonoBehaviour
{
    protected virtual void Start()
    {
        // Auto-subscribe to the sound manager
        SoundManager sm = FindObjectOfType<SoundManager>();
        if (!sm)
            Debug.LogError("Listener: " + gameObject.name + " can not hear any sound!");
        // If the SoundManager is found, subscribe the monster to all the sounds emited in the game
        else
            sm.Subscribe(this);
    }

    public bool ListensTo(ListenNoise type)
    {
        return _allNoiseListened.Contains(type);
    }

    /// <summary>
    /// This function needs to be implemented in the child. It is called when a sound is emited.
    /// </summary>
    public virtual void DetectSound(int floorSoundEmited) { }

    // Types of noises listened
    [SerializeField]
    private List<ListenNoise> _allNoiseListened = new List<ListenNoise>();
}
