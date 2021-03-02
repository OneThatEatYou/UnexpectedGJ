using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class BulletController : MonoBehaviour
{
    public string ignoreTriggerTag = "IgnoreTrigger";

    [Space]

    public float moveSpeed;
    [HideInInspector] public Vector2 dir;
    public GameObject deathParticle;
    [Tooltip("Input more than 0 to destroy the particle after the input number of seconds.")]
    public float manualDestroyParticle = 0;
    public LayerMask effectLayer;
    public AudioClip collideTriggerSFX;

    [HideInInspector] public Transform target;
    protected Rigidbody2D rb;
    protected Animator anim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    public virtual void Start()
    {

    }

    public virtual void FixedUpdate()
    {
        Move();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        OnCollision(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(ignoreTriggerTag))
        { return; }

        OnCollision(collision);
    }

    public virtual void Move()
    {
        //default movement based on object's rotation
        rb.MovePosition(rb.position - (Vector2)(transform.up * moveSpeed * Time.fixedDeltaTime));
    }

    public virtual void OnCollision(Collider2D collision)
    {
        //play audio and spawn particle
        AudioManager.PlayAudioAtPosition(collideTriggerSFX, transform.position, AudioManager.battleSfxMixerGroup);
        GameObject obj = Instantiate(deathParticle, transform.position, Quaternion.identity);

        if (manualDestroyParticle > 0)
        {
            Destroy(obj, manualDestroyParticle);
        }
    }
}
