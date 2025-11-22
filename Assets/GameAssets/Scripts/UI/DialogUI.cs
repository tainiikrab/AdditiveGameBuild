using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogUI : MonoBehaviour
{
    [Header("UI Elements")] public Image customerIcon;
    public TextMeshProUGUI dialogText;
    public TextMeshProUGUI customerName;
    public Button AnswerPref;
    public GridLayoutGroup answersField;
    public GameObject CharactedDialogue;
    public GameObject ImportantOrder;

    [Header("Text Settings")] public float textSpeed = 0.05f;
    public bool skipTextAnimation;

    private CanvasGroup canvasGroup;

    private List<string[]> _dialogBlocks;
    private List<List<string>> _answerBlocks;
    private int _currentBlockIndex;

    private Coroutine currentDialogCoroutine;
    private Coroutine textAnimationCoroutine;

    public event Action OnDialogClosed;
    public event Action OnOrderAccepted;
    public event Action OnOrderRejected;

    public bool IsTextAnimating { get; private set; }
    private bool isVisible = false;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        ToggleVisibility(false);
    }

    public void ToggleVisibility(bool isVisible)
    {
        if (isVisible == this.isVisible) return;
        gameObject.SetActive(isVisible);
        canvasGroup.alpha = isVisible ? 1 : 0;
        canvasGroup.interactable = isVisible;
        canvasGroup.blocksRaycasts = isVisible;
        this.isVisible = isVisible;
    }

    public void StartDialog(string name, Sprite icon, string[] dialogLines, string[] answerLines)
    {
        ToggleVisibility(true);

        CharactedDialogue.SetActive(true);
        ImportantOrder.SetActive(false);

        if (icon != null) customerIcon.sprite = icon;

        customerName.gameObject.SetActive(true);
        customerName.text = name;

        _dialogBlocks = SplitIntoBlocks(dialogLines);
        _answerBlocks = new List<List<string>>();
        foreach (var block in SplitIntoBlocks(answerLines))
            _answerBlocks.Add(new List<string>(block));

        _currentBlockIndex = 0;
        ShowNextDialogBlock();

        // Debug.Log($"��������: {_dialogBlocks.Count}, �������� ������: {_answerBlocks.Count}");
        // for (var i = 0; i < _answerBlocks.Count; i++)
        //     Debug.Log($"�������� ���� {i}: {string.Join(", ", _answerBlocks[i])}");
    }

    private void ShowNextDialogBlock()
    {
        ClearAnswers();

        if (_currentBlockIndex >= _dialogBlocks.Count)
        {
            EndDialog();
            return;
        }

        if (currentDialogCoroutine != null)
            StopCoroutine(currentDialogCoroutine);

        currentDialogCoroutine = StartCoroutine(ShowDialogBlock(_dialogBlocks[_currentBlockIndex]));
    }

    private IEnumerator ShowDialogBlock(string[] lines)
    {
        foreach (var line in lines) yield return StartCoroutine(ShowLine(line));

        var hasAnswers = _currentBlockIndex < _answerBlocks.Count && _answerBlocks[_currentBlockIndex].Count > 0;
        if (hasAnswers)
        {
            SpawnAnswerButtonsForCurrentBlock();
        }
        else
        {
            _currentBlockIndex++;
            ShowNextDialogBlock();
        }
    }

    private IEnumerator ShowLine(string text)
    {
        if (textAnimationCoroutine != null)
            StopCoroutine(textAnimationCoroutine);

        textAnimationCoroutine = StartCoroutine(AnimateText(text));

        var lineComplete = false;
        while (!lineComplete)
        {
            if (IsTextAnimating && Input.GetMouseButtonDown(0))
            {
                dialogText.text = text;
                IsTextAnimating = false;
            }
            else if (!IsTextAnimating && Input.GetMouseButtonDown(0))
            {
                lineComplete = true;
            }

            yield return null;
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

        foreach (var c in text)
        {
            dialogText.text += c;
            yield return new WaitForSeconds(textSpeed);

            if (!IsTextAnimating)
                yield break;
        }

        IsTextAnimating = false;
    }

    private void SpawnAnswerButtonsForCurrentBlock()
    {
        var answers = _answerBlocks[_currentBlockIndex];

        foreach (var answer in answers)
        {
            var newButton = Instantiate(AnswerPref, answersField.transform);
            newButton.gameObject.SetActive(true);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = answer;

            newButton.onClick.AddListener(() =>
            {
                Debug.Log($"������ �����: {answer}");
                HandleAnswerSelection(answer);
            });
        }

        Debug.Log($"������ {answers.Count} ������ ��� ����� {_currentBlockIndex}");
    }

    private void HandleAnswerSelection(string answer)
    {
        _currentBlockIndex++;
        ShowNextDialogBlock();
    }

    private void EndDialog()
    {
        //ImportantOrder.SetActive(true);
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
        var blocks = new List<string[]>();
        var currentBlock = new List<string>();

        foreach (var line in lines)
        {
            var trimmed = line.Trim();

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