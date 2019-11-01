using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderHelper : MonoBehaviour
{
    private void Awake()
    {
        _dd = FindObjectOfType<DontDestroyOnLoad>();
    }

    public void UpdateSoundEffects()
    {
        _dd.SetEffectsVolume(gameObject.GetComponent<Slider>().value);
        UpdateText();
    }
    public void UpdateMusic()
    {
        _dd.SetMusicVolume(gameObject.GetComponent<Slider>().value);
        UpdateText();
    }

    private void UpdateText()
    {
        transform.GetChild(3).GetComponent<Text>().text = String.Format("{0:0.00}", gameObject.GetComponent<Slider>().value);
    }

    private DontDestroyOnLoad _dd;
}
