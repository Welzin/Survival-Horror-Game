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
        _dd = FindObjectOfType<DontDestroyOnLoad>(); 
        _player = FindObjectOfType<PlayerManager>();
        _isStopped = false;
        // If there is a condition for playing patterns, play it
        if (cond.condition != Condition.Action.None)
        {
            // Play the condition before executing the patterns
            // Invoke("RunCondition", cond.timeToCheck);
        }
        // Else, invoke patterns after 1 second
        else
        {
            // Do nothing atm
            Invoke("PlayPattern", 1);
        }
    }

    protected override void Update()
    {
        if(!_dd.GamePause && !_player.IsSpeaking() && !_isStopped)
        {
            base.Update();
            // Search if the player emits light every frame
            SearchForLight();
            SearchForSound();
            ChangeStateDependingOnPlayer();
        }
    }

    private void ChangeStateDependingOnPlayer()
    {
        PlayerManager player = FindObjectOfType<PlayerManager>();
        if(player != null)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            GameObject lightObs = transform.GetChild(0).gameObject;
            if (player.CurrentFloor == currentFloor)
            {
                if(!sr.enabled) sr.enabled = true;
                if (!collider.enabled) collider.enabled = true;
                if (!lightObs.activeSelf) lightObs.SetActive(true);
            }
            else
            {
                if (sr.enabled) sr.enabled = false;
                if (collider.enabled) collider.enabled = false;
                if (lightObs.activeSelf) lightObs.SetActive(false);
            }
        }
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
                Invoke("ExecutePattern", 1);
            }
            else
            {
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
            _currentWait = StartCoroutine(_move.WaitNonBlocking(_move.IsMovementFinished, () => Invoke("ExecutePattern", toDo.intervalUntilNextAction)));
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

        // If the lamp is active and monster can see the light, its new target is the lamp position
        if (lamp.Active && lamp.CanBeSeenByMonster(this) && NumberOfObstaclesBetween(transform.position, lamp.transform.position) == 0)
        {
            SetTarget(lamp.transform.position, currentFloor);
        }

        if (_move.IsNear(transform.position, _move.Target()))
        {
            ResetTarget();
        }
    }

    public void ResetTarget()
    {
        if(_hasTarget)
        {
            _hasTarget = false;
            _move.TargetLost();
            // If the monster doesnt see anything anymore in 10 seconds, it'll go back to its pattern
            Invoke("ExecutePattern", 10);
        }
    }

    private void SetTarget(Vector2 targetPos, int floor)
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
        _move.RunTowardTarget(destination, floor);
    }

    protected 

    private void SearchForSound()
    {
        Vector2 newTarget = new Vector2(float.MaxValue, float.MaxValue);
        int floor = currentFloor;
        foreach (Noise noise in AllNoiseHeard())
        {
            Vector2 dest = GetOrigin(noise);
            float distance = Vector2.Distance(transform.position, dest);

            if (distance < Vector2.Distance(transform.position, _move.Target()) || noise.emiterType == NoiseType.Player)
            {
                newTarget = dest;
                floor = noise.floor;
                if (noise.emiterType == NoiseType.Player) break;
            }
        }
        if(newTarget.x != float.MaxValue && newTarget.y != float.MaxValue)
        {
            SetTarget(newTarget, floor);
        }
        else
        {
            if (_hasTarget && _move.IsNear(transform.position, _move.Target()))
                ResetTarget();
        }
    }

    /// <summary>
    /// Play pattern where it stopped
    /// </summary>
    public void PlayPattern()
    {
        if(_hasTarget)
        {
            _move.TargetLost();
            _hasTarget = false;
        }
        ExecutePattern();
    }

    /// <summary>
    /// Stops all actions (except detecting actions)
    /// </summary>
    public void StopActions()
    {
        CancelInvoke("ExecutePattern");
        if(_currentWait != null)
            StopCoroutine(_currentWait);
        _move.Stop();
    }

    /// <summary>
    /// Moves to the specified position. Stops all other actions
    /// </summary>
    public void MoveTo(Vector2 position, int floor)
    {
        StopActions();
        _move.RunTowardTarget(position, floor);
    }
    
    public MovementHelper MovementHelper()
    {
        return _move;
    }

    public bool InPause() => _dd.GamePause;
    public bool IsSpeaking() => _player.IsSpeaking();
    public Queue<Pattern> ActualPattern { get { return _pattern; } set { _pattern = value; } }

    public void StopEverything()
    {
        ResetTarget();
        StopActions();
        _isStopped = true;
    }

    /// <summary>
    /// Call this function if the StopEverything() function has been called before
    /// </summary>
    public void Release()
    {
        _isStopped = false;
    }

    // Draws a circle and checks if there are lights in this circle. If there are, the monster will have its target (limited by sight)
    public float lightDetectionRange = 1f;
    // Patterns of the monster when there are no target
    public List<Pattern> movementPattern;
    public float speed = 1f;
    // Behaviour when seeing a target
    public Behaviour targetBehaviour = Behaviour.Follow;
    public Condition cond;

    private Queue<Pattern> _pattern;
    // Has a target been found ?
    private bool _hasTarget;

    private MovementHelper _move;
    private Coroutine _currentWait;
    private DontDestroyOnLoad _dd;
    private PlayerManager _player;
    private bool _isStopped;
}
