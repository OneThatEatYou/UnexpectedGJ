using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float jumpVel;

    [Space(2)]

    public Vector2 offset;
    public Vector2 groundBox;
    public LayerMask groundLayer;

    [Space(2)]

    public float shootCooldown = 1f;
    float lastShootTime = 0f;

    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        lastShootTime = -shootCooldown;
    }

    void Update()
    {
        float movement = Input.GetAxis("Horizontal");
        Move(movement);

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W))
        {
            Jump();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
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

    private void Shoot()
    {
        if (Time.time - lastShootTime < shootCooldown)
        { return; }

        lastShootTime = Time.time;
        CameraAimController.instance.Shoot();
    }    

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + (Vector3)offset, groundBox);
    }
}
