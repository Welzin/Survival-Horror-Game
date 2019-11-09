using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstMission : Mission
{
    protected override IEnumerator StartLevelObject()
    {
        brokenChest.gameObject.SetActive(false);
        seau.gameObject.SetActive(false);

        yield return SaySomething("Il faut que j'aille au rez de chaussée pour sortir !");

        // On attend que le joueur est coupé le courant
        //yield return WaitForEvent(poweroff);
        tele.gameObject.SetActive(false);
        
        //yield return WaitForEvent(parentRoomDoor);
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
    
    public Door mainDoor;
    public Door parentRoomDoor;
    public LevelEvent chest;
    public LevelEvent brokenChest;
    public LevelEvent generator;
    public LevelEvent poweroff;
    public ItemObject code;
    public ItemObject seau;
    public DialogEvent tele;
}
