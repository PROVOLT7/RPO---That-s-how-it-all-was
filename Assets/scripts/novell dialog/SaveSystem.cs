using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[System.Serializable]
public class GameProgress
{
    public int currentChapter = 0;
    public int currentDialogueIndex = 0;
    public string currentSceneName = "";
    public List<bool> unlockedChapters = new List<bool> { true }; // Только глава 1 открыта
    public List<bool> completedChapters = new List<bool>(); // Какие главы пройдены
}

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance;
    public GameProgress currentProgress;

    [Header("Chapter Settings")]
    public int dialoguesPerChapter = 10; // Сколько диалогов в каждой главе

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadProgress();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!scene.name.Contains("Menu"))
        {
            currentProgress.currentSceneName = scene.name;
            SaveToPlayerPrefs();
        }
    }

    public void SaveGame(int chapter, int dialogueIndex, string sceneName = "")
    {
        currentProgress.currentChapter = chapter;
        currentProgress.currentDialogueIndex = dialogueIndex;

        if (!string.IsNullOrEmpty(sceneName))
        {
            currentProgress.currentSceneName = sceneName;
        }

        // ПРОВЕРЯЕМ ЗАВЕРШЕНИЕ ГЛАВЫ
        if (dialogueIndex >= dialoguesPerChapter)
        {
            MarkChapterCompleted(chapter);
            Debug.Log($"Chapter {chapter} completed!");
        }

        SaveToPlayerPrefs();
        Debug.Log($"Game saved: Chapter {chapter}, Dialogue {dialogueIndex}/{dialoguesPerChapter}");
    }

    // НОВЫЙ МЕТОД: отмечаем главу как завершенную и открываем следующую
    void MarkChapterCompleted(int chapter)
    {
        // Отмечаем текущую главу как завершенную
        if (chapter < currentProgress.completedChapters.Count)
        {
            currentProgress.completedChapters[chapter] = true;
        }

        // Открываем следующую главу
        int nextChapter = chapter + 1;
        if (nextChapter < currentProgress.unlockedChapters.Count)
        {
            currentProgress.unlockedChapters[nextChapter] = true;
            Debug.Log($"Chapter {nextChapter} unlocked!");
        }
    }

    void SaveToPlayerPrefs()
    {
        string json = JsonUtility.ToJson(currentProgress);
        PlayerPrefs.SetString("NovelProgress", json);
        PlayerPrefs.Save();
    }

    public void LoadProgress()
    {
        string json = PlayerPrefs.GetString("NovelProgress", "");

        if (!string.IsNullOrEmpty(json))
        {
            currentProgress = JsonUtility.FromJson<GameProgress>(json);
        }
        else
        {
            currentProgress = new GameProgress();
            // Инициализируем 4 главы
            for (int i = 0; i < 4; i++)
            {
                currentProgress.unlockedChapters.Add(i == 0); // Только глава 0 открыта
                currentProgress.completedChapters.Add(false); // Ни одна не завершена
            }
        }
    }

    public void ResetProgress()
    {
        currentProgress = new GameProgress();
        for (int i = 0; i < 4; i++)
        {
            currentProgress.unlockedChapters.Add(i == 0);
            currentProgress.completedChapters.Add(false);
        }
        PlayerPrefs.DeleteKey("NovelProgress");
        PlayerPrefs.Save();
        Debug.Log("Progress reset to default");
    }

    public bool IsChapterUnlocked(int chapterIndex)
    {
        return chapterIndex < currentProgress.unlockedChapters.Count &&
               currentProgress.unlockedChapters[chapterIndex];
    }

    public bool IsChapterCompleted(int chapterIndex)
    {
        return chapterIndex < currentProgress.completedChapters.Count &&
               currentProgress.completedChapters[chapterIndex];
    }

    public bool HasSaveGame()
    {
        return currentProgress.currentDialogueIndex > 0 &&
               !string.IsNullOrEmpty(currentProgress.currentSceneName);
    }

    public void LoadSavedScene()
    {
        if (HasSaveGame() && !string.IsNullOrEmpty(currentProgress.currentSceneName))
        {
            SceneManager.LoadScene(currentProgress.currentSceneName);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}