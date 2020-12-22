using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("General settings")]
    public float speed;
    public float maxSpeed;
    float inputWeight = 1f;

    [Space]
    public float flipTime = 1f;
    public float inviTime = 1f;
    bool isInvi = false;
    bool isStunned = false;
    public AudioClip hurtSFX;

    [Header("Jump settings")]
    public float jumpVel;
    public Vector2 groundBoxOffset;
    public Vector2 groundBox;
    public float groundedTime = 0.1f;
    float curGroundedTime;
    public LayerMask groundLayer;
    public GameObject dustAnimation;
    public AudioClip jumpSFX;
    [Space]
    public float fallMultiplier = 3f;
    public float lowJumpMultiplier = 2f;

    [Header("Shoot settings")]
    public float shootCooldown = 1f;
    float lastShootTime = 0f;
    public float slowIn;
    public float slowStay;
    public float slowOut;
    public float slowTimeScale;
    public AudioClip shootSFX;

    [Header("Slap settings")]
    public Vector2 slapBoxOffset;
    public Vector2 slapBoxSize;
    public LayerMask slappableLayer;
    public AudioClip slapSFX;

    [Header("Animation Param")]
    public string movementParam;
    public string slapParam;
    public string invicibleParam = "isInvicible";
    public string jumpParam = "Jump";
    public string landedParam = "Landed";

    [Header("Death")]
    public GameObject deathParticle;
    public AudioClip deathSFX;

    bool isFacingLeft = false;
    bool canSlowFall = true;
    Coroutine flippingCr;
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
    }

    void Update()
    {
        if (GameManager.isPaused)
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

        CheckGrounded();
        ControlJumpHeight();

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
        rb.AddForce(movement * speed * Vector2.right * inputWeight);
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), rb.velocity.y);

        anim.SetFloat(movementParam, rb.velocity.x);
    }

    private void CheckGrounded()
    {
        //dont check for grounded when player is moving upwards
        if (rb.velocity.y > 0.1f)
        {
            curGroundedTime -= Time.deltaTime;
            return; 
        }

        //check if there is ground
        Collider2D groundCol;

        if (isFacingLeft)
        {
            groundCol = Physics2D.OverlapBox((Vector2)transform.position + new Vector2(-groundBoxOffset.x, groundBoxOffset.y), groundBox, 0f, groundLayer);
        }
        else
        {
            groundCol = Physics2D.OverlapBox((Vector2)transform.position + groundBoxOffset, groundBox, 0f, groundLayer);
        }

        if (groundCol)
        {
            if (curGroundedTime < -0.1f)
            {
                //landed
                anim.SetTrigger(landedParam);

                GameObject dust = Instantiate(dustAnimation, (Vector2)transform.position + groundBoxOffset, Quaternion.identity);
                Destroy(dust, 1);
            }
            curGroundedTime = groundedTime;
        }
        else
        {
            curGroundedTime -= Time.deltaTime;
        }
    }

    public void ForceUnground()
    {
        curGroundedTime -= groundedTime;
    }

    private void Jump()
    {
        //can jump when recently grounded
        if (curGroundedTime > 0)
        {
            rb.AddForce(jumpVel * Vector2.up, ForceMode2D.Impulse);

            AudioManager.PlayAudioAtPosition(jumpSFX, transform.position, AudioManager.sfxMixerGroup);
            anim.SetTrigger(jumpParam);
            curGroundedTime = 0;
        }
    }

    private void ControlJumpHeight()
    {
        if (rb.velocity.y < 0)
        {
            //add to downwards velocity
            rb.velocity += Vector2.up * Physics2D.gravity * rb.gravityScale * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0)
        {
            //player is moving upwards
            if ((!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.W)) && canSlowFall)
            {
                //add to downwards velocity
                rb.velocity += Vector2.up * Physics2D.gravity * rb.gravityScale * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
        }
    }

    private void Shoot()
    {
        if (Time.time - lastShootTime < shootCooldown)
        { return; }

        lastShootTime = Time.time;
        CameraAimController.instance.Shoot();
        BattleManager.Instance.PlaySlowMo(slowIn, slowStay, slowOut, slowTimeScale);
        AudioManager.PlayAudioAtPosition(shootSFX, transform.position, AudioManager.sfxMixerGroup);

        //StartCoroutine(DisplayShootCooldown());

        //IEnumerator DisplayShootCooldown()
        //{
        //    float t = 0;
        //    cooldownImage.fillAmount = 1;

        //    while (cooldownImage.fillAmount != 0)
        //    {
        //        cooldownImage.fillAmount = Mathf.Lerp(1, 0, t / shootCooldown);
        //        t += Time.deltaTime;

        //        yield return null;
        //    }
        //}
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
        AudioManager.PlayAudioAtPosition(slapSFX, transform.position, AudioManager.sfxMixerGroup);
    }

    private void Flip()
    {
        if (flippingCr != null)
        {
            StopCoroutine(flippingCr);
        }

        flippingCr = StartCoroutine(Flipping(isFacingLeft, flipTime));

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

    public void Die()
    {
        //dont die if invisible
        if (isInvi)
        { return; }

        BattleManager.Instance.TakeDamage();

        Instantiate(deathParticle, transform.position, Quaternion.identity);
        AudioManager.PlayAudioAtPosition(deathSFX, transform.position, AudioManager.sfxMixerGroup);

        Destroy(gameObject);
    }

    public void TriggerInvisibility(float recoverTime)
    {
        if (isInvi)
        { return; }

        StartCoroutine(InviTimer(recoverTime));

        IEnumerator InviTimer(float time)
        {
            isInvi = true;
            anim.SetBool(invicibleParam, isInvi);
            yield return new WaitForSeconds(inviTime);
            isInvi = false;
            anim.SetBool(invicibleParam, isInvi);
        }
    }

    public void TriggerStun(float noInputTime, float recoverTime)
    {
        if (isStunned)
        { return; }

        StartCoroutine(StunTimer(recoverTime));

        IEnumerator StunTimer(float time)
        {
            isStunned = true;

            DisableSlowFall(time + noInputTime);

            rb.velocity = new Vector2(0, rb.velocity.y);
            inputWeight = 0;

            yield return new WaitForSeconds(noInputTime);

            float t = 0;
            while (inputWeight != 1)
            {
                inputWeight = Mathf.Lerp(0, 1, t / time);
                t += Time.deltaTime;
                //Debug.Log(inputWeight);
                yield return null;
            }

            isStunned = false;
            //Debug.Log("No longer stunned");
        }
    }

    Coroutine slowFallCR;
    public void DisableSlowFall(float time)
    {
        if (!canSlowFall)
        {
            StopCoroutine(slowFallCR);
        }

        slowFallCR = StartCoroutine(Trigger());

        IEnumerator Trigger()
        {
            canSlowFall = false;
            yield return new WaitForSeconds(time);
            canSlowFall = true;
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
