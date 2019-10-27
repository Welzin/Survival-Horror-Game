using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        _dd = FindObjectOfType<DontDestroyOnLoad>();
    }

    void Update()
    {
        int x = 0;
        int y = 0;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(_dd.LeftKey()))
        {
            x -= 1;
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(_dd.RightKey()))
        {
            x += 1;
        }
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(_dd.UpKey()))
        {
            y += 1;
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(_dd.DownKey()))
        {
            y -= 1;
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(_dd.BindLamp()))
        {
            ToggleLamp();
        }

        Movement(x, y);
        PlayerRotation();
    }

    /// <summary>
    /// Manages player's movement
    /// </summary>
    private void Movement(float x, float y)
    {
        Vector2 direction = new Vector2(x, y).normalized;
        Vector2 pos = transform.position;
        pos += direction * speed * Time.deltaTime;
        transform.position = pos;
    }

    /// <summary>
    /// The player look at the mouse position
    /// </summary>
    void PlayerRotation()
    {
        Vector3 v3 = Input.mousePosition;
        v3 = Camera.main.ScreenToWorldPoint(v3);
        v3.z = body.transform.position.z;
        body.transform.right = v3 - transform.position;
    }

    /// <summary>
    /// Switch the lamp activation
    /// </summary>
    void ToggleLamp()
    {
        lamp.Active = !lamp.Active;
    }

    // Defines the walking speed of the player
    public float speed;
    // The sprite of the body
    public GameObject body;
    // The lamp
    public Lamp lamp;

    // Player settings
    private DontDestroyOnLoad _dd;
}
