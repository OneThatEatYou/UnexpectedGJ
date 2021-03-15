using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_Camera : ItemController
{
    public static bool isHidingCursor = false;
    public int screenshotWidth, screenshotHeight;
    public Vector2 intendedScreenRes = new Vector2(960, 540);
    [Space]
    public float displayDuration = 3f;
    public GameObject worldCanvasObj;
    public GameObject overlayCanvasObj;
    RawImage displayImage;
    Camera ssCam;

    [Header("Flash")]
    public float flashStay;
    public float flashFade;
    public float flashAlpha;
    Image flashImage;

    public override void Awake()
    {
        base.Awake();

        displayImage = worldCanvasObj.GetComponentInChildren<RawImage>();
        ssCam = GameObject.FindGameObjectWithTag("SSCamera").GetComponent<Camera>();
        flashImage = overlayCanvasObj.GetComponentInChildren<Image>();
    }

    public override void Update()
    {
        base.Update();

        if (GameManager.isPaused)
        { return; }

        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = pos;
    }

    public override void UseItem()
    {
        base.UseItem();

        int sizeX = Mathf.RoundToInt(GameManager.ScreenSizePixel.x * (screenshotWidth / intendedScreenRes.x));
        int sizeY = Mathf.RoundToInt(GameManager.ScreenSizePixel.y * (screenshotHeight / intendedScreenRes.y));

        StartCoroutine(TakeScreenshot(sizeX, sizeY));
        StartCoroutine(FlashScreen());

        //BattleManager.Instance.PlaySlowMo(slowIn, slowStay, slowOut, slowTimeScale);
    }

    public IEnumerator TakeScreenshot(int width, int height)
    {
        yield return new WaitForEndOfFrame();

        Cursor.visible = false;
        isHidingCursor = true;

        RenderTexture rendTex = new RenderTexture(Screen.width, Screen.height, 24);
        ssCam.targetTexture = rendTex;

        //creates a blank texture
        Texture2D result = new Texture2D(width, height, TextureFormat.RGB24, false);
        ssCam.Render();

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
        worldCanvasObj.SetActive(true);
        displayImage.texture = result;

        //reset
        RenderTexture.active = null;
        ssCam.targetTexture = null;
        Destroy(rendTex);

        yield return new WaitForSeconds(displayDuration);
        worldCanvasObj.SetActive(false);

        Cursor.visible = true;
        isHidingCursor = false;
    }

    IEnumerator FlashScreen()
    {
        overlayCanvasObj.SetActive(true);
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

        overlayCanvasObj.SetActive(false);
    }
}
