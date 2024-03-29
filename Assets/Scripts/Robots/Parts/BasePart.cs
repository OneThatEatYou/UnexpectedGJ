﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
    public int nutsDropped = 0;

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
    bool isReady = false;
    public bool IsReady { get { return isReady; } }

    protected Animator anim;

    public delegate float OnGenerateCooldown(float input);
    public OnGenerateCooldown onGenerateCooldown;

    public virtual void Awake()
    {
        controller = GetComponentInParent<RobotController>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    public virtual void Start()
    {
        
    }

    public virtual void Update()
    {
        
    }

    public virtual void OnEnable()
    {
        
    }

    public virtual void OnDisable()
    {
        
    }

    public virtual void Action()
    {
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
        StopAllCoroutines();

        BattleManager.Instance.nutCache += nutsDropped;
    }

    public virtual void Explode()
    {
        //explosion
        PlayAudio(explosionSFX, transform.position);
        Instantiate(deathParticle, rb.worldCenterOfMass, Quaternion.identity);
        Controller.parts.Remove(this);
        //remove from specific part list?

        //screen shake
        CameraController.GenerateImpulse(7, 4);

        BattleManager.Instance.DropNuts(transform.position, nutsDropped);

        Destroy(gameObject);
    }

    /// <summary>
    /// this needs to be called manually after every action to set isReady to true after cooldown
    /// </summary>
    public virtual void GenerateCooldown(Vector2 cdRange)
    {
        cooldown = Random.Range(cdRange.x, cdRange.y);

        if (onGenerateCooldown != null)
        {
            cooldown = onGenerateCooldown(cooldown);
        }

        StartCoroutine(StartCooldown(cooldown));
    }

    /// <summary>
    /// Makes isReady to true after waiting for cooldown
    /// </summary>
    IEnumerator StartCooldown(float cd)
    {
        yield return new WaitForSeconds(cd);

        isReady = true;
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

    public void PlayCrouchSeq(float crouchAmount, float crouchTime, float holdTime, float releaseTime)
    {
        Sequence s = DOTween.Sequence();
        //lowers part
        s.Append(transform.DOLocalMoveY(-crouchAmount, crouchTime, false).SetRelative().SetEase(Ease.InQuad));
        //return part back by the same amount after a delay
        s.Insert(crouchTime + holdTime, transform.DOLocalMoveY(crouchAmount, releaseTime).SetRelative());
    }

    public void PlayImpactSeq(Vector2 impact, float duration, float returnDuration)
    {
        Sequence s = DOTween.Sequence();
        s.Append(transform.DOMove(impact, duration).SetRelative().SetEase(Ease.OutExpo));
        s.Append(transform.DOMove(-impact, returnDuration).SetRelative().SetEase(Ease.InOutQuad));
    }

    public void PlayAudio(AudioClip clip, Vector2 pos)
    {
        AudioManager.PlayAudioAtPosition(clip, pos, AudioManager.battleSfxMixerGroup);
    }

    //difficult to be used when there is another leg
    //public void PlayBodyShake(float angle, float variation, float totalDuration, float startDuration, float halfPeriod)
    //{
    //    int flip = 0;
    //    float time = startDuration;
    //    Quaternion start = transform.rotation;

    //    Sequence s = DOTween.Sequence();
    //    s.Append(transform.DOLocalRotate(new Vector3(0, 0, angle + variation), startDuration).SetRelative().SetEase(Ease.OutQuad));

    //    while(time < totalDuration)
    //    {
    //        if (flip == 0)
    //        {
    //            s.Append(transform.DOLocalRotate(new Vector3(0, 0, -variation), halfPeriod).SetRelative().SetEase(Ease.InOutQuad));
    //        }
    //        else
    //        {
    //            s.Append(transform.DOLocalRotate(new Vector3(0, 0, variation), halfPeriod).SetRelative().SetEase(Ease.InOutQuad));
    //        }

    //        flip = 1 - flip;
    //        time += halfPeriod;
    //    }

    //    s.Append(transform.DORotateQuaternion(start, startDuration).SetEase(Ease.OutBounce));
    //}

    public virtual void OnDrawGizmosSelected()
    {
        if (screwSpawnPos == null || screwSpawnPos.Length < 1)
        { return; }

        Gizmos.color = Color.blue;

        for (int i = 0; i < screwSpawnPos.Length; i++)
        {
            Vector3 point = GameManager.RotatePointAroundPivot((Vector2)transform.position + screwSpawnPos[i].screwPos, transform.position, transform.eulerAngles);
            Gizmos.DrawWireSphere(point, 0.3f);
        }
    }
}
