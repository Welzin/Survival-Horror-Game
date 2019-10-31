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
        // Adds the MonoBehaviour which manages the movements of a monster
        _move = gameObject.AddComponent<MovementHelper>();
        // Instanciate pattern's queue with the given pattern
        _pattern = new Queue<Pattern>(movementPattern);
        // It has no target at the start of the game
        _hasTarget = false;
        // SoundManager is the object which propagates sound within the game
        SoundManager sm = FindObjectOfType<SoundManager>();
        if(sm == null)
        {
            Debug.LogWarning("Monster: " + gameObject.name + " can not hear any sound!");
        }
        else
        {
            // If the SoundManager is found, subscribe the monster to all the sounds emited in the game
            sm.Subscribe(this);
        }
        // If there is a condition for playing patterns, play it
        if(cond.condition != Condition.Action.None)
        {
            // Play the condition before executing the patterns
            Invoke("RunCondition", cond.timeToCheck);
        }
        // Else, invoke patterns after 1 second
        else
        {
            Invoke("ExecutePattern", 1);
        }
    }

    void Update()
    {
        // Search if the player emits light every frame
        SearchForLight();
    }
    
    /// <summary>
    /// Starts the movement with the destination of the condition, and awaits the end of the movement to apply the condition
    /// </summary>
    private void RunCondition()
    {
        // Start movement of the gameObject
        _move.StartMovement(transform.position, cond.destination.transform.position);
        // Wait until the movement has ended
        StartCoroutine(_move.WaitNonBlocking(_move.isMovementFinished, CheckForPlayer));
    }

    /// <summary>
    /// Checks for player in the rectangle given by the condition
    /// </summary>
    private void CheckForPlayer()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
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

    /// <summary>
    /// It will execute the current pattern, and call this function again for the next pattern
    /// </summary>
    private void ExecutePattern()
    {
        if (_pattern.Count != 0 && !_hasTarget)
        {
            Pattern toDo = _pattern.Dequeue();
            _move.StartMovement(transform.position, toDo.goTo.transform.position);
            _pattern.Enqueue(toDo);
            Invoke("ExecutePattern", toDo.intervalUntilNextAction);
        }
        else
        {
            CancelInvoke("ExecutePattern");
        }
    }
    
    /// <summary>
    /// Checks if the gameObject is in range of the player's lamp turned on
    /// If it's in range, it will target the player.
    /// </summary>
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
        Vector2 destination;
        if(IsInvoking("RunCondition"))
        {
            CancelInvoke("RunCondition");
        }
        if (targetBehaviour == Behaviour.Follow)
        {
            destination = targetPos;
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
                    destination = new Vector2(pos.x - xDiff, pos.y + yDiff);
                }
                else
                {
                    destination = new Vector2(pos.x - xDiff, pos.y - yDiff);
                }
            }
            else
            {
                if (targetPos.y < pos.y)
                {
                    destination = new Vector2(pos.x + xDiff, pos.y + yDiff);
                }
                else
                {
                    destination = new Vector2(pos.x + xDiff, pos.y - yDiff);
                }
            }
        }
        _hasTarget = true;
        CancelInvoke("ExecutePattern");
        _move.StartMovement(transform.position, destination);
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
                if(Vector2.Distance(soundPos, transform.position) < Vector2.Distance(_move.Destination(), transform.position))
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

    private MovementHelper _move;
}
