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
        yield return null;
        /*yield return new WaitForSeconds(1);
        player.Speak("Brrr il fait froid !");
        player.Speak("Quelle est cette lumière ?");

        yield return new WaitForSeconds(1);
        player.PassDialog();
        yield return new WaitForSeconds(1);
        player.PassDialog();*/

        StopCinematic();
    }
    
    public Light lamp;
    public Light lightning;
}
