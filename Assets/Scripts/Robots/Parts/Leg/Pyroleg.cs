using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Pyroleg : Leg
{
    [Header("Movement")]
    public float minDistance = 1;
    public float raiseTime = 1; // time taken to move to raiseHeight
    public float raiseHeight;   // how high the robot is to be before moving
    public float moveSpeed = 1;
    public float pauseDur = 1;

    public Vector2 AttackPosition { get { return (Vector2)transform.position + attackOffset; } }
    [Header("Attack")]
    public Vector2 attackOffset;
    public Vector2 attackSize;
    public LayerMask effectLayer;
    public AudioClip landSFX;

    bool hitboxIsActive = false;
    ParticleSystem fireParticle;
    AudioSource aSource;

    public override void Awake()
    {
        base.Awake();

        fireParticle = GetComponentInChildren<ParticleSystem>();
        aSource = GetComponent<AudioSource>();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Action()
    {
        base.Action();

        Vector2 target = GenerateTarget(minDistance);
        StartCoroutine(Move(target));
    }

    IEnumerator Move(Vector2 target)
    {
        Sequence seq = DOTween.Sequence();

        //move up
        seq.Append(Controller.transform.DOMoveY(raiseHeight, raiseTime).SetRelative().SetEase(Ease.InOutQuad));
        seq.InsertCallback(0, ToggleAttack);
        //move horizontal
        float moveTime = Mathf.Abs(target.x - Controller.transform.position.x) / moveSpeed;
        seq.Append(Controller.transform.DOMoveX(target.x, moveTime).SetEase(Ease.InOutSine));
        seq.AppendCallback(ToggleAttack);
        //stop moving for a while after reaching target
        seq.AppendInterval(pauseDur);
        //move down
        seq.Append(Controller.transform.DOMoveY(-raiseHeight, raiseTime / 5).SetRelative().SetEase(Ease.InExpo));
        seq.AppendCallback(PlayLandSound);
        
        yield return new WaitForSeconds(seq.Duration());

        CameraController.GenerateImpulse(Vector2.down, 5, 5, 0, 0.3f, 0.5f);
        GenerateCooldown(cooldownRange);
    }

    void ToggleAttack()
    {
        if (!hitboxIsActive)
        {
            InvokeRepeating("Attack", 0, 0.1f);
            fireParticle.Play();
            aSource.Play();
        }
        else
        {
            CancelInvoke();
            fireParticle.Stop();
            aSource.Stop();
        }

        hitboxIsActive = !hitboxIsActive;
    }

    void Attack()
    {
        PlayerController player;

        Collider2D[] cols = Physics2D.OverlapBoxAll(AttackPosition, attackSize, 0, effectLayer);

        foreach (var col in cols)
        {
            if (col.TryGetComponent(out player))
            {
                player.Die();
            }
        }

        CameraController.GenerateImpulse(Vector2.down, 1, 1, 0, 0.005f, 0.005f);
    }

    void PlayLandSound()
    {
        AudioSource source = AudioManager.PlayAudioAtPosition(landSFX, transform.position, AudioManager.battleSfxMixerGroup);
        source.pitch = 0.8f;
    }

    public override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(AttackPosition, attackSize);
    }
}
