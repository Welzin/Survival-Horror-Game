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
        DontDestroyOnLoad dd = FindObjectOfType<DontDestroyOnLoad>();
        player.hud.transform.Find("Stress").gameObject.SetActive(false);
        player.hud.transform.Find("Battery").gameObject.SetActive(false);
        player.hud.transform.Find("BatteryItem").gameObject.SetActive(false);
        battery.gameObject.SetActive(false);
        keyToOpenTheRoom.gameObject.SetActive(false);
        teddy.gameObject.SetActive(false);

        // Start in the bed
        player.transform.position = initialPosition;

        // Thunder begin
        yield return new WaitForSeconds(1);
        player.hud.helper.DisplayInfo("Appuyer sur " + dd.GetKey(Controls.Interact).Item1 + " pour passer le dialogue");
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
        yield return StartCoroutine(SaySomething("J'AI PEUUUUUR"));
        yield return StartCoroutine(SaySomething("Papa : OH TU LA BOUCLE ET TU DORS !"));
        yield return new WaitForSeconds(3);
        yield return StartCoroutine(SaySomething("*Snif*"));
        yield return StartCoroutine(SaySomething("Oh j'y pense, il y a une lampe dans l'armoire, je pourrais la récupérer !"));
        
        StopCinematic(); yield return null;
        
        // Tutoriel
        player.hud.helper.DisplayInfo("Appuyer sur "
            + dd.GetKey(Controls.Up).Item1 + " "
            + dd.GetKey(Controls.Left).Item1 + " "
            + dd.GetKey(Controls.Down).Item1 + " "
            + dd.GetKey(Controls.Right).Item1 + " pour bouger.\n");
        
        yield return new WaitForSeconds(3);

        // Search lamp
        player.hud.helper.DisplayInfo("Pur récupérer un objet, appuyer sur " + dd.GetKey(Controls.Interact).Item1 + ". " +
            "Les objets sont identifiables grâce à leur petit symbole tourbillonant. Attention, ils sont presque invisibles sans lampes, " +
            "de plus, il prennent du temps à être ramassé, toute autre action annule la ramassage." + 
            "\n\nAllez jusqu'à la bibliothèque pour récupérer la lampe !");

        while (!player.inventory.HaveLamp())
        {
            yield return null;
        }

        player.hud.helper.StopDisplayingInfo();
        player.hud.helper.DisplayInfo("Appuyer sur " + dd.GetKey(Controls.Lamp).Item1 + " pour allumer la lampe ou l'éteindre.");

        StartCoroutine(SaySomething("Oh la voilà"));

        while (!player.lamp.Active)
        {
            yield return null;
        }

        player.hud.transform.Find("Battery").gameObject.SetActive(true);
        player.hud.helper.StopDisplayingInfo();
        player.hud.helper.DisplayInfo("Attention, utiliser la lampe use des piles. Une fois la jauge de la lampe complètement perdue, " +
            "la lampe ne pourra plus s'allumer. Cette jauge est visible en haut à droite de l'écran, en jaune.");
        yield return new WaitForSeconds(6);
        yield return StartCoroutine(SaySomething("Il n'y a plus beaucoup de piles dans cette lampe, il doit y en avoir une dans mon coffre à jouer !"));

        // Search battery
        player.hud.helper.StopDisplayingInfo();
        player.hud.helper.DisplayInfo("Trouver des piles permet de recharger votre lampe");
        battery.gameObject.SetActive(true);
        
        while (!player.inventory.HaveBattery())
        {
            yield return null;
        }

        player.hud.transform.Find("BatteryItem").gameObject.SetActive(true);
        player.hud.helper.StopDisplayingInfo();
        player.hud.helper.DisplayInfo("Les batteries que vous trouvez sont également affichée sur l'écran." +
            " Pour recharger votre lampe, appuyez sur " + dd.GetKey(Controls.Reload).Item1 + ". Recharger la lampe consomme une pile.");

        while (player.inventory.HaveBattery())
        {
            yield return null;
        }

        player.hud.helper.StopDisplayingInfo();
        yield return new WaitForSeconds(3);

        // Apparition of the green light
        player.hud.transform.Find("Stress").gameObject.SetActive(true);
        greenLight.SetIntensity(1f);
        player.AddStress(player.maxStress);

        StartCoroutine(SaySomething("Aaaaaaaah !"));
        
        yield return new WaitForSeconds(3);
        player.hud.helper.DisplayInfo("Le stress est un élément essentiel, vous pouvez voir votre barre actuelle de stress en haut à droite de l'écran, en rouge." +
            " Lorsque cette jauge atteint un seuil critique, votre personnage panique et devient hors de contrôle !" +
            " Heureusement pour cette fois, la batterie n'est pas complètement utilisée, donc la lampe s'allume automatiquement.");

        yield return new WaitForSeconds(7);
        yield return StartCoroutine(SaySomething("QU'EST CE QUE C'EST QUE CA ???"));
        yield return StartCoroutine(SaySomething("J'ai besoin de mon Teddy, où est mon Teddy ?"));

        // Get Teddy
        teddy.gameObject.SetActive(true);
        player.hud.helper.StopDisplayingInfo();
        player.hud.helper.DisplayInfo("Des événements peuvent considérablement augmenter votre stress. Rester dans le noir provoque également la peur." +
            " Il y a plusieurs façon de faire descendre le stress, le premier étant de se trouver dans une zone lumineuse où d'allumer la lampe." +
            " La seconde est de serrer bien fort Teddy contre vous. Aller le récupérer dans le lit !");

        while (!player.inventory.HaveTeddy())
        {
            yield return null;
        }

        yield return StartCoroutine(SaySomething("Oui ! Mon Teddy !"));
        player.hud.helper.StopDisplayingInfo();
        player.hud.helper.DisplayInfo("Pour serrer Teddy contre vous, appuyer sur " + dd.GetKey(Controls.HugTeddy).Item1 + ".");
        
        while (player.Stress() == player.maxStress)
        {
            yield return null;
        }

        player.hud.helper.StopDisplayingInfo();
        player.hud.helper.DisplayInfo("Remarquez que serrer Teddy prend du temps et empêche toute autre action ! Soyez vigilant avant de l'utiliser !" + 
            "\n\nUtiliser Teddy pour redevenir complètement calme !");

        while (player.Stress() > 0)
        {
            yield return null;
        }

        player.hud.helper.StopDisplayingInfo();
        yield return StartCoroutine(SaySomething("Mais quelle est donc cette étrange lumière ?"));
        yield return StartCoroutine(SaySomething("Je suis curieux de savoir..."));
        yield return StartCoroutine(SaySomething("Mais pour ça il faudrait que je sortes dehors !"));
        yield return StartCoroutine(SaySomething("De toute façon, je n'arrive pas à dormir, c'est décidé, je sors !"));
        yield return StartCoroutine(SaySomething("Juste, il faut que je récupère les clés pour ouvrir ma porte."));
        yield return StartCoroutine(SaySomething("Si papa savait que je gardais un double sous le tapis, il serait vert !"));
        keyToOpenTheRoom.gameObject.SetActive(true);

        Key key = (Key)keyToOpenTheRoom.item;

        while (!player.inventory.HaveKeyForDoor(key.doorToOpen))
        {
            yield return null;
        }

        player.hud.helper.DisplayInfo("Pour ouvrir une porte, placez vous devant et appuyez sur " + dd.GetKey(Controls.Interact).Item1 +
            ". Si la porte est fermée, il vous faut la clé correspondante.");

        while (key.doorToOpen.IsClosed())
        {
            yield return null;
        }

        player.hud.helper.StopDisplayingInfo();
        yield return StartCoroutine(SaySomething("La porte est ouverte"));
        yield return StartCoroutine(SaySomething("Bon, faut que je fasse attention quand même..."));
    }

    private IEnumerator StartThunder()
    {
        _thunderOn = true;
        while (_thunderOn)
        {
            lightning.Strike();
            yield return new WaitForSeconds(Random.Range(5, 13));
        }
    }

    public Light bedsideLamp;
    public Thunder lightning;
    public Light greenLight;
    public ItemObject keyToOpenTheRoom;
    public ItemObject battery;
    public ItemObject teddy;
    public Vector3 initialPosition;
    public Vector3 lampTurnOnPosition;

    private bool _thunderOn;
}
