using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : TimedEvent
{
    public enum Type
    {
        Utility,
        Key,
        Battery,
        Lamp,
        Teddy
    };

    private void Awake()
    {
        switch (itemType)
        {
            case Type.Utility:
                item = new Utility(sprite, itemName);
                break;
            case Type.Key:
                item = new Key(sprite, doorForTheKey);
                break;
            case Type.Battery:
                item = new Battery();
                break;
            case Type.Lamp:
                item = new LampItem();
                break;
            case Type.Teddy:
                item = new Teddy();
                break;
        }

        _emiter = gameObject.AddComponent<SoundEmiter>();
        _emiter.SetNoiseEmited(NoiseType.Search);
    }

    protected override bool ConditionsToRespect()
    {
        return true;
    }

    protected override void WhatToDoIfConditionNotRespected()
    {
    }

    protected override void WhatToDoBeforeEvent()
    {
        if (searchAudio != null)
        {
            _emiter.PlayCustomClip(searchAudio, intensitySoundMake);
            _emiter.EmitSoundWave(intensitySoundMake, player.CurrentFloor, eventTime);
        }
    }

    protected override void WhatToDoAfterEvent()
    {
        player.inventory.AddItem(item);

        // The object will be destroy and controller.eventInRange equal to null, the helper will not stop display, so, we stop displaying help here
        player.hud.helper.StopDisplayingHelp(Helper.Type.CatchItem);
        Destroy(gameObject);
    }

    protected override void WhatToDoOnEventInterruption()
    {
        _emiter.StopEffect();
    }

    public Item item;
    public Type itemType;
    public string itemName = "";
    public Sprite sprite;
    public Door doorForTheKey;
    public AudioClip searchAudio;
    public float intensitySoundMake;

    private SoundEmiter _emiter;
}
