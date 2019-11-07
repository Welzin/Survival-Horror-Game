using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StressBar : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        _criticalStress = false;
        _alphaMin = GetComponent<Image>().color.a;
        _widthForFull = GetComponent<RectTransform>().sizeDelta.x;

        _alphaChangeBySec = (1 - _alphaMin) / timeForBlink;
    }

    // Update is called once per frame
    void Update()
    {
        if (_criticalStress)
        {
            Blinking();
        }
    }

    public void ChangeStressPercentage(float percentage)
    {

        RectTransform rt = GetComponent<RectTransform>();
        float height = rt.sizeDelta.y;
        rt.sizeDelta = new Vector2(_widthForFull * (percentage / 100f), height);

        if (!_criticalStress && percentage >= percentageForCriticalStress)
        {
            _criticalStress = true;
            _alphaIsGrowing = true;
        }
        else if (_criticalStress && percentage < percentageForCriticalStress)
        {
            _criticalStress = false;
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

    public float percentageForCriticalStress = 70f;
    public float timeForBlink = 1f;

    private bool _criticalStress;
    private bool _alphaIsGrowing;

    // Those 3 values are calculating on the Start method and never change
    private float _alphaMin;
    private float _widthForFull;
    private float _alphaChangeBySec;
}
