using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object with this class should be place where the sound will emit
/// </summary>
public class Thunder : MonoBehaviour
{
    private void Start()
    {
        _emiter = gameObject.AddComponent<SoundEmiter>();
        _emiter.SetNoiseEmited(NoiseType.Lightning);
        _loop = false;

        _allLightning = FindObjectsOfType<Lightning>();

        foreach (Lightning light in _allLightning)
        {
            light.SetIntensity(0);
        }
    }

    private void Update()
    {
        if (objectToFollow != null)
            transform.position = objectToFollow.transform.position;
    }

    public void Strike()
    {
        StartCoroutine(NewStrike());
    }

    public void StartLoopStrike(float intervalMin, float intervalMax)
    {
        _loop = true;
        StartCoroutine(NewLoopStrike(intervalMin, intervalMax));
    }

    public void StopLoopStrike()
    {
        _loop = false;
    }

    private IEnumerator NewStrike()
    {
        int random = Random.Range(1, 6);
        StartCoroutine(StartWink(10, 15, 25));
        yield return new WaitForSeconds(Random.Range(1, 3));

        _emiter.PlayCustomClip(Resources.Load<AudioClip>("Effects/Thunder/tonnerre" + random), 200);
        _emiter.EmitSoundWave(200, 1, 0.3f);
    }

    private IEnumerator NewLoopStrike(float intervalMin, float intervalMax)
    {
        while (_loop)
        {
            Strike();
            yield return new WaitForSeconds(Random.Range(intervalMin, intervalMax));
        }
    }

    public IEnumerator StartWink(float minFrequency, float maxFrequency, int numberOfWink, float minValue = 0f, float maxValue = 1f)
    {
        bool intensify = true;

        for (int i = 0; i <= numberOfWink; i++)
        {
            if (_allLightning.Length == 0)
                break;

            float actualIntensity = _allLightning[0].intensity;
            float newIntensity;

            if (intensify)
            {
                newIntensity = Random.Range(actualIntensity, maxValue);
            }
            else
            {
                newIntensity = Random.Range(minValue, actualIntensity);
            }

            foreach (Lightning light in _allLightning)
            {
                light.SetIntensity(newIntensity);
            }

            intensify = !intensify;
            yield return new WaitForSeconds(1f / Random.Range(minFrequency, maxFrequency));
        }

        foreach (Lightning light in _allLightning)
        {
            light.SetIntensity(0);
        }
    }

    public GameObject objectToFollow;

    private SoundEmiter _emiter;
    private bool _loop;

    private Lightning[] _allLightning;
}
