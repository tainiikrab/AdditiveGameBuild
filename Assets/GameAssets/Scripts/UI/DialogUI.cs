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
    public GameObject Blur;

    [Header("Text Settings")] 
    public float textSpeed = 0.05f;
    
    [Header("Test Data")]
    [SerializeField] private string[] testAnswers = { "Да", "Нет", "Может быть" };
    [SerializeField] private string[] testDialog = { "Привет!", "Как дела?" };

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
        Blur.SetActive(true);
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
        foreach (var line in lines) 
            yield return StartCoroutine(ShowLine(line));

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

        yield return new WaitUntil(() => !IsTextAnimating);
    }

    private IEnumerator AnimateText(string text)
    {
        dialogText.text = "";
        IsTextAnimating = true;

        foreach (var c in text)
        {
            if (Input.GetMouseButton(0))
            {
                dialogText.text = text;
                break;
            }

            dialogText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        IsTextAnimating = false;
    }

    private void SpawnAnswerButtonsForCurrentBlock()
    {
        var answers = _answerBlocks[_currentBlockIndex];
        bool isLastBlock = _currentBlockIndex >= _dialogBlocks.Count - 1;

        for (int i = 0; i < answers.Count; i++)
        {
            var answer = answers[i];
            var newButton = Instantiate(AnswerPref, answersField.transform);
            newButton.gameObject.SetActive(true);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = answer;

            bool isLastButton = i == answers.Count - 1;
        
            newButton.onClick.AddListener(() =>
            {
                if (isLastBlock && isLastButton)
                {
                    EndDialog();
                }
                else
                {
                    HandleAnswerSelection(answer);
                }
            });
        }
    }

    private void HandleAnswerSelection(string answer)
    {
        _currentBlockIndex++;
        ShowNextDialogBlock();
    }

    private void EndDialog()
    {
        HideDialog();
    }

    public void HideDialog()
    {
        ToggleVisibility(false);
        Blur.SetActive(false);
        gameObject.SetActive(false);
        OnDialogClosed?.Invoke();
    }

    /// <summary>
    /// Очищает все кнопки ответов
    /// </summary>
    [ContextMenu("Clear Answer Buttons")]
    public void ClearAnswers()
    {
        foreach (Transform child in answersField.transform)
            Destroy(child.gameObject);
    }

    /// <summary>
    /// Создает тестовые кнопки ответов (для проверки в редакторе)
    /// </summary>
    [ContextMenu("Spawn Test Answer Buttons")]
    public void SpawnTestAnswerButtons()
    {
        ClearAnswers();
        
        for (int i = 0; i < testAnswers.Length; i++)
        {
            var answer = testAnswers[i];
            var newButton = Instantiate(AnswerPref, answersField.transform);
            newButton.gameObject.SetActive(true);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = answer;
            
            // Добавляем обработчик клика для тестирования
            newButton.onClick.AddListener(() =>
            {
                Debug.Log($"Выбран ответ: {answer}");
                ClearAnswers();
            });
        }
        
        Debug.Log($"Создано {testAnswers.Length} тестовых кнопок ответов");
    }

    /// <summary>
    /// Запускает тестовый диалог (для проверки в редакторе)
    /// </summary>
    [ContextMenu("Start Test Dialog")]
    public void StartTestDialog()
    {
        StartDialog("Тестовый NPC", null, testDialog, testAnswers);
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