using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screw : MonoBehaviour
{
    [HideInInspector] public BasePart connectedPart;
    public bool isFake = false;

    [Tooltip("Time taken to move screw to target pos")]
    public float unscrewTime;
    public float threadLength;
    public UnscrewDirection unscrewDir;
    [HideInInspector] public float startXPos;
    public string unscrewedLayer;

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

        if (!connectedPart)
        {
            startXPos = transform.localPosition.x;
        }
    }

    public void Unscrew()
    {
        if (currentHealth <= 0)
        { return; }

        //Debug.Log($"Unscrewing {name}.");

        currentHealth--;

        //animation

        //movement
        StopAllCoroutines();
        StartCoroutine(MoveScrew());
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

        Vector2 startPos = transform.localPosition;
        Vector2 currentPos = startPos;
        float t = 0;

        //Debug.Log($"healthPrecent: {damagePercent}, startPos: {startPos}, targetPos: {targetPos}");
        //Debug.Log($"transform.position.x: {transform.position.x}, startXPos: {startXPos}, moveDistance: {moveDistance}");
        //lerp to target
        while (transform.localPosition.x != targetPos)
        {
            currentPos.x = Mathf.Lerp(startPos.x, targetPos, Mathf.Sin((Mathf.PI / 2) * (t / unscrewTime)));
            transform.localPosition = currentPos;

            t += Time.deltaTime;
            t = Mathf.Clamp(t, 0, unscrewTime);
            yield return null;
        }

        if (currentHealth <= 0)
        {
            Detach();
        }

        //Debug.Log("Finished moving screw");
    }

    private void Detach()
    {
        //Debug.Log("Detached a screw");

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        gameObject.layer = LayerMask.NameToLayer(unscrewedLayer);

        if (connectedPart)
        {
            transform.parent = null;

            if (!isFake)
            {
                connectedPart.TakeDamage(maxHealth);
            }
        }
        else
        {
            Debug.LogWarning($"Controller for {name} not found.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, transform.position - new Vector3(threadLength, 0));
    }
}
public enum UnscrewDirection
{
    Left,
    Right
}
