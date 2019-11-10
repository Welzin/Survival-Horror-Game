using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : Mission
{
    protected override IEnumerator StartLevelObject()
    {
        battery.gameObject.SetActive(false);
        keyToOpenTheRoom.gameObject.SetActive(false);
        teddy.gameObject.SetActive(false);

        player.hud.transform.Find("Stress").gameObject.SetActive(false);
        player.hud.transform.Find("Battery").gameObject.SetActive(false);
        player.hud.transform.Find("BatteryItem").gameObject.SetActive(false);

        DontDestroyOnLoad dd = FindObjectOfType<DontDestroyOnLoad>();

        StartCoroutine(DontLoseStress());

        // Tutoriel
        player.hud.helper.DisplayInfo("Appuyer sur "
            + dd.GetKey(Controls.Up).Item1 + " "
            + dd.GetKey(Controls.Left).Item1 + " "
            + dd.GetKey(Controls.Down).Item1 + " "
            + dd.GetKey(Controls.Right).Item1 + " pour bouger.\n");

        yield return new WaitForSeconds(3);

        // Search lamp
        player.hud.helper.DisplayInfo("Pour récupérer un objet, appuyer sur " + dd.GetKey(Controls.Interact).Item1 + ". " +
            "Attention, récupérer un objet peut faire du bruit !" + "\n\nAller jusqu'à la bibliothèque pour récupérer la lampe, c'est une petite étoile tourbillonante.");

        while (!player.inventory.HaveLamp())
        {
            yield return null;
        }

        player.hud.helper.StopDisplayingInfo();
        player.hud.helper.DisplayInfo("Appuyer sur " + dd.GetKey(Controls.Lamp).Item1 + " pour allumer la lampe ou l'éteindre.");

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
        SoundEmiter emiter = greenLight.gameObject.AddComponent<SoundEmiter>();
        yield return null;
        emiter.PlayCustomClip(scream, 100);
        player.AddStress(player.maxStress);

        MusicManager m = FindObjectOfType<MusicManager>();
        m.UpdateMusic(2);
        m.PlayLoop();

        player.hud.helper.DisplayInfo("Le stress est un élément essentiel, vous pouvez voir votre barre actuelle de stress en haut à droite de l'écran, en rouge." +
            " Lorsque cette jauge atteint un seuil critique, votre personnage panique et devient hors de contrôle pendant quelques secondes !" +
            "\n\n/!\\ Rester paniqué plus de 10 secondes et c'est le game over !");
        yield return StartCoroutine(SaySomething("Aaaaaaaah !"));
        yield return StartCoroutine(SaySomething("QU'EST CE QUE C'EST QUE CA ???"));
        yield return StartCoroutine(SaySomething("J'ai besoin de mon Teddy, où est mon Teddy ?"));

        // Get Teddy
        teddy.gameObject.SetActive(true);
        player.hud.helper.StopDisplayingInfo();
        player.hud.helper.DisplayInfo("Des événements peuvent considérablement augmenter votre stress. Rester dans le noir provoque également la peur." +
            " Il y a plusieurs façon de faire descendre le stress, le premier étant de se trouver dans une zone lumineuse ou d'allumer la lampe." +
            " La seconde est de serrer bien fort Teddy contre vous. Aller le récupérer dans le lit !");

        while (!player.inventory.HaveTeddy())
        {
            yield return null;
        }

        yield return StartCoroutine(SaySomething("Oui ! Mon Teddy !"));
        player.hud.helper.StopDisplayingInfo();
        player.hud.helper.DisplayInfo("Pour serrer Teddy contre vous, appuyer sur " + dd.GetKey(Controls.HugTeddy).Item1 + ".");

        while (!player.IsHuggingTeddy())
        {
            yield return null;
        }

        player.hud.helper.StopDisplayingInfo();
        player.hud.helper.DisplayInfo("Remarquez que serrer Teddy prend du temps et empêche toute autre action ! Soyez vigilant avant de l'utiliser !" +
            " Pour arrêter le câlin, appuyer de nouveau sur " + dd.GetKey(Controls.HugTeddy).Item1 + "." +
            "\n\nUtiliser Teddy pour redevenir complètement calme");

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

        player.hud.helper.DisplayInfo("Bienvenue dans l'aventure ! Rappelez vous de ne pas faire de bruit au risque de vous faire attraper, vous pouvez courir en utilisant " +
            dd.GetKey(Controls.Run).Item1 + " mais attention, courir produit beaucoup plus de bruit !", 10);

        Stop();
    }

    private IEnumerator DontLoseStress()
    {
        while (greenLight.intensity < 1f)
        {
            player.AddStress(-player.maxStress);
            yield return null;
        }
    }

    public Light greenLight;
    public ItemObject keyToOpenTheRoom;
    public ItemObject battery;
    public ItemObject teddy;
    public AudioClip scream;
}
