using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject caiDatPanel;

    [Header("Audio")]
    [SerializeField] private Slider volumeSlider;

    private const string VolumeKey = "GameVolume";

    private void Start()
    {
        if (caiDatPanel != null)
        {
            caiDatPanel.SetActive(false);
        }

        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);
        AudioListener.volume = savedVolume;

        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    public void BatDau()
    {
        SceneManager.LoadScene("KhoiDau");
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
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;

        PlayerPrefs.SetFloat(VolumeKey, volume);
        PlayerPrefs.Save();
    }

    public void Thoat()
    {
        Application.Quit();
    }
}