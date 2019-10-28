using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        inventory = new Inventory(hud);
        _actualStress = 0;
    }

    private void Update()
    {
        if (!lamp.Active)
        {
            _actualStress += stressWithoutLightBySec * Time.deltaTime;
            hud.stressBar.ChangeStressPercentage(_actualStress / maxStress * 100);
        }
    }

    // The lamp handle
    public Lamp lamp;
    // The body which will turn
    public GameObject body;
    // The speed of the character
    public float speed = 3.0f;
    // The speed of the character
    public float maxStress = 100f;
    // The speed of the character
    public float stressWithoutLightBySec = 2f;
    // Multiplier when running
    public float runningFactor = 2f;
    // The distance to grab an object
    public float catchDistance = 0.5f;
    // The hud where everything will be displayed
    public HUD hud;
    // Inventory
    public Inventory inventory;
    
    private float _actualStress;
}
