using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicManager : MonoBehaviour
{
    private void Start()
    {
        _cinematics = GetComponentsInChildren<Cinematic>();
        _playerManager = FindObjectOfType<PlayerManager>();

        StartCoroutine(ManageCinematicOverTime());
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

    private IEnumerator ManageCinematicOverTime()
    {
        yield return null;
        StartCinematic("Intro");
    }

    private Cinematic[] _cinematics;
    private PlayerManager _playerManager;
}

public abstract class Cinematic : MonoBehaviour
{
    public void Start()
    {
        player = FindObjectOfType<PlayerManager>();
    }

    public string cinematicName;

    public void Launch()
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

    protected IEnumerator SaySomething(string text)
    {
        player.Speak(text);

        while (player.IsSpeaking())
        {
            yield return null;
        }
    }

    protected IEnumerator MoveTo(Vector3 position, LookAt lookAt = LookAt.NONE)
    {
        player.SetNewDestination(position);

        while (player.IsMoving())
        {
            yield return null;
        }

        switch (lookAt)
        {
            case LookAt.UP:
                player.controller.Movement(0, 1, false);
                break;
            case LookAt.DOWN:
                player.controller.Movement(0, -1, false);
                break;
            case LookAt.RIGHT:
                player.controller.Movement(1, 0, false);
                break;
            case LookAt.LEFT:
                player.controller.Movement(-1, 0, false);
                break;
        }

        if (lookAt != LookAt.NONE)
        {
            player.controller.Movement(0, 0, false);
        }
    }

    protected enum LookAt
    {
        NONE,
        UP,
        DOWN,
        RIGHT,
        LEFT
    };

    private bool _isPlaying;
    protected PlayerManager player;
}
