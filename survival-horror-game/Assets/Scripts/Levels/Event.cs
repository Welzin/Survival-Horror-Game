using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Event : MonoBehaviour
{
    protected void Start()
    {
        player = FindObjectOfType<PlayerManager>();
    }

    public abstract void PlayEvent();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (type == EventType.OnCollision && collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayEvent();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (type == EventType.OnCollision && collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayEvent();
        }
    }
    
    public EventType type;

    public enum EventType
    {
        OnCollision,
        OnClick
    };

    protected PlayerManager player;
}
