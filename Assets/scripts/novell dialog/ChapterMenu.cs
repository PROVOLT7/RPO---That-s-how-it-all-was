using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ChapterMenu : MonoBehaviour
{
    [Header("Chapter Settings")]
    [SerializeField] private ChapterButton[] chapterButtons;
    
    [Header("UI References")]
    [SerializeField] private Button continueButton;
    [SerializeField] private GameObject chapterSelectionPanel;
    [SerializeField] private GameObject overwriteWarningPanel; // Панель подтверждения

    [Header("Warning Panel Elements")]
    [SerializeField] private TextMeshProUGUI warningText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    [Header("Scene Names")]
    [SerializeField] private string[] chapterScenes;
    [SerializeField] private string dialogueSceneName = "DialogueScene"; // Сцена с диалогами

    private int selectedChapterIndex = -1;

    void Start()
    {
        UpdateChapterMenu();
        continueButton.onClick.AddListener(OnContinueButtonClicked);
        
        // Настраиваем кнопки подтверждения
        confirmButton.onClick.AddListener(OnConfirmOverwrite);
        cancelButton.onClick.AddListener(OnCancelOverwrite);
        
        HideOverwriteWarning();
    }

    public void ShowChapterMenu()
    {
        chapterSelectionPanel.SetActive(true);
        UpdateChapterMenu();
    }

    public void HideChapterMenu()
    {
        chapterSelectionPanel.SetActive(false);
    }

    void UpdateChapterMenu()
    {
        continueButton.interactable = SaveSystem.Instance.HasSaveGame();

        for (int i = 0; i < chapterButtons.Length; i++)
        {
            chapterButtons[i].UpdateButtonState(i);
        }
    }

    void OnContinueButtonClicked()
    {
        Debug.Log($"Continuing from saved position");
        SaveSystem.Instance.LoadSavedScene();
        HideChapterMenu();
    }

    public void StartChapter(int chapterIndex)
    {
        if (SaveSystem.Instance.IsChapterUnlocked(chapterIndex))
        {
            // Если есть активное сохранение - показываем предупреждение
            if (SaveSystem.Instance.HasSaveGame() && chapterIndex != SaveSystem.Instance.currentProgress.currentChapter)
            {
                ShowOverwriteWarning(chapterIndex);
            }
            else
            {
                LoadChapterDirectly(chapterIndex);
            }
        }
        else
        {
            Debug.Log($"Chapter {chapterIndex} is locked!");
        }
    }

    void ShowOverwriteWarning(int chapterIndex)
    {
        selectedChapterIndex = chapterIndex;
        overwriteWarningPanel.SetActive(true);
        
        string currentChapter = $"Текущий прогресс: Глава {SaveSystem.Instance.currentProgress.currentChapter + 1}";
        string warningMessage = $"Вы собираетесь начать Главу {chapterIndex + 1} с начала.\n{currentChapter}\n\nВесь текущий прогресс будет потерян!\n\nПродолжить?";
        
        warningText.text = warningMessage;
    }

    void HideOverwriteWarning()
    {
        overwriteWarningPanel.SetActive(false);
        selectedChapterIndex = -1;
    }

    void OnConfirmOverwrite()
    {
        if (selectedChapterIndex != -1)
        {
            LoadChapterDirectly(selectedChapterIndex);
        }
        HideOverwriteWarning();
    }

    void OnCancelOverwrite()
    {
        HideOverwriteWarning();
    }

    void LoadChapterDirectly(int chapterIndex)
    {
        Debug.Log($"Starting Chapter {chapterIndex}");
        
        // Сохраняем прогресс и загружаем диалоговую сцену
        SaveSystem.Instance.SaveGame(chapterIndex, 0, dialogueSceneName);
        SceneManager.LoadScene(dialogueSceneName);
        
        HideChapterMenu();
    }
}