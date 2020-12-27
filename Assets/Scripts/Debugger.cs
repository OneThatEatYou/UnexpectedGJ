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

        BattleManager.Instance.SpawnRobot();
    }

    public void KillPlayer()
    {
        PlayerController con;
        con = FindObjectOfType<PlayerController>();
        if (con)
        {
            con.Die(true);
        }
    }
}
