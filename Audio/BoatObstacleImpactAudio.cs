using UnityEngine;

public class BoatObstacleImpactAudio : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] impactClips;

    [Header("Volume")]
    [SerializeField] private float minVolume = 0.15f;
    [SerializeField] private float maxVolume = 0.55f;

    [Header("Speed")]
    [SerializeField] private float minSpeedToPlay = 0.5f;
    [SerializeField] private float speedForMaxVolume = 5f;

    [Header("Pitch Random")]
    [SerializeField] private float minPitch = 0.9f;
    [SerializeField] private float maxPitch = 1.12f;

    [Header("Cooldown")]
    [SerializeField] private float soundCooldown = 0.2f;

    private float lastSoundTime = -999f;

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.volume = 1f;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Time.time - lastSoundTime < soundCooldown)
        {
            return;
        }

        BoatController boat = collision.transform.GetComponentInParent<BoatController>();

        if (boat == null)
        {
            return;
        }

        float impactSpeed = collision.relativeVelocity.magnitude;

        if (impactSpeed < minSpeedToPlay)
        {
            return;
        }

        PlayImpactSound(impactSpeed);
        lastSoundTime = Time.time;
    }

    private void PlayImpactSound(float impactSpeed)
    {
        if (audioSource == null || impactClips == null || impactClips.Length == 0)
        {
            return;
        }

        AudioClip clip = impactClips[Random.Range(0, impactClips.Length)];

        float speed01 = Mathf.InverseLerp(
            minSpeedToPlay,
            speedForMaxVolume,
            impactSpeed
        );

        float volume = Mathf.Lerp(minVolume, maxVolume, speed01);
        audioSource.pitch = Random.Range(minPitch, maxPitch);

        audioSource.PlayOneShot(clip, volume);
    }
}