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
        if (GameManager.Instance.tutorialManager.IsShowingPopUp)
        {
            return;
        }

        Time.timeScale = 1 - Time.timeScale;
        pauseCanvas.SetActive(!pauseCanvas.activeInHierarchy);

        GameManager.isPaused = !GameManager.isPaused;
    }
}
