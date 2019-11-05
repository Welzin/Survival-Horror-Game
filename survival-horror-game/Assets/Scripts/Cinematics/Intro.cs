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
        player.transform.position = initialPosition;
        yield return new WaitForSeconds(1);
        player.Speak("ZZZZZZzzzzz");
        yield return new WaitForSeconds(3);
        StartCoroutine(lightning.StartWink(10, 25));
        
        while (lightning.IsWinking())
        {
            yield return null;
        }

        yield return new WaitForSeconds(1);
        StartCoroutine(lightning.StartWink(10, 25, 0.6f));
        player.Speak("Aaaaah! Qu'est ce que c'est ?");
        player.PassDialog();

        while (lightning.IsWinking())
        {
            yield return null;
        }

        yield return new WaitForSeconds(1);
        StartCoroutine(lightning.StartWink(10, 25, 0.6f));
        player.Speak("Quel orage !");
        player.PassDialog();

        yield return new WaitForSeconds(1);
        player.PassDialog();
        player.SetNewDestination(lampTurnOnPosition);

        while (player.IsMoving())
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        bedsideLamp.SetIntensity(1f);
        yield return new WaitForSeconds(0.5f);

        player.Speak("J'ai peur de l'orage... Brrrr !! Mais mon papa a dit qu'il fallait pas pleurer !");
        yield return new WaitForSeconds(3);
        player.PassDialog();

        player.SetNewDestination(intermediaryPosition);

        while (player.IsMoving())
        {
            yield return null;
        }

        player.SetNewDestination(libraryPosition);

        while (player.IsMoving())
        {
            yield return null;
        }

        player.Speak("Voyons si je peux passer un peu de temps à lire...");
        yield return new WaitForSeconds(2);
        StartCoroutine(bedsideLamp.StartWink(10, 100, 0, 0.5f, true));
        yield return new WaitForSeconds(0.5f);
        player.Speak("Aaaaah qu'est ce que c'est ?");
        player.PassDialog();
        yield return new WaitForSeconds(2);
        player.PassDialog();

        player.SetNewDestination(intermediaryPosition);

        while (player.IsMoving())
        {
            yield return null;
        }

        player.SetNewDestination(lampTurnOnPosition);

        player.Speak("J'ai peur");

        while (player.IsMoving())
        {
            yield return null;
        }

        player.Speak("Voyons ce que je peux faire ...");
        player.PassDialog();
        yield return new WaitForSeconds(2);
        player.PassDialog();

        while (bedsideLamp.IsWinking())
        {
            yield return null;
        }

        player.Speak("Allume toooooi ! Nooooon !");
        yield return new WaitForSeconds(1);
        greenLight.SetIntensity(0.5f);
        yield return new WaitForSeconds(1);
        player.PassDialog();
        player.Speak("Tiens l'orage s'est arrêté, mais qu'est ce que c'est que cette lumière ?");

        player.SetNewDestination(intermediaryPosition);

        while (player.IsMoving())
        {
            yield return null;
        }

        player.SetNewDestination(windowPosition);
        
        while (player.IsMoving())
        {
            yield return null;
        }

        player.PassDialog();
        player.Speak("Oooh c'est beau, ça vient de dehors, j'ai envie d'aller voir !");
        yield return new WaitForSeconds(2);
        player.PassDialog();
        player.Speak("Bon faut que je fasse attention à mes parents, s'ils me surprennent, ça va être la fessée !");
        yield return new WaitForSeconds(3);
        player.PassDialog();
        player.Speak("Mais on y voit rien... Oh j'y pense, ma lampe !");
        yield return new WaitForSeconds(1);
        player.SetNewDestination(libraryPosition);

        while (player.IsMoving())
        {
            yield return null;
        }

        player.PassDialog();
        player.Speak("Est-ce qu'elle marche toujours ?");
        yield return new WaitForSeconds(2);
        player.ToggleLamp();
        player.PassDialog();
        player.Speak("Oui !");
        yield return new WaitForSeconds(1);
        player.PassDialog();
        player.Speak("C'est parti !");
        yield return new WaitForSeconds(1);
        player.PassDialog();

        StopCinematic(); 
    }
    
    public Light bedsideLamp;
    public Light lightning;
    public Light greenLight;
    public Vector3 initialPosition;
    public Vector3 lampTurnOnPosition;
    public Vector3 intermediaryPosition;
    public Vector3 libraryPosition;
    public Vector3 windowPosition;
}
