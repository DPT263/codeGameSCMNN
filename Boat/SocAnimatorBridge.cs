using UnityEngine;

public class SocAnimatorBridge : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private PaddleRowingAnimator paddleAnimator;

    [Header("Parameters")]
    [SerializeField] private string rowPowerParameter = "RowPower";
    [SerializeField] private string turnParameter = "Turn";

    private void Start()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (paddleAnimator == null)
        {
            paddleAnimator = GetComponentInParent<PaddleRowingAnimator>();
        }
    }

    private void Update()
    {
        if (animator != null)
        {
            if (paddleAnimator != null)
            {
                // Pass the rowing power (0 to 1) directly to the animator
                animator.SetFloat(rowPowerParameter, paddleAnimator.RowPower);

                // Synchronize animation play speed and direction with the paddle's row speed
                if (paddleAnimator.IsRowing)
                {
                    float direction = Input.GetKey(KeyCode.S) ? -1f : 1f;
                    float clipLength = 2.333333f;
                    animator.speed = paddleAnimator.RowSpeed * clipLength * direction;
                }
                else
                {
                    animator.speed = 1.0f;
                }
            }

            // Pass the horizontal turn direction directly to the animator (inverted for matching physical paddle side)
            float turn = -Input.GetAxisRaw("Horizontal");
            
            // Nếu nhấn phím tiến (W) hoặc lùi (S), ép Turn = 0f để giữ animation chèo hai tay
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
            {
                turn = 0f;
            }

            animator.SetFloat(turnParameter, turn);
        }
    }
}
