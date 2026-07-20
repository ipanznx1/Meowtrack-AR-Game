using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioUIController : MonoBehaviour
{
    [Header("UI References")]
    public Button muteButton;
    public Slider volumeSlider;
    public TextMeshProUGUI volumeTextDisplay; // Optional: to show volume percentage

    private AudioManager audioManager;
    private Image muteButtonImage;
    private Color mutedColor = Color.red;
    private Color unMutedColor = Color.white;

    void Start()
    {
        // Get reference to AudioManager
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("[AudioUIController] AudioManager not found!");
            return;
        }

        // Get button image component
        if (muteButton != null)
        {
            muteButtonImage = muteButton.GetComponent<Image>();
            muteButton.onClick.AddListener(OnMuteButtonClicked);
        }

        // Setup volume slider
        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.value = audioManager.GetVolume();
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        // Update initial UI state
        UpdateMuteButtonUI();
    }

    // Called when mute button is clicked
    public void OnMuteButtonClicked()
    {
        if (audioManager != null)
        {
            audioManager.ToggleMute();
            UpdateMuteButtonUI();
        }
    }

    // Called when volume slider is changed
    public void OnVolumeChanged(float newVolume)
    {
        if (audioManager != null)
        {
            audioManager.SetVolume(newVolume);
            UpdateVolumeDisplay();
        }
    }

    // Update mute button appearance
    private void UpdateMuteButtonUI()
    {
        if (muteButton == null || muteButtonImage == null) return;

        if (audioManager.IsMuted())
        {
            muteButtonImage.color = mutedColor;
            muteButton.GetComponentInChildren<TextMeshProUGUI>().text = "✕"; // X symbol when muted
        }
        else
        {
            muteButtonImage.color = unMutedColor;
            muteButton.GetComponentInChildren<TextMeshProUGUI>().text = "♪"; // Music note when playing
        }
    }

    // Update volume display text
    private void UpdateVolumeDisplay()
    {
        if (volumeTextDisplay != null)
        {
            int volumePercent = Mathf.RoundToInt(audioManager.GetVolume() * 100f);
            volumeTextDisplay.text = $"{volumePercent}%";
        }
    }
}
