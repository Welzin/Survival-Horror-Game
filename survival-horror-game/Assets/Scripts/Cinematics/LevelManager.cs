using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
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
        MusicManager m = FindObjectOfType<MusicManager>();
        SoundManager s = FindObjectOfType<SoundManager>();
        m.UpdateMusic(0);
        m.PlayLoop(2);
        m.ChangeVolume(0.1f);
        s.UpdateSoundVolume(0.1f);

        yield return null;
        //StartCinematic("Intro");

        while (CinematicStarted())
        {
            yield return null;
        }

        m.UpdateMusic(2);
        m.PlayLoop(2);

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
    protected void StartAgain()
    {
        _isPlaying = true;
    }

    protected IEnumerator SaySomething(string text)
    {
        while (player.IsSpeaking())
        {
            player.PassDialog();
        }
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

        Look(lookAt);
    }

    protected void Look(LookAt lookAt)
    {
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
