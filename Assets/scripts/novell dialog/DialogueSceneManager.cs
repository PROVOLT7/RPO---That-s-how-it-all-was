using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogueSceneManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI chapterText;
    [SerializeField] private TextMeshProUGUI dialogueProgressText;
    [SerializeField] private Button backToMenuButton;
    [SerializeField] private Button nextDialogueButton;

    private int currentDialogueIndex = 0;

    void Start()
    {
        // Загружаем текущий прогресс
        currentDialogueIndex = SaveSystem.Instance.currentProgress.currentDialogueIndex;

        UpdateUI();

        backToMenuButton.onClick.AddListener(ReturnToMainMenu);
        nextDialogueButton.onClick.AddListener(GoToNextDialogue);
    }

    void UpdateUI()
    {
        chapterText.text = $"Глава {SaveSystem.Instance.currentProgress.currentChapter + 1}";
        dialogueProgressText.text = $"Диалог: {currentDialogueIndex + 1}/{SaveSystem.Instance.dialoguesPerChapter}";

        // Проверяем завершение главы
        if (currentDialogueIndex >= SaveSystem.Instance.dialoguesPerChapter)
        {
            dialogueProgressText.text += " ✓ ГЛАВА ЗАВЕРШЕНА!";
            dialogueProgressText.color = Color.green;
        }
    }

    void GoToNextDialogue()
    {
        currentDialogueIndex++;

        // Сохраняем прогресс
        SaveSystem.Instance.SaveGame(
            SaveSystem.Instance.currentProgress.currentChapter,
            currentDialogueIndex,
            "DialogueScene"
        );

        Debug.Log($"Dialogue progress: {currentDialogueIndex}/{SaveSystem.Instance.dialoguesPerChapter}");

        UpdateUI();

        // Если глава завершена, показываем сообщение
        if (currentDialogueIndex >= SaveSystem.Instance.dialoguesPerChapter)
        {
            Debug.Log($"CHAPTER {SaveSystem.Instance.currentProgress.currentChapter + 1} COMPLETED!");
            Debug.Log($"Next chapter unlocked: {SaveSystem.Instance.IsChapterUnlocked(SaveSystem.Instance.currentProgress.currentChapter + 1)}");
        }
    }

    void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}