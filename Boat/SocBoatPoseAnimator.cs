using UnityEngine;

public class SocBoatPoseAnimator : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private PaddleRowingAnimator paddleAnimator;

    [Header("Bones")]
    [SerializeField] private Transform bodyBone;
    [SerializeField] private Transform headBone;

    [SerializeField] private Transform leftUpperArm;
    [SerializeField] private Transform leftForeArm;

    [SerializeField] private Transform rightUpperArm;
    [SerializeField] private Transform rightForeArm;

    [Header("Body Pose")]
    [SerializeField] private Vector3 bodyReachEuler = new Vector3(8f, 0f, 0f);
    [SerializeField] private Vector3 bodyPullEuler = new Vector3(-7f, 0f, 0f);

    [SerializeField] private Vector3 headReachEuler = new Vector3(-4f, 0f, 0f);
    [SerializeField] private Vector3 headPullEuler = new Vector3(3f, 0f, 0f);

    [Header("Left Arm Pose")]
    [SerializeField] private Vector3 leftUpperArmReachEuler = new Vector3(-18f, 0f, 10f);
    [SerializeField] private Vector3 leftUpperArmPullEuler = new Vector3(16f, 0f, -8f);

    [SerializeField] private Vector3 leftForeArmReachEuler = new Vector3(8f, 0f, 0f);
    [SerializeField] private Vector3 leftForeArmPullEuler = new Vector3(34f, 0f, 0f);

    [Header("Right Arm Pose")]
    [SerializeField] private Vector3 rightUpperArmReachEuler = new Vector3(-18f, 0f, -10f);
    [SerializeField] private Vector3 rightUpperArmPullEuler = new Vector3(16f, 0f, 8f);

    [SerializeField] private Vector3 rightForeArmReachEuler = new Vector3(8f, 0f, 0f);
    [SerializeField] private Vector3 rightForeArmPullEuler = new Vector3(34f, 0f, 0f);

    [Header("Smooth")]
    [SerializeField] private float poseSmooth = 14f;
    [SerializeField] private float poseStrength = 1f;

    [Header("Sync")]
    [SerializeField] private float phaseOffset = 0f;
    [SerializeField] private bool invertPull = false;

    private Quaternion bodyStartRotation;
    private Quaternion headStartRotation;

    private Quaternion leftUpperArmStartRotation;
    private Quaternion leftForeArmStartRotation;

    private Quaternion rightUpperArmStartRotation;
    private Quaternion rightForeArmStartRotation;

    private void Awake()
    {
        if (bodyBone != null)
        {
            bodyStartRotation = bodyBone.localRotation;
        }

        if (headBone != null)
        {
            headStartRotation = headBone.localRotation;
        }

        if (leftUpperArm != null)
        {
            leftUpperArmStartRotation = leftUpperArm.localRotation;
        }

        if (leftForeArm != null)
        {
            leftForeArmStartRotation = leftForeArm.localRotation;
        }

        if (rightUpperArm != null)
        {
            rightUpperArmStartRotation = rightUpperArm.localRotation;
        }

        if (rightForeArm != null)
        {
            rightForeArmStartRotation = rightForeArm.localRotation;
        }
    }

    private void LateUpdate()
    {
        if (paddleAnimator == null)
        {
            return;
        }

        float power = paddleAnimator.RowPower * poseStrength;
        power = Mathf.Clamp01(power);

        if (power <= 0.02f)
        {
            ReturnToIdle();
            return;
        }

        
        float phase = paddleAnimator.RowPhase + phaseOffset;
        float radians = phase * Mathf.PI * 2f;

        // Dùng Cos để đồng bộ với nhịp nhúng mái chèo xuống nước trong PaddleRowingAnimator.
        // 0 = vươn người ra trước
        // 1 = kéo tay về sau
        float pull01 = (Mathf.Cos(radians) + 1f) * 0.5f;

        if (invertPull)
        {
            pull01 = 1f - pull01;
        }

        pull01 = Smooth01(pull01);

        AnimateBone(
            bodyBone,
            bodyStartRotation,
            Vector3.Lerp(bodyReachEuler, bodyPullEuler, pull01) * power
        );

        AnimateBone(
            headBone,
            headStartRotation,
            Vector3.Lerp(headReachEuler, headPullEuler, pull01) * power
        );

        AnimateBone(
            leftUpperArm,
            leftUpperArmStartRotation,
            Vector3.Lerp(leftUpperArmReachEuler, leftUpperArmPullEuler, pull01) * power
        );

        AnimateBone(
            leftForeArm,
            leftForeArmStartRotation,
            Vector3.Lerp(leftForeArmReachEuler, leftForeArmPullEuler, pull01) * power
        );

        AnimateBone(
            rightUpperArm,
            rightUpperArmStartRotation,
            Vector3.Lerp(rightUpperArmReachEuler, rightUpperArmPullEuler, pull01) * power
        );

        AnimateBone(
            rightForeArm,
            rightForeArmStartRotation,
            Vector3.Lerp(rightForeArmReachEuler, rightForeArmPullEuler, pull01) * power
        );
    }

    private void AnimateBone(Transform bone, Quaternion startRotation, Vector3 eulerOffset)
    {
        if (bone == null)
        {
            return;
        }

        Quaternion targetRotation = startRotation * Quaternion.Euler(eulerOffset);

        bone.localRotation = Quaternion.Slerp(
            bone.localRotation,
            targetRotation,
            1f - Mathf.Exp(-poseSmooth * Time.deltaTime)
        );
    }

    private void ReturnToIdle()
    {
        ReturnBone(bodyBone, bodyStartRotation);
        ReturnBone(headBone, headStartRotation);

        ReturnBone(leftUpperArm, leftUpperArmStartRotation);
        ReturnBone(leftForeArm, leftForeArmStartRotation);

        ReturnBone(rightUpperArm, rightUpperArmStartRotation);
        ReturnBone(rightForeArm, rightForeArmStartRotation);
    }

    private void ReturnBone(Transform bone, Quaternion startRotation)
    {
        if (bone == null)
        {
            return;
        }

        bone.localRotation = Quaternion.Slerp(
            bone.localRotation,
            startRotation,
            1f - Mathf.Exp(-poseSmooth * Time.deltaTime)
        );
    }

    private float Smooth01(float t)
    {
        return t * t * (3f - 2f * t);
    }
}