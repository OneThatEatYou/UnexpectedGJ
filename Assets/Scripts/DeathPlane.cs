using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPlane : MonoBehaviour
{
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
        if (!player)
        { player = FindObjectOfType<PlayerController>(); }

        Vector2 target = RobotBuilder.Instance.spawnPos;
        Vector2 time = new Vector2(3f, 1f);
        player.TriggerStun(time.y);

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        Vector2 displacement = target - (Vector2)player.transform.position;
        Vector2 force = Vector2.zero;
        Vector2 u = Vector2.zero;
        Vector2 v = Vector2.zero;
        Vector2 a = new Vector2(0, Physics2D.gravity.y * rb.gravityScale);

        u.y = (displacement.y / time.y) - (a.y * time.y);
        u.x = displacement.x / time.x;

        rb.velocity = u;

        Debug.Log($"displacement: {displacement}, a: {a}, u: {u}");
        //Debug.Break();
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
