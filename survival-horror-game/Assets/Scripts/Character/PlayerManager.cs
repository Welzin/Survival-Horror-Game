using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        inventory = new Inventory(hud);
        cinematicManager = FindObjectOfType<CinematicManager>();
        controller = FindObjectOfType<PlayerController>();
        _actualStress = 0;
        _huggingTeddy = false;
        _arrived = true;
    }

    private void Update()
    {
        Move();
        
        if (cinematicManager.CinematicStarted())
        {
            return;
        }

        ManageStress();

        // HUD change according to the lamp's battery value
        if (lamp.Active)
        {
            hud.batteryBar.ChangeBatteryPercentage(lamp.actualBattery / lamp.maxBattery * 100);
        }
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
    public IEnumerator ReloadLamp()
    {
        if (inventory.HaveBattery())
        {
            bool active = lamp.Active;
            lamp.Active = false;
            Action action = actionBar.StartAction(timeToReloadLamp);
            yield return new WaitForSeconds(timeToReloadLamp);

            if (!action.interrupted)
            {
                lamp.Reload();
                inventory.BatteryUsed();
                hud.batteryBar.ChangeBatteryPercentage(lamp.actualBattery / lamp.maxBattery * 100);
                lamp.Active = active;
            }
        }
    }

    /// <summary>
    /// Begin or stop hugging teddy
    /// </summary>
    public IEnumerator HugTeddy()
    {
        if (inventory.HaveTeddy())
        {
            Action action = actionBar.StartAction(timeToHugTeddy);
            yield return new WaitForSeconds(timeToHugTeddy);

            if (!action.interrupted)
            {
                _huggingTeddy = true;
            }
        }
        else
        {
            hud.helper.DisplayInfo(hud.helper.infoZone, "You cannot hug Teddy because he is lost :(");
        }
    }

    /// <summary>
    /// Is the player already doing an action
    /// </summary>
    /// <returns></returns>
    public bool DoingAnAction()
    {
        return actionBar.inAction;
    }

    /// <summary>
    /// Interrupt an action
    /// </summary>
    public void StopAction()
    {
        if (_huggingTeddy)
        {
            _huggingTeddy = false;
        }

        if (actionBar.inAction)
        {
            actionBar.ActionInterrupted();
        }
    }

    /// <summary>
    /// Manage all the stress feelings by the player
    /// </summary>
    private void ManageStress()
    {
        float effectiveLight = 0;

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
                    effectiveLight += factor * light.effectiveLight;
                }
            }
        }

        if (lamp.Active)
        {
            effectiveLight = Mathf.Max(1, effectiveLight);
        }


        // If the effectiv light is less than one, we consider that the player is still on the dark.
        if (effectiveLight < 1)
        {
            // But if there is some light, the stress is increase slower
            AddStress(stressInTheDark * Time.deltaTime * (1 - effectiveLight));
        }
        else
        {
            // If the light is sufficient, the stress decrease.
            AddStress(-(effectiveLight - 1) * stressRemovedWithLight * Time.deltaTime);
        }

        if (_huggingTeddy)
        {
            AddStress(- stressRemovedWileHugging * Time.deltaTime);
        }

    }

    private void AddStress(float stress)
    {
        // No less than 0, no more than maxStress
        _actualStress = Mathf.Max(Mathf.Min(_actualStress + stress, maxStress), 0);
        hud.stressBar.ChangeStressPercentage(_actualStress / maxStress * 100);
    }

    public void SetNewDestination(Vector3 destination)
    {
        _destination = destination;
        _arrived = false;
    }

    public bool IsMoving()
    {
        return !_arrived;
    }

    private void Move()
    {
        if (!_arrived)
        {
            Vector3 toGo = (_destination - controller.transform.position).normalized;
            controller.Movement(toGo.x, toGo.y, false);

            if ((controller.transform.position - _destination).magnitude < 0.1 && (controller.transform.position - _destination).magnitude > -0.1)
            {
                controller.Movement(0, 0, false);
                _arrived = true;
            }
        }
    }

    public void Speak(string newText)
    {
        hud.dialog.AddDialog(newText);
    }

    public void PassDialog()
    {
        hud.dialog.NextDialog();
    }

    public bool IsSpeaking()
    {
        return !hud.dialog.FinishSpeaking();
    }

    // The lamp handle
    public Lamp lamp;
    // The body which will turn
    public GameObject body;
    // The action bar to display when the player is doing an action
    public ActionBar actionBar;
    // The hud where everything will be displayed
    public HUD hud;
    // Inventory
    public Inventory inventory;
    // PlayerController for movement
    public PlayerController controller;
    public CinematicManager cinematicManager;
    // The speed of the character
    public float speed = 3.0f;
    // The speed of the character
    public float maxStress = 100f;
    // The stress by second in the dark
    public float stressInTheDark = 2f;
    // The stress removed by second when you are under an effectivelight = 1
    public float stressRemovedWithLight = 2f;
    // The stress removed by second when you are hugging Teddy
    public float stressRemovedWileHugging = 1f;
    // Multiplier when running
    public float runningFactor = 2f;
    // The distance to grab an object
    public float catchDistance = 0.5f;
    // The time that take the action "hug Teddy"
    public float timeToHugTeddy = 2f;
    // The time that take the action "reload lamp"
    public float timeToReloadLamp = 2f;
    
    public AudioClip footsteps;
    public AudioClip runningSteps;
    public float walkingNoise = 2f;
    
    private float _actualStress;
    private bool _huggingTeddy;
    private bool _arrived;
    private Vector3 _destination;
}
