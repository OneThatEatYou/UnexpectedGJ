using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NutDrop : MonoBehaviour
{
    public float rotSpeed;
    public Vector2 lifetimeRange;
    public float fadeTime;
    public SpriteRenderer spriteRend;

    bool isFading = false;
    float lifetime;
    float elapsed = 0;

    private void Start()
    {
        lifetime = Random.Range(lifetimeRange.x, lifetimeRange.y);
    }

    private void Update()
    {
        transform.Rotate(new Vector3(0, rotSpeed * Time.deltaTime, 0));
        elapsed += Time.deltaTime;

        if (elapsed > lifetime && !isFading)
        {
            StartCoroutine(FadeOut()); 
        }
    }

    IEnumerator FadeOut()
    {
        float t = 0;
        float alpha = 1;
        isFading = true;

        while (spriteRend.color.a != 0)
        {
            alpha = Mathf.Lerp(1, 0, t / fadeTime);
            t += Time.deltaTime;
            spriteRend.color = new Color(spriteRend.color.r, spriteRend.color.g, spriteRend.color.b, alpha);

            yield return null;
        }

        Destroy(gameObject);
    }
}
