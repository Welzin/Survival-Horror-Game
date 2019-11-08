using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunder : Light
{
    public void Strike()
    {
        StartCoroutine(NewStrike());
        _audio = gameObject.AddComponent<AudioSource>();
        _emiter = gameObject.AddComponent<SoundEmiter>();
    }

    private IEnumerator NewStrike()
    {
        int random = Random.Range(1, 6);
        StartCoroutine(StartWink(10, 25));
        yield return new WaitForSeconds(Random.Range(1, 3));

        _audio.clip = Resources.Load<AudioClip>("Effects/Thunder/tonnerre" + random);
        _audio.Play();
    }

    private AudioSource _audio;
    private SoundEmiter _emiter;
}
