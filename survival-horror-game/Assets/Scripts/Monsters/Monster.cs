using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public enum Behaviour
{
    Follow,
    Flee,
}

public class Monster : MonoBehaviour
{
    void Start()
    {
        _pattern = new Queue<Pattern>();
        _destination = transform.position;
        foreach (Pattern act in movementPattern)
        {
            _pattern.Enqueue(act);
        }
        _hasTarget = false;
        // Uncertainty depends on the speed of the mob
        _errorPercentage = speed / 50;
        SoundManager sm = FindObjectOfType<SoundManager>();
        if(sm == null)
        {
            Debug.LogWarning("Monster: " + gameObject.name + " can not hear any sound!");
        }
        else
        {
            sm.Subscribe(this);
        }
        // If nothing has alerted the monster, will check the destination at timeToCheck.
        Invoke("Check", cond.timeToCheck);
    }

    void Update()
    {
        float x = transform.position.x;
        float y = transform.position.y;
        if (Math.Abs(_destination.x - x) > _errorPercentage || Math.Abs(_destination.y - y) > _errorPercentage)
        {
            Move();
        }
        SearchForLight();
    }

    private void Check()
    {
        //_destination = cond.destination.transform.position;
        // Pathfinder:
        // MoveTo(destination) 
        PlayerController player = FindObjectOfType<PlayerController>();
        if(player != null)
        {
            if (!cond.rectangle.Contains(player.gameObject.transform.position))
            {
                Debug.Log(gameObject.name + " Not Found.");
                Invoke("ExecutePattern", 1);
            }
            else
            {
                Debug.Log(gameObject.name + " Found.");
                // Game Over
            }
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogError("Error: There is no player in the scene");
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }

    private void ExecutePattern()
    {
        if (_pattern.Count != 0 && !_hasTarget)
        {
            Pattern toDo = _pattern.Dequeue();
            float x = transform.position.x;
            float y = transform.position.y;
            _destination = toDo.goTo.transform.position;
            _pattern.Enqueue(toDo);
            Invoke("ExecutePattern", toDo.intervalUntilNextAction);
        }
        else
        {
            CancelInvoke("ExecutePattern");
        }
    }

    private void Move()
    {
        Vector2 pos = transform.position;
        Vector2 direction = (_destination - pos).normalized;
        pos += direction * speed * Time.deltaTime;
        transform.position = pos;
    }
    
    private void SearchForLight()
    {
        // Get the position of the lamp
        Lamp lamp = FindObjectOfType<Lamp>();
        if (lamp == null)
        {
#if UNITY_EDITOR
            Debug.LogError("Error: There are no lamps in the level");
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
        if (lamp.Active)
        {
            Vector2 lampPos = lamp.transform.position;
            Vector2 pos = transform.position;
            // Distance between lamp and monster
            float dist = Vector2.Distance(pos, lampPos);
            if (dist <= lightDetectionRange)
            {
                bool obstrusion = Physics2D.Linecast(pos, lampPos, LayerMask.GetMask("Obstacle"));
                if (!obstrusion)
                {
                    SetTarget(lamp.transform.position);
                }
                else
                {
                    if (_hasTarget)
                    {
                        ResetTarget();
                    }
                }
            }
        }
        else
        {
            if (_hasTarget)
            {
                ResetTarget();
            }
        }
    }

    private void ResetTarget()
    {
        _hasTarget = false;
        // If the monster doesnt see anything anymore in 10 seconds, it'll go back to its pattern
        Invoke("ExecutePattern", 10);
    }

    private void SetTarget(Vector2 targetPos)
    {
        if(IsInvoking("Check"))
        {
            CancelInvoke("Check");
        }
        if (targetBehaviour == Behaviour.Follow)
        {
            _destination = targetPos;
        }
        else
        {
            Vector2 pos = transform.position;
            float xDiff = Math.Abs(pos.x - targetPos.x);
            float yDiff = Math.Abs(pos.y - targetPos.y);
            if(targetPos.x > pos.x)
            {
                if(targetPos.y < pos.y)
                {
                    _destination = new Vector2(pos.x - xDiff, pos.y + yDiff);
                }
                else
                {
                    _destination = new Vector2(pos.x - xDiff, pos.y - yDiff);
                }
            }
            else
            {
                if (targetPos.y < pos.y)
                {
                    _destination = new Vector2(pos.x + xDiff, pos.y + yDiff);
                }
                else
                {
                    _destination = new Vector2(pos.x + xDiff, pos.y - yDiff);
                }
            }
        }
        _hasTarget = true;
        CancelInvoke("ExecutePattern");
    }

    public void DetectSound()
    {
        // Direction -> y axis
        LayerMask layer = LayerMask.GetMask("Sound");
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, soundDetectionRange, new Vector2(0, 1), Mathf.Infinity, layer);
        if (hit)
        {
            Vector2 soundPos = hit.transform.position;
            if(!_hasTarget)
            {
                SetTarget(soundPos);
            }
            else
            {
                if(Vector2.Distance(soundPos, transform.position) < Vector2.Distance(_destination, transform.position))
                {
                    SetTarget(soundPos);
                }
            }
        }
    }

    // Draws a circle and checks if there are lights in this circle. If there are, the monster will have its target (limited by sight)
    public float lightDetectionRange = 1f;
    // Draws a circle and checks if the monster hears a sound (not limited by sight)
    public float soundDetectionRange = 1f;
    // Patterns of the monster when there are no target
    public List<Pattern> movementPattern;
    public float speed = 1f;
    public AudioSource yell;
    // Behaviour when seeing a target
    public Behaviour targetBehaviour = Behaviour.Follow;
    public Condition cond;
    [Range(0, 1)]
    public int currentFloor = 0;

    private Queue<Pattern> _pattern;
    // Has a target been found ?
    private bool _hasTarget;
    // The Monster has to move to this point
    private Vector2 _destination;
    // Accepted error percentage on movement
    private float _errorPercentage;
}
