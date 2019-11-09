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

    public Door DoorBetweenNodes(Node other)
    {
        Door door = null;
        Vector2 distance = other.Position() - Position();

        RaycastHit2D hit = Physics2D.Raycast(Position(), distance.normalized, distance.magnitude, LayerMask.GetMask("Event"));
        if (hit.collider != null && hit.collider.gameObject.GetComponent<Door>())
            door = hit.collider.gameObject.GetComponent<Door>();

        return door;
    }
}
