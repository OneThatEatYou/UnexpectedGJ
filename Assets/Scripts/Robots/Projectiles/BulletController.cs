using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
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
        OnCollide();
    }

    public virtual void Move()
    {
        rb.MovePosition(rb.position - (Vector2)(transform.up * moveSpeed));
    }

    public virtual void OnCollide()
    {
        Instantiate(deathParticle, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
