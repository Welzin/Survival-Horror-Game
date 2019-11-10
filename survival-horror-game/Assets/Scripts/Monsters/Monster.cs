using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
// Still useful ?
// This class needs cleaning
public enum Behaviour
{
    Follow,
    Flee,
}

public class Monster : Listener
{
    protected override void Start()
    {
        base.Start();
        // Adds the MonoBehaviour which manages the movements of a monster
        _move = gameObject.AddComponent<MovementHelper>();
        // Instanciate pattern's queue with the given pattern
        _pattern = new Queue<Pattern>(movementPattern);
        // It has no target at the start of the game
        _hasTarget = false;
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

    protected override void Update()
    {
        base.Update();
        // Search if the player emits light every frame
        SearchForLight();
        SearchForSound();
    }
    
    /// <summary>
    /// Starts the movement with the destination of the condition, and awaits the end of the movement to apply the condition
    /// </summary>
    private void RunCondition()
    {
        // Start movement of the gameObject
        _move.StartMovement(transform.position, cond.destination);
        // Wait until the movement has ended
        StartCoroutine(_move.WaitNonBlocking(_move.IsMovementFinished, CheckForPlayer));
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
            _move.StartMovement(transform.position, toDo.goTo);
            _pattern.Enqueue(toDo);
            // Wait until the movement is finished, and call ExecutePattern again after the given time.
            StartCoroutine(_move.WaitNonBlocking(_move.IsMovementFinished, () => Invoke("ExecutePattern", toDo.intervalUntilNextAction)));
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
                    ResetTarget();
                }
            }
            else
            {
                ResetTarget();
            }
        }
    }

    private void ResetTarget()
    {
        if(_hasTarget)
        {
            _hasTarget = false;
            _move.TargetLost();
            // If the monster doesnt see anything anymore in 10 seconds, it'll go back to its pattern
            Invoke("ExecutePattern", 10);
            Debug.Log("invoking..");
        }
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
        _move.RunTowardTarget(destination);
    }

    private void SearchForSound()
    {
        Vector2 newTarget = new Vector2(float.MaxValue, float.MaxValue);
        foreach (Noise noise in AllNoiseHeard())
        {
            Vector2 dest = GetOrigin(noise);
            // If the sound is on another floor, the monster will have to do more distance to go to the origin of the sound
            if (noise.floor != currentFloor)
                dest += new Vector2(15, 15);
            if(Vector2.Distance(transform.position, dest) < Vector2.Distance(transform.position, _move.Target()))
                newTarget = dest;
        }
        if(newTarget.x != float.MaxValue && newTarget.y != float.MaxValue)
        {
            SetTarget(newTarget);
        }
        else
        {
            if (_move.IsNear(transform.position, _move.Target()))
                ResetTarget();
        }
    }

    // Draws a circle and checks if there are lights in this circle. If there are, the monster will have its target (limited by sight)
    public float lightDetectionRange = 1f;
    // Patterns of the monster when there are no target
    public List<Pattern> movementPattern;
    public float speed = 1f;
    // Behaviour when seeing a target
    public Behaviour targetBehaviour = Behaviour.Follow;
    public Condition cond;
    [Range(1, 2)]
    public int currentFloor = 0;

    private Queue<Pattern> _pattern;
    // Has a target been found ?
    private bool _hasTarget;

    private MovementHelper _move;
}
