using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                //check if instance is in scene
                instance = FindObjectOfType<GameManager>();

                if (instance == null)
                {
                    //spawn instance
                    GameObject obj = new GameObject("GameManager");
                    instance = obj.AddComponent<GameManager>();
                    DontDestroyOnLoad(obj);
                }
            }
            return instance;
        }
    }

    public AudioManager audioManager;

    public bool canRestart = false;
    public static bool isPaused = false;

    private void Awake()
    {
        audioManager = new AudioManager();
        audioManager.OnInit();
    }

    private void Update()
    {
        if (canRestart)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                canRestart = false;
            }
        }
    }

    public void ChangeScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
