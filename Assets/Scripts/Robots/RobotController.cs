using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    Transform playerPos;
    public Transform PlayerPos { get { return playerPos; } }

    public List<BasePart> parts = new List<BasePart>();
    List<BasePart> nonDetachables = new List<BasePart>();

    int maxHealth;
    int currentHealth;

    [HideInInspector] public bool isMoving = false;

    private void Awake()
    {
        playerPos = FindObjectOfType<PlayerController>().transform;
    }

    void Start()
    {
        //var parts = RobotBuilder.Instance.GetAllParts(Base);
        //foreach (var item in parts)
        //{
        //    Debug.Log(item.name);
        //}

        //Debug.Log(parts);

        maxHealth = GetMaxHealth();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(int damage)
    {
        Debug.Log($"{gameObject.name} took {damage} damage");

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Robot died");

        //detach all parts
        foreach (BasePart part in nonDetachables)
        {
            part.Detach();
        }

        //explosion

        //spawn new robot
        StartCoroutine(SpawnNewRobot(3f));

        IEnumerator SpawnNewRobot(float delay)
        {
            yield return new WaitForSeconds(delay);

            RobotBuilder.Instance.GenerateRobot();
            Destroy(gameObject);
        }
    }

    private int GetMaxHealth()
    {
        int maxHp = 0;

        for (int i = 0; i < transform.childCount; i++)
        {
            BasePart part;
            transform.GetChild(i).TryGetComponent(out part);
            if (part != null)
            {
                maxHp += part.maxHealth;
            }
        }

        return maxHp;
    }

    public void AddNonDetachablePart(BasePart nonDetachablePart)
    {
        nonDetachables.Add(nonDetachablePart);
    }
}
