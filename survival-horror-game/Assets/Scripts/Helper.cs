using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Helper : MonoBehaviour
{
    void Start()
    {
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

    public void DisplayHelp(Type type)
    {
        Debug.Log("Display");
        switch (type)
        {
            case Type.CatchItem:
                textZone.text = "Press " + _dd.Interact().Item1 + " to get this item";
                break;
        }

        _actualDisplayingTypes.Add(type);
    }

    public void StopDisplayingHelp(Type type)
    {
        Debug.Log("Stop");
        _actualDisplayingTypes.Remove(type);

        if (_actualDisplayingTypes.Count == 0)
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
    // The actual Helper type displaying
    private HashSet<Type> _actualDisplayingTypes;
    // Player settings
    private DontDestroyOnLoad _dd;
}
