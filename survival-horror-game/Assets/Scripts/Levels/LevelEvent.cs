using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEvent : TimedEvent
{
    protected void Start()
    {
        base.Start();
        _emiter = gameObject.AddComponent<SoundEmiter>();
    }

    protected override bool ConditionsToRespect()
    {
        return nameItemToHave == "" || player.inventory.HaveItem(nameItemToHave);
    }

    protected override void WhatToDoAfterEvent()
    {
        player.inventory.ItemUsed(nameItemToHave);

        if (itemGet != null)
            player.inventory.AddItem(itemGet.item);

        if (soundMakeAfter != null)
            _emiter.PlayCustomClip(soundMakeAfter);
        else
            _emiter.StopEffect();

        cannotDoEventAnymore = true;
    }

    protected override void WhatToDoBeforeEvent()
    {
        if (soundMake != null)
        {
            _emiter.PlayCustomClip(soundMake);
        }
    }

    protected override void WhatToDoIfConditionNotRespected()
    {
    }

    protected override void WhatToDoOnEventInterruption()
    {
        _emiter.StopEffect();
    }

    public AudioClip soundMake;
    public AudioClip soundMakeAfter;
    private SoundEmiter _emiter;
    public string nameItemToHave;
    public ItemObject itemGet;
}
