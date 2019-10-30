using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    private void Awake()
    {
        _node = gameObject;
    }

    public List<Node> neighbours;

    public float X() => _node.transform.position.x;
    public float Y() => _node.transform.position.y;

    private GameObject _node;
}
