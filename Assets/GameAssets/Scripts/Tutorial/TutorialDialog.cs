using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TutorialDialog : MonoBehaviour
{
    public List<GameObject> windows = new();
    public int currentWindow = 0;

    private List<Vector3> originalPositions = new();
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        // Ensure we have a CanvasGroup for fading
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        windows.Clear();
        originalPositions.Clear();

        foreach (Transform child in transform)
        {
            windows.Add(child.gameObject);
            originalPositions.Add(child.localPosition);
            child.gameObject.SetActive(false);
        }

        // Show first window
        var firstWindow = windows[currentWindow];
        firstWindow.SetActive(true);
        firstWindow.transform.localPosition = originalPositions[currentWindow];

        // Force alpha to 0 instantly
        canvasGroup.alpha = 0f;

        // Fade in quickly (0.25s)
        canvasGroup.DOFade(1f, 0.25f).SetEase(Ease.OutQuad);
    }

    public void SetActiveWindow(int index)
    {
        if (index < 0 || index >= windows.Count)
        {
            Debug.LogError("Invalid window index");
            return;
        }

        var prevPos = windows[currentWindow].transform.localPosition;
        windows[currentWindow].SetActive(false);

        currentWindow = index;
        var newWindow = windows[index];
        newWindow.SetActive(true);

        newWindow.transform.localPosition = prevPos;
        newWindow.transform.DOLocalMove(originalPositions[index], 0.5f).SetEase(Ease.OutQuad);
    }

    public void Finish()
    {
        canvasGroup.DOFade(0f, 0.5f).SetEase(Ease.InQuad).OnComplete(() => { gameObject.SetActive(false); });
    }
}