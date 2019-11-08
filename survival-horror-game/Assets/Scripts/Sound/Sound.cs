using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Every type of sound should be put here
/// </summary>
public enum SoundType
{
    Footsteps,
    RunningSteps,
    Custom,
}

[System.Serializable]
public class Sound
{
    public SoundType type;
    public AudioClip audioClip;
}