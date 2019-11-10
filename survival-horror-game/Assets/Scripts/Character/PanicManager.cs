using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanicManager : MonoBehaviour
{
    void Start()
    {
        _player = FindObjectOfType<PlayerManager>();
        _panic = false;
        _floorManagers = FindObjectsOfType<FloorManager>();
        _currentGoal = null;
    }
    
    void Update()
    {
        if(_currentGoal != null)
        {
            if(_currentPath.Count > 0)
            {
                if (IsNear(transform.position, _currentGoal.Position()))
                {
                    Door door = _currentGoal.DoorBetweenNodes(_currentPath.Peek());
                    if(door != null)
                    {
                        if (door.needAKey) { MakePlayerRun(); return; }
                        if (door.IsClosed())
                        {
                            door.OpenForMonster();
                        }
                    }
                    _currentGoal = _currentPath.Dequeue();
                    _player.SetNewDestination(_currentGoal.Position(), true);
                }
            }
        }
        
        if(_player.Stress() < 100) 
            ResetPanic(); 
    }

    public bool IsPanicking()
    {
        return _panic;
    }

    public IEnumerator StartPanicking()
    {
        _panic = true;
        StartCoroutine(CountDown(10));
        
        if(!_player.inventory.HaveLamp())
        {
            MakePlayerRun();
        }
        else if (!_player.lamp.Active)
        {
            if (_player.lamp.actualBattery > 0)
                _player.ToggleLamp();
            else
            {
                if (_player.inventory.HaveBattery())
                {
                    _player.ReloadLamp();
                    _player.ToggleLamp();
                }
                else 
                    MakePlayerRun();
            }
        }

        yield return null;
    }

    private void MakePlayerRun()
    {
        List<Noise> noiseHeard = _player.controller.AllNoiseHeard();
        Node destination = null;
        if(noiseHeard.Count > 0)
        {
            Vector2 nextDir = CheckForSound(noiseHeard);
            if (nextDir.x == 0 && nextDir.y == 0)
            {
                if(_player.inventory.HaveTeddy())
                    _player.HugTeddy();
            }
            else
            {
                destination = NearestNode((Vector2)transform.position + 100 * nextDir, _player.CurrentFloor);
            }
        }
        else
        {
            destination = FarestNode(transform.position, _player.CurrentFloor);
        }
        //Debug.Log(destination);
        if (destination != null)
            MoveToward(destination);
        Invoke("StopMovement", 4);
    }

    private Vector2 CheckForSound(List<Noise> noiseHeard)
    {
        List<Vector2> allNoiseDirection = new List<Vector2>();
        foreach (Noise noise in noiseHeard)
        {
            allNoiseDirection.Add(((Vector2)transform.position - noise.origin).normalized);
        }
        Vector2 nextDirection = new Vector2(0, 0);
        // Search for a safe direction
        for (int i = -10; i <= 10; ++i)
        {
            for (int j = -10; j <= 10; ++j)
            {
                Vector2 ij = new Vector2((float)i/10, (float)j/10);
                if (!allNoiseDirection.Contains(ij))
                {
                    nextDirection = ij;
                    break;
                }
            }
        }
        return nextDirection;
    }

    private Node NearestNode(Vector2 position, int floor)
    {
        Node[] allNodes = NodesOfFloor(floor);
        (Node, float) minNode = (allNodes[0], float.MaxValue);
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
    
    private Node FarestNode(Vector2 position, int floor)
    {
        Node[] allNodes = NodesOfFloor(floor);
        (Node, float) maxNode = (allNodes[0], float.MinValue);
        foreach (Node node in allNodes)
        {
            // Get the distance between the node and the current position
            float dist = Mathf.Sqrt(Mathf.Pow(node.X() - position.x, 2) + Mathf.Pow(node.Y() - position.y, 2));
            // If the distance is inferior from the min value, this node is nearer than the last recorded
            if (dist > maxNode.Item2)
            {
                maxNode = (node, dist);
            }
        }
        return maxNode.Item1;
    }

    private Node[] NodesOfFloor(int floor)
    {
        foreach(FloorManager fm in _floorManagers)
        {
            if (fm.floorNumber == floor)
                return fm.GetNodes();
        }
        return null;
    }

    private void MoveToward(Node dest)
    {
        Node node = NearestNode((Vector2)transform.position, _player.CurrentFloor);
        List<Node> path = Pathfinder.Path(node, dest, FindObjectsOfType<Node>());
        _currentPath = new Queue<Node>(path);
        _currentGoal = _currentPath.Dequeue();
        _player.SetNewDestination(_currentGoal.Position(), true);
    }

    private void StopMovement()
    {
        _currentPath.Clear();
        _currentGoal = null;
        _player.StopMoving();
        if(_player.inventory.HaveTeddy())
        {
            StartCoroutine(_player.HugTeddy());
        }
    }

    private void ResetPanic()
    {
        _panic = false;
    }

    private bool IsNear(Vector2 position, Vector2 goal)
    {
        return Mathf.Abs(goal.x - position.x) <= 0.2f && Mathf.Abs(goal.y - position.y) <= 0.2f;
    }

    private IEnumerator CountDown(int time)
    {
        int timer = 0;
        while(timer != time && _panic)
        {
            // If the player speak, the stress cannot go down/up. So, if the player is speaking, it's like a pause
            if (!_player.IsSpeaking())
            {
                yield return new WaitForSecondsRealtime(1);
                timer += 1;
            }
            else
            {
                yield return null;
            }
        }
        if(timer == time)
        {
            _player.GameOver();
        }
    }

    private PlayerManager _player;
    private bool _panic;
    private FloorManager[] _floorManagers;
    private Queue<Node> _currentPath;
    private Node _currentGoal;
}
