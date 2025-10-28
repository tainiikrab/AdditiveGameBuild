using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image customerIcon;
    public TextMeshProUGUI dialogText;
    public TextMeshProUGUI customerName;
    public Button AnswerPref;
    public GridLayoutGroup answersField;
    public GameObject CharactedDialogue;
    public GameObject ImportantOrder;

    [Header("Text Settings")]
    public float textSpeed = 0.05f;
    public bool skipTextAnimation;

    private CanvasGroup canvasGroup;
    private string[] currentDialogLines;
    private int currentLineIndex;

    private Coroutine textAnimationCoroutine;

    public event Action<int> OnLineFinished;
    public event Action OnDialogClosed;
    public event Action OnOrderAccepted;
    public event Action OnOrderRejected;

    public bool IsTextAnimating { get; private set; }

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void ToggleVisibility(bool isVisible)
    {
        canvasGroup.alpha = isVisible ? 1 : 0;
        canvasGroup.interactable = isVisible;
        canvasGroup.blocksRaycasts = isVisible;
    }

    public void ShowDialog(string[] text, string name, Sprite dialogIcon = null)
    {
        gameObject.SetActive(true);
        CharactedDialogue.SetActive(true);
        customerIcon.gameObject.SetActive(dialogIcon != null);
        dialogText.gameObject.SetActive(true);
        customerName.gameObject.SetActive(true);

        if (dialogIcon != null)
            customerIcon.sprite = dialogIcon;

        customerName.text = name;
        currentDialogLines = text;
        currentLineIndex = 0;

        if (textAnimationCoroutine != null)
            StopCoroutine(textAnimationCoroutine);

        StartCoroutine(AnimateCurrentLine());
    }

    private IEnumerator AnimateCurrentLine()
    {
        yield return StartCoroutine(AnimateText(currentDialogLines[currentLineIndex]));
        OnLineFinished?.Invoke(currentLineIndex);
    }

    private IEnumerator AnimateText(string text)
    {
        dialogText.text = "";
        IsTextAnimating = true;

        if (skipTextAnimation)
        {
            dialogText.text = text;
            IsTextAnimating = false;
            yield break;
        }

        foreach (char letter in text)
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }

        IsTextAnimating = false;
    }

    public void NextLine()
    {
        currentLineIndex++;

        if (currentLineIndex < currentDialogLines.Length)
        {
            StartCoroutine(AnimateCurrentLine());
        }
        else
        {
            ImportantOrder.SetActive(true);
        }
    }

    public void FinishCurrentLine()
    {
        ClearAnswers();
        NextLine();
    }

    public void HideDialog()
    {
        gameObject.SetActive(false);
        OnDialogClosed?.Invoke();
    }

    public void SpawnAnswerButton(string answerText, Action onClick)
    {
        Button newButton = Instantiate(AnswerPref, answersField.transform);
        newButton.gameObject.SetActive(true);
        newButton.GetComponentInChildren<TextMeshProUGUI>().text = answerText;
        newButton.onClick.AddListener(() => onClick?.Invoke());
    }

    public void ClearAnswers()
    {
        foreach (Transform child in answersField.transform)
            Destroy(child.gameObject);
    }
}