using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBar : MonoBehaviour
{
    private void Start()
    {
        _barMaxSize = transform.localScale.x;
        StopAction();
    }

    private void Update()
    {
        if (!_actionFinished)
        {
            _actualTime += Time.deltaTime;
            transform.localScale = new Vector2(_actualTime / _actionTime * _barMaxSize, transform.localScale.y);

            if (_actualTime >= _actionTime)
            {
                StopAction();
            }
        }
    }

    public Action StartAction(float actionTime)
    {
        if (_actualAction != null)
        {
            ActionInterrupted();
        }

        Action action = new Action();
        _actionFinished = false;
        _actualTime = 0;
        _actionTime = actionTime;
        _actualAction = action;
        return action;
    }

    public void ActionInterrupted()
    {
        if (_actualAction != null)
        {
            _actualAction.interrupted = true;
            StopAction();
            Debug.Log("An action has been interrupted");
        }
    }

    private void StopAction()
    {
        _actionFinished = true;
        _actualAction = null;
        transform.localScale = new Vector2(0, transform.localScale.y);
    }

    public bool inAction { get { return !_actionFinished; } }

    private bool _actionFinished;
    private float _actualTime;
    private float _actionTime;
    private float _barMaxSize;
    private Action _actualAction;
}

public class Action
{
    // If this bool is on true, it means that the action has been interrupted by the player
    public bool interrupted = false;
}
