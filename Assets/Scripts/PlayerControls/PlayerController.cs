using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("General settings")]
    public float maxSpeed;
    public float maxAccel;

    [Space]
    public float flipTime = 1f;
    bool isInvi = false;
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

    [Header("Item")]
    public GameObject itemObj;
    ItemController equippedItem;
    public Animator itemAnim;
    float lastUseTime = 0f;

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
    float moveWeight = 1;   //multiplied with target velocity
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
        if (GameManager.Instance.inventoryManager.equippedItemObj)
        {
            itemObj = GameManager.Instance.inventoryManager.equippedItemObj;
        }

        if (itemObj)
        {
            equippedItem = Instantiate(itemObj, transform).GetComponent<ItemController>();

            lastUseTime = -equippedItem.useCooldown;

            //setup item animation
            AnimatorOverrideController aoc = new AnimatorOverrideController(itemAnim.runtimeAnimatorController);
            aoc["player_idle"] = equippedItem.idleClip;
            aoc["player_walk"] = equippedItem.walkClip;
            aoc["player_slap"] = equippedItem.slapClip;
            itemAnim.runtimeAnimatorController = aoc;
        }
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
            UseItem();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Slap();
        }


        if (moveWeight != 1)
        {
            if (rb.velocity.x < 0.05f && rb.velocity.x > -0.05f)
            {
                //horizontal velocity close to 0
                ForceMove();
            }
        }
    }

    private void Move(float movement)
    {
        float targetVelocityX = movement * maxSpeed;
        float velocityChangeX = targetVelocityX - rb.velocity.x;
        velocityChangeX = Mathf.Clamp(velocityChangeX, -maxAccel, maxAccel);
        velocityChangeX *= moveWeight;
        rb.AddForce(new Vector2 (velocityChangeX, 0), ForceMode2D.Impulse);

        anim.SetFloat(movementParam, movement);
        itemAnim.SetFloat(movementParam, movement);
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
                itemAnim.SetTrigger(landedParam);

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
            rb.velocity = new Vector2(rb.velocity.x, jumpVel);

            AudioManager.PlayAudioAtPosition(jumpSFX, transform.position, AudioManager.battleSfxMixerGroup);
            anim.SetTrigger(jumpParam);
            itemAnim.SetTrigger(jumpParam);
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

    private void UseItem()
    {
        if (!equippedItem || Time.time - lastUseTime < equippedItem.useCooldown)
        { return; }

        lastUseTime = Time.time;
        equippedItem.UseItem();

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

        itemAnim.SetTrigger(slapParam);
        anim.SetTrigger(slapParam);
        AudioManager.PlayAudioAtPosition(slapSFX, transform.position, AudioManager.battleSfxMixerGroup);
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
            itemAnim.transform.rotation = Quaternion.Euler(0, currentAngle, 0);
            t += Time.deltaTime;
            yield return null;
        }
    }

    public void Die(bool ignoreInvi = false)
    {
        //dont die if invisible
        if (isInvi && !ignoreInvi)
        {
            return; 
        }

        BattleManager.Instance.TakeDamage();

        Instantiate(deathParticle, transform.position, Quaternion.identity);
        AudioManager.PlayAudioAtPosition(deathSFX, transform.position, AudioManager.battleSfxMixerGroup);

        Destroy(gameObject);
    }

    public void Squash(Collider2D col)
    {
        Die(true);
        Debug.Log($"Player is squashed by {col.name}");
    }

    public void TriggerInvisibility(float recoverTime)
    {
        if (isInvi)
        { return; }

        StartCoroutine(InviTimer(recoverTime));
    }

    IEnumerator InviTimer(float time)
    {
        isInvi = true;
        anim.SetBool(invicibleParam, isInvi);
        yield return new WaitForSeconds(time);
        isInvi = false;
        anim.SetBool(invicibleParam, isInvi);
    }

    Coroutine movementLockCR;
    public void TriggerMovementLock(float lockTime)
    {
        if (moveWeight != 1)
        {
            StopCoroutine(movementLockCR);
        }

        movementLockCR = StartCoroutine(StopTimer(lockTime));
    }

    IEnumerator StopTimer(float time)
    {
        float t = 0;

        moveWeight = 0;
        yield return new WaitForSeconds(time);

        while (t < (time * 0.5f))
        {
            t += Time.deltaTime;
            moveWeight = Mathf.Lerp(0, 1, t / (time * 0.5f));
            yield return null;
        }
    }

    //forces move weight to 1 and stop movement lock coroutine
    public void ForceMove()
    {
        Debug.Log("Forced move");

        StopCoroutine(movementLockCR);
        moveWeight = 1;
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
