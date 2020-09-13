using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPlane : MonoBehaviour
{
    public int damage;
    public Vector2 time = new Vector2(3f, 1f);
    public float noInputTime = 0.5f;
    public float stunRecoverTime = 1f;
    public float xVelMultiplier = 1.5f;
    public float yVelMultiplier = 1f;

    PlayerController player;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out player))
        {
            LaunchPlayerBack();
        }
        else
        {
            Destroy(collision.gameObject);
        }
    }

    public void LaunchPlayerBack()
    {
        player.TakeDamage(damage);

        Vector2 target = RobotBuilder.Instance.spawnPos;
        Vector2 t = time;
        player.TriggerStun(noInputTime, stunRecoverTime);

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        Vector2 displacement = target - (Vector2)player.transform.position;
        Vector2 force = Vector2.zero;
        Vector2 u = Vector2.zero;
        Vector2 v = Vector2.zero;
        Vector2 a = new Vector2(0, Physics2D.gravity.y * rb.gravityScale);

        u.y = (displacement.y / t.y) - (a.y * t.y);
        u.x = displacement.x / t.x * xVelMultiplier;

        //directly add to y velocity
        //smooths x velocity
        rb.velocity = u * yVelMultiplier;
        player.envVel = u;

        //Debug.Log($"displacement: {displacement}, a: {a}, u: {u}");
    }

    /*
    public void LaunchPlayerBack()
    {
        if (!player)
        { player = FindObjectOfType<PlayerController>(); }

        Vector2 target = RobotBuilder.Instance.spawnPos;
        Vector2 time = new Vector2(3f, 2f);
        player.TriggerStun(time.y);

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        Vector2 displacement = target - (Vector2)player.transform.position;
        Vector2 force = Vector2.zero;
        Vector2 u = Vector2.zero;
        Vector2 v = Vector2.zero;
        Vector2 a = new Vector2(0, Physics2D.gravity.y * rb.gravityScale);

        u = (displacement / time) - (0.5f * a * time);
        //v.x = u.x;
        v.y = Mathf.Sqrt((u.y * u.y) + (2 * a.y * displacement.y));
        force = v;
        Debug.Log($"Initial vel: {u}, v: {v}, mass: {rb.mass}, force: {force}");
        rb.velocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);

        //Debug.Break();
    }
    */
}
