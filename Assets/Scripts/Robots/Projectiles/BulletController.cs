using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class BulletController : MonoBehaviour
{
    public string ignoreTriggerTag = "IgnoreTrigger";

    [Space]

    public float moveSpeed;
    public GameObject deathParticle;
    public LayerMask effectLayer;
    public AudioClip collideTriggerSFX;

    public Transform target;
    protected Rigidbody2D rb;
    protected Animator anim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        OnCollide(collision);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(ignoreTriggerTag))
        { return; }

        OnTrigger(collision);
    }

    public virtual void Move()
    {
        rb.MovePosition(rb.position - (Vector2)(transform.up * moveSpeed * Time.fixedDeltaTime));
    }

    public virtual void OnCollide(Collision2D collision)
    {
        AudioManager.PlayAudioAtPosition(collideTriggerSFX, transform.position, AudioManager.sfxMixerGroup);
        Instantiate(deathParticle, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public virtual void OnTrigger(Collider2D collision)
    {
        AudioManager.PlayAudioAtPosition(collideTriggerSFX, transform.position, AudioManager.sfxMixerGroup);
        Instantiate(deathParticle, transform.position, Quaternion.identity);
    }
}
