using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeLevelEvent : Event
{
    private void Start()
    {
        base.Start();
        _startFade = false;
    }

    private void Update()
    {
        if (_startFade)
        {
            fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, fade.color.a + 1 / timeToFade * Time.deltaTime);

            if (fade.color.a >= 1)
            {
                FindObjectOfType<MusicManager>().StopMusic();
                SceneManager.LoadScene(sceneToLoad);
            }
        }
        else if (fade.color.a > 0)
        {
            Destroy(gameObject);
        }
    }

    public override void PlayEvent()
    {
        _startFade = true;
    }

    public string sceneToLoad;
    public float timeToFade;
    public Image fade;

    private bool _startFade;
}
