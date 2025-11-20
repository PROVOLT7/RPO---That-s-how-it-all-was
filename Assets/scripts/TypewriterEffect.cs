using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [SerializeField] private List<TextMeshProUGUI> dialogueTexts = new List<TextMeshProUGUI>();
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private bool startOnAwake = false;

    [Header("Navigation Settings")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button skipButton;
    [SerializeField] private Button endDialogueButton;
    [SerializeField] private KeyCode continueKey = KeyCode.Space;
    [SerializeField] private KeyCode skipKey = KeyCode.Escape;

    [Header("Text Order Settings")]
    [SerializeField] private bool hidePreviousTexts = true;

    [Header("Debug")]
    [SerializeField] private bool debugMode = true;

    private List<string> originalTexts = new List<string>();
    private Coroutine currentTypingCoroutine;
    private int currentTextIndex = -1;
    private bool isDialogueActive = false;

    void Awake()
    {
        // Сохраняем оригинальные тексты и очищаем поля
        foreach (var textElement in dialogueTexts)
        {
            if (textElement != null)
            {
                originalTexts.Add(textElement.text);
                textElement.text = "";
                textElement.gameObject.SetActive(false);
            }
            else
            {
                originalTexts.Add("");
            }
        }

        // Настраиваем кнопки
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(ContinueToNextText);
            continueButton.gameObject.SetActive(false);
        }

        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipCurrentText);
            skipButton.gameObject.SetActive(false);
        }

        if (endDialogueButton != null)
        {
            endDialogueButton.gameObject.SetActive(false);
        }

        if (startOnAwake && dialogueTexts.Count > 0)
        {
            StartDialogue();
        }
    }

    void Update()
    {
        if (!isDialogueActive) return;

        if (Input.GetKeyDown(continueKey))
        {
            ContinueToNextText();
        }

        if (Input.GetKeyDown(skipKey))
        {
            SkipCurrentText();
        }
    }

    // Начать весь диалог
    public void StartDialogue()
    {
        if (dialogueTexts.Count == 0)
        {
            Debug.LogWarning("No text elements found!");
            return;
        }

        if (debugMode) Debug.Log("Starting dialogue");

        isDialogueActive = true;
        currentTextIndex = -1;

        // Скрываем кнопку завершения при старте диалога
        if (endDialogueButton != null)
            endDialogueButton.gameObject.SetActive(false);

        // Показываем кнопку скипа при старте диалога
        if (skipButton != null)
            skipButton.gameObject.SetActive(true);

        ContinueToNextText();
    }

    // Продолжить к следующему тексту
    public void ContinueToNextText()
    {
        if (!isDialogueActive) return;

        // Если сейчас печатается текст, завершаем его
        if (currentTypingCoroutine != null)
        {
            FinishCurrentText();
            return;
        }

        currentTextIndex++;

        if (currentTextIndex >= dialogueTexts.Count)
        {
            EndDialogue();
            return;
        }

        StartTextTyping(currentTextIndex);
    }

    // Запустить печать конкретного текста
    private void StartTextTyping(int textIndex)
    {
        if (textIndex < 0 || textIndex >= dialogueTexts.Count)
            return;

        var textElement = dialogueTexts[textIndex];
        if (textElement == null) return;

        // Скрываем предыдущие тексты если нужно
        if (hidePreviousTexts)
        {
            for (int i = 0; i < dialogueTexts.Count; i++)
            {
                if (dialogueTexts[i] != null)
                    dialogueTexts[i].gameObject.SetActive(i == textIndex);
            }
        }

        textElement.gameObject.SetActive(true);
        textElement.text = "";

        // 🔥 ЛОГИКА КНОПОК ВО ВРЕМЯ ПЕЧАТИ:
        // ContinueButton = скрыт, SkipButton = показан, EndButton = скрыт
        if (continueButton != null)
            continueButton.gameObject.SetActive(false);

        if (skipButton != null)
            skipButton.gameObject.SetActive(true);

        if (endDialogueButton != null)
            endDialogueButton.gameObject.SetActive(false);

        currentTypingCoroutine = StartCoroutine(TypeText(textElement, originalTexts[textIndex]));
    }

    // Корутина печати текста
    private IEnumerator TypeText(TextMeshProUGUI textElement, string text)
    {
        if (debugMode) Debug.Log($"Typing text {currentTextIndex}");

        for (int i = 0; i <= text.Length; i++)
        {
            if (textElement == null) yield break;

            string currentText = text.Substring(0, i);
            textElement.text = currentText;
            yield return new WaitForSeconds(typingSpeed);
        }

        currentTypingCoroutine = null;

        // 🔥 ЛОГИКА КНОПОК ПОСЛЕ ПЕЧАТИ:
        // ContinueButton = показан, SkipButton = скрыт, EndButton = скрыт
        if (continueButton != null)
            continueButton.gameObject.SetActive(true);

        if (skipButton != null)
            skipButton.gameObject.SetActive(false);

        if (endDialogueButton != null)
            endDialogueButton.gameObject.SetActive(false);

        if (debugMode) Debug.Log($"Text {currentTextIndex} typing completed");
    }

    // Пропустить ТЕКУЩИЙ текст
    public void SkipCurrentText()
    {
        if (!isDialogueActive || currentTypingCoroutine == null) return;

        if (debugMode) Debug.Log("Skipping current text animation");

        if (currentTypingCoroutine != null)
        {
            StopCoroutine(currentTypingCoroutine);
            currentTypingCoroutine = null;
        }

        if (currentTextIndex >= 0 && currentTextIndex < dialogueTexts.Count)
        {
            var textElement = dialogueTexts[currentTextIndex];
            if (textElement != null)
            {
                textElement.text = originalTexts[currentTextIndex];
            }
        }

        // 🔥 ЛОГИКА КНОПОК ПОСЛЕ СКИПА:
        // ContinueButton = показан, SkipButton = скрыт, EndButton = скрыт
        if (continueButton != null)
            continueButton.gameObject.SetActive(true);

        if (skipButton != null)
            skipButton.gameObject.SetActive(false);

        if (endDialogueButton != null)
            endDialogueButton.gameObject.SetActive(false);
    }

    // Завершить текущую печать
    private void FinishCurrentText()
    {
        if (currentTypingCoroutine != null)
        {
            StopCoroutine(currentTypingCoroutine);
            currentTypingCoroutine = null;
        }

        if (currentTextIndex >= 0 && currentTextIndex < dialogueTexts.Count)
        {
            var textElement = dialogueTexts[currentTextIndex];
            if (textElement != null)
            {
                textElement.text = originalTexts[currentTextIndex];
            }
        }

        // 🔥 ЛОГИКА КНОПОК ПОСЛЕ ЗАВЕРШЕНИЯ:
        // ContinueButton = показан, SkipButton = скрыт, EndButton = скрыт
        if (continueButton != null)
            continueButton.gameObject.SetActive(true);

        if (skipButton != null)
            skipButton.gameObject.SetActive(false);

        if (endDialogueButton != null)
            endDialogueButton.gameObject.SetActive(false);
    }

    // Завершить диалог
    private void EndDialogue()
    {
        if (debugMode) Debug.Log("Dialogue ended");

        isDialogueActive = false;

        // 🔥 ИСПРАВЛЕНИЕ: Скрываем ВСЕ тексты при завершении диалога
        if (hidePreviousTexts)
        {
            for (int i = 0; i < dialogueTexts.Count; i++)
            {
                if (dialogueTexts[i] != null)
                    dialogueTexts[i].gameObject.SetActive(false);
            }
        }

        currentTextIndex = -1;

        // 🔥 ЛОГИКА КНОПОК ПРИ ЗАВЕРШЕНИИ ДИАЛОГА:
        // ContinueButton = скрыт, SkipButton = скрыт, EndButton = показан
        if (continueButton != null)
            continueButton.gameObject.SetActive(false);

        if (skipButton != null)
            skipButton.gameObject.SetActive(false);

        if (endDialogueButton != null)
            endDialogueButton.gameObject.SetActive(true);

        // Вызываем событие завершения диалога
        OnDialogueEnd?.Invoke();
    }

    // Событие для внешних скриптов
    public System.Action OnDialogueEnd;

    // PUBLIC МЕТОДЫ ДЛЯ УПРАВЛЕНИЯ КНОПКОЙ ЗАВЕРШЕНИЯ

    // Установить действие для кнопки завершения
    public void SetEndButtonAction(System.Action action)
    {
        if (endDialogueButton != null)
        {
            endDialogueButton.onClick.RemoveAllListeners();
            endDialogueButton.onClick.AddListener(() => action?.Invoke());
        }
    }

    // Показать/скрыть кнопку завершения вручную
    public void SetEndButtonVisible(bool visible)
    {
        if (endDialogueButton != null)
            endDialogueButton.gameObject.SetActive(visible);
    }

    public void GoToText(int textIndex)
    {
        if (textIndex >= 0 && textIndex < dialogueTexts.Count)
        {
            if (currentTypingCoroutine != null)
            {
                StopCoroutine(currentTypingCoroutine);
                currentTypingCoroutine = null;
            }

            currentTextIndex = textIndex - 1;
            ContinueToNextText();
        }
    }

    public bool IsDialogueActive => isDialogueActive;
    public int CurrentTextIndex => currentTextIndex;
    public int TotalTextCount => dialogueTexts.Count;

    void OnDestroy()
    {
        if (continueButton != null)
            continueButton.onClick.RemoveListener(ContinueToNextText);
        if (skipButton != null)
            skipButton.onClick.RemoveListener(SkipCurrentText);
    }
}