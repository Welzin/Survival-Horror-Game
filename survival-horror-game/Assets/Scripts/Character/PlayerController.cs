using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        _dd = FindObjectOfType<DontDestroyOnLoad>();
        _manager = GetComponent<PlayerManager>();
        _animator = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();
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

    void FixedUpdate()
    {
        if (_manager.cinematicManager.CinematicStarted())
        {
            return;
        }

        // Search for an item
        // The detection is done with the body because the gameObject doesn't move when the character is moving (because this is just the sprite which change)
        RaycastHit2D hit = Physics2D.Raycast(transform.position, _manager.body.transform.up, _manager.catchDistance, LayerMask.GetMask("Items"));
        Debug.DrawRay(transform.position, _manager.body.transform.up, Color.cyan);

        // If it find one and that this one is not the same than the actual item
        if (hit.collider != null && hit.collider.gameObject.GetComponent<ItemObject>() && hit.collider.gameObject.GetComponent<ItemObject>() != _itemInRange)
        {
            NewItemInRange(hit.collider.gameObject.GetComponent<ItemObject>());
        }
        // If there is no item at range but we already had one in range
        else if (hit.collider == null && _itemInRange != null)
        {
            NoMoreItem();
        }
    }

    void Update()
    {
        if (_manager.cinematicManager.CinematicStarted())
        {
            return;
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
        if (Input.GetKeyDown(_dd.GetKey(Controls.Interact).Item1) || Input.GetKeyDown(_dd.GetKey(Controls.Interact).Item2) && _itemInRange != null)
        {
            if (_manager.IsSpeaking())
            {
                _manager.PassDialog();
            }
            else
            {
                _manager.StopAction();
                StartCoroutine(GrabObject());
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
            if(_audio.isPlaying)
            {
                _audio.Stop();
            }
            return;
        }
        else
        {
            _animator.SetInteger("Mouvement", 1);
        }

        // Set the animation direction value (down, left, up, right)
        if (x < 0)
        {
            _animator.SetInteger("Direction", 1);
        }
        else if (x > 0)
        {
            _animator.SetInteger("Direction", 3);
        }
        else if (y < 0)
        {
            _animator.SetInteger("Direction", 0);
        }
        else if (y > 0)
        {
            _animator.SetInteger("Direction", 2);
        }
        // If the player is running, play running steps sounds
        if (isRunning)
        {
            if(!_audio.isPlaying)
            {
                _audio.clip = _manager.runningSteps;
                _audio.PlayOneShot(_manager.runningSteps);
            }
        }
        // Else, check if the user is running. If he's running, stop the sound and play the footstep's one, otherwise
        // just check if it's playing.
        else
        {
            if(_audio.isPlaying)
            {
                if(_audio.clip != _manager.footsteps)
                {
                    _audio.Stop();
                    _audio.clip = _manager.footsteps;
                    _audio.PlayOneShot(_manager.footsteps);
                }
            }
            else
            {
                _audio.PlayOneShot(_manager.footsteps);
            }
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
        SoundManager sm = FindObjectOfType<SoundManager>();
        if (sm != null)
        {
            float noise = isRunning ? _manager.walkingNoise : _manager.walkingNoise * _manager.runningFactor;
            sm.CreateSoundWave(transform.position, noise, _currentFloor);
        }
        else
        {
            Debug.LogWarning("Player cannot generate sound: There are no objects of type Sound Manager in the scene!");
        }
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
    private IEnumerator GrabObject()
    {
        if (_itemInRange != null)
        {
            Action action = _manager.actionBar.StartAction(_itemInRange.timeToGrabItem);
            yield return new WaitForSeconds(_itemInRange.timeToGrabItem);

            if (!action.interrupted)
            {
                _manager.inventory.AddItem(_itemInRange.item);
                Destroy(_itemInRange.gameObject);

                NoMoreItem();
            }
        }
    }

    void NewItemInRange(ItemObject item)
    {
        _itemInRange = item;
        _manager.hud.helper.DisplayHelp(Helper.Type.CatchItem);
    }

    void NoMoreItem()
    {
        _itemInRange = null;
        _manager.hud.helper.StopDisplayingHelp(Helper.Type.CatchItem);
    }
    

    // Player manager
    private PlayerManager _manager;
    // The animator
    private Animator _animator;
    // Player settings
    private DontDestroyOnLoad _dd;
    // The item in range
    private ItemObject _itemInRange;

    private AudioSource _audio;
    private int _currentFloor;
}
