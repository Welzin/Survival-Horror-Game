using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        _dd = FindObjectOfType<DontDestroyOnLoad>();
        if(_dd == null)
        {
#if UNITY_EDITOR
            Debug.LogError("Error: DontDestroyOnLoad hasn't been found in the scene.");
            UnityEditor.EditorApplication.isPlaying = false;
#else
            // Display an error message and exit the app
#endif
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
        if(Input.GetKey(_dd.Run().Item1) || Input.GetKey(_dd.Run().Item2))
        {
            isRunning = true;
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
        float s = isRunning ? speed * multiplier : speed;
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
        transform.right = v3 - transform.position;
    }

    /// <summary>
    /// Switch the lamp activation
    /// </summary>
    void ToggleLamp()
    {
        lamp.Active = !lamp.Active;
    }

    // Defines the walking speed of the player
    public float speed = 1f;
    // Multiplier when running
    public float multiplier = 2f;
    // The lamp
    public Lamp lamp;

    // Player settings
    private DontDestroyOnLoad _dd;
}
