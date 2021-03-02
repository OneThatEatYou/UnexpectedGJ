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
    public InventoryManager inventoryManager;
    
    //screen size in pixels
    public Vector2 ScreenSizePixel
    {
        get { return new Vector2(Screen.width, Screen.height); }
    }

    //screen size in world units
    public Vector2 ScreenSizeWorld
    {
        get 
        {
            float height = Camera.main.orthographicSize * 2.0f;
            float width = height * Screen.width / Screen.height;
            return new Vector2(width, height); 
        }
    }

    public static bool isPaused = false;

    private void Awake()
    {
        //initialize audio manager
        audioManager = new AudioManager();
        audioManager.OnInit();
        inventoryManager = new InventoryManager();
        inventoryManager.OnInit();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLoadScene;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLoadScene;
    }

    void OnLoadScene(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Main")
        {
            audioManager.mainMixer.SetFloat("Vol_Battle", 0);
        }
    }

    public void ChangeScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }
}
