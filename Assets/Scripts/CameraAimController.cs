using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class CameraAimController : MonoBehaviour
{
    public static CameraAimController instance;

    public int screenshotWidth, screenshotHeight;

    [Space]

    public float displayDuration = 3f;
    public GameObject displayParent;
    public RawImage display;
    public Camera cam;

    [Header("Flash")]
    public Image flashImage;
    public float flashStay;
    public float flashFade;

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
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = pos;
    }

    public void Shoot()
    {
        StartCoroutine(TakeScreenshot(screenshotWidth, screenshotHeight));
        StartCoroutine(FlashScreen());
    }

    public IEnumerator TakeScreenshot(int width, int height)
    {
        yield return new WaitForEndOfFrame();

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
        rectStartPos.y = Screen.height - height - pixelPos.y;
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
    }

    IEnumerator FlashScreen()
    {
        flashImage.gameObject.SetActive(true);
        flashImage.color = Color.white;

        yield return new WaitForSeconds(flashStay);

        float t = 0;
        float a = 1;
        while (a != 0)
        {
            a = Mathf.Lerp(1, 0, t / flashFade);
            flashImage.color = new Color(1, 1, 1, a);

            t += Time.deltaTime;

            yield return null;
        }

        flashImage.gameObject.SetActive(false);
    }
}
