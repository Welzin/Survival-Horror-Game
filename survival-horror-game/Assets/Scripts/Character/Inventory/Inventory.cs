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
        _lampInPossession = false;
    }

    public void AddItem(Item item)
    {
        if (item is Key)
        {
            _keys.Add((Key) item);
            _hud.DisplayKeys(_keys);
        }
        else if (item is Battery)
        {
            _batteryNumber += 1;
            _hud.ChangeBatteryNumber(_batteryNumber);
        }
        else if (item is LampItem)
        {
            _lampInPossession = true;
        }
        else
        {
            if (item is Teddy)
            {
                _teddy = (Teddy)item;
            }
            else
            {
                _items.Add((Utility)item);
            }

            List<Utility> itemToDisplay = new List<Utility>();
            if (_teddy != null)
            {
                itemToDisplay.Add(_teddy);
            }
            itemToDisplay.AddRange(_items);

            _hud.DisplayItems(itemToDisplay);
        }

        _hud.helper.DisplayInfo("Vous avez attrappé un objet : " + item.name, 5);
    }

    public bool HaveBattery()
    {
        return _batteryNumber > 0;
    }

    public bool HaveLamp()
    {
        return _lampInPossession;
    }

    public bool HaveTeddy()
    {
        return _teddy != null;
    }

    public bool HaveItem(string name)
    {
        foreach (Utility item in _items)
        {
            if (item.name == name)
            {
                return true;
            }
        }

        return false;
    }

    public bool HaveKeyForDoor(Door door)
    {
        foreach (Key key in _keys)
        {
            if (key.doorToOpen == door)
            {
                return true;
            }
        }

        return false;
    }

    public void BatteryUsed()
    {
        _batteryNumber -= 1;
        _hud.ChangeBatteryNumber(_batteryNumber);
    }

    public void UsedKeyForDoor(Door door)
    {
        foreach (Key key in _keys)
        {
            if (key.doorToOpen == door)
            {
                _keys.Remove(key);
                _hud.DisplayKeys(_keys);
                return;
            }
        }
    }

    public void ItemUsed(string name)
    {
        foreach (Utility item in _items)
        {
            if (item.name == name)
            {
                _items.Remove(item);

                List<Utility> itemToDisplay = new List<Utility>();
                if (_teddy != null)
                {
                    itemToDisplay.Add(_teddy);
                }
                itemToDisplay.AddRange(_items);

                _hud.DisplayItems(itemToDisplay);

                return;
            }
        }
    }

    private HUD _hud;
    private List<Utility> _items;
    private List<Key> _keys;
    private Teddy _teddy;
    private int _batteryNumber;
    private bool _lampInPossession;
}

public class Item
{
    public Item(string name)
    {
        _name = name;
    }

    public string name { get { return _name; } }

    private readonly string _name;
}

public class Utility : Item
{
    public Utility(Sprite sprite, string name) : base(name)
    {
        _sprite = sprite;
    }

    public Sprite sprite { get { return _sprite; } }

    private readonly Sprite _sprite;
}

public class Key : Utility
{
    public Key(Sprite sprite, Door doorToOpen) : base(sprite, "une clé")
    {
        _doorToOpen = doorToOpen;
    }

    public Door doorToOpen { get { return _doorToOpen; } }

    private readonly Door _doorToOpen;
}

public class LampItem : Item
{
    public LampItem() : base("la lampe") {}
}

public class Battery : Item
{
    public Battery() : base("une pile") {}
}

public class Teddy : Utility
{
    public Teddy() : base(Resources.Load<Sprite>("Sprites/Teddy"), "Teddy !!") {}
}
