using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private void Start()
    {
        player = FindObjectOfType<PlayerManager>();
        _isClosed= true;
    }

    public IEnumerator OpenTheDoor()
    {
        if (!needAKey || player.inventory.HaveKeyForDoor(this))
        {
            Action action = player.actionBar.StartAction(timeToOpen);
            yield return new WaitForSeconds(timeToOpen);

            if (!action.interrupted)
            {
                player.inventory.UsedKeyForDoor(this);
                _isClosed = false;
                GetComponent<BoxCollider2D>().enabled = false;
                GetComponent<SpriteRenderer>().enabled = false;
            }
        }
        else
        {
            player.hud.helper.DisplayHelp(Helper.Type.DoorLocked, 3);
        }
    }

    public bool IsClosed()
    {
        return _isClosed;
    }

    public bool needAKey;
    public float timeToOpen;

    private PlayerManager player;
    private bool _isClosed;
}
