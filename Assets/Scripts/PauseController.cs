using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController : MainMenuManager
{
    public GameObject pauseCanvas;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    public override void ChangeScene(int sceneIndex)
    {
        Time.timeScale = 1;
        GameManager.isPaused = false;
        base.ChangeScene(sceneIndex);
    }

    void Pause()
    {
        Time.timeScale = 1 - Time.timeScale;
        pauseCanvas.SetActive(!pauseCanvas.activeInHierarchy);

        GameManager.isPaused = !GameManager.isPaused;

        //Debug.Log("Timescale: " + Time.timeScale + ", isHidingCursor: " + CameraAimController.isHidingCursor);

        if (Time.timeScale == 1)
        {
            //unpause
            //what the hell
            //Cursor.visible = !CameraAimController.isHidingCursor;
            //Cursor.visible = false;
            //Debug.Log("Unpaused with Cursor.visible = " + Cursor.visible);
        }
        else
        {
            //pause
            Cursor.visible = true;
        }
    }
}
