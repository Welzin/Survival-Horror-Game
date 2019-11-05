using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Light2D;

public class Light : MonoBehaviour
{
    void Awake()
    {
        _isWinking = false;
        _winkingLoop = false;
        SetIntensity(intensity);
    }

    public float effectiveLight
    {
        get { return intensity * (float)strength; }
    }

    public void SetIntensity(float newIntensity)
    {
        intensity = newIntensity;
        LightSprite light = GetComponent<LightSprite>();
        light.Color = new Color(light.Color.r, light.Color.g, light.Color.b, intensity);
        transform.localScale = new Vector2(radius * 2f, radius * 2f);
    }

    public IEnumerator StartWink(float frequency, int numberOfWink, float minValue = 0f, float maxValue = 1f, bool lightShutDown = false)
    {
        _isWinking = true;
        bool intensify = true;
        float intensityAtBegin = intensity;

        for (int i = 0; i <= numberOfWink; i++)
        {
            if (i == numberOfWink)
            {
                if (lightShutDown)
                {
                    SetIntensity(0);
                }
                else
                {
                    SetIntensity(intensityAtBegin);
                }
            }
            else if (intensify)
            {
                SetIntensity(Random.Range(intensity, maxValue));
            }
            else
            {
                SetIntensity(Random.Range(minValue, intensity));
            }

            intensify = !intensify;
            yield return new WaitForSeconds(1f / frequency);
        }

        _isWinking = false;
    }

    public bool IsInWinkingLoop()
    {
        return _winkingLoop;
    }

    public void StopWinkinkLoop()
    {
        _winkingLoop = false;
    }

    public bool IsWinking()
    {
        return _isWinking;
    }
    
    // The radius of the circle
    public float radius = 1f;
    // The intensity of the light (alpha between 0 and 1)
    public float intensity = 0.5f;
    // The number of materials applies (YOU HAVE TO APPLY THEM YOURSELF ON THE MESH RENDERER)
    public int strength = 1;

    private bool _isWinking;
    private bool _winkingLoop;
}
