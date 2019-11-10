using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstMission : Mission
{
    private void Start()
    {
        _dog = niche.AddComponent<SoundEmiter>();
        _dog.SetNoiseEmited(NoiseType.Ouaf);
        
        _passDoor = false;

        StartCoroutine(BeginPattern());
    }

    private IEnumerator BeginPattern()
    {
        yield return new WaitForSeconds(1);
        //mom.StopActions();
        //dad.StopActions();
    }

    protected override IEnumerator StartLevelObject()
    {
        yield return new WaitForSeconds(1);
        dad.MoveTo(televisionPosition, 2);
        mom.PlayPattern();
        brokenChest.gameObject.SetActive(false);
        seau.gameObject.SetActive(false);

        yield return SaySomething("Il faut que j'aille au rez de chaussée pour sortir !");

        StartCoroutine(ManageDog());
        StartCoroutine(PowerOff());

        // Une fois qu'il a récupéré le carnet
        yield return WaitForItemInInventory("Carnet");
        yield return SaySomething("Il faut que je trouve le code maintenant");
        yield return SaySomething("Voyons voir...");
        yield return SaySomething("Je l'ai !");
        
        // On ajoute le code et on enlève le carnet
        player.inventory.AddItem(code.item);
        player.inventory.ItemUsed("Carnet");
        chest.textToHelp = "Pourquoi le code ne marche pas ?";

        StartCoroutine(ManageParent());

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

    private IEnumerator PassDoor()
    {
        yield return WaitForEvent(parentRoomDoor);
        yield return SaySomething("Le carnet doit se situer quelque part ici, il faut que je cherche !");
        _passDoor = true;
    }

    private IEnumerator PowerOff()
    {
        StartCoroutine(PassDoor());

        // On attend que le joueur ait passé la porte de la chambre des parents
        while (!_passDoor)
        {
            // On attend que le joueur est coupé le courant
            while (poweroff != player.GetLastEvent() && !_passDoor)
            {
                yield return null;
            }

            if (_passDoor)
            {
                player.SetLastEvent(null);
                tele.gameObject.SetActive(false);

                yield return SaySomething("Papa : Qu'est ce qu'il se passe ??? Je vais aller voir !");
                dad.MoveTo(poweroff.transform.position, 1);

                while (!dad.MovementHelper().IsMovementFinished())
                {
                    yield return null;
                }

                yield return new WaitForSeconds(4);
                yield return SaySomething("Papa : C'est bon le courant est revenu !");
                poweroff.cannotDoEventAnymore = false;
                dad.MoveTo(televisionPosition, 2);

                while (!dad.MovementHelper().IsMovementFinished())
                {
                    yield return null;
                }
            }

            if (!_passDoor)
            {
                tele.gameObject.SetActive(true);
            }
        }

        tele.gameObject.SetActive(false);
        dad.PlayPattern();
    }

    private IEnumerator ManageParent()
    {
        while (player.GetLastEvent() != chest)
        {
            yield return null;
        }
        
        dad.MoveTo(chest.transform.position, 1);
        mom.MoveTo(chest.transform.position, 1);

        while (!dad.MovementHelper().IsMovementFinished() || !mom.MovementHelper().IsMovementFinished())
        {
            yield return null;
        }

        yield return StartCoroutine(SaySomething("Maman : Qu'est ce qu'il se passe ?"));
        yield return StartCoroutine(SaySomething("Papa : Je ne sais pas, le coffre a sonné, mai sil n'y a personne ..."));
        yield return StartCoroutine(SaySomething("Maman : Je t'avais bien dit de le réparer ..."));
        yield return StartCoroutine(SaySomething("Papa : Oui je sais... Je ferais ça un autre jour..."));

        mom.PlayPattern();
        dad.PlayPattern();
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
    public Monster dad;
    public Monster mom;
    public Television television;
    public Vector2 televisionPosition;

    private SoundEmiter _dog;
    private bool _alreadyInside;
    private bool _passDoor;
}
