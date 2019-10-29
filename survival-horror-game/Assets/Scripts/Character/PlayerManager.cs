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
        ManageStress();
    }

    /// <summary>
    /// Switch the lamp activation
    /// </summary>
    public void ToggleLamp()
    {
        lamp.Active = !lamp.Active;
    }

    /// <summary>
    /// Reload the lamp if the player have a battery. Delete the battery after.
    /// </summary>
    public void ReloadLamp()
    {
        if (inventory.HaveBattery())
        {
            lamp.Reload();
            inventory.BatteryUsed();
            hud.batteryBar.ChangeBatteryPercentage(lamp.actualBattery / lamp.maxBattery * 100);
        }
    }

    /// <summary>
    /// Manage all the stress feelings by the player
    /// </summary>
    private void ManageStress()
    {
        foreach (Light light in FindObjectsOfType<Light>())
        {
            float proximity = (light.transform.position - transform.position).magnitude;
            if (proximity <= light.radius)
            {
                // Search for a wall
                // If there is a wall, the light isn't seen
                RaycastHit2D hit = Physics2D.Raycast(light.transform.position, (transform.position - light.transform.position).normalized, light.radius, LayerMask.GetMask("Obstacle"));

                if (hit.collider == null)
                {
                    // This is the factor between 0 and 1 wich say how close teh character is from the light
                    float factor = (light.radius - proximity) / light.radius;
                    // It represent the effective light which are on the character
                    float effectiveLight = factor * light.effectiveLight;

                    AddStress(-(effectiveLight * stressRemovedWithLight * Time.deltaTime));
                }
                else
                {
                    Debug.Log("obstacle between the light and player");
                }
            }
        }
        if (!lamp.Active)
        {
            AddStress(stressInTheDark * Time.deltaTime);
        }
        else
        {
            hud.batteryBar.ChangeBatteryPercentage(lamp.actualBattery / lamp.maxBattery * 100);
        }
    }

    private void AddStress(float stress)
    {
        // No less than 0, no more than maxStress
        _actualStress = Mathf.Max(Mathf.Min(_actualStress + stress, maxStress), 0);
        hud.stressBar.ChangeStressPercentage(_actualStress / maxStress * 100);
    }

    // The lamp handle
    public Lamp lamp;
    // The body which will turn
    public GameObject body;
    // The speed of the character
    public float speed = 3.0f;
    // The speed of the character
    public float maxStress = 100f;
    // The stress by second in the dark
    public float stressInTheDark = 2f;
    // The stress removed by second when you are under an effectivelight = 1
    public float stressRemovedWithLight = 2f;
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
