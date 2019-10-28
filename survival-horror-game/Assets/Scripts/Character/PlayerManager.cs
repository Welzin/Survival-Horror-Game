using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        _inventory = new Inventory(hud);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // The lamp handle
    public Lamp lamp;
    // The speed of the character
    public float speed = 3.0f;
    // Multiplier when running
    public float runningFactor = 2f;
    // The hud where everything will be displayed
    public HUD hud;
    // Inventory
    Inventory _inventory;
}
