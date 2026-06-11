using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerScene : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerController playerController;

    [Header("UI")]
    [SerializeField] private GameObject promptObject;
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private string promptMessage = "Nhấn E để xuống xuồng";

    [Header("Scene")]
    [SerializeField] private string sceneToLoad = "LaiXuong";

    [Header("Interaction")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private bool playerInside;
    private bool isLoading;

    private void Reset()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void Start()
    {
        SetPromptVisible(false);

        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }
    }

    private void Update()
    {
        if (!playerInside || isLoading)
        {
            return;
        }

        if (Input.GetKeyDown(interactKey))
        {
            LoadTargetScene();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerController == null)
        {
            return;
        }

        Transform hitRoot = other.attachedRigidbody != null
            ? other.attachedRigidbody.transform
            : other.transform.root;

        if (hitRoot != playerController.transform)
        {
            return;
        }

        playerInside = true;
        SetPromptVisible(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (playerController == null)
        {
            return;
        }

        Transform hitRoot = other.attachedRigidbody != null
            ? other.attachedRigidbody.transform
            : other.transform.root;

        if (hitRoot != playerController.transform)
        {
            return;
        }

        playerInside = false;
        SetPromptVisible(false);
    }

    private void LoadTargetScene()
    {
        isLoading = true;
        SetPromptVisible(false);

        SceneManager.LoadScene(sceneToLoad);
    }

    private void SetPromptVisible(bool visible)
    {
        if (promptObject != null)
        {
            promptObject.SetActive(visible);
        }

        if (promptText != null)
        {
            promptText.text = promptMessage;
        }
    }
}