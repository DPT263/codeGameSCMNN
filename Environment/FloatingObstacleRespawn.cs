using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FloatingObstacleRespawn : MonoBehaviour
{
    [Header("Respawn Z Settings")]
    [SerializeField] private float despawnZ = 70f;
    [SerializeField] private float respawnZ = -70f;

    [Header("Random X Range")]
    [SerializeField] private float randomXMin = -18f;
    [SerializeField] private float randomXMax = 18f;

    [Header("Y Position")]
    [SerializeField] private bool keepStartY = true;
    [SerializeField] private float fixedY = 0f;

    [Header("Random Rotation")]
    [SerializeField] private bool randomizeYRotation = true;

    private Rigidbody rb;
    private float startY;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startY = transform.position.y;
    }

    private void Update()
    {
        if (transform.position.z <= despawnZ)
        {
            RespawnAtStart();
        }
    }

    private void RespawnAtStart()
    {
        float newX = Random.Range(randomXMin, randomXMax);
        float newY = keepStartY ? startY : fixedY;

        transform.position = new Vector3(
            newX,
            newY,
            respawnZ
        );

        if (randomizeYRotation)
        {
            transform.rotation = Quaternion.Euler(
                0f,
                Random.Range(0f, 360f),
                0f
            );
        }

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Vector3 respawnCenter = new Vector3(
            (randomXMin + randomXMax) * 0.5f,
            transform.position.y,
            respawnZ
        );

        Vector3 respawnSize = new Vector3(
            Mathf.Abs(randomXMax - randomXMin),
            0.2f,
            2f
        );

        Gizmos.DrawWireCube(respawnCenter, respawnSize);

        Gizmos.color = Color.red;

        Vector3 despawnCenter = new Vector3(
            (randomXMin + randomXMax) * 0.5f,
            transform.position.y,
            despawnZ
        );

        Vector3 despawnSize = new Vector3(
            Mathf.Abs(randomXMax - randomXMin),
            0.2f,
            2f
        );

        Gizmos.DrawWireCube(despawnCenter, despawnSize);
    }
}