using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;
using Image = UnityEngine.UI.Image;
using DG.Tweening;

public class SandpaperIndicator : MonoBehaviour
{
    [SerializeField] private Image indicatorImage;

    private float smoothnessDone => SandpaperTool.smoothnessDone;
    private float requiredSmoothness => SandpaperTool.requiredSmoothness;

    [SerializeField] private Color badColor, goodColor;
    private Color currentColor;

    public void Update()
    {
        if (!SandpaperTool.isSmoothing)
        {
            if (indicatorImage.enabled)
                indicatorImage.enabled = false;
            return;
        }

        if (!indicatorImage.enabled) indicatorImage.enabled = true;

        currentColor = Color.Lerp(badColor, goodColor, smoothnessDone / requiredSmoothness);
        indicatorImage.color = currentColor;
    }
}