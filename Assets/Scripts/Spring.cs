using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    public float bounceForce = 30;
    public float cooldown = 1;
    public AudioClip bounceSFX;
    public string isActivatedParam = "isActivated";
    Animator anim;
    bool isActivated = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isActivated)
        {
            return;
        }

        Launch(collision);

        if (cooldown > 0)
        {
            StartCoroutine(actAndDeact(cooldown));
        }
    }

    void Launch(Collision2D col)
    {
        Rigidbody2D rb = col.gameObject.GetComponent<Rigidbody2D>();
 
        if (!rb || rb.velocity.y > 0.1f)
        { return; }

        if (rb.bodyType == RigidbodyType2D.Dynamic)
        {
            //check if col is player
            PlayerController playerCon;
            if (col.gameObject.TryGetComponent(out playerCon))
            {
                playerCon.DisableSlowFall(0.5f);
                playerCon.ForceUnground();
                playerCon.GetComponent<Animator>().SetTrigger(playerCon.jumpParam);
            }

            rb.AddForce(bounceForce * Vector2.up, ForceMode2D.Impulse);
            AudioManager.PlayAudioAtPosition(bounceSFX, transform.position, AudioManager.sfxMixerGroup);
        }
    }

    IEnumerator actAndDeact(float delay)
    {
        isActivated = true;
        anim.SetBool(isActivatedParam, isActivated);

        yield return new WaitForSeconds(delay);

        isActivated = false;
        anim.SetBool(isActivatedParam, isActivated);
    }
}
