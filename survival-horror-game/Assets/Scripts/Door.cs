using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private void Start()
    {
        _player = FindObjectOfType<PlayerManager>();
        _animator = GetComponent<Animator>();
    }

    public IEnumerator OpenTheDoor()
    {
        if (!needAKey || _player.inventory.HaveKeyForDoor(this))
        {
            Action action = _player.hud.actionBar.StartAction(timeToOpen);
            yield return new WaitForSeconds(timeToOpen);

            if (!action.interrupted)
            {
                _player.inventory.UsedKeyForDoor(this);
                needAKey = false;
                DoorOpening();
            }
        }
        else
        {
            _player.hud.helper.DisplayHelp(Helper.Type.DoorLocked, 3);
        }
    }

    public bool IsClosed()
    {
        return _animator.GetBool("Closing");
    }

    private void DoorOpening()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        _animator.SetBool("Closing", false);
    }

    private void DoorClosing()
    {
        GetComponent<BoxCollider2D>().enabled = true;
        _animator.SetBool("Closing", true);
    }

    public bool needAKey;
    public float timeToOpen;

    private Animator _animator;
    private PlayerManager _player;
}
