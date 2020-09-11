using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("General settings")]
    public int maxHealth;
    int currentHealth;
    public float speed;
    public float flipTime = 1f;
    public float inviTime = 1f;
    bool isInvi = false;
    bool isStunned = false;

    [Header("Jump settings")]
    public float jumpVel;
    public Vector2 groundBoxOffset;
    public Vector2 groundBox;
    public LayerMask groundLayer;

    [Header("Shoot settings")]
    public float shootCooldown = 1f;
    float lastShootTime = 0f;

    [Header("Slap settings")]
    public Vector2 slapBoxOffset;
    public Vector2 slapBoxSize;
    public LayerMask slappableLayer;

    [Header("Animation Param")]
    public string movementParam;
    public string slapParam;

    bool isFacingLeft = false;
    Rigidbody2D rb;
    SpriteRenderer rend;
    Animator anim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rend = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        lastShootTime = -shootCooldown;
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isStunned)
        { return; }

        float movement = Input.GetAxis("Horizontal");
        Move(movement);

        if (movement < 0 && !isFacingLeft)
        {
            Flip();
        }
        else if (movement > 0 && isFacingLeft)
        {
            Flip();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W))
        {
            Jump();
        }

        if (Input.GetMouseButtonDown(1))
        {
            Shoot();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Slap();
        }
    }

    private void Move(float movement)
    {
        Vector2 targetVel = new Vector2(movement * speed, rb.velocity.y);
        rb.velocity = targetVel;

        anim.SetFloat(movementParam, movement);
    }

    private void Jump()
    {
        //check if there is ground
        bool isGrounded;

        if (isFacingLeft)
        {
            isGrounded = Physics2D.OverlapBox((Vector2)transform.position + new Vector2(-groundBoxOffset.x, groundBoxOffset.y), groundBox, 0f, groundLayer);
        }
        else
        {
            isGrounded = Physics2D.OverlapBox((Vector2)transform.position + groundBoxOffset, groundBox, 0f, groundLayer);
        }

        if (isGrounded)
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
    
    private void Slap()
    {
        Collider2D col;

        if (isFacingLeft)
        {
            col = Physics2D.OverlapBox((Vector2)transform.position + new Vector2(-slapBoxOffset.x, slapBoxOffset.y), slapBoxSize, 0f, slappableLayer);
        }
        else
        {
            col = Physics2D.OverlapBox((Vector2)transform.position + slapBoxOffset, slapBoxSize, 0f, slappableLayer);
        }

        if (col)
        {
            Screw screw;
            if (col.TryGetComponent(out screw))
            {
                screw.Unscrew();
            }
        }

        anim.SetTrigger(slapParam);
    }

    private void Flip()
    {
        StopAllCoroutines();

        if (isFacingLeft)
        {
            //face to right
            //lerp angle from 180 to 90
            //change sprite?
            //lerp to 0
            StartCoroutine(Flipping(isFacingLeft, flipTime));
        }
        else
        {
            //face to left
            //lerp angle from 0 to 90
            //change sprite?
            //lerp to 180
            StartCoroutine(Flipping(isFacingLeft, flipTime));
        }

        isFacingLeft = !isFacingLeft;
    }

    private IEnumerator Flipping(bool isFacingLeft, float fullFlipTime)
    {
        float startingAngle = rend.transform.eulerAngles.y;
        float currentAngle = startingAngle;
        float targetAngle;
        float t;

        if (isFacingLeft)
        {
            //flip to right
            targetAngle = 0f;
            t = ((180f - startingAngle) / 180f) * fullFlipTime;

            //Debug.Log("startingAngle: " + startingAngle);
            //Debug.Log("t: " + t);
        }
        else
        {
            //flip to left
            targetAngle = 180f;
            t = (startingAngle / 180f) * fullFlipTime;
        }

        while (currentAngle != targetAngle)
        {
            if (isFacingLeft)
            {
                //flip to the right
                currentAngle =  Mathf.Lerp(180, 0, t / fullFlipTime);
            }
            else
            {
                //flip to the left
                currentAngle = Mathf.Lerp(0, 180, t / fullFlipTime);
            }

            rend.transform.rotation = Quaternion.Euler(0, currentAngle, 0);
            t += Time.deltaTime;
            yield return null;
        }
    }

    public void TakeDamage(int dmg)
    {
        if (isInvi)
        { return; }

        TriggerInvisibility(inviTime);

        Debug.Log("Player took " + dmg + " damage!");
        //play sfx

        currentHealth -= dmg;

        //update health ui

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player is dead");
    }

    public void TriggerInvisibility(float recoverTime)
    {
        if (isInvi)
        { return; }

        StartCoroutine(InviTimer(recoverTime));

        IEnumerator InviTimer(float time)
        {
            isInvi = true;
            yield return new WaitForSeconds(inviTime);
            isInvi = false;
        }
    }

    public void TriggerStun(float recoverTime)
    {
        if (isStunned)
        { return; }

        StartCoroutine(StunTimer(recoverTime));

        IEnumerator StunTimer(float time)
        {
            isStunned = true;
            rb.velocity = new Vector2(0, rb.velocity.y);

            yield return new WaitForSeconds(time);

            isStunned = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + (Vector3)groundBoxOffset, groundBox);

        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(transform.position + (Vector3)slapBoxOffset, slapBoxSize);
    }
}
