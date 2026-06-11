using UnityEngine;
using TMPro;

public class BoatTargetDistanceUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform boat;
    [SerializeField] private BoatTaxiMissionManager missionManager;
    [SerializeField] private TMP_Text distanceText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDistanceText();
    }

    private void UpdateDistanceText()
    {
        if(boat == null || missionManager == null || distanceText == null)
        {
            return;
        }

        Transform targetTransform = missionManager.CurrentTargetTransform;

        if(targetTransform == null)
        {
            distanceText.text = "Khoảng cách: -- m";
            return;
        }

        float distance = Vector3.Distance(boat.position, targetTransform.position);
        distanceText.text = "Khoảng cách : " + Mathf.RoundToInt(distance) + "m";
    }
}
