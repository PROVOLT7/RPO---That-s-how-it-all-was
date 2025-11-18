using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChapterButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI chapterText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Image lockIcon;
    [SerializeField] private Image completedIcon;

    [Header("Settings")]
    [SerializeField] private string chapterName = "Chapter";
    [SerializeField] private Color unlockedColor = Color.white;
    [SerializeField] private Color lockedColor = Color.gray;

    public void UpdateButtonState(int chapterIndex)
    {
        bool isUnlocked = SaveSystem.Instance.IsChapterUnlocked(chapterIndex);
        bool isCompleted = chapterIndex < SaveSystem.Instance.currentProgress.currentChapter;

        button.interactable = isUnlocked;
        chapterText.text = $"{chapterName} {chapterIndex + 1}";
        chapterText.color = isUnlocked ? unlockedColor : lockedColor;

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
                statusText.color = unlockedColor;
            }
            else
            {
                statusText.text = "Заблокирована";
                statusText.color = lockedColor;
            }
        }

        if (lockIcon != null)
        {
            lockIcon.gameObject.SetActive(!isUnlocked);
        }

        if (completedIcon != null)
        {
            completedIcon.gameObject.SetActive(isCompleted);
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => FindObjectOfType<ChapterMenu>().StartChapter(chapterIndex));
    }
}