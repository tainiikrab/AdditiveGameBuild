using System;
using System.Collections;
using System.Collections.Generic;
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

    private List<string[]> _dialogBlocks;
    private List<List<string>> _answerBlocks;
    private int _currentBlockIndex;

    private Coroutine textAnimationCoroutine;

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

    public void StartDialog(string name, Sprite icon, string[] dialogLines, string[] answerLines)
    {
        gameObject.SetActive(true);
        ToggleVisibility(true);

        CharactedDialogue.SetActive(true);
        ImportantOrder.SetActive(false);

        if (icon != null) customerIcon.sprite = icon;

        customerName.gameObject.SetActive(true);
        customerName.text = name;

        _dialogBlocks = SplitIntoBlocks(dialogLines);
        Debug.Log(answerLines.Length);
        _answerBlocks = new List<List<string>>();
        foreach (var block in SplitIntoBlocks(answerLines))
            _answerBlocks.Add(new List<string>(block));

        _currentBlockIndex = 0;
        ShowNextDialogBlock();
        Debug.Log($"Диалогов: {_dialogBlocks.Count}, ответных блоков: {_answerBlocks.Count}");
        for (int i = 0; i < _answerBlocks.Count; i++)
        {
            Debug.Log($"Ответный блок {i}: {string.Join(", ", _answerBlocks[i])}");
        }
    }

    private void ShowNextDialogBlock()
    {
        ClearAnswers();

        if (_currentBlockIndex >= _dialogBlocks.Count)
        {
            EndDialog();
            return;
        }

        string[] currentTextBlock = _dialogBlocks[_currentBlockIndex];
        StartCoroutine(ShowDialogBlock(currentTextBlock));
    }

    private IEnumerator ShowDialogBlock(string[] lines)
    {
        foreach (var line in lines)
        {
            yield return StartCoroutine(AnimateText(line));
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        }

        bool hasAnswers = _currentBlockIndex < _answerBlocks.Count && _answerBlocks[_currentBlockIndex].Count > 0;
        if (hasAnswers)
            SpawnAnswerButtonsForCurrentBlock();
        else
        {
            _currentBlockIndex++;
            ShowNextDialogBlock();
        }
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

    private void SpawnAnswerButtonsForCurrentBlock()
    {
        var answers = _answerBlocks[_currentBlockIndex];

        foreach (var answer in answers)
        {
            Button newButton = Instantiate(AnswerPref, answersField.transform);
            newButton.gameObject.SetActive(true);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = answer;

            newButton.onClick.AddListener(() =>
            {
                Debug.Log($"?? Выбран ответ: {answer}");
                HandleAnswerSelection(answer);
            });
        }
        Debug.Log($"Создаю {answers.Count} кнопок для блока {_currentBlockIndex}");
    }

    private void HandleAnswerSelection(string answer)
    {
        _currentBlockIndex++;
        ShowNextDialogBlock();
    }

    private void EndDialog()
    {
        ImportantOrder.SetActive(true);
        StartCoroutine(WaitForDialogClose());
    }

    private IEnumerator WaitForDialogClose()
    {
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        HideDialog();
    }

    public void HideDialog()
    {
        ToggleVisibility(false);
        gameObject.SetActive(false);
        OnDialogClosed?.Invoke();
    }

    public void ClearAnswers()
    {
        foreach (Transform child in answersField.transform)
            Destroy(child.gameObject);
    }

    private List<string[]> SplitIntoBlocks(string[] lines)
    {
        List<string[]> blocks = new List<string[]>();
        List<string> currentBlock = new List<string>();

        foreach (var line in lines)
        {
            string trimmed = line.Trim();

            if (string.IsNullOrWhiteSpace(trimmed) || trimmed.Contains("*"))
            {
                if (currentBlock.Count > 0)
                {
                    blocks.Add(currentBlock.ToArray());
                    currentBlock.Clear();
                }
                continue;
            }

            currentBlock.Add(trimmed);
        }

        if (currentBlock.Count > 0)
            blocks.Add(currentBlock.ToArray());

        return blocks;
    }
}
