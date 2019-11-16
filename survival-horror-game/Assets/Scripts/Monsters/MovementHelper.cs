using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class will help the associated GameObject to do its movements. 
/// The movements will be calculated to be within the pathfinder's node network.
/// </summary>
public class MovementHelper : MonoBehaviour
{
    void Start()
    {
        _allNodes = FindObjectsOfType<Node>();
        _currentPath = new Queue<Node>();
        _target = new Vector2(float.MaxValue, float.MaxValue);
        // Might be modified to be a class with informations such as speed, name, ...
        _mainScript = GetComponent<Monster>();
        if(_mainScript == null)
        {
            Debug.LogError("Monster component not found on " + gameObject.name + ". Speed set to 1.");
            _speed = 1; ;
        }
        else
        {
            _speed = _mainScript.speed;
        }
        _currentDestination = null;
        // Uncertainty depends on the speed
        _errorPercentage = _speed / 50;
        _floorManagers = new List<FloorManager>();
        _floorManagers.AddRange(FindObjectsOfType<FloorManager>());
        _floorManagers.Sort((el1, el2) => (el1.floorNumber.CompareTo(el2.floorNumber)));
        _lastDoor = null;
        _isOpeningDoor = false;
        _animator = GetComponent<Animator>();
        if(!gameObject.GetComponent<SoundEmiter>())
        {
            _sound = gameObject.AddComponent<SoundEmiter>();
            _sound.SetNoiseEmited(NoiseType.Monster);
        }
    }

    /// <summary>
    /// Manages movement. If there is something enqueued, it will move the associated gameObject to the current node.
    /// </summary>
    void FixedUpdate()
    {
        if(!_mainScript.InPause())
        {
            // If the gameObject is near the destination's node and there are still enqueued nodes, we can change the destination.
            if (_currentDestination != null && !_isMovementFinished)
            {
                if(IsNear(transform.position, _currentDestination.Position()))
                {
                    if (_currentPath.Count > 0)
                    {
                        // If it opened a door on the last node, close it
                        if (_lastDoor != null)
                        {
                            StartCoroutine(ChangeDoorState(_lastDoor, !_lastDoor.IsClosed()));
                            _lastDoor = null;
                        }
                        // Check if a door exists between the current and next node
                        Door door = _currentDestination.DoorBetweenNodes(_currentPath.Peek());
                        // If there is a door, checks if it can be opened without key
                        if (door != null)
                        {
                            if (door.needAKey)
                            {
                                _currentPath.Clear();
                                return;
                            }
                            // If it doesn't need a key, open the door
                            StartCoroutine(ChangeDoorState(door, door.IsClosed()));
                            _lastDoor = door;
                        }
                        _currentDestination = _currentPath.Dequeue();
                        CheckFloor();
                    }
                    // If there isn't any enqueued node, we inform that the movement is finished
                    else
                    {
                        _animator.SetInteger("Mouvement", 0);
                        _isMovementFinished = true;
                    }
                }
                else
                {
                    if(!_isOpeningDoor)
                    {
                        _animator.SetInteger("Mouvement", 1);
                        // If the gameObject isn't near, we move it
                        Move();
                    }
                }
            }
            if(_currentDestination == null && (_target.x != float.MaxValue || _target.y != float.MaxValue))
            {
                if (!IsNear(transform.position, _target) && !WallBetween(transform.position, _target))
                {
                    _animator.SetInteger("Mouvement", 1);
                    Move(_target);
                }
                else
                    _mainScript.ResetTarget();
            }

        }
        else
            _animator.SetInteger("Mouvement", 0);
    }

    private void CheckFloor()
    {
        int currentFloor = _mainScript.currentFloor;
        if(currentFloor == 2 && _currentDestination == _floorManagers[0].linkNode)
        {
            _mainScript.currentFloor = 1;
        }
        else if (currentFloor == 1 && _currentDestination == _floorManagers[1].linkNode)
        {
            _mainScript.currentFloor = 2;
        }
    }

    /// <summary>
    /// Calculates and enqueue the path to take to go from the start to the goal.
    /// </summary>
    /// <param name="start">Current position of the object to move (supposedly the gameObject)</param>
    /// <param name="goal">Position the object has to reach</param>
    public void StartMovement(Vector2 start, Vector2 goal, int goalFloor)
    {
        StartMovement(NearestNode(start, _mainScript.currentFloor), NearestNode(goal, goalFloor));
    }
    public void StartMovement(Node start, Vector2 goal, int goalFloor)
    {
        StartMovement(start, NearestNode(goal, goalFloor));
    }    
    public void StartMovement(Vector2 start, Node goal)
    {
        StartMovement(NearestNode(start, _mainScript.currentFloor), goal);
    }

    /// <summary>
    /// Calculates and enqueue the path to take to go from the start node to the goal node.
    /// </summary>
    /// <param name="start">Current node</param>
    /// <param name="goal">Node the object has to reach</param>
    public void StartMovement(Node start, Node goal)
    {
        _currentPath = new Queue<Node>(Pathfinder.Path(start, goal, _allNodes));
        if (_currentPath.Count == 0)
        {
#if UNITY_EDITOR
            Debug.LogError("Error: Path is empty!");
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            // Else, ...
            // Currently nothing, might do smthg
        }
        else
        {
            Node lastDest = _currentDestination;
            // Stores the first destination
            while (_currentDestination == lastDest && _currentPath.Count > 0)
            {
                _currentDestination = _currentPath.Dequeue();
            } 
            // Begins the movement
            _isMovementFinished = false;
        }
    }

    /// <summary>
    /// Goes towards the nearest node of the target, then pursue it.
    /// </summary>
    /// <param name="target"></param>
    public void RunTowardTarget(Vector2 target, int targetFloor)
    {
        Node node = NearestNode(target, targetFloor);
        float dist = Vector2.Distance(transform.position, target);
        if (targetFloor != _mainScript.currentFloor)
            dist += 20;
        // If the mob is closer than a node to the target, it can run on the target.
        // But if the mob is really close from the target, it will still run on the target and not stop.
        if (Vector2.Distance(target, node.Position()) > dist || dist < 0.5f)
        {
            // I may have to add WaitNonBlocking here with the target to tempo if it's called again before the previous movement
            // has ended and the node is the same.
            if(!WallBetween(transform.position, target))
            {
                _currentDestination = null;
                _target = target;
            }
        }
        // Else, we redo the path and wait for the end of the movement
        else
        {
            if(_isMovementFinished)
            {
                StartMovement(NearestNode(transform.position, _mainScript.currentFloor), node);
                // Wait until movement is finished, then forcefully run towards target
                _target = target;
                StartCoroutine(WaitNonBlocking(IsMovementFinished, () => _currentDestination = null));
            }
        }
    }

    /// <summary>
    /// Call this function to cancel the forceful chase
    /// </summary>
    public void TargetLost()
    {
        _animator.SetInteger("Mouvement", 0);
        _isMovementFinished = true;
        _target = new Vector2(float.MaxValue, float.MaxValue);
    }

    /// <summary>
    /// Waits while the predicate is false. Runs the given function after
    /// </summary>
    /// <param name="predicate">Wait until the predicate is true</param>
    /// <param name="onComplete">Launches this function after the wait</param>
    public IEnumerator WaitNonBlocking(System.Func<bool> predicate, System.Action onComplete)
    {
        while (!predicate())
        {
            yield return new WaitForFixedUpdate();
        }
        onComplete();
    }

    /// <summary>
    /// Returns the nearest node in the pathfinding network from the given Vector2
    /// </summary>
    /// <param name="position">Position of the object</param>
    /// <returns>Node nearest to the given position</returns>
    private Node NearestNode(Vector2 position, int floor)
    {
        // Instantiate to _allNodes[0] because a Node is a MonoBehaviour and can not be instanciated
        // with new Node().
        (Node, float) minNode = (_allNodes[0], float.MaxValue);
        Node[] allNodes = _floorManagers[floor - 1].GetNodes();
        foreach (Node node in allNodes)
        {
            // Get the distance between the node and the current position
            float dist = Mathf.Sqrt(Mathf.Pow(node.X() - position.x, 2) + Mathf.Pow(node.Y() - position.y, 2));
            // If the distance is inferior from the min value, this node is nearer than the last recorded
            if (dist < minNode.Item2)
            {
                minNode = (node, dist);
            }
        }
        return minNode.Item1;
    }

    /// <summary>
    /// Checks if a position is near to the given node (depending on the errorPercentage)
    /// </summary>
    /// <param name="position">Position to check</param>
    /// <param name="goal">Goal to check</param>
    /// <returns>True if the position and node are near</returns>
    public bool IsNear(Vector2 position, Vector2 goal)
    {
        return Mathf.Abs(goal.x - position.x) <= _errorPercentage && Mathf.Abs(goal.y - position.y) <= _errorPercentage;
    }

    /// <summary>
    /// Default Move() function to move to the destination node
    /// </summary>
    private void Move()
    {
        Move(_currentDestination.Position());
    }

    /// <summary>
    /// Specialized move to move toward a goal
    /// </summary>
    /// <param name="goal">Target to move toward</param>
    private void Move(Vector2 goal)
    {
        Vector2 pos = transform.position;
        Vector2 direction = (goal - pos).normalized;
        SetAnimator(direction);
        pos += direction * _speed * Time.deltaTime;
        transform.position = pos;

        if(!_sound.IsPlaying())
        {
            _sound.EmitSoundWave(2, _mainScript.currentFloor, Noise.OneFrame());
            _sound.PlayEffect(SoundType.Footsteps);
        }
    }

    /// <summary>
    /// Call this function to know if the current movement has ended or not
    /// </summary>
    /// <returns>Current state of _isMovementFinished</returns>
    public bool IsMovementFinished()
    {
        return _isMovementFinished;
    }

    public Vector2 Target() => _target;

    /// <summary>
    /// Call this function to get the current destination
    /// </summary>
    /// <returns>Position of the current destination</returns>
    public Vector2 Destination()
    {
        if(_currentDestination != null)
        {
            return _currentDestination.Position();
        }
        else
        {
            return _target;
        }
    }

    private IEnumerator ChangeDoorState(Door door, bool predicate)
    {
        _animator.SetInteger("Mouvement", 0);
        _isOpeningDoor = true;
        if (predicate)
            door.OpenForMonster();

        yield return new WaitForSeconds(door.eventTime);
        _isOpeningDoor = false;
    }

    private void SetAnimator(Vector2 direction)
    {
        float x = direction.x;
        float y = direction.y;
        if (Mathf.Abs(x) >= Mathf.Abs(y))
        {
            if (x < 0)
            {
                _animator.SetInteger("Direction", 1);
            }
            else if (x > 0)
            {
                _animator.SetInteger("Direction", 3);
            }
        }
        else
        {
            if (y < 0)
            {
                _animator.SetInteger("Direction", 0);
            }
            else if (y > 0)
            {
                _animator.SetInteger("Direction", 2);
            }
        }
    }

    public void Stop()
    {
        _currentDestination = null;
        _currentPath.Clear();
        _isMovementFinished = true;
        _animator.SetInteger("Mouvement", 0);
    }

    private bool WallBetween(Vector2 position, Vector2 goal)
    {
        LayerMask mask = LayerMask.GetMask("Wall");
        RaycastHit2D hit = Physics2D.Linecast(position, goal, mask);

        return !(hit.collider == null);
    }

    // All nodes in the pathfinder network
    private Node[] _allNodes;
    // Current path is saved here
    private Queue<Node> _currentPath;
    // Stores the current node to reach
    private Node _currentDestination;
    // At which maximum radius can the gameObject stops its movement
    private float _errorPercentage;
    // Speed of attached gameObject
    private float _speed;
    // True when a movement has ended
    private bool _isMovementFinished;
    // Records if there currently is a target
    private Vector2 _target;
    // All floors manager sorted in floor number
    private List<FloorManager> _floorManagers;
    // Associated Monster script
    private Monster _mainScript;
    // Last door opened 
    private Door _lastDoor;
    // Wait for door animation
    private bool _isOpeningDoor;
    // Get all the animations
    private Animator _animator;

    private SoundEmiter _sound;
}
