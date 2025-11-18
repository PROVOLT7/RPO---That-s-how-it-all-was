using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[System.Serializable]
public class GameProgress
{
    public int currentChapter = 0;
    public int currentDialogueIndex = 0;
    public string currentSceneName = "";
    public List<bool> unlockedChapters = new List<bool> { true };
}

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance;
    public GameProgress currentProgress;

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
        Debug.Log($"Scene loaded: {scene.name}");
        
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
        
        if (chapter < currentProgress.unlockedChapters.Count - 1)
        {
            currentProgress.unlockedChapters[chapter + 1] = true;
        }
        
        SaveToPlayerPrefs();
        Debug.Log($"Game saved: Chapter {chapter}, Dialogue {dialogueIndex}, Scene: {currentProgress.currentSceneName}");
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
            for (int i = 0; i < 10; i++)
            {
                currentProgress.unlockedChapters.Add(i == 0);
            }
        }
    }

    public void ResetProgress()
    {
        currentProgress = new GameProgress();
        for (int i = 0; i < 10; i++)
        {
            currentProgress.unlockedChapters.Add(i == 0);
        }
        PlayerPrefs.DeleteKey("NovelProgress");
        PlayerPrefs.Save();
        Debug.Log("Progress reset to default");
    }

    public bool IsChapterUnlocked(int chapterIndex)
    {
        if (chapterIndex < currentProgress.unlockedChapters.Count)
        {
            return currentProgress.unlockedChapters[chapterIndex];
        }
        return false;
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
        else
        {
            Debug.LogError("No saved scene found!");
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}