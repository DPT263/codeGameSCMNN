using UnityEngine;

public class SmartCameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private Rigidbody targetRb;

    [Header("Base Follow")]
    [SerializeField] private Vector3 baseOffset = new Vector3(0f, 15f, -11f);
    [SerializeField] private float smoothTime = 0.3f;

    [Header("Look Ahead")]
    [SerializeField] private float lookAheadDistance = 1.5f;
    [SerializeField] private float lookAheadSmoothTime = 0.22f;
    [SerializeField] private float minMoveSpeedForLookAhead = 0.1f;

    [Header("Dead Zone")]
    [SerializeField] private Vector2 deadZoneSize = new Vector2(2.5f, 2f); //vùng kéo camera theo target, nếu target đi ra khỏi vùng này thì camera mới bị kéo theo. Nếu target đi trong vùng này thì camera đứng yên.

    [Header("Camera Bounds")]
    [SerializeField] private bool useBounds = true;
    [SerializeField] private Vector2 minBounds = new Vector2(-10f, -10f); //mép trái map
    [SerializeField] private Vector2 maxBounds = new Vector2(10f, 10f); //mép phải map

    private Vector3 currentVelocity;// Tốc độ hiện tại của camera 
    private Vector3 currentLookAhead; //Độ dịch chuyển look ahead hiện tại của camera, nó sẽ được làm mượt dần về phía desiredLookAhead
    private Vector3 lookAheadVelocity;

    // Đây là điểm mà camera thật sự muốn bám theo.
    // Nó không nhảy theo target ngay lập tức vì còn chịu ảnh hưởng của dead zone.
    private Vector3 followCenter;

    private void Awake()
    {
        if (target != null && targetRb == null)
        {
            targetRb = target.GetComponent<Rigidbody>();
        }

        if (target != null)
        {
            followCenter = target.position;
        }
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        UpdateDeadZoneCenter();
        UpdateLookAhead();

        Vector3 desiredPosition = followCenter + baseOffset + currentLookAhead;
        desiredPosition.y = baseOffset.y;

        if (useBounds)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.x);
            desiredPosition.z = Mathf.Clamp(desiredPosition.z, minBounds.y, maxBounds.y);
        }

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref currentVelocity,
            smoothTime
        );
    }

    private void UpdateDeadZoneCenter()
    {
        Vector3 targetPos = target.position;

        float halfDeadZoneX = deadZoneSize.x * 0.5f;
        float halfDeadZoneZ = deadZoneSize.y * 0.5f;

        // Nếu target đi vượt ra ngoài dead zone theo trục X,
        // followCenter mới bị kéo theo.
        if (targetPos.x > followCenter.x + halfDeadZoneX)
        {
            followCenter.x = targetPos.x - halfDeadZoneX;
        }
        else if (targetPos.x < followCenter.x - halfDeadZoneX)
        {
            followCenter.x = targetPos.x + halfDeadZoneX;
        }

        // Tương tự cho trục Z.
        if (targetPos.z > followCenter.z + halfDeadZoneZ)
        {
            followCenter.z = targetPos.z - halfDeadZoneZ;
        }
        else if (targetPos.z < followCenter.z - halfDeadZoneZ)
        {
            followCenter.z = targetPos.z + halfDeadZoneZ;
        }

        // Không dùng Y vì game của em đang chủ yếu đi trên mặt phẳng.
        followCenter.y = targetPos.y;
    }

    private void UpdateLookAhead()
    {
        Vector3 desiredLookAhead = Vector3.zero;

        if (targetRb != null)
        {
            Vector3 planarVelocity = targetRb.linearVelocity;
            planarVelocity.y = 0f;

            if (planarVelocity.magnitude > minMoveSpeedForLookAhead)
            {
                desiredLookAhead = planarVelocity.normalized * lookAheadDistance;
            }
        }

        currentLookAhead = Vector3.SmoothDamp(
            currentLookAhead,
            desiredLookAhead,
            ref lookAheadVelocity,
            lookAheadSmoothTime
        );
    }

    private void OnDrawGizmosSelected()
    {
        if (target == null)
        {
            return;
        }

        // Vẽ dead zone trong Scene view cho dễ chỉnh.
        Gizmos.color = Color.yellow;

        Vector3 center = Application.isPlaying ? followCenter : target.position;
        Vector3 size = new Vector3(deadZoneSize.x, 0.1f, deadZoneSize.y);
        Gizmos.DrawWireCube(center, size);

        // Vẽ bounds map.
        if (useBounds)
        {
            Gizmos.color = Color.cyan;

            Vector3 boundsCenter = new Vector3(
                (minBounds.x + maxBounds.x) * 0.5f,
                center.y,
                (minBounds.y + maxBounds.y) * 0.5f
            );

            Vector3 boundsSize = new Vector3(
                maxBounds.x - minBounds.x,
                0.1f,
                maxBounds.y - minBounds.y
            );

            Gizmos.DrawWireCube(boundsCenter, boundsSize);
        }
    }
    public void SetTarget(Transform newTarget, Rigidbody newTargetRb = null)
    {
        target = newTarget;

        if (newTarget == null)
        {
            targetRb = null;
            return;
        }

        targetRb = newTargetRb != null ? newTargetRb : newTarget.GetComponent<Rigidbody>();
        followCenter = newTarget.position;
        currentVelocity = Vector3.zero;
    }
}