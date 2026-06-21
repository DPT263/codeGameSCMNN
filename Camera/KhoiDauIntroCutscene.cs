using System.Collections;
using TMPro;
using UnityEngine;

public class KhoiDauIntroCutscene : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        public string speaker;

        [TextArea(2, 5)]
        public string text;
    }

    [Header("Player")]
    [SerializeField] private Transform player;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private string isMovingParameter = "IsMoving";

    [Header("Player Points")]
    [SerializeField] private Transform introPlayerPoint;
    [SerializeField] private Transform afterIntroSpawnPoint;

    [Header("Camera")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private SmartCameraFollow smartCameraFollow;
    [SerializeField] private Transform introCameraPoint;
    [SerializeField] private Transform gameplayCameraPoint;

    [Header("Scene Objects")]
    [SerializeField] private GameObject introRoomRoot;
    [SerializeField] private GameObject[] hideDuringIntro;
    [SerializeField] private GameObject[] showAfterIntro;

    [Header("Fade")]
    [SerializeField] private CanvasGroup fadeGroup;
    [SerializeField] private float fadeDuration = 0.8f;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text continueHintText;
    [SerializeField] private KeyCode nextKey = KeyCode.E;

    [Header("Dialogue Lines")]
    [SerializeField] private DialogueLine[] dialogueLines =
    {
        new DialogueLine
        {
            speaker = "Ngoại",
            text = "Sóc à... nước nổi năm nay lên nhanh quá, Ngoại lại bệnh nên chưa ra chợ được."
        },
        new DialogueLine
        {
            speaker = "Sóc",
            text = "Ngoại cứ nghỉ đi, con sẽ tìm cách kiếm tiền mua thuốc cho Ngoại."
        },
        new DialogueLine
        {
            speaker = "Chú Tư",
            text = "Sóc, chú đang cần người phụ lái xuồng đưa đón bà con ngoài chợ nổi."
        },
        new DialogueLine
        {
            speaker = "Chú Tư",
            text = "Mỗi ngày con cố gắng chạy đủ chuyến, kiếm tiền lo thuốc men cho Ngoại."
        },
        new DialogueLine
        {
            speaker = "Sóc",
            text = "Dạ, con sẽ cố gắng. Sáng nay con ra bến phụ chú."
        }
    };

    [Header("Quest After Intro")]
    [SerializeField] private QuestPanelUI questPanelUI;

    [TextArea(2, 4)]
    [SerializeField] private string questTitle = "Nhiệm vụ đầu tiên";

    [TextArea(2, 6)]
    [SerializeField] private string questBody = "Ra bến gặp chú Tư để nhận việc lái xuồng.";

    private bool isPlayingIntro;

    private void Start()
    {
        StartCoroutine(PlayIntroRoutine());
    }

    private IEnumerator PlayIntroRoutine()
    {
        // Chờ 1 frame để các script UI khác chạy Start() xong trước.
        yield return null;

        AutoFindReferences();

        isPlayingIntro = true;

        SetDialogueVisible(false);
        SetPlayerControl(false);

        if (playerAnimator != null)
        {
            playerAnimator.Play("Sitting");
        }

        if (introRoomRoot != null)
        {
            introRoomRoot.SetActive(true);
        }

        SetObjectsActive(hideDuringIntro, false);
        SetObjectsActive(showAfterIntro, false);

        MoveToPoint(player, introPlayerPoint);

        if (smartCameraFollow != null)
        {
            smartCameraFollow.enabled = false;
        }

        MoveToPoint(mainCamera != null ? mainCamera.transform : null, introCameraPoint);

        SetFadeInstant(1f, true);

        yield return Fade(1f, 0f);

        yield return PlayDialogue();

        yield return Fade(0f, 1f);

        SetDialogueVisible(false);

        MoveToPoint(player, afterIntroSpawnPoint);

        if (gameplayCameraPoint != null)
        {
            MoveToPoint(mainCamera != null ? mainCamera.transform : null, gameplayCameraPoint);
        }

        if (introRoomRoot != null)
        {
            introRoomRoot.SetActive(false);
        }

        SetObjectsActive(hideDuringIntro, true);
        SetObjectsActive(showAfterIntro, true);

        if (smartCameraFollow != null)
        {
            smartCameraFollow.SetTarget(player, playerRb);
            smartCameraFollow.enabled = true;
        }

        SetPlayerControl(true);

        if (playerAnimator != null)
        {
            playerAnimator.Play("metarig|Idle");
        }

        if (questPanelUI != null)
        {
            questPanelUI.ShowQuest(questTitle, questBody, true);
        }

        yield return Fade(1f, 0f);

        isPlayingIntro = false;
    }

    private void AutoFindReferences()
    {
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        if (player == null && playerController != null)
        {
            player = playerController.transform;
        }

        if (playerRb == null && player != null)
        {
            playerRb = player.GetComponent<Rigidbody>();
        }

        if (playerAnimator == null && player != null)
        {
            playerAnimator = player.GetComponentInChildren<Animator>();
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (smartCameraFollow == null && mainCamera != null)
        {
            smartCameraFollow = mainCamera.GetComponent<SmartCameraFollow>();
        }

        if (questPanelUI == null)
        {
            questPanelUI = FindObjectOfType<QuestPanelUI>();
        }
    }

    private IEnumerator PlayDialogue()
    {
        if (dialogueLines == null || dialogueLines.Length == 0)
        {
            yield break;
        }

        SetDialogueVisible(true);

        for (int i = 0; i < dialogueLines.Length; i++)
        {
            ShowDialogueLine(dialogueLines[i]);

            // Chờ thả phím để tránh bị nhảy nhiều dòng nếu người chơi đang giữ E.
            while (Input.GetKey(nextKey))
            {
                yield return null;
            }

            while (!Input.GetKeyDown(nextKey))
            {
                yield return null;
            }
        }

        SetDialogueVisible(false);
    }

    private void ShowDialogueLine(DialogueLine line)
    {
        string speaker = line != null ? line.speaker : "";
        string text = line != null ? line.text : "";

        if (speakerText != null)
        {
            speakerText.text = speaker;
        }

        if (dialogueText != null)
        {
            if (speakerText == null && !string.IsNullOrEmpty(speaker))
            {
                dialogueText.text = speaker + ": " + text;
            }
            else
            {
                dialogueText.text = text;
            }
        }

        if (continueHintText != null)
        {
            continueHintText.text = "Nhấn E";
        }
    }

    private void SetPlayerControl(bool enabled)
    {
        if (playerController != null)
        {
            playerController.enabled = enabled;
        }

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }

        if (playerAnimator != null && !string.IsNullOrEmpty(isMovingParameter))
        {
            playerAnimator.SetBool(isMovingParameter, false);
        }
    }

    private void MoveToPoint(Transform target, Transform point)
    {
        if (target == null || point == null)
        {
            return;
        }

        target.position = point.position;
        target.rotation = point.rotation;
    }

    private void SetDialogueVisible(bool visible)
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(visible);
        }
    }

    private void SetObjectsActive(GameObject[] objects, bool active)
    {
        if (objects == null)
        {
            return;
        }

        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] != null)
            {
                objects[i].SetActive(active);
            }
        }
    }

    private void SetFadeInstant(float alpha, bool blockRaycasts)
    {
        if (fadeGroup == null)
        {
            return;
        }

        fadeGroup.gameObject.SetActive(true);
        fadeGroup.alpha = alpha;
        fadeGroup.blocksRaycasts = blockRaycasts;
    }

    private IEnumerator Fade(float fromAlpha, float toAlpha)
    {
        if (fadeGroup == null)
        {
            yield break;
        }

        fadeGroup.gameObject.SetActive(true);
        fadeGroup.blocksRaycasts = true;

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            float t = fadeDuration <= 0f ? 1f : timer / fadeDuration;
            fadeGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, t);

            yield return null;
        }

        fadeGroup.alpha = toAlpha;

        if (toAlpha <= 0.01f)
        {
            fadeGroup.blocksRaycasts = false;
        }
    }
}