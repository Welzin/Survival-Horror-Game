using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstMission : Mission
{
    private void Start()
    {
        _dog = niche.AddComponent<SoundEmiter>();
        _dog.SetNoiseEmited(NoiseType.Ouaf);
    }

    protected override IEnumerator StartLevelObject()
    {
        brokenChest.gameObject.SetActive(false);
        seau.gameObject.SetActive(false);

        yield return SaySomething("Il faut que j'aille au rez de chaussée pour sortir !");

        StartCoroutine(ManageDog());

        // On attend que le joueur est coupé le courant
        yield return WaitForEvent(poweroff);
        tele.gameObject.SetActive(false);

        StartCoroutine(ManagePoweroff());
        
        // On attend que le joueur ait passé la porte de la chambre des parents
        yield return WaitForEvent(parentRoomDoor);
        yield return SaySomething("Le carnet doit se situer quelque part ici, il faut que je cherche !");

        // Une fois qu'il a récupéré le carnet
        yield return WaitForItemInInventory("Carnet");
        yield return SaySomething("Il faut que je trouve le code maintenant");
        yield return SaySomething("Voyons voir...");
        yield return SaySomething("Je l'ai !");
        
        // On ajoute le code et on enlève le carnet
        player.inventory.AddItem(code.item);
        player.inventory.ItemUsed("Carnet");
        chest.textToHelp = "Pourquoi le code ne marche pas ?";

        // Le seau était disable pour éviter de faire le generator avant le reste
        seau.gameObject.SetActive(true);

        // Il va être obligé de détruire le générateur pour ouvrir le coffre
        yield return WaitForEvent(generator);
        brokenChest.gameObject.SetActive(true);
        chest.gameObject.SetActive(false);

        // Lorsqu'il ouvre la porte, la cinématique commence
        yield return WaitForEvent(mainDoor);
        Stop();
    }

    private IEnumerator ManageDog()
    {
        _alreadyInside = false;

        while (player.GetLastEvent() != dogEat)
        {
            if (!_alreadyInside && player.CurrentFloor == 1 && (player.transform.position - niche.transform.position).magnitude < 5f)
            {
                _alreadyInside = true;
                _dog.PlayCustomClip(aboiement, 20);
                _dog.EmitSoundWave(20, 1, 4);
                StartCoroutine(WaitDog());
            }

            yield return null;
        }
    }

    private IEnumerator WaitDog()
    {
        yield return new WaitForSecondsRealtime(4f);
        _alreadyInside = false;
    }

    private IEnumerator ManagePoweroff()
    {
        while (player.GetLastEvent() != parentRoomDoor)
        {
            generator.cannotDoEventAnymore = false;
            yield return null;
        }

        generator.cannotDoEventAnymore = true;
    }
    
    public Door mainDoor;
    public Door parentRoomDoor;
    public LevelEvent chest;
    public LevelEvent brokenChest;
    public LevelEvent generator;
    public LevelEvent poweroff;
    public LevelEvent dogEat;
    public ItemObject code;
    public ItemObject seau;
    public DialogEvent tele;
    public GameObject niche;
    public AudioClip aboiement;

    private SoundEmiter _dog;
    private bool _alreadyInside;
}
