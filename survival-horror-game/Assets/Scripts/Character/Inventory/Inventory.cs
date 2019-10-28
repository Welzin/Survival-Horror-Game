using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public Inventory(HUD hud)
    {
        _hud = hud;
        _items = new List<Item>();
        _keys = new List<Key>();
    }

    public void AddItem(Item item)
    {
        _items.Add(item);
        DisplayUI();
    }

    public void AddKey(Key key)
    {
        _keys.Add(key);
        DisplayUI();
    }

    private void DisplayUI()
    {
        _hud.Clear();
        _hud.DisplayItems(_items);
        _hud.DisplayKeys(_keys);
    }
    
    public HUD _hud;
    private List<Item> _items;
    private List<Key> _keys;
}

public class Item
{
    public Item(Sprite sprite)
    {
        _sprite = sprite;
    }

    public Sprite sprite { get { return _sprite; } }

    private readonly Sprite _sprite;
}

public class Key : Item
{
    public Key(Sprite sprite) : base(sprite) {}
}
