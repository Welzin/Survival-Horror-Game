using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEvent : TimedEvent
{
    protected void Awake()
    {
        _emiter = gameObject.AddComponent<SoundEmiter>();
        _emiter.SetNoiseEmited(NoiseType.Event);
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

        cannotDoEventAnymore = true;

        if (soundMakeAfter != null)
        {
            _emiter.PlayCustomClip(soundMakeAfter, intensitySoundMakeAfter);
            _emiter.EmitSoundWave(intensitySoundMakeAfter, player.CurrentFloor, soundMakeAfterTime);

            StartCoroutine(StopSoundMakeAfter());
        }
        else
        {
            _emiter.StopEffect();
        }
    }

    protected override void WhatToDoBeforeEvent()
    {
        if (soundMake != null)
        {
            Debug.Log(intensitySoundMake);
            _emiter.PlayCustomClip(soundMake, intensitySoundMake);
            _emiter.EmitSoundWave(intensitySoundMake, player.CurrentFloor, eventTime);
        }
    }

    protected override void WhatToDoIfConditionNotRespected()
    {
    }

    protected override void WhatToDoOnEventInterruption()
    {
        _emiter.StopEffect();
    }

    private IEnumerator StopSoundMakeAfter()
    {
        yield return new WaitForSecondsRealtime(soundMakeAfterTime);
        _emiter.StopEffect();
    }

    public AudioClip soundMake;
    public float intensitySoundMake;
    public AudioClip soundMakeAfter;
    public float intensitySoundMakeAfter;
    public float soundMakeAfterTime;
    private SoundEmiter _emiter;
    public string nameItemToHave;
    public ItemObject itemGet;
}
