using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float jumpVel;

    [Space]

    public Vector2 offset;
    public Vector2 groundBox;
    public LayerMask groundLayer;

    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        float movement = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W))
        {
            Jump();
        }

        Move(movement);
    }

    private void Move(float movement)
    {
        Vector2 targetVel = new Vector2(movement * speed, rb.velocity.y);
        rb.velocity = targetVel;
    }

    private void Jump()
    {
        //check if there is ground
        if (Physics2D.OverlapBox((Vector2)transform.position + offset, groundBox, 0f, groundLayer))
        {
            Vector2 targetVel = new Vector2(rb.velocity.x, jumpVel);
            rb.velocity = targetVel;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + (Vector3)offset, groundBox);
    }
}
