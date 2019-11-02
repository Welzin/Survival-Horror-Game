using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Helper : MonoBehaviour
{
    void Start()
    {
        _infoDisplay = false;
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

    private void Update()
    {
        if (_infoDisplay)
        {
            _actualDisplayingTime += Time.deltaTime;

            if (_actualDisplayingTime >= timeTextIsDisplay)
            {
                _infoDisplay = false;
                _info = "";
                DisplayOtherHelp();
            }
        }
    }

    public enum Type
    {
        CatchItem,
    };

    public void DisplayInfo(Text zone, string info)
    {
        _info = info;
        zone.text = info;
        _infoDisplay = true;
        _actualDisplayingTime = 0;
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
    // When an info is displaying, 
    public float timeTextIsDisplay;

    // Is There an info display
    private bool _infoDisplay;
    // time from which the text is displaying
    private float _actualDisplayingTime;
    // The info to display
    private string _info;

    // The actual Helper type displaying
    private HashSet<Type> _actualDisplayingTypes;
    // Player settings
    private DontDestroyOnLoad _dd;
}
