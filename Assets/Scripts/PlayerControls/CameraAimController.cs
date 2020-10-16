using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class CameraAimController : MonoBehaviour
{
    public static CameraAimController instance;
    public static bool isHidingCursor = false;

    public int screenshotWidth, screenshotHeight;
    public Vector2 intendedScreenRes = new Vector2(960, 540);

    [Space]

    public float displayDuration = 3f;
    public GameObject displayParent;
    public RawImage display;
    public Camera cam;

    [Header("Flash")]
    public Image flashImage;
    public float flashStay;
    public float flashFade;
    public float flashAlpha;

    public Vector2Int ScreenSize
    {
        get 
        {
            return new Vector2Int(Screen.width, Screen.height);
        }
    }

    private void Awake()
    {
        #region Singleton

        if (instance == null)
        { instance = this; }
        else if (instance != this)
        {
            Debug.LogWarning("Destroying an extra CameraAimController.");
            Destroy(gameObject);
        }

        #endregion
    }

    void Update()
    {
        if (GameManager.isPaused)
        { return; }

        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = pos;
    }

    public void Shoot()
    {
        int sizeX = Mathf.RoundToInt(ScreenSize.x * (screenshotWidth / intendedScreenRes.x));
        int sizeY = Mathf.RoundToInt(ScreenSize.y * (screenshotHeight / intendedScreenRes.y));

        StartCoroutine(TakeScreenshot(sizeX, sizeY));
        StartCoroutine(FlashScreen());
    }

    public IEnumerator TakeScreenshot(int width, int height)
    {
        yield return new WaitForEndOfFrame();

        Cursor.visible = false;
        isHidingCursor = true;

        RenderTexture rendTex = new RenderTexture(Screen.width, Screen.height, 24);
        cam.targetTexture = rendTex;

        //creates a blank texture
        Texture2D result = new Texture2D(width, height, TextureFormat.RGB24, false);
        cam.Render();

        RenderTexture.active = rendTex;

        //calculates the position to read
        Vector2 pixelPos = (Vector2)Input.mousePosition - new Vector2(width / 2, height / 2);
        pixelPos.x = Mathf.Clamp(pixelPos.x, 0, Screen.width - width);
        pixelPos.y = Mathf.Clamp(pixelPos.y, 0, Screen.height - height);

        //Debug.Log(pixelPos);

        Vector2 rectStartPos;
        rectStartPos.x = pixelPos.x;
#if UNITY_WEBGL
        
        rectStartPos.y = pixelPos.y;
#else
        rectStartPos.y = Screen.height - height - pixelPos.y;
#endif
        rectStartPos.x = Mathf.Clamp(rectStartPos.x, 0, Screen.width - width);
        rectStartPos.y = Mathf.Clamp(rectStartPos.y, 0, Screen.height - height);

        //rect starts from top left while Input.mousePosition starts from bottom left
        Rect rect = new Rect(rectStartPos.x, rectStartPos.y, width, height);
        result.ReadPixels(rect, 0, 0);
        result.Apply();

        //saves the texture as png file
        //byte[] byteArray = result.EncodeToPNG();
        //System.IO.File.WriteAllBytes(Application.dataPath + "/CameraScreenshot.png", byteArray);
        //Debug.Log("Saved screenshot.");

        //apply the result to display
        displayParent.SetActive(true);
        display.texture = result;

        //reset
        RenderTexture.active = null;
        cam.targetTexture = null;
        Destroy(rendTex);

        yield return new WaitForSeconds(displayDuration);
        displayParent.SetActive(false);

        Cursor.visible = true;
        isHidingCursor = false;
    }

    IEnumerator FlashScreen()
    {
        flashImage.gameObject.SetActive(true);
        flashImage.color = new Color(1, 1, 1, flashAlpha);

        yield return new WaitForSeconds(flashStay);

        float t = 0;
        float a = flashAlpha;
        while (a != 0)
        {
            a = Mathf.Lerp(flashAlpha, 0, t / flashFade);
            flashImage.color = new Color(1, 1, 1, a);

            t += Time.deltaTime;

            yield return null;
        }

        flashImage.gameObject.SetActive(false);
    }
}
