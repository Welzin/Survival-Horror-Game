using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialog : MonoBehaviour
{
    private void Start()
    {
        _texts = new List<string>();
    }

    public bool FinishSpeaking()
    {
        return _texts.Count == 0;
    }

    public void AddDialog(string dialog)
    {
        _texts.Add(dialog);
        if (_texts.Count == 1)
        {
            textZone.text = _texts[0];
        }
    }

    public void NextDialog()
    {
        if (_texts.Count > 0)
        {
            _texts.RemoveAt(0);

            if (_texts.Count > 0)
            {
                textZone.text = _texts[0];
            }
            else
            {
                textZone.text = "";
            }
        }
    }

    private List<string> _texts;

    public Text textZone;
}
