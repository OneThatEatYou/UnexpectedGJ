using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_Headlight : ItemController
{
    public RawImage rawImage;
    public Vector2 intendedScreenRes = new Vector2(960, 540);

    RenderTexture rendTex;
    Camera ssCam;

    public override void Awake()
    {
        base.Awake();

        ssCam = GameObject.FindGameObjectWithTag("SSCamera").GetComponent<Camera>();
    }

    public override void Start()
    {
        base.Start();

        rendTex = new RenderTexture(Screen.width, Screen.height, 24);
        ssCam.targetTexture = rendTex;
        rawImage.texture = rendTex;

        Vector2 worldSize = new Vector2(rawImage.rectTransform.rect.width, rawImage.rectTransform.rect.height);
        Vector2Int pixelSize = Vector2Int.zero;
        Debug.Log(pixelSize);

        Texture2D result = new Texture2D(pixelSize.x, pixelSize.y, TextureFormat.RGB24, false);
        ssCam.Render();

        RenderTexture.active = rendTex;

        Vector2 imagePixelPos = Camera.main.WorldToScreenPoint(rawImage.rectTransform.anchoredPosition);
        Vector2 rectStartPos;
        rectStartPos.x = imagePixelPos.x;
#if UNITY_WEBGL
        
        rectStartPos.y = imagePixelPos.y;
#else
        rectStartPos.y = Screen.height - pixelSize.y - imagePixelPos.y;
#endif
        rectStartPos.x = Mathf.Clamp(rectStartPos.x, 0, Screen.width - pixelSize.x);
        rectStartPos.y = Mathf.Clamp(rectStartPos.y, 0, Screen.height - pixelSize.y);

        //rect starts from top left while Input.mousePosition starts from bottom left
        Rect rect = new Rect(rectStartPos.x, rectStartPos.y, pixelSize.x, pixelSize.y);
        result.ReadPixels(rect, 0, 0);
        result.Apply();

        rawImage.texture = result;
    }

    public override void Update()
    {
        base.Update();
    }
}
