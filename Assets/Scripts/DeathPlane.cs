using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPlane : MonoBehaviour
{
    public int damage;
    public Vector2 time = new Vector2(3f, 1f);
    public float noInputTime = 0.5f;
    public float stunRecoverTime = 1f;

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

    //public void LaunchPlayerBack()
    //{
    //    player.TakeDamage(damage);

    //    Vector2 target = RobotBuilder.Instance.spawnPos;
    //    Vector2 t = time;
    //    player.TriggerStun(noInputTime, stunRecoverTime);

    //    Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
    //    Vector2 displacement = target - (Vector2)player.transform.position;
    //    Vector2 u = Vector2.zero;
    //    Vector2 a = new Vector2(0, Physics2D.gravity.y * rb.gravityScale);

    //    u.y = (displacement.y / t.y) - (0.5f * a.y * t.y);
    //    u.x = displacement.x / t.x;

    //    //directly add to y velocity
    //    //smooths x velocity
    //    rb.velocity = new Vector2 (rb.velocity.x, u.y);
    //    player.envVel = u;

    //    //Debug.Log($"displacement: {displacement}, a: {a}, u: {u}");
    //}

    public void LaunchPlayerBack()
    {
        player.TakeDamage(damage);

        Vector2 target = RobotBuilder.Instance.spawnPos;
        Vector2 t = time;
        player.TriggerStun(noInputTime, stunRecoverTime);

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        Vector2 displacement = target - (Vector2)player.transform.position;
        Vector2 u = Vector2.zero;
        Vector2 a = new Vector2(0, Physics2D.gravity.y * rb.gravityScale);

        u.y = (displacement.y / t.y) - (0.5f * a.y * t.y);
        u.x = displacement.x / t.x;

        //directly add to y velocity
        //smooths x velocity
        rb.AddForce(new Vector2(rb.velocity.x, u.y), ForceMode2D.Impulse);

        //Debug.Log($"displacement: {displacement}, a: {a}, u: {u}");
    }
}
