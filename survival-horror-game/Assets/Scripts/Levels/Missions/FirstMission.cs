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

        Invoke("BeginPattern", 3);
    }

    private void BeginPattern()
    {
        mom.StopActions();
        _dadPattern = dad.ActualPattern;
        DadGoToTelevision();
    }

    private void DadGoToTelevision()
    {
        Queue<Pattern> televisionPatterns = new Queue<Pattern>();
        televisionPatterns.Enqueue(new Pattern
        {
            goTo = televisionNode,
            intervalUntilNextAction = 1
        });
        dad.ActualPattern = televisionPatterns;
        dad.PlayPattern();
    }

    private void DadGoToPoweroff()
    {
        Queue<Pattern> poweroffPatterns = new Queue<Pattern>();
        poweroffPatterns.Enqueue(new Pattern
        {
            goTo = poweroffNode,
            intervalUntilNextAction = 1
        });
        dad.ActualPattern = poweroffPatterns;
        dad.PlayPattern();
    }

    private void DadFollowHisPattern()
    {
        dad.ActualPattern = _dadPattern;
        dad.PlayPattern();
    }

    protected override IEnumerator StartLevelObject()
    {
        yield return new WaitForSeconds(1);
        mom.PlayPattern();
        brokenChest.gameObject.SetActive(false);
        seau.gameObject.SetActive(false);

        yield return SaySomething(new Dialog("Il faut que j'aille au rez de chaussée pour sortir !"));

        StartCoroutine(ManageDog());
        StartCoroutine(PowerOff());

        // Une fois qu'il a récupéré le carnet
        yield return WaitForItemInInventory("Carnet");
        yield return SaySomething(new Dialog("Il faut que je trouve le code maintenant"));
        yield return SaySomething(new Dialog("Voyons voir..."));
        yield return SaySomething(new Dialog("Je l'ai !", Expression.HAPPY));
        
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
        while (player.GetLastEvent() != dogEat)
        {
            if (player.CurrentFloor == 1 && (player.transform.position - niche.transform.position).magnitude < 5f)
            {
                _dog.PlayCustomClip(aboiement, 20);
                _dog.EmitSoundWave(20, 1, 4);
                yield return new WaitForSecondsRealtime(4f);
            }

            yield return null;
        }
    }

    private IEnumerator PassDoor()
    {
        yield return WaitForEvent(parentRoomDoor);
        yield return SaySomething(new Dialog("Le carnet doit se situer quelque part ici, il faut que je cherche !"));
        _passDoor = true;

        // On ne peut plus couper le courant une fois la porte passée
        poweroff.cannotDoEventAnymore = true;

        // Le père reprend son pattern, il ne reste plus devant la télé
        DadFollowHisPattern();
    }

    private IEnumerator PowerOff()
    {
        StartCoroutine(PassDoor());

        // On attend que le joueur ait passé la porte de la chambre des parents
        while (!_passDoor)
        {
            // On attend que le joueur ait coupé le courant
            yield return WaitForEvent(poweroff);
            StopCoroutine(DadComeBackToTelevision());

            // On fait ça pour éviter que ça tourne en boucle si le joueur n'a pas fait de nouvel événement à la fin de la boucle
            player.SetLastEvent(null);

            // Permet de passer dans le salon
            tele.gameObject.SetActive(false);

            // Le père va voir dans le garage
            yield return SaySomething(new Dialog("Qu'est ce qu'il se passe ??? Je vais aller voir !", Expression.SURPRISED, Person.DAD));
            DadGoToPoweroff();

            // On attend que le père soit arrivé au garage
            while (!dad.MovementHelper().IsMovementFinished())
            {
                yield return null;
            }

            // Le courant a été rétabli donc le père retourne en haut et le joueur peut de nouveau couper le courant
            yield return new WaitForSeconds(4);
            yield return SaySomething(new Dialog("C'est bon le courant est revenu !", Expression.HAPPY, Person.DAD));
            poweroff.cannotDoEventAnymore = false;
            DadGoToTelevision();
            StartCoroutine(DadComeBackToTelevision());
        }
    }

    private IEnumerator DadComeBackToTelevision()
    {
        while (!dad.MovementHelper().IsMovementFinished())
        {
            yield return null;
        }

        tele.gameObject.SetActive(true);
        television.TurnOn();
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

        yield return StartCoroutine(SaySomething(new Dialog("Qu'est ce qu'il se passe ?", Expression.SURPRISED, Person.MOM)));
        yield return StartCoroutine(SaySomething(new Dialog("Je ne sais pas, le coffre a sonné, mais il n'y a personne ...", Expression.SURPRISED, Person.DAD)));
        yield return StartCoroutine(SaySomething(new Dialog("Je t'avais bien dit de le réparer ...", Expression.ANGRY, Person.MOM)));
        yield return StartCoroutine(SaySomething(new Dialog("Oui je sais... Je ferais ça un autre jour...", Expression.DEAD, Person.DAD)));

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
    public Node televisionNode;
    public Node poweroffNode;

    private SoundEmiter _dog;
    private bool _passDoor;
    private Queue<Pattern> _dadPattern;
}
