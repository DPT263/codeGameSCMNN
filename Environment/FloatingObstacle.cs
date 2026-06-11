using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FloatingObstacle : MonoBehaviour
{
    [Header("Physics Setup")]
    [SerializeField] private bool setupRigidbodyOnAwake = true;

    [Header("Push Settings")]
    [SerializeField] private float minPushForce = 1.5f; //xuồng chạy chậm vẫn đẩy vật cản một chút.
    [SerializeField] private float maxPushForce = 5f; //lực đẩy mạnh nhất.
    [SerializeField] private float boatSpeedForMaxPush = 6f; //tốc độ xuồng đạt mức này thì dùng lực đẩy tối đa.
    [SerializeField] private float spinForce = 1.5f; //lực xoay nhẹ.
    [SerializeField] private float pushCooldown = 0.15f; //tránh 1 va chạm bị tính đẩy quá nhiều lần liên tục.

    [Header("Water Resistance")]
    [SerializeField] private float waterSlowdown = 1.8f; //giả lập lực cản nước.
    [SerializeField] private float maxDriftSpeed = 3f; //giới hạn tốc độ trôi để vật cản không bay quá xa.

    [Header("Auto Drift")]
    [SerializeField] private bool autoDrift = true; //bật/tắt tự trôi
    [SerializeField] private Vector3 driftDirection = new Vector3(0f, 0f, 1f); //hướng dòng nước chính
    [SerializeField] private float driftForce = 0.2f; //lực trôi
    [SerializeField] private float sideDriftStrength = 0.08f; //độ lắc ngang nhẹ
    [SerializeField] private float sideDriftSpeed = 0.5f; //tốc độ lắc ngang
    [SerializeField] private float autoSpinForce = 0.05f; //xoay nhẹ khi trôi

    [Header("Drift Random")]
    [SerializeField] private bool randomizeDriftOnStart = true; // mỗi vật cản lệch hướng một chút
    [SerializeField] private float randomDriftAngle = 10f; //góc lệch ngẫu nhiên

    private float driftSeed;

    private float lastPushTime = -999f;
    private Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (setupRigidbodyOnAwake)
        {
            rb.useGravity = false;

            rb.constraints |= RigidbodyConstraints.FreezePositionY;
            rb.constraints |= RigidbodyConstraints.FreezeRotationX;
            rb.constraints |= RigidbodyConstraints.FreezeRotationZ;
        }

        driftSeed = Random.Range(0f, 1000f);

        if (randomizeDriftOnStart)
        {
            float randomAngle = Random.Range(-randomDriftAngle, randomDriftAngle);
            driftDirection = Quaternion.Euler(0f, randomAngle, 0f) * driftDirection;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        ApplyAutoDrift();
        
        Vector3 flatVelocity = rb.linearVelocity; //lấy vận tốc hiện tại của vật cản.
        flatVelocity.y = 0f;

        if (flatVelocity.magnitude > maxDriftSpeed)
        {
            flatVelocity = flatVelocity.normalized * maxDriftSpeed; //Nếu trôi quá nhanh thì giới hạn lại.
        }

        flatVelocity = Vector3.Lerp( //giảm dần vận tốc dần về 0 để giả lập lực cản nước.
            flatVelocity,
            Vector3.zero,
            waterSlowdown * Time.fixedDeltaTime
        );

        rb.linearVelocity = new Vector3(flatVelocity.x, 0f, flatVelocity.z); 
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Time.time - lastPushTime < pushCooldown)
        {
            return;
        }

        BoatController boat = collision.transform.GetComponentInParent<BoatController>();

        if (boat == null)
        {
            return;
        }

        Rigidbody boatRb = boat.GetComponent<Rigidbody>();

        PushAwayFromBoat(boat.transform, boatRb);

        lastPushTime = Time.time;
    }

    private void PushAwayFromBoat(Transform boatTransform, Rigidbody boatRb)
    {
        Vector3 pushDirection = transform.position - boatTransform.position; //lấy hướng từ xuồng → vật cản
        pushDirection.y = 0f;

        if (pushDirection.sqrMagnitude < 0.001f)
        {
            pushDirection = -boatTransform.forward;
        }

        pushDirection.Normalize();

        float boatSpeed = 0f;

        if (boatRb != null)
        {
            Vector3 boatVelocity = boatRb.linearVelocity;
            boatVelocity.y = 0f;
            boatSpeed = boatVelocity.magnitude;
        }

        float speed01 = Mathf.InverseLerp(0f, boatSpeedForMaxPush, boatSpeed); //biến tốc độ xuồng thành giá trị từ 0 đến 1.
        float finalPushForce = Mathf.Lerp(minPushForce, maxPushForce, speed01);

        rb.AddForce(pushDirection * finalPushForce, ForceMode.Impulse);

        float randomSpin = Random.Range(-spinForce, spinForce);
        rb.AddTorque(Vector3.up * randomSpin, ForceMode.Impulse);
    }

    private void ApplyAutoDrift()
    {
        if (!autoDrift)
        {
            return;
        }

        Vector3 mainDirection = driftDirection;
        mainDirection.y = 0f;

        if (mainDirection.sqrMagnitude < 0.001f)
        {
            mainDirection = Vector3.forward;
        }

        mainDirection.Normalize();

        Vector3 sideDirection = Vector3.Cross(Vector3.up, mainDirection).normalized;

        float sideNoise = Mathf.Sin((Time.time + driftSeed) * sideDriftSpeed);

        Vector3 finalDirection =
            mainDirection +
            sideDirection * sideNoise * sideDriftStrength;

        finalDirection.y = 0f;
        finalDirection.Normalize();

        rb.AddForce(finalDirection * driftForce, ForceMode.Acceleration);

        if (autoSpinForce > 0f)
        {
            rb.AddTorque(Vector3.up * sideNoise * autoSpinForce, ForceMode.Acceleration);
        }
    }
}
