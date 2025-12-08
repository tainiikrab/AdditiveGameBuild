using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class ScreenShoter : MonoBehaviour
{
    public Camera screenshotCamera;
    public Image ScreenShotPreview;
    public int resolutionWidth = 1920;
    public int resolutionHeight = 1080;

    private Texture2D lastScreenshot;

    public void MakeScreenshot()
    {
        StartCoroutine(Capture());
    }

    IEnumerator Capture()
    {
        // ������� RenderTexture
        RenderTexture rt = new RenderTexture(resolutionWidth, resolutionHeight, 24);
        screenshotCamera.targetTexture = rt;
        screenshotCamera.Render();

        RenderTexture.active = rt;

        // ������� ��������
        lastScreenshot = new Texture2D(resolutionWidth, resolutionHeight, TextureFormat.RGB24, false);
        
        // ������ �������
        lastScreenshot.ReadPixels(new Rect(0, 0, resolutionWidth, resolutionHeight), 0, 0);
        lastScreenshot.Apply();

        // �������
        screenshotCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        // ������� ������
        Sprite previewSprite = Sprite.Create(
            lastScreenshot,
            new Rect(0, 0, lastScreenshot.width, lastScreenshot.height),
            new Vector2(0.5f, 0.5f)
        );

        // ������������� � UI
        ScreenShotPreview.sprite = previewSprite;
        ScreenShotPreview.preserveAspect = true;
        ScreenShotPreview.gameObject.SetActive(true);

        yield return null;
    }

    public void RestartScreenshotPreview()
    {
        ScreenShotPreview.sprite = null;
        ScreenShotPreview.gameObject.SetActive(false);

        if (lastScreenshot != null)
        {
            Destroy(lastScreenshot);
            lastScreenshot = null;
        }
    }

    public void SaveCurrentScreenshot()
    {
        if (lastScreenshot == null)
        {
            Debug.LogWarning("No screenshot to save!");
            return;
        }

        string folderPath = Application.persistentDataPath + "/CustomizedFiguresScreenShots";
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string filename = "Screenshot_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string fullPath = Path.Combine(folderPath, filename);

        // ��������� ��������
        // File.WriteAllBytes(fullPath, lastScreenshot.EncodeToPNG());
        ScreenShotPreview.gameObject.SetActive(false);
        Debug.Log("Screenshot saved: " + fullPath);
        SceneSwitchManager.OpenScene(SceneName.MainScene);
    }
}