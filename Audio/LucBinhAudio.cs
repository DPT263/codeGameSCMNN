using UnityEngine;

public class LucBinhAudio : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource plantAudioSource;
    [SerializeField] private AudioSource boatCreakAudioSource;
    [SerializeField] private Rigidbody boatRb;

    [Header("Plant Volume")]
    [SerializeField] private float plantMaxVolume = 0.25f;

    [Header("Boat Creak Volume")]
    [SerializeField] private float creakMaxVolume = 0.18f;

    [Header("Fade")]
    [SerializeField] private float fadeSpeed = 8f;
    [SerializeField] private float exitDelay = 0.25f;

    [Header("Boat Movement")]
    [SerializeField] private bool requireBoatMoving = true;
    [SerializeField] private float minBoatSpeedToPlay = 0.2f;
    [SerializeField] private float boatSpeedForMaxVolume = 4f;

    [Header("Pitch")]
    [SerializeField] private float minPitch = 0.95f;
    [SerializeField] private float maxPitch = 1.08f;

    private float lastPlantTouchTime = -999f;

    private void Awake()
    {
        if (boatRb == null)
        {
            boatRb = GetComponent<Rigidbody>();
        }

        SetupAudioSource(plantAudioSource);
        SetupAudioSource(boatCreakAudioSource);
    }

    private void Update()
    {
        bool touchingPlant = Time.time - lastPlantTouchTime <= exitDelay;

        float boatSpeed = GetBoatFlatSpeed();
        bool boatMovingEnough = boatSpeed >= minBoatSpeedToPlay;

        bool shouldPlay = touchingPlant;

        if (requireBoatMoving)
        {
            shouldPlay = touchingPlant && boatMovingEnough;
        }

        float speed01 = Mathf.InverseLerp(
            minBoatSpeedToPlay,
            boatSpeedForMaxVolume,
            boatSpeed
        );

        float plantTargetVolume = shouldPlay
            ? Mathf.Lerp(plantMaxVolume * 0.45f, plantMaxVolume, speed01)
            : 0f;

        float creakTargetVolume = shouldPlay
            ? Mathf.Lerp(creakMaxVolume * 0.35f, creakMaxVolume, speed01)
            : 0f;

        float targetPitch = Mathf.Lerp(minPitch, maxPitch, speed01);

        UpdateLoopAudio(
            plantAudioSource,
            plantTargetVolume,
            targetPitch
        );

        UpdateLoopAudio(
            boatCreakAudioSource,
            creakTargetVolume,
            targetPitch
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        TryDetectWaterPlantZone(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryDetectWaterPlantZone(other);
    }

    private void TryDetectWaterPlantZone(Collider other)
    {
        BoatSlowZone slowZone = other.GetComponentInParent<BoatSlowZone>();

        if (slowZone == null)
        {
            return;
        }

        lastPlantTouchTime = Time.time;
    }

    private void SetupAudioSource(AudioSource source)
    {
        if (source == null)
        {
            return;
        }

        source.playOnAwake = false;
        source.loop = false;
        source.volume = 0f;
    }

    private void UpdateLoopAudio(AudioSource source, float targetVolume, float targetPitch)
    {
        if (source == null)
        {
            return;
        }

        source.volume = Mathf.MoveTowards(
            source.volume,
            targetVolume,
            fadeSpeed * Time.deltaTime
        );

        source.pitch = targetPitch;

        if (targetVolume > 0.01f && !source.isPlaying)
        {
            source.Play();
        }

        if (targetVolume <= 0.01f && source.isPlaying && source.volume <= 0.01f)
        {
            source.Stop();
        }
    }

    private float GetBoatFlatSpeed()
    {
        if (boatRb == null)
        {
            return 0f;
        }

        Vector3 velocity = boatRb.linearVelocity;
        velocity.y = 0f;

        return velocity.magnitude;
    }
}