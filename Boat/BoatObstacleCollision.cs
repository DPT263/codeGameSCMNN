using UnityEngine;

public class BoatObstacleCollision : MonoBehaviour
{
    [Header("Mission")]
    [SerializeField] private BoatTaxiMissionManager missionManager;

    [Header("Collision")]
    [SerializeField] private string obstacleTag = "Obstacle";
    [SerializeField] private float hitCooldown = 1.5f;
    [SerializeField] private int penaltyPerHit = 2000;

    private float lastHitTime = -999f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (missionManager == null)
        {
             missionManager = FindObjectOfType<BoatTaxiMissionManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag(obstacleTag))
        {
            return;
        }

        if (Time.time - lastHitTime < hitCooldown)
        {
            return;
        }

        if (missionManager == null)
        {
            return;
        }

        bool hitHandled = missionManager.TryApplyObstacleHit(penaltyPerHit);

        if (hitHandled)
        {
            lastHitTime = Time.time;
        }
    }
}
