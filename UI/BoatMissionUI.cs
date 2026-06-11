using TMPro;
using UnityEngine;
using System.Collections;

public class BoatMissionUI : MonoBehaviour
{
    [Header("Main Texts")]
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text tripText;
    [SerializeField] private TMP_Text objectiveText;
    [SerializeField] private TMP_Text timeText;

    [Header("Message")]
    [SerializeField] private GameObject messageObject;
    [SerializeField] private TMP_Text messageText;
    
    [Header("Day Summary")]
    [SerializeField] private GameObject daySummaryPanel;
    [SerializeField] private TMP_Text summaryTitleText;
    [SerializeField] private TMP_Text summaryBodyText;
    [SerializeField] private TMP_Text summaryHintText;

    [Header("Default Values")]
    [SerializeField] private int targetMoney = 50000;
    [SerializeField] private int targetTrips = 5;

    [Header("Current Trip")]
    [SerializeField] private GameObject tripStatusObject;
    [SerializeField] private TMP_Text tripStatusText;

    [Header("Penalty Popup")]
    [SerializeField] private GameObject penaltyPopupObject;
    [SerializeField] private TMP_Text penaltyPopupText;
    [SerializeField] private float penaltyPopupDuration = 0.6f;

    private Coroutine penaltyPopupRoutine; //coroutine là một phương thức đặc biệt trong Unity cho phép thực hiện các tác vụ theo thời gian, như chờ đợi hoặc lặp lại hành động sau một khoảng thời gian nhất định.

    private int currentMoney;
    private int currentTrips;

    private void Start()
    {
        SetMoney(0, targetMoney);
        SetTrips(0, targetTrips);
        SetObjective("Đi đến điểm đón khách");
        SetTime(7, 0);
        HideMessage();
        HideTripStatus();
        HidePenaltyPopup();
        HideDaySummary();
    }

    public void SetMoney(int money, int target)
    {
        currentMoney = money;
        targetMoney = target;

        if (moneyText != null)
        {
            moneyText.text = "Tiền: " + FormatMoney(currentMoney) + " / " + FormatMoney(targetMoney);
        }
    }

    public void SetTrips(int trips, int target)
    {
        currentTrips = trips;
        targetTrips = target;

        if (tripText != null)
        {
            tripText.text = "Chuyến: " + currentTrips + " / " + targetTrips;
        }
    }

    public void SetObjective(string objective)
    {
        if (objectiveText != null)
        {
            objectiveText.text = "Nhiệm vụ: " + objective;
        }
    }

    public void SetTime(int hour, int minute)
    {
        if (timeText != null)
        {
            timeText.text = "GIỜ: " + hour.ToString("00") + ":" + minute.ToString("00");
        }
    }

    public void ShowMessage(string message)
    {
        if (messageObject != null)
        {
            messageObject.SetActive(true);
        }

        if (messageText != null)
        {
            messageText.text = message;
        }
    }

    public void HideMessage()
    {
        if (messageObject != null)
        {
            messageObject.SetActive(false);
        }
    }

    public void HideDaySummary()
    {
        if (daySummaryPanel != null)
        {
            daySummaryPanel.SetActive(false);
        }
    }

    public void ShowDaySummary(
        int trips, 
        int targetTrips, 
        int money, 
        int targetMoney,
        int totalCollisionCount, 
        int totalPenalty, 
        int exp,
        BoatWeekProgressManager.DayResult dayResult)
    {
        if (daySummaryPanel != null)
        {
            daySummaryPanel.SetActive(true);
        }
        if (summaryTitleText != null)
        {
            summaryTitleText.text = "TỔNG KẾT NGÀY " + dayResult.day;
        }

        bool goalCompleted = trips >= targetTrips || money >= targetMoney;
        string goalText = goalCompleted ? "Hoàn thành" : "Chưa đạt";

        if (summaryBodyText != null)
        {
            string resultText = "Qua ngày";

            if (dayResult.gameOver)
            {
                resultText = "Thua - Sóc không còn đủ tiền chăm Ngoại";
            }
            else if (dayResult.weekCompleted)
            {
                resultText = "Hoàn thành một tuần - mở màn cứu hộ động vật";
            }

            summaryBodyText.text =
                "Chuyến hoàn thành: " + trips + " / " + targetTrips + "\n" +
                "Tiền kiếm trong ngày: " + FormatMoney(money) + " / " + FormatMoney(targetMoney) + "\n" +
                "Nhiệm vụ ngày: " + goalText + "\n" +
                "Thưởng nhiệm vụ: +" + FormatMoney(dayResult.goalBonus) + "\n" +
                "Tổng va chạm: " + totalCollisionCount + "\n" +
                "Tổng tiền bị phạt: -" + FormatMoney(totalPenalty) + "\n" +
                "Tiền thuốc cho Ngoại: -" + FormatMoney(dayResult.careCost) + "\n" +
                "Tổng tiền còn lại của Sóc: " + FormatMoney(dayResult.totalMoneyAfterDay) + "\n" +
                "EXP lái xuồng: +" + exp + "\n" +
                "Kết quả: " + resultText;
        }

        if (summaryHintText != null)
        {
            summaryHintText.text = "Nhấn Enter để tiếp tục";
        }
    }

    public void SetTripStatus(
        bool hasPassenger,
        int baseReward,
        int currentPenalty,
        int collisionCount,
        int minReward
    )
    {
        if (!hasPassenger)
        {
            HideTripStatus();
            return;
        }

        int safeMinReward = Mathf.Clamp(minReward, 0, baseReward); //Clamp sẽ giới hạn giá trị của minReward trong khoảng từ 0 đến baseReward để đảm bảo rằng nó không vượt quá baseReward và không âm.
        int maxPenalty = baseReward - safeMinReward;
        int shownPenalty = Mathf.Min(currentPenalty, maxPenalty);
        int tempReward = baseReward - shownPenalty;

        if (tripStatusObject != null)
        {
            tripStatusObject.SetActive(true);
        }

        if (tripStatusText != null)
        {
            tripStatusText.text =
                "CHUYẾN HIỆN TẠI\n" +
                "Tiền chuyến: " + FormatMoney(baseReward) + "\n" +
                "Va chạm: " + collisionCount + "\n" +
                "Phạt: -" + FormatMoney(shownPenalty) + "\n" +
                "Tạm nhận: " + FormatMoney(tempReward);
        }
    }
    
    public void HideTripStatus()
    {
        if (tripStatusObject != null)
        {
            tripStatusObject.SetActive(false);
        }
    }

    public void ShowPenaltyPopup(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        if (penaltyPopupObject != null)
        {
            penaltyPopupObject.SetActive(true);
        }

        if (penaltyPopupText != null)
        {
            penaltyPopupText.text = "-" + FormatMoney(amount);
        }

        if (penaltyPopupRoutine != null)
        {
            StopCoroutine(penaltyPopupRoutine);
        }

        penaltyPopupRoutine = StartCoroutine(HidePenaltyPopupAfterDelay());
    }

    private IEnumerator HidePenaltyPopupAfterDelay() //IEnumerator là một kiểu trả về đặc biệt trong Unity cho phép bạn tạo ra các phương thức có thể tạm dừng và tiếp tục thực thi sau một khoảng thời gian hoặc điều kiện nhất định.
    {
        yield return new WaitForSeconds(penaltyPopupDuration); 
        HidePenaltyPopup();
    }

    public void HidePenaltyPopup()
    {
        if (penaltyPopupObject != null)
        {
            penaltyPopupObject.SetActive(false);
        }

        penaltyPopupRoutine = null;
    }

    private string FormatMoney(int amount)
    {
        return amount.ToString("N0") + "đ";
    }
}