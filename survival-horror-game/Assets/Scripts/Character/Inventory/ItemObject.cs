using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    private void Start()
    {
        item = new Item(sprite);
    }
    
    public Sprite sprite;
    public Item item;
}
