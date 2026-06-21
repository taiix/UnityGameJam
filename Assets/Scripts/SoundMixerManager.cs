using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundMixerManager : MonoBehaviour
{
    // Mixer exposed parameter names
    private const string MasterParam = "masterVolume";
    private const string MusicParam = "musicVolume";
    private const string SfxParam = "soundEffectsVolume";

    // PlayerPrefs keys
    private const string MasterKey = "vol_master";
    private const string MusicKey = "vol_music";
    private const string SfxKey = "vol_sfx";
    private const string FullscreenKey = "display_fullscreen";
    private const string ResolutionKey = "display_resolution";
    private const string QualityKey = "graphics_quality";
    private const string VSyncKey = "graphics_vsync";

    [Header("Audio Settings")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private TextMeshProUGUI[] audioTexts; // 0 = master, 1 = music, 2 = sfx
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Space]
    [Header("Display Settings")]
    [SerializeField] private Slider fullscreenSlider;
    [SerializeField] private TextMeshProUGUI resolutionText;

    [Space]
    [Header("Graphics Settings")]
    [SerializeField] private TextMeshProUGUI qualityText;
    [SerializeField] private Slider vSyncSlider; // optional, 0 = off, 1 = on

    private Resolution[] resolutions;
    private int currentResIndex;

    // ---------- Setup ----------

    private void Start()
    {
        InitFullscreen();
        InitResolution();
        InitQuality();
        InitVSync();
        InitAudio();
    }

    private void InitAudio()
    {
        LoadVolume(MasterKey, masterSlider, SetMasterVolume);
        LoadVolume(MusicKey, musicSlider, SetMusicVolume);
        LoadVolume(SfxKey, sfxSlider, SetSoundsVolume);
 
    }

    private void LoadVolume(string key, Slider slider, System.Action<float> apply)
    {
        float fallback = slider != null ? slider.value : 0.75f;
        float v = PlayerPrefs.GetFloat(key, fallback);
        if (slider != null) slider.SetValueWithoutNotify(v);
        apply(v);
    }

    private void InitFullscreen()
    {
        if (fullscreenSlider == null)
        {
            Debug.LogWarning("Fullscreen slider is not assigned in the inspector.");
            return;
        }

        bool fs = PlayerPrefs.GetInt(FullscreenKey, Screen.fullScreen ? 1 : 0) == 1;
        fullscreenSlider.SetValueWithoutNotify(fs ? 1 : 0);
        Screen.fullScreen = fs;
    }

    private void InitResolution()
    {
        resolutions = Screen.resolutions;

        int saved = PlayerPrefs.GetInt(ResolutionKey, -1);
        if (saved >= 0 && saved < resolutions.Length)
        {
            currentResIndex = saved;
            Resolution r = resolutions[currentResIndex];
            Screen.SetResolution(r.width, r.height, Screen.fullScreen);
        }
        else
        {
            currentResIndex = System.Array.FindIndex(resolutions,
                r => r.width == Screen.width && r.height == Screen.height);

            if (currentResIndex < 0)
                currentResIndex = resolutions.Length - 1;
        }

        UpdateResolutionLabel();
    }

    private void InitQuality()
    {
        int level = PlayerPrefs.GetInt(QualityKey, QualitySettings.GetQualityLevel());
        level = Mathf.Clamp(level, 0, QualitySettings.names.Length - 1);
        QualitySettings.SetQualityLevel(level, true);
        if (qualityText != null) qualityText.text = QualitySettings.names[level];
    }

    private void InitVSync()
    {
        int v = PlayerPrefs.GetInt(VSyncKey, QualitySettings.vSyncCount > 0 ? 1 : 0);
        QualitySettings.vSyncCount = v;
        if (vSyncSlider != null) vSyncSlider.SetValueWithoutNotify(v);
    }

    // ---------- Audio ----------

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat(MasterParam, Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f);
        audioTexts[0].text = $"{(int)(volume * 100)}%";
        masterSlider.value = volume;
        PlayerPrefs.SetFloat(MasterKey, volume);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat(MusicParam, Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f);
        audioTexts[1].text = $"{(int)(volume * 100)}%";
        musicSlider.value = volume;
        PlayerPrefs.SetFloat(MusicKey, volume);
    }

    public void SetSoundsVolume(float volume)
    {
        audioMixer.SetFloat(SfxParam, Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f);
        audioTexts[2].text = $"{(int)(volume * 100)}%";
        sfxSlider.value = volume;
        PlayerPrefs.SetFloat(SfxKey, volume);
    }

    // ---------- Display ----------

    public void Fullscreen()
    {
        if (fullscreenSlider == null)
            return;

        bool goFullscreen = fullscreenSlider.value != 1;
        fullscreenSlider.value = goFullscreen ? 1 : 0;
        Screen.fullScreen = goFullscreen;
        PlayerPrefs.SetInt(FullscreenKey, goFullscreen ? 1 : 0);
    }

    public void Resolutions(int step)
    {
        currentResIndex = ((currentResIndex + step) % resolutions.Length + resolutions.Length)
                          % resolutions.Length;

        Resolution r = resolutions[currentResIndex];
        Screen.SetResolution(r.width, r.height, Screen.fullScreen);
        UpdateResolutionLabel();
        PlayerPrefs.SetInt(ResolutionKey, currentResIndex);
    }

    private void UpdateResolutionLabel()
    {
        Resolution r = resolutions[currentResIndex];
        resolutionText.text = $"{r.width}x{r.height} x {r.refreshRateRatio}Hz";
    }

    // ---------- Graphics ----------

    public void Quality(bool increase)
    {
        int level = QualitySettings.GetQualityLevel() + (increase ? 1 : -1);
        level = Mathf.Clamp(level, 0, QualitySettings.names.Length - 1);

        QualitySettings.SetQualityLevel(level, true);
        if (qualityText != null) qualityText.text = QualitySettings.names[level];
        PlayerPrefs.SetInt(QualityKey, level);
    }

    public void VSync()
    {
        if (vSyncSlider == null)
            return;

        bool active = vSyncSlider.value != 1;
        vSyncSlider.value = active ? 1 : 0;
        QualitySettings.vSyncCount = active ? 1 : 0;
        PlayerPrefs.SetInt(VSyncKey, active ? 1 : 0);
    }

    // ---------- Lifecycle ----------

    private void OnApplicationQuit() => PlayerPrefs.Save();

    private void OnApplicationPause(bool paused)
    {
        if (paused) PlayerPrefs.Save();
    }

    // ---------- Helpers ----------

    private string DbToPercent(float db) => $"{(int)(Mathf.Pow(10, db / 20f) * 100)}%";
}