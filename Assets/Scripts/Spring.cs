using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    public float bounceVel;
    public AudioClip bounceSFX;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();

        if (rb && rb.bodyType == RigidbodyType2D.Dynamic)
        {
            PlayerController playerCon;
            if (collision.gameObject.TryGetComponent(out playerCon))
            {
                playerCon.DisableBetterJump(1);
            }
            rb.velocity = new Vector2(rb.velocity.x, bounceVel);
            AudioManager.PlayAudioAtPosition(bounceSFX, transform.position, AudioManager.sfxMixerGroup);
        }
    }
}
