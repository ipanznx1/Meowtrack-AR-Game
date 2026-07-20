using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Helper script to automatically setup the audio system UI
/// Attach this to an empty GameObject and set it to "DontDestroyOnLoad"
/// Then call SetupAudioUI() from the menu or inspector
/// </summary>
public class AudioSetupHelper : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip backgroundMusicClip;
    [Range(0f, 1f)] public float defaultVolume = 0.5f;

    [Header("UI Canvas")]
    public Canvas targetCanvas;

    [ContextMenu("Setup Audio UI")]
    public void SetupAudioUI()
    {
        if (targetCanvas == null)
        {
            Debug.LogError("[AudioSetupHelper] Target Canvas not assigned!");
            return;
        }

        // Step 1: Create AudioManager
        GameObject audioManagerGO = new GameObject("AudioManager");
        AudioManager audioManager = audioManagerGO.AddComponent<AudioManager>();
        audioManager.backgroundMusicClip = backgroundMusicClip;
        audioManager.defaultVolume = defaultVolume;
        
        // Only call DontDestroyOnLoad during play mode
        if (Application.isPlaying)
        {
            DontDestroyOnLoad(audioManagerGO);
        }
        Debug.Log("[AudioSetupHelper] AudioManager created");

        // Step 2: Create Mute Button
        Button muteButton = CreateButton(targetCanvas, "MuteButton", 
            new Vector2(50, -50), new Vector2(60, 60), "🔊");
        Debug.Log("[AudioSetupHelper] Mute button created");

        // Step 3: Create Volume Slider
        Slider volumeSlider = CreateSlider(targetCanvas, "VolumeSlider",
            new Vector2(-120, -50), new Vector2(200, 30));
        Debug.Log("[AudioSetupHelper] Volume slider created");

        // Step 4: Create Volume Text
        TextMeshProUGUI volumeText = CreateText(targetCanvas, "VolumeText",
            new Vector2(-340, -50), new Vector2(50, 30), "50%");
        Debug.Log("[AudioSetupHelper] Volume text created");

        // Step 5: Create AudioUIController
        GameObject audioUIControllerGO = new GameObject("AudioUIController");
        audioUIControllerGO.transform.SetParent(targetCanvas.transform, false);
        AudioUIController audioUIController = audioUIControllerGO.AddComponent<AudioUIController>();
        audioUIController.muteButton = muteButton;
        audioUIController.volumeSlider = volumeSlider;
        audioUIController.volumeTextDisplay = volumeText;
        Debug.Log("[AudioSetupHelper] AudioUIController created and configured");

        Debug.Log("[AudioSetupHelper] ✅ Audio system setup complete!");
    }

    private Button CreateButton(Canvas canvas, string name, Vector2 position, Vector2 size, string buttonText)
    {
        // Create button GameObject
        GameObject buttonGO = new GameObject(name);
        buttonGO.transform.SetParent(canvas.transform, false);

        // Setup RectTransform
        RectTransform rectTransform = buttonGO.AddComponent<RectTransform>();
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = size;

        // Add Image component
        Image image = buttonGO.AddComponent<Image>();
        image.color = Color.white;

        // Add Button component
        Button button = buttonGO.AddComponent<Button>();

        // Create text child
        GameObject textGO = new GameObject("Text (TMP)");
        textGO.transform.SetParent(buttonGO.transform, false);
        RectTransform textRect = textGO.AddComponent<RectTransform>();
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TextMeshProUGUI textMesh = textGO.AddComponent<TextMeshProUGUI>();
        textMesh.text = "♪"; // Music note symbol - universal support
        textMesh.fontSize = 36;
        textMesh.alignment = TextAlignmentOptions.Center;

        return button;
    }

    private Slider CreateSlider(Canvas canvas, string name, Vector2 position, Vector2 size)
    {
        // Create slider GameObject
        GameObject sliderGO = new GameObject(name);
        sliderGO.transform.SetParent(canvas.transform, false);

        // Setup RectTransform
        RectTransform rectTransform = sliderGO.AddComponent<RectTransform>();
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = size;

        // Add Image component
        Image image = sliderGO.AddComponent<Image>();
        image.color = new Color(0.1f, 0.1f, 0.1f, 0.5f);

        // Add Slider component
        Slider slider = sliderGO.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 0.5f;

        // Create background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(sliderGO.transform, false);
        RectTransform bgRect = background.AddComponent<RectTransform>();
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);

        // Create fill
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(sliderGO.transform, false);
        RectTransform fillRect = fill.AddComponent<RectTransform>();
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(0.2f, 0.8f, 1f, 1f);

        slider.fillRect = fillRect;

        return slider;
    }

    private TextMeshProUGUI CreateText(Canvas canvas, string name, Vector2 position, Vector2 size, string text)
    {
        // Create text GameObject
        GameObject textGO = new GameObject(name);
        textGO.transform.SetParent(canvas.transform, false);

        // Setup RectTransform
        RectTransform rectTransform = textGO.AddComponent<RectTransform>();
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = size;

        // Add TextMeshProUGUI
        TextMeshProUGUI textMesh = textGO.AddComponent<TextMeshProUGUI>();
        textMesh.text = text;
        textMesh.fontSize = 24;
        textMesh.alignment = TextAlignmentOptions.Center;

        return textMesh;
    }
}
