using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// handles how modules behave
/// </summary>
public class BasePart : MonoBehaviour
{
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
    float lastShootTime = 0f;
    public float LastShootTime { get { return lastShootTime; } }
    float initialCooldown;
    public float InitialCooldown { get { return initialCooldown; } }

    public GameObject deathParticle;
    public AudioClip explosionSFX;

    [System.Serializable]
    public struct ScrewSpawnPos
    {
        public UnscrewDirection unscrewDir;
        public Vector2 screwPos;
    }

    public ScrewSpawnPos[] screwSpawnPos;

    private RobotController controller;
    public RobotController Controller
    {
        get { return controller; }
    }

    protected Rigidbody2D rb;
    bool isDisabled = false;
    public bool IsDisabled { get { return isDisabled; } }

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
        lastShootTime = Time.time;
    }

    public virtual void TakeDamage(int damage)
    {
        //Debug.Log($"{gameObject.name} took {damage} damage");
    }

    public virtual void Detach()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1;

        isDisabled = true;
    }

    public virtual void Explode()
    {
        //explosion
        AudioManager.PlayAudioAtPosition(explosionSFX, transform.position, AudioManager.sfxMixerGroup);
        Instantiate(deathParticle, rb.worldCenterOfMass, Quaternion.identity);

        Controller.parts.Remove(this);

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

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }

    public virtual void OnDrawGizmosSelected()
    {
        if (screwSpawnPos.Length < 1)
        { return; }

        Gizmos.color = Color.blue;

        for (int i = 0; i < screwSpawnPos.Length; i++)
        {
            Vector3 point = RotatePointAroundPivot((Vector2)transform.position + screwSpawnPos[i].screwPos, transform.position, transform.eulerAngles);
            Gizmos.DrawWireSphere(point, 0.3f);
        }
    }
}
