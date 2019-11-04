using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicManager : MonoBehaviour
{
    private void Start()
    {
        _cinematics = GetComponentsInChildren<Cinematic>();
        StartCinematic("Intro");
    }

    public void StartCinematic(string name)
    {
        if (CinematicStarted())
        {
            Debug.LogWarning("A cinematic is already playing");
            return;
        }

        foreach(Cinematic cinematic in _cinematics)
        {
            if (cinematic.cinematicName == name)
            {
                cinematic.Launch();
                return;
            }
        }
    }

    public bool CinematicStarted()
    {
        foreach (Cinematic cinematic in _cinematics)
        {
            if (cinematic.isPlaying)
                return true;
        }

        return false;
    }

    private Cinematic[] _cinematics;
}

public abstract class Cinematic : MonoBehaviour
{
    public void Start()
    {
        player = FindObjectOfType<PlayerManager>();
    }

    public string cinematicName;

    public virtual void Launch()
    {
        _isPlaying = true;
        StartCoroutine(StartCinematic());
    }

    public bool isPlaying
    {
        get { return _isPlaying; }
        set { _isPlaying = value; }
    }

    protected abstract IEnumerator StartCinematic();
    protected void StopCinematic()
    {
        _isPlaying = false;
    }

    private bool _isPlaying;
    protected PlayerManager player;
}
