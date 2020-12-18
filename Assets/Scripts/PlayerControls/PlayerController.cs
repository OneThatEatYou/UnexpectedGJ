using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("General settings")]
    public int maxHealth;
    int currentHealth;
    [Space]
    public float speed;
    public float maxSpeed;
    
    float inputWeight = 1f;
    float envWeight;
    [Space]
    public float flipTime = 1f;
    public float inviTime = 1f;
    bool isInvi = false;
    bool isStunned = false;
    public RectTransform hpPanel;
    Image[] healthImages;           //left most health has index 0
    Animator[] healthAnims;
    public string healthBeatParam = "BeatRate";
    public float maxBeatRate = 1f;
    public Sprite fullHealth;
    public Sprite emptyHealth;
    public AudioClip hurtSFX;

    public int CurrentHealth
    {
        get { return currentHealth; }

        set
        {
            currentHealth = value;
            UpdateHealth();
        }
    }

    [Header("Jump settings")]
    public float jumpVel;
    public Vector2 groundBoxOffset;
    public Vector2 groundBox;
    public float groundedTime = 0.1f;
    float curGroundedTime;
    public LayerMask groundLayer;
    public AudioClip jumpSFX;
    public GameObject dustAnimation;

    [Header("Shoot settings")]
    public float shootCooldown = 1f;
    float lastShootTime = 0f;
    public float slowIn;
    public float slowStay;
    public float slowOut;
    public float slowTimeScale;
    public Image cooldownImage;
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
    public GameObject deathCanvas;
    public GameObject deathParticle;
    public AudioClip deathSFX;

    bool isFacingLeft = false;
    bool isDead = false;
    Coroutine flippingCr;
    Rigidbody2D rb;
    SpriteRenderer rend;
    Animator anim;
    BetterJump betterJumpScript;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rend = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponent<Animator>();
        betterJumpScript = GetComponent<BetterJump>();
    }

    void Start()
    {
        lastShootTime = -shootCooldown;
        currentHealth = maxHealth;

        RegisterHealthPanel();
    }

    void Update()
    {
        if (isDead)
        { return; }

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
        rb.AddForce(movement * speed * Vector2.right);
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

    private void Shoot()
    {
        if (Time.time - lastShootTime < shootCooldown)
        { return; }

        lastShootTime = Time.time;
        CameraAimController.instance.Shoot();
        StartCoroutine(StartSlowMo());

        AudioManager.PlayAudioAtPosition(shootSFX, transform.position, AudioManager.sfxMixerGroup);

        StartCoroutine(DisplayShootCooldown());

        IEnumerator DisplayShootCooldown()
        {
            float t = 0;
            cooldownImage.fillAmount = 1;

            while (cooldownImage.fillAmount != 0)
            {
                cooldownImage.fillAmount = Mathf.Lerp(1, 0, t / shootCooldown);
                t += Time.deltaTime;

                yield return null;
            }
        }

        IEnumerator StartSlowMo()
        {
            float t = 0;
            float tScale = Time.timeScale;

            while (t < slowIn)
            {
                //decrease time scale
                float p = Mathf.Sin(Mathf.PI/2 * t / slowIn);
                Time.timeScale = Mathf.Lerp(tScale, slowTimeScale, t);
                t += Time.unscaledDeltaTime;

                yield return null;
            }
            Time.timeScale = slowTimeScale;

            yield return new WaitForSecondsRealtime(slowStay);

            t = 0;
            while (t < slowOut)
            {
                //increase time scale
                float p = Mathf.Cos(Mathf.PI / 2 * t / slowOut);
                Time.timeScale = Mathf.Lerp(slowTimeScale, 1, t);
                t += Time.unscaledDeltaTime;

                yield return null;
            }
            Time.timeScale = 1;
        }
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

    public void TakeDamage(int dmg)
    {
        if (isInvi)
        { return; }

        TriggerInvisibility(inviTime);

        //Debug.Log("Player took " + dmg + " damage!");

        CurrentHealth -= dmg;

        //update health ui

        if (CurrentHealth <= 0)
        {
            Die();
        }
        else
        {
            AudioManager.PlayAudioAtPosition(hurtSFX, transform.position, AudioManager.sfxMixerGroup);
        }
    }

    void Die()
    {
        Debug.Log("Player is dead");

        isDead = true;

        deathCanvas.SetActive(true);
        GameManager.Instance.canRestart = true;
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

            DisableBetterJump(time + noInputTime);

            rb.velocity = new Vector2(0, rb.velocity.y);
            inputWeight = 0;
            envWeight = 1;

            yield return new WaitForSeconds(noInputTime);

            float t = 0;
            while (inputWeight != 1)
            {
                inputWeight = Mathf.Lerp(0, 1, t / time);
                envWeight = 1 - inputWeight;
                t += Time.deltaTime;
                //Debug.Log(inputWeight);
                yield return null;
            }

            isStunned = false;
            //Debug.Log("No longer stunned");
        }
    }

    void RegisterHealthPanel()
    {
        healthImages = new Image[maxHealth];
        healthAnims = new Animator[maxHealth];

        for (int i = 0; i < maxHealth; i++)
        {
            Image img = hpPanel.GetChild(i).GetComponent<Image>();

            if (!img)
            { Debug.LogWarning("Health image not found"); }

            healthImages[i] = img;

            Animator anim = hpPanel.GetChild(i).GetComponent<Animator>();
            if (!anim)
            { Debug.LogWarning("Health animator not found"); }
            healthAnims[i] = anim;
        }
    }

    void UpdateHealth()
    {
        float beatRate = (1f - ((float)CurrentHealth / maxHealth)) * maxBeatRate;
        
        //set fullHealth
        for (int i = 0; i < CurrentHealth; i++)
        {
            healthImages[i].sprite = fullHealth;
            healthAnims[i].SetFloat(healthBeatParam, 1f + beatRate);
        }

        //set emptyHealth
        for (int i = CurrentHealth; i < maxHealth; i++)
        {
            healthImages[i].sprite = emptyHealth;
            healthAnims[i].enabled = false;
        }
    }

    Coroutine disablingBetterJump;
    public void DisableBetterJump(float time)
    {
        if (betterJumpScript.isDisabled)
        {
            StopCoroutine(disablingBetterJump);
        }

        disablingBetterJump = StartCoroutine(Trigger());

        IEnumerator Trigger()
        {
            betterJumpScript.isDisabled = true;
            yield return new WaitForSeconds(time);
            betterJumpScript.isDisabled = false;
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
