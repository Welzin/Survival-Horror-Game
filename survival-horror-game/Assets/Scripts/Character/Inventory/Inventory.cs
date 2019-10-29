using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public Inventory(HUD hud)
    {
        _hud = hud;
        _items = new List<Utility>();
        _keys = new List<Key>();
        _teddy = null;
        _batteryNumber = 0;
    }

    public void AddItem(Item item)
    {
        if (item is Key)
        {
            _keys.Add((Key) item);
            _hud.DisplayKeys(_keys);
        }
        else if (item is Teddy)
        {
            _teddy = (Teddy) item;
        }
        else if (item is Battery)
        {
            _batteryNumber += 1;
            _hud.ChangeBatteryNumber(_batteryNumber);
        }
        else if (item is Item)
        {
            _items.Add((Utility) item);
            _hud.DisplayItems(_items, _teddy);
        }
    }

    public bool HaveBattery()
    {
        return _batteryNumber > 0;
    }

    public void BatteryUsed()
    {
        _batteryNumber -= 1;
        _hud.ChangeBatteryNumber(_batteryNumber);
    }
    
    private HUD _hud;
    private List<Utility> _items;
    private List<Key> _keys;
    private Teddy _teddy;
    private int _batteryNumber;
}

public class Item
{
    public Item() {}
}

public class Utility : Item
{
    public Utility(Sprite sprite) : base()
    {
        _sprite = sprite;
    }

    public Sprite sprite { get { return _sprite; } }

    private readonly Sprite _sprite;
}

public class Key : Utility
{
    public Key(Sprite sprite, Door doorToOpen) : base(sprite)
    {
        _doorToOpen = doorToOpen;
    }

    public Door doorToOpen { get { return _doorToOpen; } }

    private readonly Door _doorToOpen;
}

public class Battery : Item
{
    public Battery() : base() {}
}

public class Teddy : Utility
{
    public Teddy() : base(Resources.Load<Sprite>("Sprites/Teddy")) {}
}
