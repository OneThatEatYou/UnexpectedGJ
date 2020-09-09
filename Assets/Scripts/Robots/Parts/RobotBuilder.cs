using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//stores parts
public class RobotBuilder : MonoBehaviour
{
    public GameObject screwPrefab;

    [Space]

    public Head[] heads;
    public Body[] bodies;
    public Hand[] hands;
    public Leg[] legs;

    [Space]
    public string slappableLayerName = "Slappable";
    public string robotSortingLayer;
    public int robotLayerOrder;

    [Space]
    
    public Vector2 spawnPos = Vector2.zero;

    #region Singleton
    private static RobotBuilder instance;
    public static RobotBuilder Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<RobotBuilder>();

                //if no parts manager present in scene
                if (instance == null)
                {
                    //GameObject obj = new GameObject("RobotBuilder");
                    //instance = obj.AddComponent<RobotBuilder>();
                    //Debug.Log("Spawning a new RobotBuilder.");

                    Debug.LogError("RobotBuilder not found.");
                    Debug.Break();
                }
            }

            return instance;
        }
    }
    #endregion

    private void Start()
    {
        GenerateRobot();
    }

    [ContextMenu("Generate Robot")]
    public virtual void GenerateRobot()
    {
        Debug.Log("Generating a robot.");

        //declare what to generate
        Head newHead;
        Body newBody;
        Hand[] newHands = new Hand[2];
        Leg[] newLegs = new Leg[2];

        //generate parts
        newHead = Instantiate(heads[Random.Range(0, heads.Length)]);
        newBody = Instantiate(bodies[Random.Range(0, bodies.Length)]);

        for (int i = 0; i < newHands.Length; i++)
        {
            newHands[i] = Instantiate(hands[Random.Range(0, hands.Length)]);
        }

        for (int i = 0; i < newLegs.Length; i++)
        {
            newLegs[i] = Instantiate(legs[Random.Range(0, legs.Length)]);
        }

        //spawn base and assign parts
        BaseRobot newBaseRobot = new BaseRobot(newHead, newBody, newHands, newLegs);
        GameObject newRobot = new GameObject("NewRobot");
        RobotController newController = newRobot.AddComponent<RobotController>();
        newController.Base = newBaseRobot;

        //get ground height
        RaycastHit2D hit = Physics2D.Raycast(spawnPos, Vector2.down);
        float groundHeight = hit.point.y;

        //get body x range
        float bodyXRange = newBody.sprite.bounds.extents.x;

        //place legs based on width of body
        for (int i = 0; i < newLegs.Length; i++)
        {
            Vector2 legPos;
            legPos.x = Random.Range(spawnPos.x + bodyXRange - newLegs[i].sprite.bounds.extents.x, spawnPos.x - bodyXRange + newLegs[i].sprite.bounds.extents.x);
            legPos.y = groundHeight + newLegs[i].sprite.bounds.size.y;

            SpawnPart(newController, newLegs[i], newLegs[i].sprite, newRobot.transform, legPos);
        }

        //get the lowest leg
        float lowestLegPos = groundHeight;
        foreach (Leg leg in newLegs)
        {
            if (leg.sprite.bounds.size.y + groundHeight > lowestLegPos)
            {
                lowestLegPos = leg.sprite.bounds.size.y + groundHeight;
                //Debug.Log($"Top of lowest leg found at {lowestLegPos}.");
            }
        }

        //place body based on height of legs
        Vector2 bodyPos;
        bodyPos.x = spawnPos.x;
        bodyPos.y = lowestLegPos + newBody.sprite.bounds.extents.y;
        GameObject body = SpawnPart(newController, newBody, newBody.sprite, newRobot.transform, bodyPos);

        //place head based on position and width of body
        Vector2 headPos;
        headPos.x = Random.Range(spawnPos.x + bodyXRange, spawnPos.x - bodyXRange);
        headPos.y = newBody.sprite.bounds.extents.y + newHead.sprite.bounds.extents.y + bodyPos.y;
        SpawnPart(newController, newHead, newHead.sprite, newRobot.transform, headPos);

        //place hands based on position of body
        int flip = Random.Range(0, 1);
        foreach (Hand hand in newHands)
        {
            Vector2 handPos;
            flip = 1 - flip;

            if (flip == 0)
            {
                //spawn on left
                handPos.x = spawnPos.x - newBody.sprite.bounds.extents.x - hand.leftHandSprite.bounds.extents.x;
                handPos.y = Random.Range(bodyPos.y - newBody.sprite.bounds.extents.y, bodyPos.y + newBody.sprite.bounds.extents.y);
                SpawnPart(newController, hand, hand.leftHandSprite, newRobot.transform, handPos);
            }
            else
            {
                //spawn on right
                handPos.x = spawnPos.x + newBody.sprite.bounds.extents.x + hand.rightHandSprite.bounds.extents.x;
                handPos.y = Random.Range(bodyPos.y - newBody.sprite.bounds.extents.y, bodyPos.y + newBody.sprite.bounds.extents.y);
                SpawnPart(newController, hand, hand.rightHandSprite, newRobot.transform, handPos);
            }
        }
    }

    public GameObject SpawnPart(RobotController robotCon, BasePart part, Sprite sprite, Transform parent, Vector2 pos)
    {
        GameObject obj = new GameObject(part.partName);
        obj.layer = LayerMask.NameToLayer(slappableLayerName);
        obj.transform.SetParent(parent);
        obj.transform.localScale = Vector3.one;
        obj.transform.position = pos;

        SpriteRenderer rend = obj.AddComponent<SpriteRenderer>();
        rend.sprite = sprite;
        rend.sortingLayerName = robotSortingLayer;
        rend.sortingOrder = robotLayerOrder;

        PartController pCon = obj.AddComponent<PartController>();
        pCon.part = part;
        part.PartController = pCon;
        pCon.robotController = robotCon;

        obj.AddComponent<PolygonCollider2D>();
        Rigidbody2D rb = obj.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        return obj;
    }

    //returns all the parts of a robot
    public List<BasePart> GetAllParts(BaseRobot robot)
    {
        List<BasePart> parts = new List<BasePart>();

        parts.Add(robot.head);

        parts.Add(robot.body);

        foreach (Hand hand in robot.hands)
        {
            parts.Add(hand);
        }
        foreach (Leg leg in robot.legs)
        {
            parts.Add(leg);
        }

        return parts;
    }
}
