using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotHandler : MonoBehaviour
{
    int a = 0;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            ScreenCapture.CaptureScreenshot(Application.dataPath + a.ToString() + ".png");
            Debug.Log("Captured screenshot");
            a++;
        }
    }
}
