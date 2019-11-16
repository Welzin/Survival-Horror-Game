using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Listener
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _dd = FindObjectOfType<DontDestroyOnLoad>();
        _manager = GetComponent<PlayerManager>();
        _animator = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();

        _eventInRange = null;
        _currentFloor = 2;

        if (_dd == null)
        {
#if UNITY_EDITOR
            Debug.LogError("Error: DontDestroyOnLoad hasn't been found in the scene.");
            UnityEditor.EditorApplication.isPlaying = false;
#else
            // Display an error message and exit the app
#endif
        }

    }

    protected override void Update()
    {
        base.Update();

        if (_dd.GamePause)
        {
            return;
        }

        // In all case, we can pass dialogs
        if ((Input.GetKeyDown(_dd.GetKey(Controls.Interact).Item1) || Input.GetKeyDown(_dd.GetKey(Controls.Interact).Item2)) && _manager.IsSpeaking())
        {
            _manager.PassDialog();
        }

        // If we are in a cinematic, in panic or speaking, the player have no control
        if (_manager.levelManager.CinematicStarted())
        {
            return;
        }

        // If the player is hugging, he can stop hugging but cannot move
        if (_manager.IsHuggingTeddy())
        {
            if (Input.GetKeyDown(_dd.GetKey(Controls.HugTeddy).Item1) || Input.GetKeyDown(_dd.GetKey(Controls.HugTeddy).Item2))
            {
                _manager.StopHuggingTeddy();
            }

            return;
        }

        // If you are in action, or in cinematic, you cannot move the lamp
        if (!_manager.DoingAnAction())
        {
            LampRotation();
        }

        if (!_manager.InPanic() && !_manager.IsSpeaking())
        {
            SearchEventInRange();

            float x = 0;
            float y = 0;
            bool isRunning = false;
            // When something cannot be done at the same time than doing an action, StopAction() is called to stop it.
            // We prefer to stop the action instead of avoid teh possibility to move etc...
            // StopAction must be called BEFORE the new action. (Otherwise this is the new action that will be stopped)
            if (Input.GetKey(_dd.GetKey(Controls.Left).Item1) || Input.GetKey(_dd.GetKey(Controls.Left).Item2))
            {
                _manager.StopAction();
                x -= 1;
            }
            if (Input.GetKey(_dd.GetKey(Controls.Right).Item1) || Input.GetKey(_dd.GetKey(Controls.Right).Item2))
            {
                _manager.StopAction();
                x += 1;
            }
            if (Input.GetKey(_dd.GetKey(Controls.Up).Item1) || Input.GetKey(_dd.GetKey(Controls.Up).Item2))
            {
                _manager.StopAction();
                y += 1;
            }
            if (Input.GetKey(_dd.GetKey(Controls.Down).Item1) || Input.GetKey(_dd.GetKey(Controls.Down).Item2))
            {
                _manager.StopAction();
                y -= 1;
            }
            if (Input.GetKeyDown(_dd.GetKey(Controls.Lamp).Item1) || Input.GetKeyDown(_dd.GetKey(Controls.Lamp).Item2))
            {
                _manager.StopAction();
                _manager.ToggleLamp();
            }
            if (Input.GetKey(_dd.GetKey(Controls.Run).Item1) || Input.GetKey(_dd.GetKey(Controls.Run).Item2))
            {
                isRunning = true;
            }
            if ((Input.GetKeyDown(_dd.GetKey(Controls.Interact).Item1) || Input.GetKeyDown(_dd.GetKey(Controls.Interact).Item2)) && _eventInRange != null)
            {
                _manager.StopAction();
                _eventInRange.PlayEvent();
            }
            if (Input.GetKeyDown(_dd.GetKey(Controls.Reload).Item1) || Input.GetKeyDown(_dd.GetKey(Controls.Reload).Item2))
            {
                _manager.StopAction();
                StartCoroutine(_manager.ReloadLamp());
            }
            if (Input.GetKeyDown(_dd.GetKey(Controls.HugTeddy).Item1) || Input.GetKeyDown(_dd.GetKey(Controls.HugTeddy).Item2))
            {
                _manager.StopAction();
                StartCoroutine(_manager.HugTeddy());
            }

            Movement(x, y, isRunning);
        }
    }

    /// <summary>
    /// Manages player's movement
    /// </summary>
    public void Movement(float x, float y, bool isRunning)
    {
        // Currently 0.4f, may be changed to a public var
        if(isRunning)
        {
            _animator.speed = _manager.runningFactor;
        }
        else
        {
            _animator.speed = 1;
        }

        // Set the animation mouvement value (idle, walking, running...)
        if (x == 0 && y == 0)
        {
            _animator.SetInteger("Mouvement", 0);
            _manager.Emiter.StopEffect();
            return;
        }
        else
        {
            _animator.SetInteger("Mouvement", 1);
        }

        // Set the animation direction value (down, left, up, right)
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

        // If the player is running, play running steps sounds
        if (isRunning)
        {
            _manager.Emiter.PlayEffect(SoundType.RunningSteps);
        }
        // Else, check if the user is running. If he's running, stop the sound and play the footstep's one, otherwise
        // just check if it's playing.
        else
        {
            _manager.Emiter.PlayEffect(SoundType.Footsteps);
        }
        // Calculation of the new position
        Vector2 direction = new Vector2(x, y).normalized;
        Vector2 pos = transform.position;
        float s = isRunning ? _manager.speed * _manager.runningFactor : _manager.speed;
        pos += direction * s * Time.deltaTime;

        // The body is looking at this direction to allow detection with raycast (see FixedUpdate)
        _manager.body.transform.up = pos - (Vector2)_manager.body.transform.position;
        transform.position = pos;

        // Generate sound
        float noise = isRunning ? _manager.walkingNoise : _manager.walkingNoise * _manager.runningFactor;
        _manager.Emiter.EmitSoundWave(noise, _currentFloor, 1);
    }

    /// <summary>
    /// Search an event
    /// </summary>
    void SearchEventInRange()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, _manager.body.transform.up, _manager.catchDistance, LayerMask.GetMask("Event"));
        Event hitEvent = null;

        if (hit.collider != null)
            hitEvent = hit.collider.gameObject.GetComponent<Event>();

        if (hitEvent != null && hitEvent.type == Event.EventType.OnClick)
        {
            // The new event is nearest than the older
            if (_eventInRange != null && _eventInRange != hitEvent && (_eventInRange.transform.position - transform.position).magnitude < (hitEvent.transform.position - transform.position).magnitude)
            {
                NoMoreItemInRange();
            }

            // If their is no actual event in range, the new one is this one
            if (_eventInRange == null)
            {
                _eventInRange = hit.collider.gameObject.GetComponent<Event>();

                if (_eventInRange is ItemObject)
                {
                    _manager.hud.helper.DisplayHelp(Helper.Type.CatchItem);
                }
                else if (_eventInRange is Door)
                {
                    Door door = (Door)_eventInRange;
                    if (door.IsClosed())
                    {
                        _manager.hud.helper.DisplayHelp(Helper.Type.OpenDoor);
                    }
                    else
                    {
                        _manager.hud.helper.DisplayHelp(Helper.Type.CloseDoor);
                    }
                }
                else if (_eventInRange is LevelEvent)
                {
                    _manager.hud.helper.DisplayHelp(Helper.Type.InteractWithEnvironment);
                }
            }
        }
        else
        {
            NoMoreItemInRange();
        }
    }

    void NoMoreItemInRange()
    {
        if (_eventInRange is ItemObject)
        {
            _manager.hud.helper.StopDisplayingHelp(Helper.Type.CatchItem);
        }
        else if (_eventInRange is Door)
        {
            Door door = (Door)_eventInRange;
            if (door.IsClosed())
            {
                _manager.hud.helper.StopDisplayingHelp(Helper.Type.OpenDoor);
            }
            else
            {
                _manager.hud.helper.StopDisplayingHelp(Helper.Type.CloseDoor);
            }
        }
        else if (_eventInRange is LevelEvent)
        {
            _manager.hud.helper.StopDisplayingHelp(Helper.Type.InteractWithEnvironment);
        }

        _eventInRange = null;
    }

    /// <summary>
    /// The player look at the mouse position
    /// </summary>
    void LampRotation()
    {
        Vector3 v3 = Input.mousePosition;
        v3 = Camera.main.ScreenToWorldPoint(v3);
        v3.z = transform.position.z;
        _manager.lamp.transform.up = v3 - _manager.lamp.transform.position;
    }

    // Player manager
    private PlayerManager _manager;
    // The animator
    private Animator _animator;
    // Player settings
    private DontDestroyOnLoad _dd;

    private AudioSource _audio;
    private int _currentFloor;

    private Event _eventInRange;
}
