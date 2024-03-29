﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using DG.Tweening;

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
    public TutorialManager tutorialManager;

    public Image fadeImage;
    
    //screen size in pixels
    public static Vector2 ScreenSizePixel
    {
        get { return new Vector2(Screen.width, Screen.height); }
    }

    //screen size in world units
    public static Vector2 ScreenSizeWorld
    {
        get 
        {
            float height = Camera.main.orthographicSize * 2.0f;
            float width = height * Screen.width / Screen.height;
            return new Vector2(width, height); 
        }
    }

    public static bool isPaused = false;
    public static bool clockedIn = false;

    private void Awake()
    {
        audioManager = new AudioManager();
        audioManager.OnInit();
        inventoryManager = new InventoryManager();
        inventoryManager.OnInit();
        tutorialManager = new TutorialManager();
        tutorialManager.OnInit();

        //creates image overlay for fading in and out
        GameObject fadeCanvasGO = new GameObject("FadeCanvas");
        Canvas fadeCanvas = fadeCanvasGO.AddComponent<Canvas>();
        fadeCanvasGO.AddComponent<GraphicRaycaster>();

        CanvasScaler canvasScaler = fadeCanvasGO.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(960, 540);
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScaler.matchWidthOrHeight = 0.5f;
        canvasScaler.referencePixelsPerUnit = 8;

        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 2;
        fadeCanvasGO.GetComponent<RectTransform>().sizeDelta = ScreenSizePixel;
        fadeImage = fadeCanvasGO.AddComponent<Image>();
        fadeImage.raycastTarget = false;
        fadeImage.color = Color.clear;
        fadeCanvasGO.transform.SetParent(transform);
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


            tutorialManager.QueuePopUp(0);

            if (inventoryManager.equippedItem != null)
            {
                tutorialManager.QueuePopUp(1);
            }
        }
        else if (scene.name == "MainMenu")
        {
            if (clockedIn)
            {
                Destroy(FindObjectOfType<MainMenuManager>().startAnimator.gameObject);
            }
        }

        //fade in scene
        Color fadeCol = fadeImage.color;
        fadeCol.a = 0;
        fadeImage.DOColor(fadeCol, 1).SetEase(Ease.InOutSine);

        if (!GlobalSettings.isBloomEnabled)
        {
            DisableBloom();
        }
    }

    public void ChangeScene(int sceneIndex)
    {
        StartCoroutine(FadeOutAndChangeScene(sceneIndex));
    }

    IEnumerator FadeOutAndChangeScene(int sceneIndex)
    {
        //fade out scene
        Color fadeCol = fadeImage.color;
        fadeCol.a = 1;
        fadeImage.DOColor(fadeCol, 1).SetEase(Ease.InOutSine);
        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(sceneIndex);
    }

    public void DisableBloom()
    {
        var volumes = FindObjectsOfType<Volume>();

        foreach (var item in volumes)
        {
            Debug.Log($"Disabled {item.name}");
            item.enabled = false;
        }
    }

    public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }
}
