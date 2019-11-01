using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BindHelper : MonoBehaviour
{
    public void RecordBind()
    {
        DisplayKey("Push a key");
        _waitForKey = true;
    }

    private void Update()
    {
        if(_waitForKey)
        {
            foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(vKey))
                {
                    DisplayKey(vKey.ToString());
                    UpdateDD(vKey);
                    _waitForKey = false;
                    break;
                }
            }
        }
    }

    private void DisplayKey(string key)
    {
        gameObject.transform.GetChild(0).GetComponent<Text>().text = key;
    }

    private void UpdateDD(KeyCode key)
    {
        var dd = FindObjectOfType<DontDestroyOnLoad>();
        Enum.TryParse(keyName.text, out Controls actualControl);
        var control = dd.GetKey(actualControl);
        if(buttonNumber == 1)
        {
            dd.SetKey(actualControl, key, control.Item2);
        }
        else
        {
            dd.SetKey(actualControl, control.Item1, key);
        }
    }

    public Text keyName;
    [Range(1, 2)]
    public int buttonNumber = 1;

    private bool _waitForKey;
}
