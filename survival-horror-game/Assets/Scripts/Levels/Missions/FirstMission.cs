using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstMission : Mission
{
    protected override IEnumerator StartLevelObject()
    {
        yield return SaySomething("Il faut que j'aille au rez de chaussée pour sortir !");

        yield return WaitForItemInInventory("carnet");

        yield return SaySomething("Il faut que je trouve le code maintenant");
        yield return SaySomething("Voyons voir...");
        yield return SaySomething("Je l'ai !");

        Utility code = new Utility(Resources.Load<Sprite>(""), "Code");
        player.inventory.AddItem(code);

        StartCoroutine(ToSayWhenNotGoodCode());
    }

    private IEnumerator ToSayWhenNotGoodCode()
    {
        while (!player.inventory.HaveItem("DecryptedCode"))
        {
            yield return WaitForEvent(coffre);
            if (!player.inventory.HaveItem("DecryptedCode"))
                yield return SaySomething("Pourquoi le code ne marche pas ?");
        }
    }

    public List<LevelEvent> windowsForReadTheCode;
    public LevelEvent coffre;
}
