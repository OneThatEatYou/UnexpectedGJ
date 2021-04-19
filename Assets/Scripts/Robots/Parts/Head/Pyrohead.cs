using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Pyrohead : Head
{
    [Header("Bomb")]
    public GameObject bombPrefab;
    public Vector2 bombSpawnOffset;
    public Vector2 BombSpawnPos { get { return (Vector2)transform.position + bombSpawnOffset; } }

    [Header("Animation")]
    public Transform mouth;
    public float mouthOpenTime = 0.8f;
    public float bombSpawnDelay = 1;
    public float mouthCloseDelay = 1.5f;
    public AudioClip mouthOpenSFX;
    public AudioClip bombSpawnSFX;


    public override void Action()
    {
        base.Action();

        StartCoroutine(SpawnBombCR());
    }

    IEnumerator SpawnBombCR()
    {
        Sequence seq = DOTween.Sequence();

        AudioManager.PlayAudioAtPosition(mouthOpenSFX, transform.position, AudioManager.battleSfxMixerGroup);

        // open mouth
        seq.Append(mouth.DOLocalMoveY(-0.7f, mouthOpenTime).SetRelative().SetEase(Ease.OutBounce));
        seq.AppendInterval(bombSpawnDelay);
        seq.AppendCallback(SpawnBomb);
        // close mouth
        seq.AppendInterval(mouthCloseDelay);
        seq.Append(mouth.DOLocalMoveY(0.7f, mouthOpenTime).SetRelative());

        yield return new WaitForSeconds(seq.Duration());

        GenerateCooldown(cooldownRange);
    }

    void SpawnBomb()
    {
        if (bombPrefab)
        {
            Instantiate(bombPrefab, BombSpawnPos, Quaternion.identity);
            AudioManager.PlayAudioAtPosition(bombSpawnSFX, transform.position, AudioManager.battleSfxMixerGroup);
        }
        else
        {
            Debug.LogWarning("No bomb prefab assigned");
        }
    }

    public override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(BombSpawnPos, 0.3f);
    }
}
