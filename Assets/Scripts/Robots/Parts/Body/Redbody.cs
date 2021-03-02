using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Redbody : Body
{
    [Header("Blast Settings")]
    public Vector2 centerOffset;
    public float knockbackHRad;
    public float knockbackSRad;
    public float knockbackForce;
    public SpriteRenderer blastRend;
    Material blastMat;
    public float minOuterRad;
    public float chargeDur;
    public float holdDur;
    public int numOfLoops;
    public float blastWait = 0.4f;
    public float blastDur;
    [ColorUsage(true, true)]
    public Color blastColour;

    public override void Awake()
    {
        base.Awake();

        blastMat = blastRend.material;
    }

    public override void Action()
    {
        base.Action();
        //StartCoroutine(StartKnockback());
        StartCoroutine(Knockback());
    }

    IEnumerator Knockback()
    {
        AnimateBlast();
        yield return new WaitForSeconds(chargeDur + holdDur + blastWait);

        Collider2D[] cols = Physics2D.OverlapCircleAll((Vector2)transform.position + centerOffset, knockbackSRad);

        //list to store rbs that are knocked back so force is not applied to rb with multiple colliders
        List<Rigidbody2D> rbs = new List<Rigidbody2D>();

        foreach (var col in cols)
        {
            Rigidbody2D rb;
            if (col.TryGetComponent(out rb) && rb.bodyType == RigidbodyType2D.Dynamic && !rbs.Contains(rb))
            {
                rbs.Add(rb);

                Vector2 dis = rb.position - (Vector2)transform.position;
                Vector2 dir = dis.normalized;
                //rb.AddForce(dir * knockbackForce);
                Vector2 force;

                if (dis.magnitude < knockbackHRad)
                {
                    force = knockbackForce * dir;
                }
                else
                {
                    force = Mathf.Lerp(knockbackForce, knockbackForce * 0.5f, Mathf.Sin(Mathf.PI / 2 * ((dis.magnitude - knockbackHRad) / (knockbackSRad - knockbackHRad)))) * dir;
                }
                rb.AddForce(force, ForceMode2D.Impulse);
                //Debug.Log($"Knocked back {rb.name} with force: {force}");
            }
        }
        GenerateCooldown(cooldownRange);
    }

    void AnimateBlast()
    {
        //charge and fade in
        //hold
        //blast and fade out

        //reset
        blastMat.SetFloat("_OuterRadius", knockbackSRad);
        blastMat.SetFloat("_InnerRadius", 1.8f);
        blastMat.SetColor("_Color", new Vector4(1, 1, 1, 0));

        //float holdLoopDur = 0.5f;
        //int holdLoops = holdDur / holdLoopDur;

        Sequence s = DOTween.Sequence();
        s.Append(blastMat.DOFloat(minOuterRad, "_OuterRadius", chargeDur).SetEase(Ease.OutSine));
        s.Insert(0, blastMat.DOColor(blastColour, "_Color", chargeDur * 0.9f).SetEase(Ease.InSine));
        s.Insert(0, blastMat.DOFloat(0, "_InnerRadius", chargeDur*0.7f).SetEase(Ease.InQuart));
        s.Append(blastMat.DOFloat(minOuterRad * 1.08f, "_OuterRadius", holdDur / numOfLoops).SetLoops(numOfLoops, LoopType.Yoyo).SetEase(Ease.OutSine));
        s.AppendInterval(blastWait);
        s.Append(blastMat.DOFloat(knockbackSRad, "_OuterRadius", blastDur).SetEase(Ease.OutCubic));
        s.Insert(chargeDur + holdDur + blastWait + blastDur * 0.1f, blastMat.DOColor(Color.clear, "_Color", blastDur * 0.9f));
        s.Insert(chargeDur + holdDur + blastWait + blastDur, blastMat.DOFloat(2, "_InnerRadius", blastDur).SetEase(Ease.OutCubic));
    }

    public override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + centerOffset, knockbackHRad);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere((Vector2)transform.position + centerOffset, knockbackSRad);
    }
}
