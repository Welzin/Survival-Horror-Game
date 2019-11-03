using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    private void Awake()
    {
        _node = gameObject;
    }
    public float X() => _node.transform.position.x;
    public float Y() => _node.transform.position.y;
    public Vector2 Position() => _node.transform.position;
    public void SetGO(GameObject go) { _node = go; }

    public List<Node> neighbours;
    private GameObject _node;

    private void OnValidate()
    {
        foreach(Node node in neighbours)
        {
            if (node != null && !node.neighbours.Contains(this))
            {
                node.neighbours.Add(this);
            }
        }
    }
}
