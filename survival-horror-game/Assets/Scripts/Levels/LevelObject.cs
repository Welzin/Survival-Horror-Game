using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelObject : MonoBehaviour
{
    public void OnEnable()
    {
        player = FindObjectOfType<PlayerManager>();
    }

    public virtual void Launch()
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
    protected virtual void Stop()
    {
        _isLaunched = false;
    }

    protected IEnumerator SaySomething(Dialog dialog)
    {
        if (player.IsSpeaking())
        {
            player.PassDialog();
        }

        player.Speak(dialog);

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
    public override void Launch()
    {
        player.transform.position = whereToBegin;

        foreach (GameObject go in FindObjectOfType<LevelManager>().elementsToHideOnCinematic)
        {
            go.SetActive(false);
        }

        base.Launch();
    }

    protected override void Stop()
    {
        foreach (GameObject go in FindObjectOfType<LevelManager>().elementsToHideOnCinematic)
        {
            go.SetActive(true);
        }

        base.Stop();
    }

    public Vector2 whereToBegin;
}

public abstract class Mission : LevelObject
{
    protected IEnumerator WaitForItemInInventory(string name)
    {
        while (!player.inventory.HaveItem(name))
        {
            yield return null;
        }
    }

    protected IEnumerator WaitForEvent(TimedEvent levelEvent)
    {
        while (levelEvent != player.GetLastEvent())
        {
            yield return null;
        }
    }

    protected IEnumerator WaitForOneEventBetween(List<TimedEvent> events)
    {
        bool eventDone = false;

        while (!eventDone)
        {
            foreach (TimedEvent levelEvent in events)
            {
                if (player.GetLastEvent() == levelEvent)
                {
                    eventDone = true;
                    break;
                }
            }

            yield return null;
        }
    }

    protected IEnumerator WaitForMultipleEvents(List<TimedEvent> events)
    {
        List<TimedEvent> copy = events;

        while (events.Count != 0)
        {
            foreach (TimedEvent levelEvent in copy)
            {
                if (player.GetLastEvent() == levelEvent)
                {
                    events.Remove(levelEvent);
                    break;
                }
            }

            yield return null;
        }
    }
}