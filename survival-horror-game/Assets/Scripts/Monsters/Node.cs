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
        //Vector2 distance = other.Position() - Position();
        List<RaycastHit2D> hits = new List<RaycastHit2D>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.layerMask = LayerMask.GetMask("Event");
        Physics2D.Linecast(Position(), other.Position(), filter, hits);
        foreach(RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject.GetComponent<Door>())
            {
                door = hit.collider.gameObject.GetComponent<Door>();
                break;
            }
        }
        return door;
    }
}
