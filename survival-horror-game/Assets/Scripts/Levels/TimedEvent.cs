using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TimedEvent : Event
{
    protected void Start()
    {
        base.Start();
        cannotDoEventAnymore = false;
    }

    public override void PlayEvent()
    {
        if (!cannotDoEventAnymore && ConditionsToRespect())
        {
            WhatToDoBeforeEvent();
            StartCoroutine(StartTimedEvent());
        }
        else if(!cannotDoEventAnymore)
        {
            if (textToHelp != "")
                player.Speak(textToHelp);

            WhatToDoIfConditionNotRespected();
        }
        else
        {
            if (disableEventText != "")
                player.Speak(disableEventText);
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
        float time = 0f;

        while (!action.interrupted && time < eventTime)
        {
            yield return new WaitForSecondsRealtime(0.5f);
            time += 0.5f;
        }

        if (!action.interrupted)
        {
            if (victoryText != "")
                player.Speak(victoryText);

            player.SetLastEvent(this);
            WhatToDoAfterEvent();

            if (disableEvent)
                gameObject.SetActive(false);
        }
        else
        {
            WhatToDoOnEventInterruption();
        }
    }

    public float eventTime;
    public string textToHelp;
    public string victoryText;
    public string disableEventText;
    public bool disableEvent;

    public bool cannotDoEventAnymore;
}
