using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button chaptersButton;
    [SerializeField] private Button resetButton; // ДОБАВИЛИ КНОПКУ СБРОСА
    [SerializeField] private Button exitButton;

    [Header("Reset Confirmation")]
    [SerializeField] private GameObject resetConfirmationPanel; // Панель подтверждения сброса
    [SerializeField] private Button confirmResetButton;
    [SerializeField] private Button cancelResetButton;

    private ChapterMenu chapterMenu;

    void Start()
    {
        chapterMenu = FindObjectOfType<ChapterMenu>();

        continueButton.onClick.AddListener(OnContinue);
        chaptersButton.onClick.AddListener(OnChapters);
        resetButton.onClick.AddListener(OnResetClicked); // ОБРАБОТЧИК СБРОСА
        exitButton.onClick.AddListener(OnExit);

        // Настраиваем кнопки подтверждения сброса
        confirmResetButton.onClick.AddListener(OnConfirmReset);
        cancelResetButton.onClick.AddListener(OnCancelReset);

        // Скрываем панель подтверждения
        resetConfirmationPanel.SetActive(false);

        continueButton.interactable = SaveSystem.Instance.HasSaveGame();
    }

    void OnContinue()
    {
        chapterMenu.OnContinueButtonClicked();
    }

    void OnChapters()
    {
        chapterMenu.ShowChapterMenu();
    }

    // НОВЫЙ МЕТОД: при нажатии на сброс
    void OnResetClicked()
    {
        Debug.Log("Reset button clicked");
        resetConfirmationPanel.SetActive(true);
    }

    // НОВЫЙ МЕТОД: подтверждение сброса
    void OnConfirmReset()
    {
        Debug.Log("Resetting progress...");
        SaveSystem.Instance.ResetProgress();
        resetConfirmationPanel.SetActive(false);

        // Обновляем состояние кнопок
        continueButton.interactable = false;

        Debug.Log("Progress reset complete");
    }

    // НОВЫЙ МЕТОД: отмена сброса
    void OnCancelReset()
    {
        resetConfirmationPanel.SetActive(false);
    }

    void OnExit()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}