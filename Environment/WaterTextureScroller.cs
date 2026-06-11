using UnityEngine;

public class WaterTextureScroller : MonoBehaviour
{
    [Header("Water Renderer")]
    [SerializeField] private Renderer waterRenderer;
    [Header("Texture Settings")]
    [SerializeField] private string texturePropertyName = "_MainTex";
    [Header("Scroll Speed")]
    [SerializeField] private Vector2 scrollSpeed = new Vector2(0.02f, 0.01f);
    private Vector2 currentOffset; //vị trí hiện tại của texture.
    private Material waterMaterial; //material đang được script điều khiển.

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (waterRenderer == null)
        {
            waterRenderer = GetComponent<Renderer>();
        }

        if (waterRenderer != null)
        {
            waterMaterial = waterRenderer.material;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (waterMaterial == null)
        {
            return;
        }

        currentOffset += scrollSpeed * Time.deltaTime;
        waterMaterial.SetTextureOffset(texturePropertyName, currentOffset);
    }
}
