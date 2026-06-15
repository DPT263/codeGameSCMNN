using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameSettingsUI : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private KeyCode toggleKey = KeyCode.Escape;
    [SerializeField] private bool pauseGameWhenOpen = true;

    [Header("Panels")]
    [SerializeField] private GameObject caiDatPanel;
    [SerializeField] private GameObject controlGuidePanel;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Volume Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Display Settings")]
    [SerializeField] private Toggle fullScreenToggle;
    [SerializeField] private TMP_Dropdown qualityDropdown;

    private const string MasterVolumeKey = "Setting_MasterVolume";
    private const string BGMVolumeKey = "Setting_BGMVolume";
    private const string SFXVolumeKey = "Setting_SFXVolume";

    private const string FullScreenKey = "Setting_FullScreen";
    private const string QualityKey = "Setting_Quality";

    private const string MasterVolumeParam = "MasterVolume";
    private const string BGMVolumeParam = "BGMVolume";
    private const string SFXVolumeParam = "SFXVolume";

    private bool isSettingsOpen;
    private float previousTimeScale = 1f;

    private void Start()
    {
        SetupPanels();
        SetupAudioSettings();
        SetupDisplaySettings();
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            if (controlGuidePanel != null && controlGuidePanel.activeSelf)
            {
                DongHuongDanDieuKhien();
                return;
            }

            ChuyenTrangThaiCaiDat();
        }
    }

    private void OnDisable()
    {
        if (isSettingsOpen && pauseGameWhenOpen)
        {
            Time.timeScale = previousTimeScale;
        }
    }

    private void SetupPanels()
    {
        isSettingsOpen = false;

        if (caiDatPanel != null)
        {
            caiDatPanel.SetActive(false);
        }

        if (controlGuidePanel != null)
        {
            controlGuidePanel.SetActive(false);
        }

        if (pauseGameWhenOpen)
        {
            Time.timeScale = 1f;
        }
    }

    private void SetupAudioSettings()
    {
        float masterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
        float bgmVolume = PlayerPrefs.GetFloat(BGMVolumeKey, 1f);
        float sfxVolume = PlayerPrefs.GetFloat(SFXVolumeKey, 1f);

        ApplyVolume(MasterVolumeParam, masterVolume);
        ApplyVolume(BGMVolumeParam, bgmVolume);
        ApplyVolume(SFXVolumeParam, sfxVolume);

        if (masterSlider != null)
        {
            masterSlider.SetValueWithoutNotify(masterVolume);
            masterSlider.onValueChanged.RemoveListener(SetMasterVolume);
            masterSlider.onValueChanged.AddListener(SetMasterVolume);
        }

        if (bgmSlider != null)
        {
            bgmSlider.SetValueWithoutNotify(bgmVolume);
            bgmSlider.onValueChanged.RemoveListener(SetBGMVolume);
            bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.SetValueWithoutNotify(sfxVolume);
            sfxSlider.onValueChanged.RemoveListener(SetSFXVolume);
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }
    }

    private void SetupDisplaySettings()
    {
        bool savedFullScreen = PlayerPrefs.GetInt(
            FullScreenKey,
            Screen.fullScreen ? 1 : 0
        ) == 1;

        Screen.fullScreen = savedFullScreen;

        if (fullScreenToggle != null)
        {
            fullScreenToggle.SetIsOnWithoutNotify(savedFullScreen);
            fullScreenToggle.onValueChanged.RemoveListener(SetFullScreen);
            fullScreenToggle.onValueChanged.AddListener(SetFullScreen);
        }

        int currentQuality = QualitySettings.GetQualityLevel();
        int savedQuality = PlayerPrefs.GetInt(QualityKey, currentQuality);

        if (QualitySettings.names.Length > 0)
        {
            savedQuality = Mathf.Clamp(
                savedQuality,
                0,
                QualitySettings.names.Length - 1
            );

            QualitySettings.SetQualityLevel(savedQuality, true);
        }

        if (qualityDropdown != null)
        {
            qualityDropdown.ClearOptions();

            List<string> qualityOptions = new List<string>();

            for (int i = 0; i < QualitySettings.names.Length; i++)
            {
                qualityOptions.Add(QualitySettings.names[i]);
            }

            qualityDropdown.AddOptions(qualityOptions);
            qualityDropdown.SetValueWithoutNotify(savedQuality);
            qualityDropdown.RefreshShownValue();

            qualityDropdown.onValueChanged.RemoveListener(SetQuality);
            qualityDropdown.onValueChanged.AddListener(SetQuality);
        }
    }

    public void ChuyenTrangThaiCaiDat()
    {
        if (isSettingsOpen)
        {
            DongCaiDat();
        }
        else
        {
            MoCaiDat();
        }
    }

    public void MoCaiDat()
    {
        isSettingsOpen = true;

        if (caiDatPanel != null)
        {
            caiDatPanel.SetActive(true);
        }

        if (pauseGameWhenOpen)
        {
            previousTimeScale = Time.timeScale > 0.0001f ? Time.timeScale : 1f;
            Time.timeScale = 0f;
        }
    }

    public void DongCaiDat()
    {
        isSettingsOpen = false;

        if (caiDatPanel != null)
        {
            caiDatPanel.SetActive(false);
        }

        if (controlGuidePanel != null)
        {
            controlGuidePanel.SetActive(false);
        }

        if (pauseGameWhenOpen)
        {
            Time.timeScale = previousTimeScale;
        }
    }

    public void MoHuongDanDieuKhien()
    {
        if (controlGuidePanel != null)
        {
            controlGuidePanel.SetActive(true);
        }
    }

    public void DongHuongDanDieuKhien()
    {
        if (controlGuidePanel != null)
        {
            controlGuidePanel.SetActive(false);
        }
    }

    public void SetMasterVolume(float value)
    {
        ApplyVolume(MasterVolumeParam, value);
        PlayerPrefs.SetFloat(MasterVolumeKey, value);
        PlayerPrefs.Save();
    }

    public void SetBGMVolume(float value)
    {
        ApplyVolume(BGMVolumeParam, value);
        PlayerPrefs.SetFloat(BGMVolumeKey, value);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        ApplyVolume(SFXVolumeParam, value);
        PlayerPrefs.SetFloat(SFXVolumeKey, value);
        PlayerPrefs.Save();
    }

    private void ApplyVolume(string parameterName, float value)
    {
        if (audioMixer == null)
        {
            return;
        }

        value = Mathf.Clamp01(value);

        float volumeDb;

        if (value <= 0.0001f)
        {
            volumeDb = -80f;
        }
        else
        {
            volumeDb = Mathf.Log10(value) * 20f;
        }

        audioMixer.SetFloat(parameterName, volumeDb);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;

        PlayerPrefs.SetInt(FullScreenKey, isFullScreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetQuality(int qualityIndex)
    {
        if (QualitySettings.names.Length <= 0)
        {
            return;
        }

        qualityIndex = Mathf.Clamp(
            qualityIndex,
            0,
            QualitySettings.names.Length - 1
        );

        QualitySettings.SetQualityLevel(qualityIndex, true);

        PlayerPrefs.SetInt(QualityKey, qualityIndex);
        PlayerPrefs.Save();
    }
}