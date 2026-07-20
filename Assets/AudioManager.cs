using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Singleton instance
    public static AudioManager instance;

    [Header("Audio Sources")]
    private AudioSource backgroundMusicSource;

    [Header("Settings")]
    public AudioClip backgroundMusicClip;
    [Range(0f, 1f)] public float defaultVolume = 0.5f;
    private float currentVolume = 0.5f;
    private bool isMuted = false;

    void Awake()
    {
        // Singleton pattern - ensure only one AudioManager exists
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize audio source if not already present
        backgroundMusicSource = GetComponent<AudioSource>();
        if (backgroundMusicSource == null)
        {
            backgroundMusicSource = gameObject.AddComponent<AudioSource>();
        }

        // Load saved settings
        LoadAudioSettings();

        // Setup background music
        if (backgroundMusicClip != null)
        {
            backgroundMusicSource.clip = backgroundMusicClip;
            backgroundMusicSource.volume = currentVolume;
            backgroundMusicSource.loop = true;
            backgroundMusicSource.playOnAwake = false;
        }
    }

    void Start()
    {
        // Play background music on start
        if (backgroundMusicClip != null && !backgroundMusicSource.isPlaying)
        {
            PlayBackgroundMusic();
        }
    }

    // Play background music
    public void PlayBackgroundMusic()
    {
        if (backgroundMusicSource != null && backgroundMusicClip != null && !backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Play();
            Debug.Log("[AudioManager] Background music started");
        }
    }

    // Stop background music
    public void StopBackgroundMusic()
    {
        if (backgroundMusicSource != null && backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Stop();
            Debug.Log("[AudioManager] Background music stopped");
        }
    }

    // Set volume
    public void SetVolume(float volume)
    {
        currentVolume = Mathf.Clamp01(volume);
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.volume = isMuted ? 0f : currentVolume;
        }
        SaveAudioSettings();
        Debug.Log($"[AudioManager] Volume set to: {currentVolume}");
    }

    // Get current volume
    public float GetVolume()
    {
        return currentVolume;
    }

    // Toggle mute
    public void ToggleMute()
    {
        isMuted = !isMuted;
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.volume = isMuted ? 0f : currentVolume;
        }
        SaveAudioSettings();
        Debug.Log($"[AudioManager] Mute toggled: {isMuted}");
    }

    // Check if muted
    public bool IsMuted()
    {
        return isMuted;
    }

    // Save audio settings to PlayerPrefs
    private void SaveAudioSettings()
    {
        PlayerPrefs.SetFloat("AudioVolume", currentVolume);
        PlayerPrefs.SetInt("AudioMuted", isMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    // Load audio settings from PlayerPrefs
    private void LoadAudioSettings()
    {
        currentVolume = PlayerPrefs.GetFloat("AudioVolume", defaultVolume);
        isMuted = PlayerPrefs.GetInt("AudioMuted", 0) == 1;
    }
}
