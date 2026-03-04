using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Provides a singleton service for managing and persisting audio settings, including master, music, and sound effects
/// volume levels, across game sessions and scenes.
/// </summary>
/// <remarks>This service automatically loads saved audio preferences on initialization and applies them to the
/// assigned audio mixer. Volume levels can be adjusted and saved, ensuring user preferences are maintained between
/// sessions. The service persists for the lifetime of the application and is accessible via the static Instance
/// property.</remarks>
public class AudioSettingsService : MonoBehaviour
{
    public static AudioSettingsService Instance { get; private set; }

    [Header("Audio Mixer")]
    [Tooltip("The AudioMixer asset that contains the exposed parameters for master, music, and FX volume control.")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Exposed Parameters")]
    [SerializeField] private string masterVolumeParameter = "MasterVolumeDb";
    [SerializeField] private string musicVolumeParameter = "MusicVolumeDb";
    [SerializeField] private string fxVolumeParameter = "FxVolumeDb";

    [Header("Defaults")]
    [SerializeField, Range(0, 100)] private int defaultMasterVolumePercent = 80;
    [SerializeField, Range(0, 100)] private int defaultMusicVolumePercent = 80;
    [SerializeField, Range(0, 100)] private int defaultFxVolumePercent = 80;

    private const string MasterVolumePrefKey = "Audio.MasterPercent";
    private const string MusicVolumePrefKey = "Audio.MusicPercent";
    private const string FxVolumePrefKey = "Audio.FxPercent";

    private int masterVolumePercent;
    private int musicVolumePercent;
    private int fxVolumePercent;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadAndApply();
    }

    public int GetMasterVolumePercent() => masterVolumePercent;

    public int GetMusicVolumePercent() => musicVolumePercent;

    public int GetFxVolumePercent() => fxVolumePercent;

    public void SetMasterVolumePercent(int value, bool persist = true)
    {
        masterVolumePercent = Mathf.Clamp(value, 0, 100);
        ApplyMixerVolume(masterVolumeParameter, masterVolumePercent);

        if (persist)
            Save();
    }

    public void SetMusicVolumePercent(int value, bool persist = true)
    {
        musicVolumePercent = Mathf.Clamp(value, 0, 100);
        ApplyMixerVolume(musicVolumeParameter, musicVolumePercent);

        if (persist)
            Save();
    }

    public void SetFxVolumePercent(int value, bool persist = true)
    {
        fxVolumePercent = Mathf.Clamp(value, 0, 100);
        ApplyMixerVolume(fxVolumeParameter, fxVolumePercent);

        if (persist)
            Save();
    }

    private void LoadAndApply()
    {
        masterVolumePercent = Mathf.Clamp(PlayerPrefs.GetInt(MasterVolumePrefKey, defaultMasterVolumePercent), 0, 100);
        musicVolumePercent = Mathf.Clamp(PlayerPrefs.GetInt(MusicVolumePrefKey, defaultMusicVolumePercent), 0, 100);
        fxVolumePercent = Mathf.Clamp(PlayerPrefs.GetInt(FxVolumePrefKey, defaultFxVolumePercent), 0, 100);

        SetMasterVolumePercent(masterVolumePercent, persist: false);
        SetMusicVolumePercent(musicVolumePercent, persist: false);
        SetFxVolumePercent(fxVolumePercent, persist: false);
    }

    private void Save()
    {
        PlayerPrefs.SetInt(MasterVolumePrefKey, masterVolumePercent);
        PlayerPrefs.SetInt(MusicVolumePrefKey, musicVolumePercent);
        PlayerPrefs.SetInt(FxVolumePrefKey, fxVolumePercent);
        PlayerPrefs.Save();
    }

    private void ApplyMixerVolume(string parameterName, int percentValue)
    {
        if (audioMixer == null)
        {
            Debug.LogWarning("AudioSettingsService: AudioMixer is not assigned.");
            return;
        }

        if (string.IsNullOrWhiteSpace(parameterName))
        {
            Debug.LogWarning("AudioSettingsService: Mixer parameter name is empty.");
            return;
        }

        float decibels = ToDecibels(percentValue / 100f);
        audioMixer.SetFloat(parameterName, decibels);
    }

    private static float ToDecibels(float normalizedLinear)
    {
        if (normalizedLinear <= 0.0001f)
            return -80f;

        return Mathf.Log10(normalizedLinear) * 20f;
    }
}