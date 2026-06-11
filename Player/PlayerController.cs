using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")] //tiêu đề trong Inspector
    [SerializeField] private float moveSpeed = 5f; // Tốc độ di chuyển của người chơi
    [SerializeField] private float rotationSpeed = 10f; // Tốc độ xoay của người chơi

    [Header("Animator")]
    [SerializeField] private Animator animator; // Tham chiếu đến Animator để điều khiển hoạt ảnh
    [SerializeField] private string isMovingParameter = "IsMoving"; // Tên tham số trong Animator để kiểm soát hoạt ảnh di chuyển

    private Rigidbody rb;  
    //Vector3 là  x y z
    private Vector3 movementInput; // Đầu vào di chuyển từ người chơi

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

//Hàm update là hàm mà unity gọi mỗi frame (FPS), dùng để xử lý các logic liên quan đến input và các hành động không liên quan đến vật lý
    private void Update()
    {
        // Lấy đầu vào di chuyển từ người chơi
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        movementInput = new Vector3(moveHorizontal, 0f, moveVertical).normalized;

        if (animator != null)
        {
            bool isMoving = movementInput.sqrMagnitude > 0.001f;
            animator.SetBool(isMovingParameter, isMoving);
        }
    }
//Phụ thuộc physics, hàm xử lý vật lý (Rigidbody)
    private void FixedUpdate()
    {
        Vector3 targetVelocity = movementInput * moveSpeed; // Tính toán vận tốc 
        Vector3 newVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z); 
        rb.linearVelocity = newVelocity; // Cập nhật vận tốc của rigidbody, giữ nguyên vận tốc theo trục y để không ảnh hưởng đến trọng lực như nhảy hoặc rơi

        if (movementInput.sqrMagnitude > 0.001f) //sqrMagnitude là bình phương của độ dài vector, kiểm tra nếu có đầu vào di chuyển đủ lớn để xoay
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementInput); //Quaternion dùng để xoay object
            Quaternion smoothRotation = Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );

            rb.MoveRotation(smoothRotation);
        }
    }
}
