using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FloorManager : MonoBehaviour
{
    private void Start()
    {
        _allFloorNodes = transform.GetComponentsInChildren<Node>();
    }

    public Node[] GetNodes()
    {
        return _allFloorNodes;
    }

    [Header("Please, put all nodes of the corresponding floor as child of this object")]
    public int floorNumber;
    public Node linkNode;

    private Node[] _allFloorNodes;
}
