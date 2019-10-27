using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Light2D;

public class Movement : MonoBehaviour
{
    public GameObject body;
    public Lamp lamp;
    public float speed = 5.0f;

    void Update()
    {
        PlayerRotation();

        if (Input.GetKeyDown(KeyCode.A))
            ToggleLamp();
        else if (Input.GetKey(KeyCode.Z))
        {
            Vector3 newPos = transform.position;
            newPos.y += speed * Time.deltaTime;
            transform.position = newPos;
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            Vector3 newPos = transform.position;
            newPos.x -= speed * Time.deltaTime;
            transform.position = newPos;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Vector3 newPos = transform.position;
            newPos.y -= speed * Time.deltaTime;
            transform.position = newPos;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Vector3 newPos = transform.position;
            newPos.x += speed * Time.deltaTime;
            transform.position = newPos;
        }
    }

    void PlayerRotation()
    {
        Vector3 v3 = Input.mousePosition;
        v3.z = 9.0f;
        v3 = Camera.main.ScreenToWorldPoint(v3);
        body.transform.right = v3 - transform.position;
    }

    void ToggleLamp()
    {
        lamp.Active = !lamp.Active;
    }
}
