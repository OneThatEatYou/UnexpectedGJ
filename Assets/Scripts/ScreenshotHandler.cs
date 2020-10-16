using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotHandler : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            string time = System.DateTime.Now.ToString("HH-mm-yyyy-MM-dd");
            string filename = Application.dataPath + "/" + time + ".png";
            ScreenCapture.CaptureScreenshot(filename);
            Debug.Log("Captured screenshot as " + filename);
        }
    }
}
