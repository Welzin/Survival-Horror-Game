using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro : Cinematic
{
    private void Update()
    {
    }

    protected override IEnumerator StartCinematic()
    {
        player.hud.transform.Find("Stress").gameObject.SetActive(false);
        player.hud.transform.Find("Battery").gameObject.SetActive(false);
        player.hud.transform.Find("BatteryItem").gameObject.SetActive(false);

        // Start in the bed
        player.transform.position = initialPosition;

        // Thunder begin
        yield return new WaitForSeconds(1);
        player.hud.helper.DisplayInfo("Appuyer sur " + FindObjectOfType<DontDestroyOnLoad>().GetKey(Controls.Interact).Item1 + " pour passer le dialogue");
        yield return StartCoroutine(SaySomething("Zzzzzzzzz"));
        player.hud.helper.StopDisplayingInfo();
        yield return new WaitForSeconds(3);
        StartCoroutine(StartThunder());

        // Some text
        yield return new WaitForSeconds(6);
        yield return StartCoroutine(SaySomething("Aaaah !"));
        yield return StartCoroutine(SaySomething("Qu'est ce que c'est ?"));
        yield return StartCoroutine(SaySomething("J'ai peur de l'orage !!!"));

        // Turn on the light
        yield return StartCoroutine(MoveTo(lampTurnOnPosition, LookAt.RIGHT));
        yield return new WaitForSeconds(0.5f);
        bedsideLamp.SetIntensity(1f);
        yield return new WaitForSeconds(0.5f);

        // Return at bed
        yield return StartCoroutine(MoveTo(initialPosition, LookAt.DOWN));
        yield return StartCoroutine(SaySomething("Je ne vais pas réussir à me rendormir... Maintenant"));
        yield return StartCoroutine(SaySomething("Je ne sais pas quoi faire"));
        yield return StartCoroutine(SaySomething("..."));
        yield return new WaitForSeconds(3);
        yield return StartCoroutine(SaySomething("Surtout ne pas pleurer"));
        yield return new WaitForSeconds(3);

        // The light is winking
        StartCoroutine(bedsideLamp.StartWink(10, 50, 0, 1, true));
        yield return StartCoroutine(SaySomething("QU'EST CE QUE..."));

        // The light is shutting down
        while (bedsideLamp.IsWinking())
        {
            yield return null;
        }

        yield return StartCoroutine(SaySomething("QU'EST CE QUI SE PASSE"));
        yield return StartCoroutine(SaySomething("Bon, pas de panique, il y a une lampe dans l'armoire, il faut que je la réupère !"));
        yield return StartCoroutine(SaySomething("Viiiiite !"));

        StopCinematic();
    }

    private IEnumerator StartThunder()
    {
        _thunderOn = true;
        while (_thunderOn)
        {
            StartCoroutine(lightning.StartWink(10, 25));
            yield return new WaitForSeconds(Random.Range(5, 13));
        }
    }

    public Light bedsideLamp;
    public Light lightning;
    public Vector3 initialPosition;
    public Vector3 lampTurnOnPosition;

    private bool _thunderOn;
}
