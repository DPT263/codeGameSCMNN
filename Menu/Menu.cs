using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Menu : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string startSceneName = "KhoiDau";

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

    private void Start()
    {
        SetupPanels();
        SetupAudioSettings();
        SetupDisplaySettings();
    }

    private void SetupPanels()
    {
        if (caiDatPanel != null)
        {
            caiDatPanel.SetActive(false);
        }

        if (controlGuidePanel != null)
        {
            controlGuidePanel.SetActive(false);
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
            masterSlider.onValueChanged.AddListener(SetMasterVolume);
        }

        if (bgmSlider != null)
        {
            bgmSlider.SetValueWithoutNotify(bgmVolume);
            bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.SetValueWithoutNotify(sfxVolume);
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
            fullScreenToggle.onValueChanged.AddListener(SetFullScreen);
        }

        int savedQuality = PlayerPrefs.GetInt(
            QualityKey,
            QualitySettings.GetQualityLevel()
        );

        savedQuality = Mathf.Clamp(
            savedQuality,
            0,
            QualitySettings.names.Length - 1
        );

        QualitySettings.SetQualityLevel(savedQuality, true);

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

            qualityDropdown.onValueChanged.AddListener(SetQuality);
        }
    }

    public void BatDau()
    {
        SceneManager.LoadScene(startSceneName);
    }

    public void MoCaiDat()
    {
        if (caiDatPanel != null)
        {
            caiDatPanel.SetActive(true);
        }
    }

    public void DongCaiDat()
    {
        if (caiDatPanel != null)
        {
            caiDatPanel.SetActive(false);
        }

        if (controlGuidePanel != null)
        {
            controlGuidePanel.SetActive(false);
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
        qualityIndex = Mathf.Clamp(
            qualityIndex,
            0,
            QualitySettings.names.Length - 1
        );

        QualitySettings.SetQualityLevel(qualityIndex, true);

        PlayerPrefs.SetInt(QualityKey, qualityIndex);
        PlayerPrefs.Save();
    }

    public void Thoat()
    {
        Application.Quit();
    }
}