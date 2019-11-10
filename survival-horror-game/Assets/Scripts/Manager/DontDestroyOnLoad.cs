using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Controls
{
    Left,
    Right,
    Up,
    Down,
    Lamp,
    Run,
    Interact,
    Reload,
    HugTeddy,
}
public class DontDestroyOnLoad : MonoBehaviour
{
    void Awake()
    {
        _soundPower = 1;
        _musicPower = 1;
        _allKeys = new Dictionary<Controls, (KeyCode, KeyCode)>();
        _tutorialDone = false;

        AssignKey(Controls.Left, KeyCode.Q, KeyCode.LeftArrow);
        AssignKey(Controls.Right, KeyCode.D, KeyCode.RightArrow);
        AssignKey(Controls.Up, KeyCode.Z, KeyCode.UpArrow);
        AssignKey(Controls.Down, KeyCode.S, KeyCode.DownArrow);
        AssignKey(Controls.Lamp, KeyCode.A, KeyCode.Mouse0);
        AssignKey(Controls.Run, KeyCode.LeftControl, KeyCode.LeftShift);
        AssignKey(Controls.Interact, KeyCode.E, KeyCode.Mouse1);
        AssignKey(Controls.Reload, KeyCode.R, KeyCode.None);
        AssignKey(Controls.HugTeddy, KeyCode.F, KeyCode.None);

        DontDestroyOnLoad(gameObject);
    }

    private void AssignKey(Controls key, KeyCode keyOne, KeyCode keyTwo)
    {
        _allKeys[key] = (keyOne, keyTwo);
    }

    public Dictionary<Controls, (KeyCode, KeyCode)> AllKeys()
    {
        return _allKeys;
    }

    public (KeyCode, KeyCode) GetKey(Controls control)
    {
        return _allKeys[control];
    }

    public void SetKey(Controls control, KeyCode keyOne, KeyCode keyTwo)
    {
        AssignKey(control, keyOne, keyTwo);
    }

    public float GetEffectsVolume()
    {
        return _soundPower;
    }
    public float GetMusicVolume()
    {
        return _musicPower;
    }   
    
    public void SetEffectsVolume(float volume)
    {
        _soundPower = volume;
        SoundManager sm = GetComponent<SoundManager>();
        if (!sm) Debug.LogError("Error: SoundManager not found.");
        else sm.UpdateSoundVolume(volume);
    }
    public void SetMusicVolume(float volume)
    {
        _musicPower = volume;
        MusicManager mm = GetComponent<MusicManager>();
        if(!mm) { Debug.LogError("Error: MusicManager not found"); }
        else
        {
            mm.ChangeVolume(volume);
        }
    }

    public bool TutorialDone { get { return _tutorialDone; } set { _tutorialDone = value; } }

    private Dictionary<Controls, (KeyCode, KeyCode)> _allKeys;
    private float _soundPower;
    private float _musicPower;
    private bool _tutorialDone;
}
