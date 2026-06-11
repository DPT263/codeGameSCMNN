using UnityEngine;
using TMPro;
// script giúp object chữ luôn quay theo hướng camera
public class BillboardText : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera targetCamera;

    [Header("Rotation Offset")]
    [SerializeField] private Vector3 rotationOffset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main; // tự lấy main camera
        }
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void LateUpdate()
    {
        if (targetCamera == null)
            return;

        transform.rotation = targetCamera.transform.rotation * Quaternion.Euler(rotationOffset);
    }
}