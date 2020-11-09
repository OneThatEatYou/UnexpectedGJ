using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// handles how modules behave
/// </summary>
public class BasePart : MonoBehaviour
{
    [Header("Basic Settings")]
    public int maxHealth;
    int currentHealth;
    public int CurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = value; }
    }

    public Vector2 cooldownRange;
    public Vector2 initialCooldownRange = new Vector2(1f, 3f);
    float cooldown;
    public float Cooldown { get { return cooldown; } }
    float initialCooldown;
    public float InitialCooldown { get { return initialCooldown; } }

    [Space]

    public GameObject deathParticle;
    public AudioClip explosionSFX;

    [System.Serializable]
    public struct ScrewSpawnPos
    {
        public UnscrewDirection unscrewDir;
        public Vector2 screwPos;
    }

    [Space]

    public ScrewSpawnPos[] screwSpawnPos;

    private RobotController controller;
    public RobotController Controller { get { return controller; } }

    protected Rigidbody2D rb;
    bool isDisabled = false;
    public bool IsDisabled { get { return isDisabled; } }
    bool isReady = true;
    public bool IsReady { get { return isReady; } }

    bool isAnimating = false;

    public virtual void Awake()
    {
        controller = GetComponentInParent<RobotController>();
        rb = GetComponent<Rigidbody2D>();
    }

    public virtual void Start()
    {
        initialCooldown = Random.Range(initialCooldownRange.x, initialCooldownRange.y);
    }

    public virtual void Update()
    {
        
    }

    public virtual void Action()
    {
        GenerateCooldown();
        isReady = false;
    }

    public virtual void TakeDamage(int damage)
    {
        //Debug.Log($"{gameObject.name} took {damage} damage");
    }

    public virtual void Detach()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1;
        transform.parent = null;

        isDisabled = true;
    }

    public virtual void Explode()
    {
        //explosion
        AudioManager.PlayAudioAtPosition(explosionSFX, transform.position, AudioManager.sfxMixerGroup);
        Instantiate(deathParticle, rb.worldCenterOfMass, Quaternion.identity);

        Controller.parts.Remove(this);

        //remove from specific part list?

        //screen shake
        CameraController.GenerateImpulse(7, 4);

        Destroy(gameObject);
    }

    public virtual void GenerateCooldown()
    {
        cooldown = Random.Range(cooldownRange.x, cooldownRange.y);
    }

    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDisabled)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                Explode();
            }
        }
    }

    public IEnumerator StartCooldown(float multiplier)
    {
        yield return new WaitForSeconds(Cooldown * multiplier);

        isReady = true;
    }

    public void Crouch(float crouchAmount, float crouchTime, float holdTime, float releaseTime)
    {
        if (!isAnimating)
        {
            StartCoroutine(CrouchCrt());
        }

        IEnumerator CrouchCrt()
        {
            isAnimating = true;

            float t = 0;
            float initialLocalPos = transform.localPosition.y;
            float finalLocalPos = initialLocalPos + crouchAmount;

            //lower position of body
            while (t < crouchTime)
            {
                t += Time.deltaTime;
                t = Mathf.Clamp(t, 0, crouchTime);
                //interpolate to crouch amount
                float tmp = Mathf.Sin((Mathf.PI / 2) * t / crouchTime);
                transform.localPosition = new Vector2(transform.localPosition.x, Mathf.Lerp(initialLocalPos, finalLocalPos, tmp));

                yield return null;
            }

            yield return new WaitForSeconds(holdTime);

            t = 0;
            while (t < releaseTime)
            {
                t += Time.deltaTime;

                //interpolate to original position
                transform.localPosition = new Vector2(transform.localPosition.x, Mathf.Lerp(finalLocalPos, initialLocalPos, t / releaseTime));

                yield return null;
            }

            isAnimating = false;
        }
    }

    public virtual void OnDrawGizmosSelected()
    {
        if (screwSpawnPos.Length < 1)
        { return; }

        Gizmos.color = Color.blue;

        for (int i = 0; i < screwSpawnPos.Length; i++)
        {
            Vector3 point = GameManager.RotatePointAroundPivot((Vector2)transform.position + screwSpawnPos[i].screwPos, transform.position, transform.eulerAngles);
            Gizmos.DrawWireSphere(point, 0.3f);
        }
    }
}
