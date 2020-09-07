using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    [HideInInspector] public BaseRobot baseRobot;

    // Start is called before the first frame update
    void Start()
    {
        //var parts = RobotBuilder.Instance.GetAllParts(baseRobot);
        //foreach (var item in parts)
        //{
        //    Debug.Log(item.name);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
