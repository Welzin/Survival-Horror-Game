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
        if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(_dd.LeftKey()))
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
        Movement(x, y);
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

    // Defines the walking speed of the player
    public float speed;
    // Player settings
    private DontDestroyOnLoad _dd;
}
