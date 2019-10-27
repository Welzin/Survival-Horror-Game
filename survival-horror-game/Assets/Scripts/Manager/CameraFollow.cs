using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        _character = FindObjectOfType<PlayerController>().gameObject;
        _playerPos = _character.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 currentPlayerPos = _character.transform.position;
        if(currentPlayerPos != _playerPos)
        {
            transform.position = new Vector3(currentPlayerPos.x, currentPlayerPos.y, transform.position.z);
            _playerPos = currentPlayerPos;
        }
    }

    private GameObject _character;
    private Vector2 _playerPos;
}
