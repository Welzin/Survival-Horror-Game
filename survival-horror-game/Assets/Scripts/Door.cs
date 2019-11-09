using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : TimedEvent
{
    protected void Start()
    {
        base.Start();
        _animator = GetComponent<Animator>();
        _emiter = gameObject.AddComponent<SoundEmiter>();
    }
    
    public void OpenForMonster()
    {
        if (IsClosed())
        {
            DoorOpening();
        }
        else
        {
            DoorClosing();
        }
    }

    protected override bool ConditionsToRespect()
    {
        return !needAKey || player.inventory.HaveKeyForDoor(this);
    }

    protected override void WhatToDoIfConditionNotRespected()
    {
        player.hud.helper.DisplayHelp(Helper.Type.DoorLocked, 3);
        _emiter.PlayCustomClip(doorLocked);
    }

    protected override void WhatToDoBeforeEvent()
    {
    }

    protected override void WhatToDoAfterEvent()
    {
        if (IsClosed())
        {
            DoorOpening();
            player.hud.helper.DisplayHelp(Helper.Type.CloseDoor);
            player.hud.helper.StopDisplayingHelp(Helper.Type.OpenDoor);

            if (needAKey)
            {
                player.inventory.UsedKeyForDoor(this);
                needAKey = false;
                _emiter.PlayCustomClip(doorUnlocked);
            }
            else
            {
                _emiter.PlayCustomClip(doorOpening);
            }
        }
        else
        {
            DoorClosing();
            player.hud.helper.DisplayHelp(Helper.Type.OpenDoor);
            player.hud.helper.StopDisplayingHelp(Helper.Type.CloseDoor);
        }
    }

    protected override void WhatToDoOnEventInterruption()
    {
    }

    public bool IsClosed()
    {
        return _animator.GetBool("Closing");
    }

    private void DoorOpening()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
        lightObstacle.SetActive(false);
        _animator.SetBool("Closing", false);
    }

    private void DoorClosing()
    {
        GetComponent<BoxCollider2D>().isTrigger = false;
        lightObstacle.SetActive(true);
        _animator.SetBool("Closing", true);
        _emiter.PlayCustomClip(doorClosing);
    }

    public bool needAKey;
    public GameObject lightObstacle;
    public AudioClip doorLocked;
    public AudioClip doorUnlocked;
    public AudioClip doorOpening;
    public AudioClip doorClosing;

    private Animator _animator;
    private SoundEmiter _emiter;
}
