using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueSceneManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI chapterText;
    [SerializeField] private Button backToMenuButton;

    void Start()
    {
        // Показываем текущую главу
        chapterText.text = $"Глава {SaveSystem.Instance.currentProgress.currentChapter + 1}";
        
        backToMenuButton.onClick.AddListener(ReturnToMainMenu);
        
        Debug.Log($"Dialogue scene loaded for Chapter {SaveSystem.Instance.currentProgress.currentChapter + 1}");
    }

    void ReturnToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
    
    // Метод для перехода к следующему диалогу (будет использоваться в системе диалогов)
    public void GoToNextDialogue(int dialogueIndex)
    {
        SaveSystem.Instance.SaveGame(
            SaveSystem.Instance.currentProgress.currentChapter, 
            dialogueIndex, 
            "DialogueScene"
        );
    }
}