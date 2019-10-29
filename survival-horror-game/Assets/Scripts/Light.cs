using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Light2D;

public class Light : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
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
    
    // The radius of the circle
    public float radius = 1f;
    // The intensity of the light (alpha between 0 and 1)
    public float intensity = 0.5f;
    // The number of materials applies (YOU HAVE TO APPLY THEM YOURSELF ON THE MESH RENDERER)
    public int strength = 1;
}
