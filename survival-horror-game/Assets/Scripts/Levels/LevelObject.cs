using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelObject : MonoBehaviour
{
    public void OnEnable()
    {
        player = FindObjectOfType<PlayerManager>();
    }

    public void Launch()
    {
        _isLaunched = true;
        StartCoroutine(StartLevelObject());
    }

    public bool isLaunched
    {
        get { return _isLaunched; }
        set { _isLaunched = value; }
    }

    protected abstract IEnumerator StartLevelObject();
    protected void Stop()
    {
        _isLaunched = false;
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
    
    public string levelObjectName;
    protected PlayerManager player;
    private bool _isLaunched;
}

public abstract class Cinematic : LevelObject
{

}

public abstract class Mission : LevelObject
{

}
