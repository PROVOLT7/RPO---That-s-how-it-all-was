using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChapterButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button button;
    [SerializeField] private Image lockIcon;
    [SerializeField] private Image completedIcon;
    [SerializeField] private TextMeshProUGUI statusText; // Добавили статус текст

    public void UpdateButtonState(int chapterIndex)
    {
        bool isUnlocked = SaveSystem.Instance.IsChapterUnlocked(chapterIndex);
        bool isCompleted = SaveSystem.Instance.IsChapterCompleted(chapterIndex);

        Debug.Log($"Chapter {chapterIndex}: Unlocked={isUnlocked}, Completed={isCompleted}");

        // Управляем кнопкой
        button.interactable = isUnlocked;

        // Управляем замком
        if (lockIcon != null)
        {
            lockIcon.gameObject.SetActive(!isUnlocked);
            Debug.Log($"Lock icon for chapter {chapterIndex}: {!isUnlocked}");
        }

        // Управляем галочкой завершения
        if (completedIcon != null)
        {
            completedIcon.gameObject.SetActive(isCompleted);
        }

        // Обновляем текст статуса
        if (statusText != null)
        {
            if (isCompleted)
            {
                statusText.text = "Пройдена";
                statusText.color = Color.green;
            }
            else if (isUnlocked)
            {
                statusText.text = "Доступна";
                statusText.color = Color.white;
            }
            else
            {
                statusText.text = "Заблокирована";
                statusText.color = Color.gray;
            }
        }

        // Назначаем обработчик клика
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => OnChapterButtonClicked(chapterIndex));
    }

    void OnChapterButtonClicked(int chapterIndex)
    {
        Debug.Log($"Chapter button {chapterIndex} clicked!");

        ChapterMenu chapterMenu = FindObjectOfType<ChapterMenu>();
        if (chapterMenu != null)
        {
            chapterMenu.StartChapter(chapterIndex);
        }
    }
}