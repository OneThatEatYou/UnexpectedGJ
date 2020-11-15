using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugger : MonoBehaviour
{
    public void SpawnNewRobot()
    {
        RobotController curRobot = FindObjectOfType<RobotController>();

        if (curRobot)
        {
            Destroy(curRobot.gameObject);
        }

        RobotBuilder.Instance.GenerateRobot();
    }
}
