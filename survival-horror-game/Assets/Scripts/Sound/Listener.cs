using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// All the noises that can be listened. 
/// </summary>
public enum NoiseType
{
    Player,
    Monster,
    Lightning,
    Search,
    Ouaf,
    Event,
}

/// <summary>
/// Listens to any sound. It registers itself in SoundManager. Please call base.Start() and base.Update() in the corresponding functions.
/// </summary>
public class Listener : MonoBehaviour
{
    protected virtual void Start()
    {
        // Auto-subscribe to the sound manager
        SoundManager sm = FindObjectOfType<SoundManager>();
        if (!sm)
            Debug.LogError("Listener: " + gameObject.name + " can not hear any sound!");
        // If the SoundManager is found, subscribe the monster to all the sounds emited in the game
        else
            sm.Subscribe(this);
        _allNoisesHeard = new Dictionary<Noise, float>();
    }

    protected virtual void Update()
    {
        List<Noise> buffer = new List<Noise>();
        // Checks if there is any sound which is finished in the list. 
        if(_allNoisesHeard.Count > 0)
        {
            foreach (Noise noise in _allNoisesHeard.Keys)
            {
                if (noise.duration <= 0)
                {
                    buffer.Add(noise);
                }
            }
            foreach (Noise noise in buffer)
            {
                _allNoisesHeard.Remove(noise);
            }
        }
    }
    
    /// <summary>
    /// Detect if there is a door or a wall between two points.
    /// A sound isn't heard if there is an obstacle
    /// </summary>
    /// <param name="position"></param>
    /// <param name="goal"></param>
    /// <returns></returns>
    protected int NumberOfObstaclesBetween(Vector2 position, Vector2 goal)
    {
        int number = 0;

        // First, check for walls
        List<RaycastHit2D> hits = new List<RaycastHit2D>();
        ContactFilter2D filter = new ContactFilter2D
        {
            layerMask = LayerMask.GetMask("Wall")
        };

        Physics2D.Linecast(position, goal, filter, hits);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                number++;
            }
        }

        // Then, check for doors
        hits = new List<RaycastHit2D>();
        filter = new ContactFilter2D
        {
            layerMask = LayerMask.GetMask("Event")
        };

        Physics2D.Linecast(position, goal, filter, hits);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject.GetComponent<Door>())
            {
                if (hit.collider.gameObject.GetComponent<Door>().IsClosed())
                {
                    Debug.Log(hit.collider.gameObject);
                    number++;
                }
            }
        }

        return number;
    }

    /// <summary>
    /// Checks if the object listens to the given type
    /// </summary>
    public bool ListensTo(NoiseType type)
    {
        return _allNoiseListened.Contains(type);
    }

    /// <summary>
    /// This function needs to be implemented in the child. It is called when a sound is emited.
    /// </summary>
    public void DetectSound(Noise noise)
    {
        float dist = Vector2.Distance(transform.position, noise.origin);

        // If the sound is on another floor, the monster will have to do more distance to go to the origin of the sound
        if (noise.floor != currentFloor)
        {
            dist += 20;
        }
        else
        {
            dist += NumberOfObstaclesBetween(transform.position, noise.origin) * 10;

            if (noise.emiterType == NoiseType.Player)
                Debug.Log(NumberOfObstaclesBetween(transform.position, noise.origin));
        }

        // Distance between object and the source of the noise has to be lesser than the radius + the range of hearing :
        // it means that the radius of the noise and the range of hearing are intersecting.
        if(dist <= noise.radius + _hearRange)
        {
            _allNoisesHeard.Add(noise, CalculateIntensity(dist, noise.radius));
        }
    }

    /// <summary>
    /// Get the intensity felt of a noise 
    /// </summary>
    public float IntensityFelt(Noise noise)
    {
        if (_allNoisesHeard.ContainsKey(noise))
            return _allNoisesHeard[noise];
        else
            return 0;
    }

    /// <summary>
    /// Total intensity of all the noises heard
    /// </summary>
    public float TotalIntensity()
    {
        float total = 0;
        foreach(float intensity in _allNoisesHeard.Values)
        {
            total += intensity;
        }
        return total;
    }

    /// <summary>
    /// Get all the noises that the listener is hearing
    /// </summary>
    public List<Noise> AllNoiseHeard()
    {
        List<Noise> allNoise = new List<Noise>();
        allNoise.AddRange(_allNoisesHeard.Keys);
        return allNoise;
    }

    public Vector2 GetOrigin(Noise noise)
    {
        return noise.origin;
    }

    /// <summary>
    /// Calculates the intensity of the noise heard with the radius. We assume that the object is in range to hear the noise.
    /// </summary>
    private float CalculateIntensity(float dist, float radius)
    {
        if (_hearRange >= dist)
            return 2 * radius;
        // distance - (radius + hearRange) => intersection of the 2 circles
        return Mathf.Abs(dist - radius - _hearRange);
    }

    public void DeleteNoise(Noise noise)
    {
        _allNoisesHeard.Remove(noise);
    }


    // Types of noises listened
    [SerializeField]
    private List<NoiseType> _allNoiseListened = new List<NoiseType>();
    // Range of the sounds heard
    [SerializeField]
    private float _hearRange = 1f;
    // All noises that the listener is currently hearing with their intensity felt.
    private Dictionary<Noise, float> _allNoisesHeard;

    [Range(1, 2)]
    public int currentFloor = 0;
}
