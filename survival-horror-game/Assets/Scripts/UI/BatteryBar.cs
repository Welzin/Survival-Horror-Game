using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatteryBar : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        _criticalBattery = false;
        _alphaMin = GetComponent<Image>().color.a;
        _widthForFull = GetComponent<RectTransform>().sizeDelta.x;

        _alphaChangeBySec = (1 - _alphaMin) / timeForBlink;
    }

    // Update is called once per frame
    void Update()
    {
        if (_criticalBattery)
        {
            Blinking();
        }
    }

    public void ChangeBatteryPercentage(float percentage)
    {
        RectTransform rt = GetComponent<RectTransform>();
        float height = rt.sizeDelta.y;
        rt.sizeDelta = new Vector2(_widthForFull * (percentage / 100f), height);

        if (!_criticalBattery && percentage <= percentageForCriticalBattery)
        {
            _criticalBattery = true;
            _alphaIsGrowing = true;
        }
        else if (_criticalBattery && percentage > percentageForCriticalBattery)
        {
            _criticalBattery = false;
            ChangeAlpha(_alphaMin);
        }
    }

    private void Blinking()
    {
        Image image = GetComponent<Image>();
        if (_alphaIsGrowing)
        {
            ChangeAlpha(image.color.a + _alphaChangeBySec * Time.deltaTime);
            _alphaIsGrowing = !(image.color.a >= 1);
        }
        else
        {
            ChangeAlpha(image.color.a - _alphaChangeBySec * Time.deltaTime);
            _alphaIsGrowing = image.color.a <= _alphaMin;
        }
    }

    private void ChangeAlpha(float alpha)
    {
        Image image = GetComponent<Image>();
        image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
    }

    public float percentageForCriticalBattery = 30f;
    public float timeForBlink = 1f;

    private bool _criticalBattery;
    private bool _alphaIsGrowing;

    // Those 3 values are calculating on the Start method and never change
    private float _alphaMin;
    private float _widthForFull;
    private float _alphaChangeBySec;
}
