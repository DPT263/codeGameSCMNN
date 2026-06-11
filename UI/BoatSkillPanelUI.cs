using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BoatSkillPanelUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BoatSkillManager boatSkillManager;
    [SerializeField] private GameObject rootPanel;

    [Header("Input")]
    [SerializeField] private KeyCode toggleKey = KeyCode.K;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text expText;
    [SerializeField] private TMP_Text speedBonusText;
    [SerializeField] private TMP_Text speedMultiplierText;
    [SerializeField] private TMP_Text hintText;
    [SerializeField] private Slider expSlider;

    private bool isOpen;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetPanelVisible(false);
        RefreshUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isOpen = !isOpen;
            SetPanelVisible(isOpen);

            if (isOpen)
            {
                RefreshUI();
            }
        }

        if (isOpen)
        {
            RefreshUI();
        }
    }

    private void SetPanelVisible(bool visible)
    {
        if (rootPanel != null)
        {
            rootPanel.SetActive(visible);
        }
    }

    private void RefreshUI()
    {
        if (boatSkillManager == null)
        {
            return;
        }

        int level = boatSkillManager.CurrentLevel;
        int currentExp = boatSkillManager.CurrentExp;
        int expToNext = boatSkillManager.ExpToNextLevel;
        int bonusPercent = boatSkillManager.CurrentSpeedBonusPercent;
        float multiplier = boatSkillManager.CurrentSpeedMultiplier;

        if (levelText != null)
        {
            levelText.text = "Cấp: " + level; 
        }

        if (expText != null)
        {
            expText.text = "EXP: " + currentExp + " / " + expToNext;
        }

        if (speedBonusText != null)
        {
            speedBonusText.text = "Bonus tốc độ: +" + bonusPercent + "%";
        }

        if (speedMultiplierText != null)
        {
            speedMultiplierText.text = "Hệ số tốc độ: x" + multiplier.ToString("0.0");
        }

        if (hintText != null)
        {
            hintText.text = "Nhấn K để đóng";
        }

        if (expSlider != null)
        {
            expSlider.maxValue = expToNext;
            expSlider.value = currentExp;
        }
    }
}
