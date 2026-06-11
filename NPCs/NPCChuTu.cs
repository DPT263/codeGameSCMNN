using TMPro;
using UnityEngine;

public class NPCChuTu : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Rigidbody playerRb;

    [Header("Prompt UI")]
    [SerializeField] private GameObject promptObject;
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private string promptMessage = "Nhấn E để nói chuyện";

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text continueHintText;

    [Header("Dialogue Lines")]
    [TextArea(2, 5)]
    [SerializeField] private string[] dialogueLines =
    {
        "Sóc hả con, mấy nay con đã quen với việc chèo xuồng chưa?",
        "Con giúp chú ra chợ đưa đón bà con đi qua sông cho thuận tiện nhé."
    };

    [Header("Quest UI")]
    [SerializeField] private QuestPanelUI questPanelUI;

    [TextArea(2, 4)]
    [SerializeField] private string questTitle = "Nhiệm vụ của chú Tư";

    [TextArea(2, 6)]
    [SerializeField] private string questBody =
        "Lái xuồng đến chợ.";

    [Header("Interaction")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private bool playerInside;
    private bool isTalking;
    private int currentLineIndex;
    private bool hasGivenQuest;

    private void Start()
    {
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        if (playerController != null && playerRb == null)
        {
            playerRb = playerController.GetComponent<Rigidbody>();
        }

        SetPromptVisible(false);
        SetDialogueVisible(false);
    }

    private void Update()
    {
        if (!playerInside)
        {
            return;
        }

        if (Input.GetKeyDown(interactKey))
        {
            if (!isTalking)
            {
                StartDialogue();
            }
            else
            {
                ShowNextLine();
            }
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

        if (!isTalking)
        {
            SetPromptVisible(true);
        }
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

        if (isTalking)
        {
            EndDialogue();
        }
    }

    private void StartDialogue()
    {
        if (dialogueLines == null || dialogueLines.Length == 0)
        {
            return;
        }

        isTalking = true;
        currentLineIndex = 0;

        SetPromptVisible(false);
        SetDialogueVisible(true);
        UpdateDialogueText();

        if (playerController != null)
        {
            playerController.enabled = false;
        }

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }
    }

    private void ShowNextLine()
    {
        currentLineIndex++;

        if (currentLineIndex >= dialogueLines.Length)
        {
            EndDialogue();
            return;
        }

        UpdateDialogueText();
    }

    private void EndDialogue()
    {
        isTalking = false;
        SetDialogueVisible(false);

        if (playerController != null)
        {
            playerController.enabled = true;
        }

        if (!hasGivenQuest && questPanelUI != null)
        {
            questPanelUI.ShowQuest(questTitle, questBody, true);
            hasGivenQuest = true;
        }

        if (playerInside)
        {
            SetPromptVisible(true);
        }
    }

    private void UpdateDialogueText()
    {
        if (dialogueText != null && currentLineIndex >= 0 && currentLineIndex < dialogueLines.Length)
        {
            dialogueText.text = dialogueLines[currentLineIndex];
        }

        if (continueHintText != null)
        {
            continueHintText.text = "Nhấn E";
        }
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

    private void SetDialogueVisible(bool visible)
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(visible);
        }
    }
}