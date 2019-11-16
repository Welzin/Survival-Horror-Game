using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro : Cinematic
{
    public void ToDoIfNoIntro()
    {
        lightning.StartLoopStrike(8, 12);
    }

    protected override IEnumerator StartLevelObject()
    {
        DontDestroyOnLoad dd = FindObjectOfType<DontDestroyOnLoad>();

        // Thunder begin
        yield return new WaitForSeconds(1);
        player.hud.helper.DisplayInfo("Appuyez sur " + dd.GetKey(Controls.Interact).Item1 + " pour continuer.");
        yield return StartCoroutine(SaySomething(new Dialog("Zzzzzzzzz", Expression.SLEEPING)));
        player.hud.helper.StopDisplayingInfo();
        yield return new WaitForSeconds(3);
        lightning.Strike();

        // Some text
        yield return new WaitForSeconds(6);
        lightning.StartLoopStrike(8, 12);
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(SaySomething(new Dialog("Aaaah !", Expression.AFRAID)));
        yield return StartCoroutine(SaySomething(new Dialog("Qu'est ce que c'est ?", Expression.AFRAID)));
        yield return StartCoroutine(SaySomething(new Dialog("J'ai peur de l'orage !!!", Expression.SAD)));

        // Turn on the light
        yield return StartCoroutine(MoveTo(lampTurnOnPosition, LookAt.RIGHT));
        yield return new WaitForSeconds(0.5f);
        bedsideLamp.SetIntensity(1f);
        yield return new WaitForSeconds(0.5f);

        // Return at bed
        yield return StartCoroutine(MoveTo(whereToBegin, LookAt.DOWN));
        yield return StartCoroutine(SaySomething(new Dialog("Je ne vais pas réussir à me rendormir maintenant...", Expression.DISAPPOINTED)));
        yield return StartCoroutine(SaySomething(new Dialog("Je ne sais pas quoi faire", Expression.DISAPPOINTED)));
        yield return StartCoroutine(SaySomething(new Dialog("...", Expression.SAD)));
        yield return new WaitForSeconds(3);
        yield return StartCoroutine(SaySomething(new Dialog("Surtout ne pas pleurer", Expression.SAD)));
        yield return new WaitForSeconds(3);

        // The light is winking
        StartCoroutine(bedsideLamp.StartWink(10, 15, 50, 0, 1, true));
        yield return StartCoroutine(SaySomething(new Dialog("QU'EST CE QUE...", Expression.SURPRISED)));

        // The light is shutting down
        while (bedsideLamp.IsWinking())
        {
            yield return null;
        }

        yield return StartCoroutine(SaySomething(new Dialog("QU'EST CE QU'IL SE PASSE", Expression.AFRAID)));
        yield return StartCoroutine(SaySomething(new Dialog("J'AI PEUUUUUR", Expression.AFRAID)));
        yield return StartCoroutine(SaySomething(new Dialog("OH TU LA BOUCLES ET TU DORS !", Expression.ANGRY, Person.DAD)));
        yield return new WaitForSeconds(3);
        yield return StartCoroutine(SaySomething(new Dialog("*Snif*", Expression.SAD)));
        yield return StartCoroutine(SaySomething(new Dialog("Oh j'y pense, il y a une lampe dans l'armoire, je pourrais la récupérer !")));
        
        Stop();
    }

    public Light bedsideLamp;
    public Thunder lightning;
    public Vector2 lampTurnOnPosition;
}
