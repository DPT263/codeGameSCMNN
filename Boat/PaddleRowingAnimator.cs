using UnityEngine;

public class PaddleRowingAnimator : MonoBehaviour
{
    [Header("Paddle Pivots")]
    [SerializeField] private Transform leftPaddlePivot;
    [SerializeField] private Transform rightPaddlePivot;

    [Header("Smooth Rowing")]
    [SerializeField] private float rowSpeed = 1.15f;
    [SerializeField] private float swingAngle = 28f;
    [SerializeField] private float waterDipAngle = -8f;
    [SerializeField] private float airLiftAngle = 10f;

    [Header("Smoothness")]
    [SerializeField] private float powerSmoothTime = 0.12f;
    [SerializeField] private float rotationSmooth = 14f;

    [Header("Axes")]
    [SerializeField] private Vector3 swingAxis = Vector3.up;
    [SerializeField] private Vector3 dipAxis = Vector3.forward;

    [Header("Left Paddle Direction")]
    [SerializeField] private float leftSwingSign = 1f;
    [SerializeField] private float leftDipSign = 1f;

    [Header("Right Paddle Direction")]
    [SerializeField] private float rightSwingSign = 1f;
    [SerializeField] private float rightDipSign = 1f;

    [Header("Phase")]
    [SerializeField] private float rightPhaseOffset = 0f;

    private Quaternion leftStartRotation;
    private Quaternion rightStartRotation;

    private float rowPhase;
    private float leftPower;
    private float rightPower;
    private float leftPowerVelocity;
    private float rightPowerVelocity;

    public float RowPhase
    {
        get { return rowPhase; }
    }

    public float RowPower
    {
        get { return Mathf.Max(leftPower, rightPower); }
    }

    public bool IsRowing
    {
        get { return RowPower > 0.05f; }
    }

    public float RowSpeed
    {
        get { return rowSpeed; }
    }
    private void Awake()
    {
        if (leftPaddlePivot != null)
        {
            leftStartRotation = leftPaddlePivot.localRotation;
        }

        if (rightPaddlePivot != null)
        {
            rightStartRotation = rightPaddlePivot.localRotation;
        }
    }

    private void LateUpdate()
    {
        float targetLeftPower = 0f;
        float targetRightPower = 0f;

        bool moveForward = Input.GetKey(KeyCode.W);
        bool moveBackward = Input.GetKey(KeyCode.S);
        float turn = Input.GetAxisRaw("Horizontal");

        if (moveForward || moveBackward)
        {
            targetLeftPower = 1f;
            targetRightPower = 1f;
        }

        if (turn < -0.01f) // A
        {
            targetRightPower = 1f;
            targetLeftPower = Mathf.Max(targetLeftPower, 0.25f);
        }
        else if (turn > 0.01f) // D
        {
            targetLeftPower = 1f;
            targetRightPower = Mathf.Max(targetRightPower, 0.25f);
        }

        leftPower = Mathf.SmoothDamp(
            leftPower,
            targetLeftPower,
            ref leftPowerVelocity,
            powerSmoothTime
        );

        rightPower = Mathf.SmoothDamp(
            rightPower,
            targetRightPower,
            ref rightPowerVelocity,
            powerSmoothTime
        );

        float maxPower = Mathf.Max(leftPower, rightPower);

        if (maxPower > 0.02f)
        {
            float direction = moveBackward ? -1f : 1f;
            rowPhase += Time.deltaTime * rowSpeed * direction;
        }
        else
        {
            rowPhase = 0f;
        }

        float leftPhase = rowPhase;
        float rightPhase = rowPhase;

        // Khi đi thẳng tới/lùi, ép 2 tay chèo dùng chung nhịp.
        // Không cho tay phải lệch pha nữa.
        if (!moveForward && !moveBackward)
        {
            rightPhase = rowPhase + rightPhaseOffset;
        }

        AnimateOnePaddle(
            leftPaddlePivot,
            leftStartRotation,
            leftPower,
            leftPhase,
            leftSwingSign,
            leftDipSign
        );

        AnimateOnePaddle(
            rightPaddlePivot,
            rightStartRotation,
            rightPower,
            rightPhase,
            rightSwingSign,
            rightDipSign
        );
    }

    private void AnimateOnePaddle(
        Transform pivot,
        Quaternion startRotation,
        float power,
        float phase,
        float swingSign,
        float dipSign
    )
    {
        if (pivot == null)
        {
            return;
        }

        if (power <= 0.02f)
        {
            pivot.localRotation = Quaternion.Slerp(
                pivot.localRotation,
                startRotation,
                1f - Mathf.Exp(-rotationSmooth * Time.deltaTime)
            );

            return;
        }

        Vector3 safeSwingAxis = swingAxis.sqrMagnitude < 0.001f
            ? Vector3.up
            : swingAxis.normalized;

        Vector3 safeDipAxis = dipAxis.sqrMagnitude < 0.001f
            ? Vector3.forward
            : dipAxis.normalized;

        float radians = phase * Mathf.PI * 2f;

        float swing = Mathf.Sin(radians) * swingAngle * power * swingSign;

        float dip01 = (Mathf.Cos(radians) + 1f) * 0.5f;
        dip01 = Smooth01(dip01);

        float dip = Mathf.Lerp(airLiftAngle, waterDipAngle, dip01) * power * dipSign;

        Quaternion targetRotation =
            startRotation
            * Quaternion.AngleAxis(swing, safeSwingAxis)
            * Quaternion.AngleAxis(dip, safeDipAxis);

        pivot.localRotation = Quaternion.Slerp(
            pivot.localRotation,
            targetRotation,
            1f - Mathf.Exp(-rotationSmooth * Time.deltaTime)
        );
    }

    private float Smooth01(float t)
    {
        return t * t * (3f - 2f * t);
    }
}