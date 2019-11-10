using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Noise
{
    /// <summary>
    /// Creates a new Noise.
    /// </summary>
    /// <param name="source">Source of the noise. Taken as the origin of the noise.</param>
    /// <param name="intensity">Intensity of the sound. Diameter in which the sound will be diffused.</param>
    /// <param name="floor">Floor of the sound emited</param>
    /// <param name="emiter">Emiter type</param>
    /// <param name="duration">Duration of the sound. The noise will be destroyed when the duration hits 0. Precision of .1</param>
    public Noise(SoundEmiter source, float intensity, int floor, NoiseType emiter, float duration)
    {
        _source = source;
        _intensity = intensity;
        _floor = floor;
        _emiter = emiter;
        _duration = duration;
        if(duration != _oneFrame)
            source.StartCoroutine(UpdateDuration());
    }

    /// <summary>
    /// Updates the resting duration of the noise 
    /// </summary>
    private IEnumerator UpdateDuration()
    {
        while(_duration > 0)
        {
            yield return new WaitForSeconds(0.1f);
            _duration -= 0.1f;
        }
    }

    public static float OneFrame()
    {
        return _oneFrame;
    }


    private readonly SoundEmiter _source;
    private readonly float _intensity;
    private readonly int _floor;
    private readonly NoiseType _emiter;
    private float _duration;
    private static readonly float _oneFrame = 0.05f;

    public NoiseType emiterType => _emiter;
    public int floor => _floor;
    public float duration => _duration;
    public Vector2 origin => _source.transform.position;
    public float radius => _intensity / 2;
    public float intensity => _intensity;
}
