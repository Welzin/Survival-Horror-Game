using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public enum Type
    {
        Key,
        Battery,
        Teddy,
        Other
    };

    private void Start()
    {
        switch (type)
        {
            case Type.Key:
                item = new Key(sprite, doorForTheKey);
                break;
            case Type.Battery:
                item = new Battery();
                break;
            case Type.Teddy:
                item = new Teddy();
                break;
            case Type.Other:
                item = new Item(sprite);
                break;
        }
    }
    
    public Item item;
    public Type type;
    public Sprite sprite;
    public Door doorForTheKey;
}
