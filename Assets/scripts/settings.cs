using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameSettingsManager : MonoBehaviour
{
    [Header("Resolution Settings")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    
    [Header("Audio Settings")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private TextMeshProUGUI volumeText;
    
    [Header("Buttons")]
    [SerializeField] private Button applyButton;
    [SerializeField] private Button resetButton;
    
    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;
    
    private int currentResolutionIndex;
    private bool isFullscreen;
    private float currentVolume;
    
    // Доступные разрешения
    private readonly Vector2Int[] targetResolutions = {
        new Vector2Int(1920, 1080),
        new Vector2Int(1366, 768),
        new Vector2Int(1280, 720),
        new Vector2Int(800, 600)
    };

    void Start()
    {
        InitializeResolutions();
        LoadSettings();
        SetupUIListeners();
    }
    
    void InitializeResolutions()
    {
        resolutionDropdown.ClearOptions();
        
        List<string> options = new List<string>();
        filteredResolutions = new List<Resolution>();
        
        // Фильтруем только нужные разрешения
        foreach (var targetRes in targetResolutions)
        {
            // Ищем подходящее разрешение среди доступных
            foreach (var res in Screen.resolutions)
            {
                if (res.width == targetRes.x && res.height == targetRes.y)
                {
                    filteredResolutions.Add(res);
                    options.Add($"{res.width} x {res.height}");
                    break;
                }
            }
        }
        
        // Если не нашли нужные разрешения, создаем их вручную
        if (filteredResolutions.Count == 0)
        {
            foreach (var targetRes in targetResolutions)
            {
                Resolution newRes = new Resolution();
                newRes.width = targetRes.x;
                newRes.height = targetRes.y;
                newRes.refreshRate = Screen.currentResolution.refreshRate;
                
                filteredResolutions.Add(newRes);
                options.Add($"{targetRes.x} x {targetRes.y}");
            }
        }
        
        resolutionDropdown.AddOptions(options);
    }
    
    void SetupUIListeners()
    {
        // Разрешение
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        
        // Полноэкранный режим
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggle);
        
        // Громкость
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        
        // Кнопки
        applyButton.onClick.AddListener(ApplySettings);
        resetButton.onClick.AddListener(ResetSettings);
    }
    
    void OnResolutionChanged(int index)
    {
        currentResolutionIndex = index;
        UpdateApplyButton();
    }
    
    void OnFullscreenToggle(bool isOn)
    {
        isFullscreen = isOn;
        UpdateApplyButton();
    }
    
    void OnVolumeChanged(float volume)
    {
        currentVolume = volume;
        UpdateVolumeDisplay();
        // Применяем громкость сразу
        musicSource.volume = currentVolume;
    }
    
    void UpdateVolumeDisplay()
    {
        if (volumeText != null)
        {
            volumeText.text = $"{Mathf.RoundToInt(currentVolume * 100)}%";
        }
    }
    
    void UpdateApplyButton()
    {
        applyButton.interactable = true;
    }
    
    public void ApplySettings()
    {
        // Применяем разрешение и режим окна
        Resolution selectedResolution = filteredResolutions[currentResolutionIndex];
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, isFullscreen);
        
        // Сохраняем настройки
        SaveSettings();
        
        // Делаем кнопку неактивной
        applyButton.interactable = false;
        
        Debug.Log($"Settings applied: {selectedResolution.width}x{selectedResolution.height}, Fullscreen: {isFullscreen}");
    }
    
    public void ResetSettings()
    {
        // Сбрасываем к настройкам по умолчанию
        currentResolutionIndex = 0; // 1920x1080
        isFullscreen = true;
        currentVolume = 0.7f;
        
        // Обновляем UI
        resolutionDropdown.value = currentResolutionIndex;
        fullscreenToggle.isOn = isFullscreen;
        volumeSlider.value = currentVolume;
        
        // Применяем сброшенные настройки
        ApplySettings();
        
        Debug.Log("Settings reset to default");
    }
    
    void SaveSettings()
    {
        PlayerPrefs.SetInt("ResolutionIndex", currentResolutionIndex);
        PlayerPrefs.SetInt("IsFullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.SetFloat("MasterVolume", currentVolume);
        PlayerPrefs.Save();
    }
    
    void LoadSettings()
    {
        // Загружаем сохраненные настройки или используем значения по умолчанию
        currentResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", 0);
        isFullscreen = PlayerPrefs.GetInt("IsFullscreen", 1) == 1;
        currentVolume = PlayerPrefs.GetFloat("MasterVolume", 0.7f);
        
        // Применяем загруженные настройки к UI
        resolutionDropdown.value = currentResolutionIndex;
        fullscreenToggle.isOn = isFullscreen;
        volumeSlider.value = currentVolume;
        
        // Применяем настройки
        ApplySettings();
        
        // Делаем кнопку неактивной после загрузки
        applyButton.interactable = false;
    }
    
    void OnDestroy()
    {
        // Отписываемся от событий
        resolutionDropdown.onValueChanged.RemoveListener(OnResolutionChanged);
        fullscreenToggle.onValueChanged.RemoveListener(OnFullscreenToggle);
        volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
        applyButton.onClick.RemoveListener(ApplySettings);
        resetButton.onClick.RemoveListener(ResetSettings);
    }
}