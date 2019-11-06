using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Helper : MonoBehaviour
{
    void Start()
    {
        _info = "";
        _dd = FindObjectOfType<DontDestroyOnLoad>();
        if (_dd == null)
        {
#if UNITY_EDITOR
            Debug.LogError("Error: DontDestroyOnLoad hasn't been found in the scene.");
            UnityEditor.EditorApplication.isPlaying = false;
#else
            // Display an error message and exit the app
#endif
        }

        _actualDisplayingTypes = new HashSet<Type>();
    }

    public enum Type
    {
        CatchItem,
    };

    public void DisplayInfo(string info, float time = 0f)
    {
        _info = info;
        textZone.text = info;

        if (time != 0f)
        {
            StartCoroutine(StopDisplayingInfoAfter(info, time));
        }
    }

    public void DisplayHelp(Type type)
    {
        switch (type)
        {
            case Type.CatchItem:
                textZone.text = "Press " + _dd.GetKey(Controls.Interact).Item1 + " to get this item";
                break;
        }

        _actualDisplayingTypes.Add(type);
    }

    public void StopDisplayingHelp(Type type)
    {
        _actualDisplayingTypes.Remove(type);
        DisplayOtherHelp();
    }

    public void StopDisplayingInfo()
    {
        _info = "";
        DisplayOtherHelp();
    }

    private IEnumerator StopDisplayingInfoAfter(string info, float time)
    {
        yield return new WaitForSeconds(time);

        // Sinon c'est que l'info a changé depuis
        if (_info == info)
        {
            StopDisplayingInfo();
        }
    }

    private void DisplayOtherHelp()
    {
        if (_info != "")
        {
            textZone.text = _info;
        }
        else if (_actualDisplayingTypes.Count == 0)
        {
            textZone.text = "";
        }
        else
        {
            foreach (Type toDisplay in _actualDisplayingTypes)
            {
                DisplayHelp(toDisplay);
                break;
            }
        }
    }

    public Text textZone;
    public Text infoZone;

    // The info to display
    private string _info;

    // The actual Helper type displaying
    private HashSet<Type> _actualDisplayingTypes;
    // Player settings
    private DontDestroyOnLoad _dd;
}
