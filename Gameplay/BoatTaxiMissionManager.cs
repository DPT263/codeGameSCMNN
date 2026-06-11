using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class BoatTaxiMissionManager : MonoBehaviour
{
    [Header("Zones")]
    [SerializeField] private BoatMissionZone pickupZone;
    [SerializeField] private BoatMissionZone dropoffZone;

    [Header("Zone Pools")]
    [SerializeField] private BoatMissionZone[] pickupZonePool;
    [SerializeField] private BoatMissionZone[] dropoffZonePool;

    [Header("UI")]
    [SerializeField] private BoatMissionUI missionUI;

    [Header("Time")]
    [SerializeField] private BoatDayTimeManager dayTimeManager;
    [SerializeField] private bool hasShownTimeOverNotice;
    [SerializeField] private bool dayFinished;

    [Header("Skill")]
    [SerializeField] private BoatSkillManager boatSkillManager;

    [Header("Passenger Visual")]
    [SerializeField] private GameObject passengerOnBoat;

    [Header("Mission State")]
    [SerializeField] private bool hasPassenger;
    
    [Header("Progress")]
    [SerializeField] private int currentMoney;
    [SerializeField] private int targetMoney = 50000;

    [SerializeField] private int currentTrips;
    [SerializeField] private int targetTrips = 5;

    [SerializeField] private int rewardPerTrip = 10000;

    [Header("Obstacle Penalty")]
    [SerializeField] private int currentTripPenalty;
    [SerializeField] private int currentTripCollisionCount; //số lần va chạm
    [SerializeField] private int minRewardPerTrip = 2000; 

    [Header("Day Summary Stats")]
    [SerializeField] private int totalDayPenalty;
    [SerializeField] private int totalDayCollisionCount;

    [Header("Input")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private KeyCode continueKey = KeyCode.Return;

    [Header("After Day")]
    [SerializeField] private bool reloadCurrentScene = true;
    [SerializeField] private string nextSceneName = "";

    [Header("Week Progress")]
    [SerializeField] private BoatWeekProgressManager weekProgressManager;
    private BoatWeekProgressManager.DayResult lastDayResult;
    
    public Transform CurrentTargetTransform 
    {
        get
        {
            if (hasPassenger)
            {
                return dropoffZone != null ? dropoffZone.transform : null; 
            }

            return pickupZone != null ? pickupZone.transform : null;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        currentMoney = 0;
        currentTrips = 0;
        hasPassenger = false;

        if(passengerOnBoat != null)
        {
            passengerOnBoat.SetActive(false);
        }

        SelectNewRoute();

        UpdateMissionUI();
        UpdateZoneVisibility();

        if (missionUI != null)
        {
            missionUI.HideMessage();
        }

        if (weekProgressManager == null)
        {
            weekProgressManager = FindObjectOfType<BoatWeekProgressManager>();
        }
    }

    private void Update()
    {
        if (dayFinished)
        {
            HandleDayFinishedInput();
            return;
        }

        CheckTimeOver();

        if(Input.GetKeyDown(interactKey))
        {
            HandleInteraction();
        }
    }

    private void HandleDayFinishedInput()
    {
        if(Input.GetKeyDown(continueKey))
        {
            ContinueAfterDay();
        }
    }
    
    private void ContinueAfterDay()
    {
        if (lastDayResult.gameOver)
        {
            if (weekProgressManager != null)
            {
                weekProgressManager.ResetWeekProgress();
            }

            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
            return;
        }

        if (lastDayResult.weekCompleted)
        {
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
                return;
            }

            Debug.LogWarning("Đã hoàn thành tuần nhưng chưa đặt Next Scene Name.");
            return;
        }

        if (reloadCurrentScene)
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
            return;
        }

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
    
    private void CheckTimeOver()
    {
        if (dayTimeManager == null)
        {
            return;
        }

        if (!dayTimeManager.IsDayEnded)
        {
            return;
        }

        if (hasShownTimeOverNotice)
        {
            return;
        }

        hasShownTimeOverNotice = true;
        UpdateZoneVisibility();

        if (hasPassenger)
        {
            if (missionUI != null)
            {
                missionUI.ShowMessage("Đã 17:00! Hãy trả khách cuối cùng.");
                missionUI.SetObjective("Trả khách cuối cùng");
            }
        }
        else
        {
            FinishDay();
        }
    }

    private bool CanAcceptNewPassenger()
    {
        if (dayTimeManager == null)
        {
            return true;
        }

        return !dayTimeManager.IsDayEnded; 
    }
    
    private bool IsZoneReady(BoatMissionZone zone)
    {
        return zone != null
            && zone.gameObject.activeInHierarchy
            && zone.BoatInside;
    }

    private void HandleInteraction()
    {
        if (!hasPassenger && IsZoneReady(pickupZone))
        {
            if (!CanAcceptNewPassenger())
            {
                FinishDay();
                return;
            }

            PickupPassenger();
            return;
        }
        if (hasPassenger && IsZoneReady(dropoffZone))
        {
            DropoffPassenger();
            return;
        }
        if(missionUI != null)
        {
            if(!hasPassenger)
            {
                missionUI.ShowMessage("Đi đến điểm đón khách để bắt đầu chuyến đi.");
            }
            else
            {
                missionUI.ShowMessage("Đi đến điểm trả khách để hoàn thành chuyến đi.");
            }
        }
    }

    private void PickupPassenger()
    {
        hasPassenger = true;
        currentTripPenalty = 0;
        currentTripCollisionCount = 0;

        if(passengerOnBoat != null)
        {
            passengerOnBoat.SetActive(true);
        }
        UpdateMissionUI();
        UpdateZoneVisibility();
        if(missionUI != null)
        {
            missionUI.ShowMessage("Đã đón khách! Hãy đến điểm trả khách.");
        }
    }

    private void DropoffPassenger()
    {
        hasPassenger = false;
        if(passengerOnBoat != null)
        {
            passengerOnBoat.SetActive(false);
        }
        
        int safeMinReward = Mathf.Clamp(minRewardPerTrip, 0, rewardPerTrip);
        int maxPenalty = rewardPerTrip - safeMinReward;

        int penaltyUsed = Mathf.Min(currentTripPenalty, maxPenalty);
        int finalReward = rewardPerTrip - penaltyUsed;
        int collisionCount = currentTripCollisionCount;
                
        currentMoney += finalReward;
        currentTrips++;

        totalDayPenalty += penaltyUsed;
        totalDayCollisionCount += collisionCount;
        
        currentTripPenalty = 0;
        currentTripCollisionCount = 0;

        if (dayTimeManager == null || !dayTimeManager.IsDayEnded)
        {
            SelectNewRoute();
        }

        UpdateMissionUI();
        UpdateZoneVisibility();
        if (dayTimeManager != null && dayTimeManager.IsDayEnded)  
        {
            FinishDay();
            return;
        }

        if (currentTrips >= targetTrips || currentMoney >= targetMoney)
        {
            if (missionUI != null)
            {
                missionUI.ShowMessage("Đã đạt mục tiêu chính! Còn thời gian thì có thể nhận thêm chuyến.");
            }

            return;
        }

        if (missionUI != null)
        {
            if (penaltyUsed > 0)
            {
                missionUI.ShowMessage(
                    "Hoàn thành chuyến! +" +
                    rewardPerTrip.ToString("N0") +
                    "đ - " +
                    penaltyUsed.ToString("N0") +
                    "đ phạt = +" +
                    finalReward.ToString("N0") +
                    "đ"
                );
            }
            else
            {
                missionUI.ShowMessage("Hoàn thành chuyến! +" + rewardPerTrip.ToString("N0") + "đ");
            }
        }
    }

    private void UpdateMissionUI()
    {
        if (missionUI == null)
        {
            return;
        }   

        missionUI.SetMoney(currentMoney, targetMoney);
        missionUI.SetTrips(currentTrips, targetTrips);

        if (hasPassenger && !dayFinished)
        {
            missionUI.SetTripStatus(
                true,
                rewardPerTrip,
                currentTripPenalty,
                currentTripCollisionCount,
                minRewardPerTrip
            );
        }
        else
        {
            missionUI.HideTripStatus();
        }

        if (dayFinished)
        {
            return;
        }

        if (hasPassenger)
        {
            missionUI.SetObjective("Đi đến điểm trả khách");
        }
        else if (currentTrips >= targetTrips || currentMoney >= targetMoney)
        {
            missionUI.SetObjective("Đã đủ mục tiêu, có thể nhận thêm chuyến");
        }
        else
        {
            missionUI.SetObjective("Đi đến điểm đón khách");
        }
    }

    private BoatMissionZone GetRandomZone(BoatMissionZone[] zones)
    {
        if (zones == null || zones.Length == 0)
        {
            return null;
        }

        int index = Random.Range(0, zones.Length);
        return zones[index];
    }

    private void HideZonePool(BoatMissionZone[] zones)
    {
        if (zones == null)
        {
            return;
        }

        for (int i = 0; i < zones.Length; i++)
        {
            if (zones[i] != null)
            {
                zones[i].gameObject.SetActive(false);
            }
        }
    }

    private void SelectNewRoute()
    {
        HideZonePool(pickupZonePool);
        HideZonePool(dropoffZonePool);

        BoatMissionZone newPickupZone = GetRandomZone(pickupZonePool);
        BoatMissionZone newDropoffZone = GetRandomZone(dropoffZonePool);
    
        if (newPickupZone != null)
        {
            pickupZone = newPickupZone;
        }
        if (newDropoffZone != null)
        {
            dropoffZone = newDropoffZone;
        }
    }
    
    private void UpdateZoneVisibility()
    {
        bool timeOver = dayTimeManager != null && dayTimeManager.IsDayEnded; 

        if (pickupZone != null)
        {
            pickupZone.gameObject.SetActive(!hasPassenger && !timeOver && !dayFinished);
        }

        if (dropoffZone != null)
        {
            dropoffZone.gameObject.SetActive(hasPassenger && !dayFinished);
        }
    }

    private void FinishDay()
    {
        dayFinished = true;
        hasPassenger = false;
        
        if (passengerOnBoat != null)
        {
            passengerOnBoat.SetActive(false);
        }

        if (pickupZone != null)
        {
            pickupZone.gameObject.SetActive(false);
        }

        if (dropoffZone != null)
        {
            dropoffZone.gameObject.SetActive(false);
        }

        int earnedExp = currentTrips; // 1exp mỗi chuyến 
        if (boatSkillManager != null)
        {
            boatSkillManager.AddExp(earnedExp);
        }

        bool goalCompleted = currentTrips >= targetTrips || currentMoney >= targetMoney;

        if (weekProgressManager != null)
        {
            lastDayResult = weekProgressManager.ApplyEndOfDay(currentMoney, goalCompleted);
        }

        if (missionUI != null)
        {
            missionUI.ShowDaySummary(
                currentTrips, 
                targetTrips, 
                currentMoney, 
                targetMoney, 
                totalDayCollisionCount,
                totalDayPenalty,
                earnedExp,
                lastDayResult
            );

            if (lastDayResult.gameOver)
            {
                missionUI.SetObjective("Thua");
                missionUI.ShowMessage("Sóc không còn đủ tiền chăm Ngoại. Nhấn Enter để chơi lại.");
            }
            else if (lastDayResult.weekCompleted)
            {
                missionUI.SetObjective("Hoàn thành một tuần");
                missionUI.ShowMessage("Sóc đã vượt qua một tuần! Nhấn Enter để sang màn mới.");
            }
            else if (goalCompleted)
            {
                missionUI.SetObjective("Hoàn thành nhiệm vụ ngày");
                missionUI.ShowMessage("Hoàn thành nhiệm vụ của chú Tư! Nhấn Enter để sang ngày tiếp theo.");
            }
            else
            {
                missionUI.SetObjective("Chưa đạt mục tiêu ngày");
                missionUI.ShowMessage("Đã hết ngày. Nhấn Enter để sang ngày tiếp theo.");
            }
        }
    }

    public bool TryApplyObstacleHit(int penaltyAmount)
    {
        if (dayFinished)
        {
            return false;
        }

        if (!hasPassenger)
        {
            return false;
        }

        if (penaltyAmount <= 0)
        {
            return false;
        }

        currentTripCollisionCount++;

        int safeMinReward = Mathf.Clamp(minRewardPerTrip, 0, rewardPerTrip);
        int maxPenalty = rewardPerTrip - safeMinReward;

        if (currentTripPenalty >= maxPenalty)
        {
            UpdateMissionUI();
            return true;
        }

        int remainingPenaltyCapacity = maxPenalty - currentTripPenalty;
        int actualPenalty = Mathf.Min(penaltyAmount, remainingPenaltyCapacity);

        currentTripPenalty += actualPenalty;

        if (missionUI != null && actualPenalty > 0)
        {
            missionUI.ShowPenaltyPopup(actualPenalty);
        }

        UpdateMissionUI();

        return true;
    }
}
