using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// The monster will move according to these action
// Each monster will move the same amount of time
// But their speed is different.
// More actions can be added later
public enum Actions
{
    Up,
    Down,
    Left,
    Right,
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
        // In 1s, the monster will execute its first pattern
        Invoke("ExecutePattern", 1);
    }

    void Update()
    {
        float x = transform.position.x;
        float y = transform.position.y;
        if(Math.Abs(_destination.x - x) > _errorPercentage || Math.Abs(_destination.y - y) > _errorPercentage)
        {
            Move();
        }
        SearchForLight();
    }

    private void ExecutePattern()
    {
        if(_pattern.Count != 0 && !_hasTarget)
        {
            Pattern toDo = _pattern.Dequeue();
            float x = transform.position.x;
            float y = transform.position.y;
            int amplitude = toDo.movementAmplitude;
            switch (toDo.action)
            {
                case Actions.Up:
                    {
                        _destination = new Vector2(x, y + amplitude);
                        break;
                    }
                case Actions.Down:
                    {
                        _destination = new Vector2(x, y - amplitude);
                        break;
                    }
                case Actions.Right:
                    {
                        _destination = new Vector2(x + amplitude, y);
                        break;
                    }
                case Actions.Left:
                    {
                        _destination = new Vector2(x - amplitude, y);
                        break;
                    }
            }
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
        if(lamp == null)
        {
#if UNITY_EDITOR
            Debug.LogError("Error: There are no lamps in the level");
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
        if(lamp.Active)
        {
            Vector2 lampPos = lamp.transform.position;
            Vector2 pos = transform.position;
            // Distance between lamp and monster
            float dist = Vector2.Distance(pos, lampPos);
            if(dist <= lightDetectionRange)
            {
                RaycastHit2D obstrusion = Physics2D.Linecast(pos, lampPos, LayerMask.GetMask("LightObstacles"));
                Debug.Log(obstrusion.collider);
                if (obstrusion.collider == null)
                {
                    _hasTarget = true;
                    _destination = lamp.transform.position;
                    CancelInvoke("ExecutePattern");
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
            if(_hasTarget)
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

    // Draws a circle and checks if there are lights in this circle. If there are, the monster will have its target (limited by sight)
    public float lightDetectionRange = 1f;
    // Draws a circle and checks if the monster hears a sound (not limited by sight)
    public float soundDetectionRange = 1f;
    // Patterns of the monster when there are no target
    public List<Pattern> movementPattern;
    public float speed = 1f;
    public AudioSource yell;
    // Need a public var to know what to do when a target is found.

    private Queue<Pattern> _pattern;
    // Has a target been found ?
    private bool _hasTarget;
    // The Monster has to move to this point
    private Vector2 _destination;
    // Accepted error percentage on movement
    private float _errorPercentage;
}
