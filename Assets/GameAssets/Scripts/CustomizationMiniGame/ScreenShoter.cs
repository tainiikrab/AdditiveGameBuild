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
        RenderTexture rt = new RenderTexture(resolutionWidth, resolutionHeight, 24);
        screenshotCamera.targetTexture = rt;
        screenshotCamera.Render();

        RenderTexture.active = rt;

        Texture2D image = new Texture2D(resolutionWidth, resolutionHeight, TextureFormat.RGB24, false);
        image.ReadPixels(new Rect(0, 0, resolutionWidth, resolutionHeight), 0, 0);
        image.Apply();

        screenshotCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        lastScreenshot = image;

        Texture2D flipped = FlipTextureVertically(image);

        Sprite previewSprite = Sprite.Create(
            flipped,
            new Rect(0, 0, flipped.width, flipped.height),
            new Vector2(0.5f, 0.5f)
        );

        ScreenShotPreview.sprite = previewSprite;
        ScreenShotPreview.preserveAspect = true;
        ScreenShotPreview.gameObject.SetActive(true);

        yield return null;
    }

    private Texture2D FlipTextureVertically(Texture2D original)
    {
        int w = original.width;
        int h = original.height;

        Texture2D flipped = new Texture2D(w, h, original.format, false);

        for (int y = 0; y < h; y++)
            flipped.SetPixels(0, h - 1 - y, w, 1, original.GetPixels(0, y, w, 1));

        flipped.Apply();
        return flipped;
    }

    public void RestartScreenshotPreview()
    {
        ScreenShotPreview.sprite = null;
        ScreenShotPreview.gameObject.SetActive(false);

        lastScreenshot = null;
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

        File.WriteAllBytes(fullPath, lastScreenshot.EncodeToPNG());
        ScreenShotPreview.gameObject.SetActive(false);
        Debug.Log("Screenshot saved: " + fullPath);
    }
}
