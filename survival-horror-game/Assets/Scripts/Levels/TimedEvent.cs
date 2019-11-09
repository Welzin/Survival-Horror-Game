using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TimedEvent : Event
{
    public void AddSomethingToSayWhenConditionNotRespected(string text)
    {
        _textToSayWhenConditionIsNotRespected = text;
    }

    public void AddSomethingToSayWhenEventIsDone(string text)
    {
        _textToSayWhenActionIsDone = text;
    }

    public override void PlayEvent()
    {
        if (ConditionsToRespect())
        {
            WhatToDoBeforeEvent();
            StartCoroutine(StartTimedEvent());
        }
        else
        {
            player.Speak(_textToSayWhenConditionIsNotRespected);
            WhatToDoIfConditionNotRespected();
        }
    }
    
    protected abstract bool ConditionsToRespect();
    protected abstract void WhatToDoIfConditionNotRespected();
    protected abstract void WhatToDoBeforeEvent();
    protected abstract void WhatToDoAfterEvent();
    protected abstract void WhatToDoOnEventInterruption();

    private IEnumerator StartTimedEvent()
    {
        Action action = player.hud.actionBar.StartAction(eventTime);
        yield return new WaitForSeconds(eventTime);

        if (!action.interrupted)
        {
            player.Speak(_textToSayWhenActionIsDone);
            WhatToDoAfterEvent();
        }
        else
        {
            WhatToDoOnEventInterruption();
        }
    }

    public float eventTime;
    private string _textToSayWhenConditionIsNotRespected;
    private string _textToSayWhenActionIsDone;
}
