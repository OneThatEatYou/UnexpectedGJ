using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSquashRelay : MonoBehaviour
{
    public float squashTime;

    PlayerController playerCon;
    float time = 0;

    private void Awake()
    {
        playerCon = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //ignore trigger entering the squash zone
        if (collision.isTrigger)
        { return; }

        time += Time.fixedDeltaTime;

        //only get squashed when squash box is triggered for some time
        if (time > squashTime)
        {
            playerCon.Squash(collision);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exited squash trigger");
        time = 0;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        time = 0;
    }
}
