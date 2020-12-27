using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPlane : MonoBehaviour
{
    //game object is tagged with IgnoreTrigger so that bullet does not explode on spawn

    PlayerController player;
    Egg egg;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out player))
        {
            DamagePlayer();
        }
        else if (collision.TryGetComponent(out egg) && !egg.hatched)
        {
            SpawnEgg();
        }
        else
        {
            Destroy(collision.gameObject);
        }
    }

    void DamagePlayer()
    {
        player.Die(true);
    }

    void SpawnEgg()
    {
        BattleManager.Instance.SpawnEgg(0);
        Destroy(egg.gameObject);
        Debug.LogWarning("Egg fell out of death plane. Spawning a new egg.");
    }
}
