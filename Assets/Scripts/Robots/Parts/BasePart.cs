using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePart : MonoBehaviour
{
    public int maxHealth;
    int currentHealth;
    public Vector2 cooldownRange;
    float cooldown;
    float lastShootTime = 0f;
    public GameObject deathParticle;

    [System.Serializable]
    public struct ScrewSpawnPos
    {
        public UnscrewDirection unscrewDir;
        public Vector2 screwStartRange;
        public Vector2 screwEndRange;
    }

    public ScrewSpawnPos[] screwSpawnPos;

    private RobotController controller;
    public RobotController Controller
    {
        get { return controller; }
    }

    Rigidbody2D rb;
    bool isDead = false;

    public virtual void Awake()
    {
        controller = GetComponentInParent<RobotController>();
        rb = GetComponent<Rigidbody2D>();
    }

    public virtual void Start()
    {
        currentHealth = maxHealth;
        GenerateCooldown();
    }

    public virtual void Update()
    {
        if (Time.time > lastShootTime + cooldown && !isDead)
        {
            Action();
            GenerateCooldown();
            lastShootTime = Time.time;
        }
    }

    public virtual void Action()
    {
        
    }

    public virtual void TakeDamage(int damage)
    {
        Debug.Log($"{gameObject.name} took {damage} damage");

        currentHealth -= damage;

        if (currentHealth <= 0 && Controller.CanDetach(this))
        {
            Detach();
        }
    }

    public virtual void Detach()
    {
        controller.TakeDamage(maxHealth);
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1;

        Controller.CheckDetachables();

        isDead = true;
    }

    public virtual void Explode()
    {
        //explosion
        Instantiate(deathParticle, rb.worldCenterOfMass, Quaternion.identity);

        //screen shake

        Destroy(gameObject);
    }

    public virtual void GenerateCooldown()
    {
        cooldown = Random.Range(cooldownRange.x, cooldownRange.y);
    }

    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                Explode();
            }
        }
    }

    public virtual void OnDrawGizmosSelected()
    {
        if (screwSpawnPos.Length < 1)
        { return; }

        Gizmos.color = Color.blue;

        for (int i = 0; i < screwSpawnPos.Length; i++)
        {
            Gizmos.DrawLine(screwSpawnPos[i].screwStartRange, screwSpawnPos[i].screwEndRange);
        }
    }
}
