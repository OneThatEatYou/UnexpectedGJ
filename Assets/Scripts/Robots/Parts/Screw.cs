using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screw : MonoBehaviour
{
    [HideInInspector] public PartController controller;

    [Tooltip("Time taken to move screw to target pos")]
    public float unscrewTime;
    public float threadLength;
    public UnscrewDirection unscrewDir;
    float startXPos;

    public int maxHealth;
    int currentHealth;

    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        startXPos = transform.position.x;
    }

    public void Unscrew()
    {
        if (currentHealth <= 0)
        { return; }

        Debug.Log($"Unscrewing {name}.");

        currentHealth--;

        //animation

        //movement
        StopAllCoroutines();
        StartCoroutine(MoveScrew());

        if (currentHealth <= 0)
        {
            Detach();
        }
    }

    IEnumerator MoveScrew()
    {
        float damagePercent = 1 - (float)currentHealth / maxHealth;
        float moveDistance = damagePercent * threadLength;
        float targetPos;
        if (unscrewDir == UnscrewDirection.Left)
        {
            targetPos = startXPos - moveDistance;
        }
        else
        {
            targetPos = startXPos + moveDistance;
        }

        Vector2 startPos = transform.position;
        Vector2 currentPos = startPos;
        float t = 0;

        //Debug.Log($"healthPrecent: {damagePercent}, startPos: {startPos}, targetPos: {targetPos}");
        //Debug.Log($"transform.position.x: {transform.position.x}, startXPos: {startXPos}, moveDistance: {moveDistance}");
        //lerp to target
        while (Mathf.Abs(transform.position.x - startXPos) <= moveDistance)
        {
            currentPos.x = Mathf.Lerp(startPos.x, targetPos, Mathf.Sin((Mathf.PI / 2) * (t / unscrewTime)));
            transform.position = currentPos;

            t += Time.deltaTime;
            t = Mathf.Clamp(t, 0, unscrewTime);
            yield return null;
        }

        Debug.Log("Finished moving screw");
    }

    private void Detach()
    {
        Debug.Log("Detached a screw");

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        if (controller)
        {
            controller.TakeDamage(maxHealth);
        }
        else
        {
            Debug.LogWarning($"Controller for {name} not found.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(threadLength, 0));
    }

    public enum UnscrewDirection
    {
        Left,
        Right
    }
}
