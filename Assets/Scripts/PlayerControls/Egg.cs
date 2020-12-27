using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour
{
    public Vector2 spawnPos;
    public GameObject eggParticle;
    public float force;

    Rigidbody2D rb;
    [HideInInspector] public bool hatched = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float dir = Input.GetAxis("Horizontal");
        Move(dir);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Hatch();
    }

    void Hatch()
    {
        //this function may be called multiple times if it collided with more than one object
        if (hatched)
        { return; }

        hatched = true;
        BattleManager.Instance.RespawnPlayer((Vector2)transform.position + spawnPos);
        if (eggParticle)
        {
            Instantiate(eggParticle, (Vector2)transform.position + spawnPos, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    void Move(float dir)
    {
        rb.AddForce(dir * force * Vector2.right);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + spawnPos, 0.3f);
    }
}
