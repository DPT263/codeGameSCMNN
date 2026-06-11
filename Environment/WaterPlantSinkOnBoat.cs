using UnityEngine;

public class WaterPlantSinkOnBoat : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private Transform visualRoot;

    [Header("Sink Settings")]
    [SerializeField] private float sinkDepth = 0.18f;
    [SerializeField] private float sinkSpeed = 10f;
    [SerializeField] private float riseSpeed = 3.5f;

    [Header("Boat Detection")]
    [SerializeField] private float staySinkDuration = 0.25f;

    private Vector3 startLocalPosition;
    private float lastBoatTouchTime = -999f;

    private void Awake()
    {
        if (visualRoot == null)
        {
            visualRoot = transform;
        }

        startLocalPosition = visualRoot.localPosition;
    }

    private void Update()
    {
        bool shouldSink = Time.time - lastBoatTouchTime <= staySinkDuration;

        Vector3 targetPosition = startLocalPosition;

        if (shouldSink)
        {
            targetPosition = startLocalPosition + Vector3.down * sinkDepth;
        }

        float speed = shouldSink ? sinkSpeed : riseSpeed;

        visualRoot.localPosition = Vector3.Lerp(
            visualRoot.localPosition,
            targetPosition,
            speed * Time.deltaTime
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsBoat(other))
        {
            lastBoatTouchTime = Time.time;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (IsBoat(other))
        {
            lastBoatTouchTime = Time.time;
        }
    }

    private bool IsBoat(Collider other)
    {
        BoatController boat = other.GetComponentInParent<BoatController>();
        return boat != null;
    }
}