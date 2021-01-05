using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RobotBuilder : MonoBehaviour
{
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

    public LayerMask groundLayer;
    public Vector2 playGroundXRange;
    [Space]
    public GameObject screwPrefab;
    [Space]
    public List<SpawnablePart> spawnableParts = new List<SpawnablePart>();
    [ReadOnly] public List<SpawnablePart> heads = new List<SpawnablePart>();
    [ReadOnly] public List<SpawnablePart> bodies = new List<SpawnablePart>();
    [ReadOnly] public List<SpawnablePart> l_Hands = new List<SpawnablePart>();
    [ReadOnly] public List<SpawnablePart> r_Hands = new List<SpawnablePart>();
    [ReadOnly] public List<SpawnablePart> legs = new List<SpawnablePart>();
    [Space]
    public Vector2 spawnPos = Vector2.zero;
    public float initDelay = 1.5f;

    [ContextMenu("Generate Robot")]
    public virtual RobotController GenerateRobot()
    {
        Debug.Log("Generating a robot.");

        //get ground height
        RaycastHit2D hit = Physics2D.Raycast(spawnPos, Vector2.down, 100, groundLayer);
        float groundHeight = hit.point.y;

        //declare what to generate
        SpawnablePart newHead;
        SpawnablePart newBody;
        SpawnablePart[] newHands_L = new SpawnablePart[1];
        SpawnablePart[] newHands_R = new SpawnablePart[1];
        SpawnablePart[] newLegs = new SpawnablePart[2];

        //instantiate base
        GameObject newRobot = new GameObject("NewRobot");
        RobotController newController = newRobot.AddComponent<RobotController>();

        //select spawnablePart to be used
        newHead = heads[Random.Range(0, heads.Count)];
        newBody = bodies[Random.Range(0, bodies.Count)];
        for (int i = 0; i < newHands_L.Length; i++)
        {
            newHands_L[i] = l_Hands[Random.Range(0, l_Hands.Count)];
        }
        for (int i = 0; i < newHands_R.Length; i++)
        {
            newHands_R[i] = r_Hands[Random.Range(0, r_Hands.Count)];
        }
        for (int i = 0; i < newLegs.Length; i++)
        {
            newLegs[i] = legs[Random.Range(0, legs.Count)];
        }

        //width of body
        float bodyXRange = newBody.Extents.x;

        //place legs based on width of body
        for (int i = 0; i < newLegs.Length; i++)
        {
            Vector3 legPos;
            legPos.x = Random.Range(spawnPos.x + bodyXRange - newLegs[i].ILocalPivot.x, spawnPos.x - bodyXRange + newLegs[i].LocalPivot.x);
            legPos.y = groundHeight + newLegs[i].LocalPivot.y;
            legPos.z = 0;
            SpawnPart(newLegs[i], newController, legPos);
        }

        //get the highest leg
        float legHeight = 0;
        for (int i = 0; i < newLegs.Length; i++)
        {
            if (newLegs[i].Size.y > legHeight)
            {
                legHeight = newLegs[i].Size.y;
            }
        }

        //place body based on top of highest leg
        Vector3 bodyPos;
        bodyPos.x = spawnPos.x;
        bodyPos.y = groundHeight + legHeight + newBody.LocalPivot.y;
        bodyPos.z = 0;
        BasePart bodyPart = SpawnPart(newBody, newController, bodyPos);
        SpawnScrews(bodyPart);

        //place head based on position and width of body
        Vector3 headPos;
        headPos.x = Random.Range(spawnPos.x - bodyXRange + newHead.Extents.x, spawnPos.x + bodyXRange - newHead.Extents.x);
        headPos.y = groundHeight + legHeight + newBody.Size.y + newHead.LocalPivot.y;
        headPos.z = 0;
        BasePart headPart = SpawnPart(newHead, newController, headPos, true);
        SpawnScrews(headPart);

        //place hands
        Vector3 handPos;
        for (int i = 0; i < newHands_L.Length; i++)
        {
            handPos.x = spawnPos.x + newBody.Extents.x + newHands_L[i].LocalPivot.x;
            handPos.y = Random.Range(groundHeight + legHeight, groundHeight + legHeight + newBody.Size.y);
            handPos.z = 0;
            BasePart handPart_L = SpawnPart(newHands_L[i], newController, handPos, true);
            SpawnScrews(handPart_L);
        }
        for (int i = 0; i < newHands_R.Length; i++)
        {
            handPos.x = spawnPos.x - newBody.Extents.x - newHands_R[i].ILocalPivot.x;
            handPos.y = Random.Range(groundHeight + legHeight, groundHeight + legHeight + newBody.Size.y);
            handPos.z = 0;
            BasePart handPart_R = SpawnPart(newHands_R[i], newController, handPos, true);
            SpawnScrews(handPart_R);
        }

        newController.DelayInit(initDelay);

        return newController;
    }

    [ContextMenu("Sort Parts")]
    public void SortParts()
    {
        foreach (SpawnablePart part in spawnableParts.ToList())
        {
            switch(part.partType)
            {
                case PartType.Head:
                    heads.Add(part);
                    break;
                case PartType.Body:
                    bodies.Add(part);
                    break;
                case PartType.LHand:
                    l_Hands.Add(part);
                    break;
                case PartType.RHand:
                    r_Hands.Add(part);
                    break;
                case PartType.Leg:
                    legs.Add(part);
                    break;
            }
        }
        spawnableParts = new List<SpawnablePart>();
    }

    void SpawnScrews(BasePart part)
    {
        //spawn screws based on max hp of each parts

        //Screw screwScript = screwPrefab.GetComponent<Screw>();
        int numberOfScrews = part.maxHealth;
        int numberOfFakes = numberOfScrews;
        //the first sprite renderer found in child will be used in rendering queue
        string spriteSortingLayer = part.GetComponentInChildren<SpriteRenderer>().sortingLayerName;
        int spriteSortingOrder = part.GetComponentInChildren<SpriteRenderer>().sortingOrder - 1;
        var screwSpawnPosList = part.screwSpawnPos.ToList();
        BasePart.ScrewSpawnPos spawnPos;

        void SpawnScrew(bool isReal)
        {
            GameObject screw = Instantiate(screwPrefab, part.transform);
            Screw thisScrew = screw.GetComponent<Screw>();
            if (!isReal)
            {
                thisScrew.isFake = true;

                //destroy the first child object with sprite renderer
                SpriteRenderer shine = screw.GetComponentInChildren<SpriteRenderer>();
                DestroyImmediate(shine.gameObject);
            }

            SpriteRenderer[] rend = screw.GetComponentsInChildren<SpriteRenderer>();

            //set the sorting queue for shining and normal screw sprites
            for (int k = 0; k < rend.Length; k++)
            {
                rend[k].sortingLayerName = spriteSortingLayer;
                rend[k].sortingOrder = spriteSortingOrder - k;
            }

            //place the screw in correct position
            if (screwSpawnPosList.Count == 0)
            {
                Debug.LogWarning($"No available screw slot found in {part.name}. Returning.");
                if (isReal)
                { 
                    part.maxHealth--; 
                }
                return;
            }

            int j = Random.Range(0, screwSpawnPosList.Count);
            spawnPos = screwSpawnPosList[j];
            screwSpawnPosList.RemoveAt(j);
            thisScrew.connectedPart = part;
            thisScrew.unscrewDir = spawnPos.unscrewDir;
            screw.transform.localPosition = spawnPos.screwPos;

            //set orientation of screw
            switch (thisScrew.unscrewDir)
            {
                case UnscrewDirection.Left:
                    //default orientation
                    break;
                case UnscrewDirection.Right:
                    //flip horizontally
                    screw.transform.localScale = new Vector2(-1, 1);
                    break;
            }
        }

        for (int i = 0; i < numberOfScrews; i++)
        {
            SpawnScrew(true);
        }

        for (int i = 0; i < numberOfFakes; i++)
        {
            SpawnScrew(false);
        }
    }

    //instantiate the part into the scene and assign it in RobotController
    BasePart SpawnPart(SpawnablePart obj, RobotController controller, Vector3 pos, bool childOfBody = false)
    {
        GameObject go;
        if (childOfBody)
        {
            if (controller.body)
            {
                go = Instantiate(obj.partPrefab, controller.body.transform);
            }
            else
            {
                Debug.LogWarning("Body not found in controller");
                go = Instantiate(obj.partPrefab, controller.transform);
            }
        }
        else
        {
            go = Instantiate(obj.partPrefab, controller.transform);
        }
        go.transform.position = pos;

        BasePart part = go.GetComponent<BasePart>();
        controller.AssignPartToPartList(part);

        return part;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(new Vector2(playGroundXRange.x, 0), new Vector2(playGroundXRange.y, 0));
    }
}
