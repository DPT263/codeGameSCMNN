using TMPro;
using UnityEngine;

public class QuestPanelUI : MonoBehaviour
{
    [Header("Panel Parts")]
    [SerializeField] private GameObject rootObject;
    [SerializeField] private GameObject contentObject;

    [Header("Texts")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private TMP_Text toggleButtonText;

    [Header("Input")]
    [SerializeField] private KeyCode toggleKey = KeyCode.Tab;

    private bool isVisible;
    private bool isExpanded = true;

    private void Start()
    {
        ApplyState();
    }

    private void Update()
    {
        if (!isVisible)
        {
            return;
        }

        if (Input.GetKeyDown(toggleKey))
        {
            ToggleExpand();
        }
    }

    public void ShowQuest(string title, string body, bool expanded = true)
    {
        isVisible = true;
        isExpanded = expanded;

        if (titleText != null)
        {
            titleText.text = title;
        }

        if (bodyText != null)
        {
            bodyText.text = body;
        }

        ApplyState();
    }

    public void HideQuest()
    {
        isVisible = false;
        ApplyState();
    }

    public void ToggleExpand()
    {
        if (!isVisible)
        {
            return;
        }

        isExpanded = !isExpanded;
        ApplyState();
    }

    public void SetExpanded(bool expanded)
    {
        isExpanded = expanded;
        ApplyState();
    }

    private void ApplyState()
    {
        if (rootObject != null)
        {
            rootObject.SetActive(isVisible);
        }

        if (contentObject != null)
        {
            contentObject.SetActive(isVisible && isExpanded);
        }

    }
}