using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public enum Type
    {
        Utility,
        Key,
        Battery,
        Teddy
    };

    private void Start()
    {
        switch (type)
        {
            case Type.Utility:
                item = new Utility(sprite, itemName);
                break;
            case Type.Key:
                item = new Key(sprite, doorForTheKey);
                break;
            case Type.Battery:
                item = new Battery();
                break;
            case Type.Teddy:
                item = new Teddy();
                break;
        }
    }
    
    public Item item;
    public Type type;
    public string itemName = "";
    public Sprite sprite;
    public Door doorForTheKey;
}
