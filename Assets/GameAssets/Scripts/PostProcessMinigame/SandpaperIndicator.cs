using System;
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

    [SerializeField] private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup.alpha = 0;
    }

    public void Update()
    {
        if (!SandpaperTool.isSmoothing)
        {
            if (canvasGroup.alpha >= 0.99f)
                canvasGroup.DOFade(0, 0.2f);
            return;
        }

        if (canvasGroup.alpha <= 0.01f) canvasGroup.DOFade(1, 0.2f);

        currentColor = Color.Lerp(badColor, goodColor, smoothnessDone / requiredSmoothness);
        indicatorImage.color = currentColor;
    }
}