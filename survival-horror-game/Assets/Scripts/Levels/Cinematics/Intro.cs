using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro : Cinematic
{
    private void Update()
    {
    }

    protected override IEnumerator StartLevelObject()
    {
        DontDestroyOnLoad dd = FindObjectOfType<DontDestroyOnLoad>();

        // Thunder begin
        yield return new WaitForSeconds(1);
        player.hud.helper.DisplayInfo("Appuyer sur " + dd.GetKey(Controls.Interact).Item1 + " pour passer le dialogue");
        yield return StartCoroutine(SaySomething("Zzzzzzzzz"));
        player.hud.helper.StopDisplayingInfo();
        yield return new WaitForSeconds(3);
        lightning.Strike();

        // Some text
        yield return new WaitForSeconds(6);
        lightning.StartLoopStrike(8, 12);
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(SaySomething("Aaaah !"));
        yield return StartCoroutine(SaySomething("Qu'est ce que c'est ?"));
        yield return StartCoroutine(SaySomething("J'ai peur de l'orage !!!"));

        // Turn on the light
        yield return StartCoroutine(MoveTo(lampTurnOnPosition, LookAt.RIGHT));
        yield return new WaitForSeconds(0.5f);
        bedsideLamp.SetIntensity(1f);
        yield return new WaitForSeconds(0.5f);

        // Return at bed
        yield return StartCoroutine(MoveTo(whereToBegin, LookAt.DOWN));
        yield return StartCoroutine(SaySomething("Je ne vais pas réussir à me rendormir... Maintenant"));
        yield return StartCoroutine(SaySomething("Je ne sais pas quoi faire"));
        yield return StartCoroutine(SaySomething("..."));
        yield return new WaitForSeconds(3);
        yield return StartCoroutine(SaySomething("Surtout ne pas pleurer"));
        yield return new WaitForSeconds(3);

        // The light is winking
        StartCoroutine(bedsideLamp.StartWink(10, 15, 50, 0, 1, true));
        yield return StartCoroutine(SaySomething("QU'EST CE QUE..."));

        // The light is shutting down
        while (bedsideLamp.IsWinking())
        {
            yield return null;
        }

        yield return StartCoroutine(SaySomething("QU'EST CE QUI SE PASSE"));
        yield return StartCoroutine(SaySomething("J'AI PEUUUUUR"));
        yield return StartCoroutine(SaySomething("Papa : OH TU LA BOUCLE ET TU DORS !"));
        yield return new WaitForSeconds(3);
        yield return StartCoroutine(SaySomething("*Snif*"));
        yield return StartCoroutine(SaySomething("Oh j'y pense, il y a une lampe dans l'armoire, je pourrais la récupérer !"));
        
        Stop();
    }

    public Light bedsideLamp;
    public Thunder lightning;
    public Vector2 lampTurnOnPosition;
}
