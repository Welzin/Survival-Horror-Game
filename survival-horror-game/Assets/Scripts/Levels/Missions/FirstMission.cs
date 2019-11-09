using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstMission : Mission
{
    protected override IEnumerator StartLevelObject()
    {
        mainDoor.AddSomethingToSayWhenConditionNotRespected("La porte est fermée, les clés doivent se trouver dans le coffre de la salle de jeu, je devrais aller vérifier !");

        yield return StartCoroutine(SaySomething("Il faut que j'aille au rez de chaussée pour sortir !"));
    }

    public Door mainDoor;
}
