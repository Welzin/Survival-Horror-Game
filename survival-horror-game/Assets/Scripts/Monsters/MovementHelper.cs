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
        // Uncertainty depends on the speed
        _errorPercentage = _speed / 50;
        _floorManagers = new List<FloorManager>();
        _floorManagers.AddRange(FindObjectsOfType<FloorManager>());
        _floorManagers.Sort((el1, el2) => (el1.floorNumber.CompareTo(el2.floorNumber)));
    }

    /// <summary>
    /// Manages movement. If there is something enqueued, it will move the associated gameObject to the current node.
    /// </summary>
    void FixedUpdate()
    {
        // If the gameObject is near the destination's node and there are still enqueued nodes, we can change the destination.
        if (_currentDestination != null && !_isMovementFinished)
        {
            if(IsNear(transform.position, _currentDestination.Position()))
            {
                if (_currentPath.Count > 0)
                {
                    _currentDestination = _currentPath.Dequeue();
                    CheckFloor();
                }
                // If there isn't any enqueued node, we inform that the movement is finished
                else
                {
                    _isMovementFinished = true;
                }
            }
            else
            {
                // If the gameObject isn't near, we move it
                Move();
            }
        }
        if(_currentDestination == null && (_target.x != float.MaxValue || _target.y != float.MaxValue))
        {
            if(!IsNear(transform.position, _target))
            {
                Move(_target);
            }
        }
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
            // Stores the first destination
            _currentDestination = _currentPath.Dequeue();
            // Begins the movement
            _isMovementFinished = false;
        }
    }

    /// <summary>
    /// Goes towards the nearest node of the target, then pursue it.
    /// </summary>
    /// <param name="target"></param>
    public void RunTowardTarget(Vector2 target)
    {
        Node node = NearestNode(target, _mainScript.currentFloor);
        float dist = Vector2.Distance(transform.position, target);
        // If the mob is closer than a node to the target, it can run on the target.
        // But if the mob is really close from the target, it will still run on the target and not stop.
        if (Vector2.Distance(target, node.Position()) > dist || dist < 0.5f)
        {
            // I may have to add WaitNonBlocking here with the target to tempo if it's called again before the previous movement
            // has ended and the node is the same.
            _currentDestination = null;
            _target = target;
        }
        // Else, we redo the path and wait for the end of the movement
        else
        {
            StartMovement(NearestNode(transform.position, _mainScript.currentFloor), node);
            // Wait until movement is finished, then forcefully run towards target
            _target = target;
            StartCoroutine(WaitNonBlocking(isMovementFinished, ()=>_currentDestination = null));
        }
    }

    /// <summary>
    /// Call this function to cancel the forceful chase
    /// </summary>
    public void TargetLost()
    {
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
    private bool IsNear(Vector2 position, Vector2 goal)
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
        pos += direction * _speed * Time.deltaTime;
        transform.position = pos;
    }

    /// <summary>
    /// Call this function to know if the current movement has ended or not
    /// </summary>
    /// <returns>Current state of _isMovementFinished</returns>
    public bool isMovementFinished()
    {
        return _isMovementFinished;
    }

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
            return new Vector2(float.MaxValue, float.MaxValue);
        }
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
}
