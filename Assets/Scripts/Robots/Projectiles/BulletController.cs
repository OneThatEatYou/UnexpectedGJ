using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public string ignoreTriggerTag = "IgnoreTrigger";

    [Space]

    public float moveSpeed;
    public GameObject deathParticle;
    public LayerMask effectLayer;

    protected Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
        rb.MovePosition(rb.position - (Vector2)(transform.up * moveSpeed));
    }

    public virtual void OnCollide(Collision2D collision)
    {
        Instantiate(deathParticle, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public virtual void OnTrigger(Collider2D collision)
    {
        Instantiate(deathParticle, transform.position, Quaternion.identity);
    }
}
