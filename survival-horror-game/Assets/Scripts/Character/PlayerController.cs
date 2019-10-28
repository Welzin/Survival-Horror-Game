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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, _manager.catchDistance, LayerMask.GetMask("Items"));

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
        if (Input.GetKey(_dd.LeftKey().Item1) || Input.GetKey(_dd.LeftKey().Item2))
        {
            x -= 1;
        }
        if (Input.GetKey(_dd.RightKey().Item1) || Input.GetKey(_dd.RightKey().Item2))
        {
            x += 1;
        }
        if (Input.GetKey(_dd.UpKey().Item1) || Input.GetKey(_dd.UpKey().Item2))
        {
            y += 1;
        }
        if (Input.GetKey(_dd.DownKey().Item1) || Input.GetKey(_dd.DownKey().Item2))
        {
            y -= 1;
        }
        if (Input.GetKeyDown(_dd.BindLamp().Item1) || Input.GetKeyDown(_dd.BindLamp().Item2))
        {
            ToggleLamp();
        }
        if (Input.GetKey(_dd.Run().Item1) || Input.GetKey(_dd.Run().Item2))
        {
            isRunning = true;
        }
        if (Input.GetKeyDown(_dd.Interact().Item1) || Input.GetKeyDown(_dd.Interact().Item2) && _itemInRange != null)
        {
            GrabObject();
        }

        Movement(x, y, isRunning);
        PlayerRotation();
    }

    /// <summary>
    /// Manages player's movement
    /// </summary>
    private void Movement(int x, int y, bool isRunning)
    {
        Vector2 direction = new Vector2(x, y).normalized;
        Vector2 pos = transform.position;
        float s = isRunning ? _manager.speed * _manager.runningFactor : _manager.speed;
        pos += direction * s * Time.deltaTime;
        transform.position = pos;
    }

    /// <summary>
    /// The player look at the mouse position
    /// </summary>
    void PlayerRotation()
    {
        Vector3 v3 = Input.mousePosition;
        v3 = Camera.main.ScreenToWorldPoint(v3);
        v3.z = transform.position.z;
        transform.up = v3 - transform.position;
    }

    /// <summary>
    /// Switch the lamp activation
    /// </summary>
    void ToggleLamp()
    {
        _manager.lamp.Active = !_manager.lamp.Active;
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
    // Player settings
    private DontDestroyOnLoad _dd;
    // The item in range
    private ItemObject _itemInRange;
}
