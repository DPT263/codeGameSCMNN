using TMPro;
using UnityEngine;

public class BoatTargetArrowUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform boat;
    [SerializeField] private BoatTaxiMissionManager missionManager;
    [SerializeField] private Camera worldCamera;

    [Header("UI")]
    [SerializeField] private GameObject arrowGroup;
    [SerializeField] private RectTransform arrowRect;
    [SerializeField] private TMP_Text hintText;

    [Header("Settings")]
    [SerializeField] private float hideWhenCloserThan = 2f;

    private void Start()
    {
        if (worldCamera == null)
        {
            worldCamera = Camera.main;
        }
    }

    private void Update()
    {
        UpdateArrow();
    }

    private void UpdateArrow()
    {
        if (boat == null || missionManager == null || arrowRect == null || worldCamera == null)
        {
            SetArrowVisible(false);
            return;
        }

        Transform target = missionManager.CurrentTargetTransform;

        if (target == null)
        {
            SetArrowVisible(false);
            return;
        }

        float distance = Vector3.Distance(boat.position, target.position);

        if (distance <= hideWhenCloserThan)
        {
            SetArrowVisible(false);
            return;
        }

        Vector3 boatScreenPosition = worldCamera.WorldToScreenPoint(boat.position);
        Vector3 targetScreenPosition = worldCamera.WorldToScreenPoint(target.position);

        Vector3 screenDirection = targetScreenPosition - boatScreenPosition;

        if (screenDirection.sqrMagnitude < 0.001f)
        {
            SetArrowVisible(false);
            return;
        }

        float angle = Mathf.Atan2(screenDirection.y, screenDirection.x) * Mathf.Rad2Deg;
        arrowRect.localRotation = Quaternion.Euler(0f, 0f, angle - 90f);

        SetArrowVisible(true);

        if (hintText != null)
        {
            hintText.text = "Hướng mục tiêu";
        }
    }

    private void SetArrowVisible(bool visible)
    {
        if (arrowGroup != null)
        {
            arrowGroup.SetActive(visible);
        }
    }
}