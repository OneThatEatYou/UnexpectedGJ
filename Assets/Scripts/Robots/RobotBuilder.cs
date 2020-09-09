using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//stores parts
public class RobotBuilder : MonoBehaviour
{
    public GameObject screwPrefab;

    [Space]

    public GameObject[] heads;
    public GameObject[] bodies;
    public GameObject[] hands_L;
    public GameObject[] hands_R;
    public GameObject[] legs;

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

        //get ground height
        RaycastHit2D hit = Physics2D.Raycast(spawnPos, Vector2.down);
        float groundHeight = hit.point.y;

        //declare what to generate
        GameObject newHead;
        GameObject newBody;
        GameObject[] newHands_L = new GameObject[1];
        GameObject[] newHands_R = new GameObject[1];
        GameObject[] newLegs = new GameObject[2];

        //instantiate base
        GameObject newRobot = new GameObject("NewRobot");
        RobotController newController = newRobot.AddComponent<RobotController>();

        //generate parts
        newHead = Instantiate(heads[Random.Range(0, heads.Length)], newRobot.transform);
        newBody = Instantiate(bodies[Random.Range(0, bodies.Length)], newRobot.transform);

        for (int i = 0; i < newHands_L.Length; i++)
        {
            newHands_L[i] = Instantiate(hands_L[Random.Range(0, hands_L.Length)], newRobot.transform);
        }
        for (int i = 0; i < newHands_R.Length; i++)
        {
            newHands_R[i] = Instantiate(hands_R[Random.Range(0, hands_R.Length)], newRobot.transform);
        }

        for (int i = 0; i < newLegs.Length; i++)
        {
            newLegs[i] = Instantiate(legs[Random.Range(0, legs.Length)], newRobot.transform);
        }

        //assign sprites
        Sprite newHeadSprite = newHead.GetComponent<SpriteRenderer>().sprite;
        Sprite newBodySprite = newBody.GetComponent<SpriteRenderer>().sprite;
        Sprite[] newHand_LSprites = new Sprite[newHands_L.Length];
        Sprite[] newHand_RSprites = new Sprite[newHands_R.Length];
        Sprite[] newLegSprites = new Sprite[newLegs.Length];
        for (int i = 0; i < newHands_L.Length; i++)
        {
            newHand_LSprites[i] = newHands_L[i].GetComponent<SpriteRenderer>().sprite;
        }
        for (int i = 0; i < newHands_R.Length; i++)
        {
            newHand_RSprites[i] = newHands_R[i].GetComponent<SpriteRenderer>().sprite;
        }
        for (int i = 0; i < newLegs.Length; i++)
        {
            newLegSprites[i] = newLegs[i].GetComponent<SpriteRenderer>().sprite;
        }

        //get body x range
        float bodyXRange = newBodySprite.bounds.extents.x;

        //place legs based on width of body
        for (int i = 0; i < newLegs.Length; i++)
        {
            Vector2 legPos;
            legPos.x = Random.Range(spawnPos.x + bodyXRange - newLegSprites[i].bounds.extents.x, spawnPos.x - bodyXRange + newLegSprites[i].bounds.extents.x);
            legPos.y = groundHeight + newLegSprites[i].bounds.size.y;
            newLegs[i].transform.position = legPos;
        }

        //get the lowest leg
        float lowestLegPos = groundHeight;
        for (int i = 0; i < newLegs.Length; i++)
        {
            if (newLegSprites[i].bounds.size.y + groundHeight > lowestLegPos)
            {
                lowestLegPos = newLegSprites[i].bounds.size.y + groundHeight;
            }
        }

        //place body based on height of legs
        Vector2 bodyPos;
        bodyPos.x = spawnPos.x;
        bodyPos.y = lowestLegPos + newBodySprite.bounds.extents.y;
        newBody.transform.position = bodyPos;

        //place head based on position and width of body
        Vector2 headPos;
        headPos.x = Random.Range(spawnPos.x + bodyXRange, spawnPos.x - bodyXRange);
        headPos.y = newBodySprite.bounds.extents.y + newHeadSprite.bounds.extents.y + bodyPos.y;
        newHead.transform.position = headPos;

        //place hands
        Vector2 handPos;
        for (int i = 0; i < newHands_L.Length; i++)
        {
            handPos.x = spawnPos.x - newBodySprite.bounds.extents.x - newHand_LSprites[i].bounds.extents.x;
            handPos.y = Random.Range(bodyPos.y - newBodySprite.bounds.extents.y, bodyPos.y + newBodySprite.bounds.extents.y);
            newHands_L[i].transform.position = handPos;
        }
        for (int i = 0; i < newHands_L.Length; i++)
        {
            handPos.x = spawnPos.x + newBodySprite.bounds.extents.x + newHand_LSprites[i].bounds.extents.x;
            handPos.y = Random.Range(bodyPos.y - newBodySprite.bounds.extents.y, bodyPos.y + newBodySprite.bounds.extents.y);
            newHands_R[i].transform.position = handPos;
        }
    }
}
