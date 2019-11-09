using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogEvent : Event
{
    public override void PlayEvent()
    {
        StartCoroutine(Speak());
    }

    private IEnumerator Speak()
    {
        foreach (string dialog in whatToSay)
        {
            player.Speak(dialog);
            while (player.IsSpeaking())
            {
                yield return null;
            }
        }

        if (justOnce)
        {
            Destroy(this);
        }
    }

    public List<string> whatToSay;
    public bool justOnce;
}
