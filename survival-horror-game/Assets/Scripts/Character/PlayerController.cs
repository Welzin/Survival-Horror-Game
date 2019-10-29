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
        int x = 0;
        int y = 0;
        bool isRunning = false;
        // When something cannot be done at the same time than doing an action, StopAction() is called to stop it.
        // We prefer to stop the action instead of avoid teh possibility to move etc...
        // StopAction must be called BEFORE the new action. (Otherwise this is the new action that will be stopped)
        if (Input.GetKey(_dd.LeftKey().Item1) || Input.GetKey(_dd.LeftKey().Item2))
        {
            _manager.StopAction();
            x -= 1;
        }
        if (Input.GetKey(_dd.RightKey().Item1) || Input.GetKey(_dd.RightKey().Item2))
        {
            _manager.StopAction();
            x += 1;
        }
        if (Input.GetKey(_dd.UpKey().Item1) || Input.GetKey(_dd.UpKey().Item2))
        {
            _manager.StopAction();
            y += 1;
        }
        if (Input.GetKey(_dd.DownKey().Item1) || Input.GetKey(_dd.DownKey().Item2))
        {
            _manager.StopAction();
            y -= 1;
        }
        if (Input.GetKeyDown(_dd.BindLamp().Item1) || Input.GetKeyDown(_dd.BindLamp().Item2))
        {
            _manager.StopAction();
            _manager.ToggleLamp();
        }
        if (Input.GetKey(_dd.Run().Item1) || Input.GetKey(_dd.Run().Item2))
        {
            isRunning = true;
        }
        if (Input.GetKeyDown(_dd.Interact().Item1) || Input.GetKeyDown(_dd.Interact().Item2) && _itemInRange != null)
        {
            _manager.StopAction();
            GrabObject();
        }
        if (Input.GetKeyDown(_dd.Reload().Item1) || Input.GetKeyDown(_dd.Reload().Item2))
        {
            _manager.StopAction();
            _manager.ReloadLamp();
        }
        if (Input.GetKeyDown(_dd.HugTeddy().Item1) || Input.GetKeyDown(_dd.HugTeddy().Item2))
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
    private void Movement(int x, int y, bool isRunning)
    {
        // Set the animation mouvement value (idle, walking, running...)
        if (x == 0 && y == 0)
        {
            _animator.SetInteger("Mouvement", 0);
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
            sm.CreateSoundWave(transform.position, 4);
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
    void GrabObject()
    {
        if (_itemInRange != null)
        {
            _manager.inventory.AddItem(_itemInRange.item);
            Destroy(_itemInRange.gameObject);

            NoMoreItem();
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
}
