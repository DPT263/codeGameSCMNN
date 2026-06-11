using UnityEngine;

public class EnvironmentAudioZone : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource zoneAudioSource;

    [Header("Volume")]
    [SerializeField] private float maxVolume = 0.3f;
    [SerializeField] private float fadeSpeed = 1.5f;

    [Header("Boat Detection")]
    [SerializeField] private float exitDelay = 0.25f; 

    private float lastBoatTouchTime = -999f;
    private bool boatRecentlyInside;

    private void Reset()
    {
        Collider col = GetComponent<Collider>();

        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void Awake()
    {
        if (zoneAudioSource == null)
        {
            zoneAudioSource = GetComponent<AudioSource>();
        }

        if (zoneAudioSource != null)
        {
            zoneAudioSource.playOnAwake = false;
            zoneAudioSource.loop = true;
            zoneAudioSource.volume = 0f;
        }
    }

    private void Update()
    {
        if (zoneAudioSource == null)
        {
            return;
        }

        boatRecentlyInside = Time.time - lastBoatTouchTime <= exitDelay;

        float targetVolume = boatRecentlyInside ? maxVolume : 0f;

        zoneAudioSource.volume = Mathf.MoveTowards(
            zoneAudioSource.volume,
            targetVolume,
            fadeSpeed * Time.deltaTime
        );

        if (boatRecentlyInside && !zoneAudioSource.isPlaying)
        {
            zoneAudioSource.Play();
        }

        if (!boatRecentlyInside && zoneAudioSource.isPlaying && zoneAudioSource.volume <= 0.01f)
        {
            zoneAudioSource.Stop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        TryDetectBoat(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryDetectBoat(other);
    }

    private void TryDetectBoat(Collider other)
    {
        BoatController boat = other.GetComponentInParent<BoatController>();

        if (boat == null)
        {
            return;
        }

        lastBoatTouchTime = Time.time;
    }

    private void OnDisable()
    {
        if (zoneAudioSource != null)
        {
            zoneAudioSource.Stop();
            zoneAudioSource.volume = 0f;
        }

        boatRecentlyInside = false;
    }
}