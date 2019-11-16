using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderHelper : MonoBehaviour
{
    private void OnEnable()
    {
        _dd = FindObjectOfType<DontDestroyOnLoad>();
        musicsSlider.value = _dd.GetMusicVolume();
        effectsSlider.value = _dd.GetEffectsVolume();
    }

    public void UpdateSoundEffects()
    {
        _dd.SetEffectsVolume(effectsSlider.value);
        UpdateText(effectsSlider);
    }
    public void UpdateMusic()
    {
        _dd.SetMusicVolume(musicsSlider.value);
        UpdateText(musicsSlider);
    }

    private void UpdateText(Slider slider)
    {
        slider.GetComponentInChildren<Text>().text = String.Format("{0:0.00}", slider.value);
    }

    private DontDestroyOnLoad _dd;

    public Slider musicsSlider;
    public Slider effectsSlider;
}
