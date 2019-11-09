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

        if (victoryText != "")
            player.Speak(victoryText);

        if (itemGet != null)
            player.inventory.AddItem(itemGet.item);

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
        if (textToHelp != "")
            player.Speak(textToHelp);
    }

    protected override void WhatToDoOnEventInterruption()
    {
        _emiter.StopEffect();
    }

    public AudioClip soundMake;
    private SoundEmiter _emiter;
    public string nameItemToHave;
    public ItemObject itemGet;
}
