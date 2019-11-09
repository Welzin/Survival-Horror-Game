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

        _itemInRange = null;
        _doorInRange = null;
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

        if (_manager.levelManager.CinematicStarted())
        {
            if (Input.GetKeyDown(_dd.GetKey(Controls.Interact).Item1) || Input.GetKeyDown(_dd.GetKey(Controls.Interact).Item2) && _manager.IsSpeaking())
            {
                _manager.PassDialog();
            }

            return;
        }

        // Search for an item and a door
        // The detection is done with the body because the gameObject doesn't move when the character is moving (because this is just the sprite which change)
        RaycastHit2D hit = Physics2D.Raycast(transform.position, _manager.body.transform.up, _manager.catchDistance, LayerMask.GetMask("Items"));
        RaycastHit2D hit2 = Physics2D.Raycast(transform.position, _manager.body.transform.up, _manager.catchDistance, LayerMask.GetMask("Door"));
        
        if (hit.collider != null && hit.collider.gameObject.GetComponent<ItemObject>())
        {
            if (_itemInRange == null)
            {
                _itemInRange = hit.collider.gameObject.GetComponent<ItemObject>();
                _manager.hud.helper.DisplayHelp(Helper.Type.CatchItem);
            }
        }
        else if (_itemInRange != null)
        {
            _itemInRange = null;
            _manager.hud.helper.StopDisplayingHelp(Helper.Type.CatchItem);
        }

        if (hit2.collider != null && hit2.collider.gameObject.GetComponent<Door>())
        {
            if (_doorInRange == null)
            {
                _doorInRange = hit2.collider.gameObject.GetComponent<Door>();
                _manager.hud.helper.DisplayHelp(Helper.Type.OpenDoor);
            }
        }
        else if (_doorInRange != null)
        {
            _doorInRange = null;
            _manager.hud.helper.StopDisplayingHelp(Helper.Type.OpenDoor);
        }

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
        if (Input.GetKeyDown(_dd.GetKey(Controls.Interact).Item1) || Input.GetKeyDown(_dd.GetKey(Controls.Interact).Item2))
        {
            if (_manager.IsSpeaking())
            {
                _manager.PassDialog();
            }
            else if (_itemInRange != null)
            {
                _manager.StopAction();
                StartCoroutine(GrabObject(_itemInRange));
            }
            else if (_doorInRange != null)
            {
                _manager.StopAction();
                StartCoroutine(_doorInRange.OpenTheDoor());
            }
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

        // If you are in action, you cannot move the lamp
        if (!_manager.DoingAnAction())
        {
            LampRotation();
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
    /// The player look at the mouse position
    /// </summary>
    void LampRotation()
    {
        Vector3 v3 = Input.mousePosition;
        v3 = Camera.main.ScreenToWorldPoint(v3);
        v3.z = transform.position.z;
        _manager.lamp.transform.up = v3 - _manager.lamp.transform.position;
    }

    /// <summary>
    /// Grab the item in range and put it into the inventory
    /// </summary>
    private IEnumerator GrabObject(ItemObject item)
    {
        Action action = _manager.hud.actionBar.StartAction(item.timeToGrabItem);
        yield return new WaitForSeconds(item.timeToGrabItem);

        if (!action.interrupted)
        {
            _manager.inventory.AddItem(item.item);

            // The object will be destroy and _itemInRange equal to null, the helper will not stop display, so, we stop displaying help here
            _manager.hud.helper.StopDisplayingHelp(Helper.Type.CatchItem);
            Destroy(item.gameObject);
        }
    }

    // Player manager
    private PlayerManager _manager;
    // The animator
    private Animator _animator;
    // Player settings
    private DontDestroyOnLoad _dd;

    private AudioSource _audio;
    private int _currentFloor;

    private ItemObject _itemInRange;
    private Door _doorInRange;
}
