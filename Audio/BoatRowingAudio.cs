using UnityEngine;

public class BoatRowingAudio : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PaddleRowingAnimator paddleAnimator;
    [SerializeField] private AudioSource rowingAudioSource;

    [Header("Volume")]
    [SerializeField] private float maxVolume = 0.35f;
    [SerializeField] private float fadeSpeed = 8f;
    [SerializeField] private float minPowerToPlay = 0.05f;

    [Header("Pitch")]
    [SerializeField] private float minPitch = 0.9f;
    [SerializeField] private float maxPitch = 1.15f;

    private void Awake()
    {
        if (rowingAudioSource != null)
        {
            rowingAudioSource.playOnAwake = false;
            rowingAudioSource.loop = true;
            rowingAudioSource.volume = 0f;
        }
    }

    private void Update()
    {
        if (paddleAnimator == null || rowingAudioSource == null)
        {
            return;
        }

        float rowPower = Mathf.Clamp01(paddleAnimator.RowPower);
        bool shouldPlay = paddleAnimator.IsRowing && rowPower > minPowerToPlay;

        float targetVolume = shouldPlay ? maxVolume * rowPower : 0f;

        rowingAudioSource.volume = Mathf.MoveTowards(
            rowingAudioSource.volume,
            targetVolume,
            fadeSpeed * Time.deltaTime
        );

        rowingAudioSource.pitch = Mathf.Lerp(minPitch, maxPitch, rowPower);

        if (shouldPlay && !rowingAudioSource.isPlaying)
        {
            rowingAudioSource.Play();
        }

        if (!shouldPlay && rowingAudioSource.isPlaying && rowingAudioSource.volume <= 0.01f)
        {
            rowingAudioSource.Stop();
        }
    }
}