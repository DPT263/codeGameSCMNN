using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoatController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveForce = 9f; 
    [SerializeField] private float reverseForce = 4f;
    [SerializeField] private float turnTorque = 5f;
    [SerializeField] private float maxSpeed = 6f;

    [Header("Paddle Visuals")]
    [SerializeField] private Transform leftPaddle;
    [SerializeField] private Transform rightPaddle;
    [SerializeField] private float paddleAngle = 25f;
    [SerializeField] private float paddleSpeed = 7f;

    private Rigidbody rb;
    private Quaternion leftStartRotation;
    private Quaternion rightStartRotation;
    private float baseMoveForce; // Lực chạy ban đầu 
    private float baseMaxSpeed; // tốc độ tối đa ban đầu
    private float skillSpeedMultiplier = 1f; //tốc độ cộng thêm do kỹ năng lái xuồng
    private float environmentSpeedMultiplier = 1f; //tốc độ bị ảnh hưởng bởi môi trường như lục bình

    //Awake là hàm được gọi khi đối tượng được khởi tạo, trước cả Start. Lưu lại các giá trị ban đầu của các biến
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        baseMoveForce = moveForce; 
        baseMaxSpeed = maxSpeed; 

        rb.useGravity = false;
        rb.constraints |= RigidbodyConstraints.FreezePositionY;
        rb.constraints |= RigidbodyConstraints.FreezeRotationX;
        rb.constraints |= RigidbodyConstraints.FreezeRotationZ;

        if (leftPaddle != null)
        {
            leftStartRotation = leftPaddle.localRotation;
        }

        if (rightPaddle != null)
        {
            rightStartRotation = rightPaddle.localRotation;
        }
    }

    private void FixedUpdate()
    {
        float throttle = 0f;

        if (Input.GetKey(KeyCode.W))
        {
            throttle = 1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            throttle = -0.6f;
        }

        float turn = Input.GetAxisRaw("Horizontal");

        Vector3 flatVelocity = rb.linearVelocity;
        flatVelocity.y = 0f;

        if (throttle > 0f && flatVelocity.magnitude < maxSpeed)
        {
            rb.AddForce(transform.forward * moveForce, ForceMode.Acceleration);
        }
        else if (throttle < 0f && flatVelocity.magnitude < maxSpeed * 0.5f)
        {
            rb.AddForce(transform.forward * throttle * reverseForce, ForceMode.Acceleration);
        }

        if (Mathf.Abs(turn) > 0.01f)
        {
            rb.AddTorque(Vector3.up * turn * turnTorque, ForceMode.Acceleration);
        }
    }

    private void Update()
    {
        AnimatePaddles();
    }

    private void AnimatePaddles()
    {
        bool isMoving =
            Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.S) ||
            Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.01f;

        if (!isMoving)
        {
            if (leftPaddle != null)
            {
                leftPaddle.localRotation = Quaternion.Slerp(
                    leftPaddle.localRotation,
                    leftStartRotation,
                    Time.deltaTime * 6f
                );
            }

            if (rightPaddle != null)
            {
                rightPaddle.localRotation = Quaternion.Slerp(
                    rightPaddle.localRotation,
                    rightStartRotation,
                    Time.deltaTime * 6f
                );
            }

            return;
        }

        float angle = Mathf.Sin(Time.time * paddleSpeed) * paddleAngle;

        if (leftPaddle != null)
        {
            leftPaddle.localRotation = leftStartRotation * Quaternion.AngleAxis(angle, Vector3.right);
        }

        if (rightPaddle != null)
        {
            rightPaddle.localRotation = rightStartRotation * Quaternion.AngleAxis(-angle, Vector3.right);
        }
    }

    public void ApplySpeedMultiplier(float multiplier)
    {
        skillSpeedMultiplier = Mathf.Max(0.1f, multiplier);
        RefreshFinalSpeed();
    }
    public void SetEnvironmentSpeedMultiplier(float multiplier)
    {
        environmentSpeedMultiplier = Mathf.Clamp(multiplier, 0.1f, 1f);
        RefreshFinalSpeed();
    }

    public void ClearEnvironmentSpeedMultiplier() //rời khỏi lục bình thì tốc độ về bình thường
    {
        environmentSpeedMultiplier = 1f;
        RefreshFinalSpeed();
    }

    private void RefreshFinalSpeed()
    {
        float finalMultiplier = skillSpeedMultiplier * environmentSpeedMultiplier;

        moveForce = baseMoveForce * finalMultiplier;
        maxSpeed = baseMaxSpeed * finalMultiplier;
    }
}