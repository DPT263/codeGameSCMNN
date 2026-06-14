using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class SceneMusicManager : MonoBehaviour
{
    public static SceneMusicManager Instance;

    [Header("Audio Source")]
    [SerializeField] private AudioSource musicSource;

    [Header("Music Clips")]
    [SerializeField] private AudioClip menuAndIntroMusic;
    [SerializeField] private AudioClip gameplayMusic;

    [Header("Scene Names")]
    [SerializeField] private string menuSceneName = "Menu";
    [SerializeField] private string introSceneName = "KhoiDau";
    [SerializeField] private string gameplaySceneName = "LaiXuong";

    [Header("Scene Volumes")]
    [SerializeField] private float menuVolume = 0.16f;
    [SerializeField] private float introVolume = 0.07f;
    [SerializeField] private float gameplayVolume = 0.08f;

    [Header("Gameplay Music")]
    [SerializeField] private bool stopMusicInGameplayScene = true;

    [Header("Fade")]
    [SerializeField] private float fadeDuration = 1.5f;

    private Coroutine fadeRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource == null)
        {
            musicSource = GetComponent<AudioSource>();
        }

        if (menuAndIntroMusic == null && musicSource.clip != null)
        {
            menuAndIntroMusic = musicSource.clip;
        }

        SetupAudioSource();

        SceneManager.sceneLoaded += OnSceneLoaded;

        string currentSceneName = SceneManager.GetActiveScene().name;
        ApplyMusicForScene(currentSceneName, false);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void SetupAudioSource()
    {
        if (musicSource == null)
        {
            return;
        }

        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.spatialBlend = 0f;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyMusicForScene(scene.name, true);
    }

    private void ApplyMusicForScene(string sceneName, bool useFade)
    {
        if (sceneName == menuSceneName)
        {
            PlayMusic(menuAndIntroMusic, menuVolume, useFade);
            return;
        }

        if (sceneName == introSceneName)
        {
            PlayMusic(menuAndIntroMusic, introVolume, useFade);
            return;
        }

        if (sceneName == gameplaySceneName)
        {
            if (stopMusicInGameplayScene || gameplayMusic == null)
            {
                FadeOutAndStop(useFade);
            }
            else
            {
                PlayMusic(gameplayMusic, gameplayVolume, useFade);
            }

            return;
        }
    }

    private void PlayMusic(AudioClip clip, float targetVolume, bool useFade)
    {
        if (musicSource == null || clip == null)
        {
            return;
        }

        if (musicSource.clip != clip)
        {
            musicSource.clip = clip;
            musicSource.volume = 0f;
            musicSource.Play();
        }
        else if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }

        FadeToVolume(targetVolume, useFade);
    }

    private void FadeOutAndStop(bool useFade)
    {
        if (musicSource == null)
        {
            return;
        }

        if (!musicSource.isPlaying)
        {
            return;
        }

        if (!useFade)
        {
            musicSource.Stop();
            musicSource.volume = 0f;
            return;
        }

        StartFade(0f, true);
    }

    private void FadeToVolume(float targetVolume, bool useFade)
    {
        if (musicSource == null)
        {
            return;
        }

        targetVolume = Mathf.Clamp01(targetVolume);

        if (!useFade)
        {
            musicSource.volume = targetVolume;
            return;
        }

        StartFade(targetVolume, false);
    }

    private void StartFade(float targetVolume, bool stopAfterFade)
    {
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        fadeRoutine = StartCoroutine(FadeRoutine(targetVolume, stopAfterFade));
    }

    private IEnumerator FadeRoutine(float targetVolume, bool stopAfterFade)
    {
        float startVolume = musicSource.volume;
        float timer = 0f;

        if (fadeDuration <= 0f)
        {
            musicSource.volume = targetVolume;

            if (stopAfterFade)
            {
                musicSource.Stop();
            }

            fadeRoutine = null;
            yield break;
        }

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            float t = timer / fadeDuration;
            musicSource.volume = Mathf.Lerp(startVolume, targetVolume, t);

            yield return null;
        }

        musicSource.volume = targetVolume;

        if (stopAfterFade)
        {
            musicSource.Stop();
        }

        fadeRoutine = null;
    }
}