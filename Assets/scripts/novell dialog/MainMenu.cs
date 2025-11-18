using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button chaptersButton;
    [SerializeField] private Button exitButton;

    [Header("Managers")]
    [SerializeField] private ChapterMenu chapterMenu;

    void Start()
    {
        continueButton.onClick.AddListener(OnContinue);
        chaptersButton.onClick.AddListener(OnChapters);
        exitButton.onClick.AddListener(OnExit);

        continueButton.interactable = SaveSystem.Instance.HasSaveGame();
    }

    void OnContinue()
    {
        Debug.Log("Continue game clicked");
        chapterMenu.OnContinueButtonClicked();
    }

    void OnChapters()
    {
        Debug.Log("Chapter selection opened");
        chapterMenu.ShowChapterMenu();
    }

    void OnExit()
    {
        Debug.Log("Game exited");
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}