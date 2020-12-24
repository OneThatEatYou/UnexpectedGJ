using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPlane : MonoBehaviour
{
    //game object is tagged with IgnoreTrigger so that bullet does not explode on spawn

    PlayerController player;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out player))
        {
            DamagePlayer();
        }
        else
        {
            Destroy(collision.gameObject);
        }
    }

    public void DamagePlayer()
    {
        player.Die();
    }
}
