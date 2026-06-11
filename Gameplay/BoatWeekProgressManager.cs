using UnityEngine;

public class BoatWeekProgressManager : MonoBehaviour
{
    [Header("Week Progress")]
    [SerializeField] private int currentDay = 1;
    [SerializeField] private int daysToUnlockNextScene = 7;

    [Header("Money")]
    [SerializeField] private int totalSocMoney = 50000; //để test
    [SerializeField] private int dailyCareCost = 20000;
    [SerializeField] private int dailyGoalBonus = 15000;

    private const string DayKey = "WeekProgress_CurrentDay";
    private const string MoneyKey = "WeekProgress_TotalSocMoney";

    public int CurrentDay
    {
        get { return currentDay; }
    }

    public int DaysToUnlockNextScene
    {
        get { return daysToUnlockNextScene; }
    }

    public int TotalSocMoney
    {
        get { return totalSocMoney; }
    }

    public int DailyCareCost
    {
        get { return dailyCareCost; }
    }

    public int DailyGoalBonus
    {
        get { return dailyGoalBonus; }
    }

    public struct DayResult
    {
        public int day;
        public int dayEarnedMoney;
        public int goalBonus;
        public int careCost;
        public int totalMoneyAfterDay;
        public bool goalCompleted;
        public bool gameOver;
        public bool weekCompleted;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadProgress();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetInt(DayKey, currentDay);
        PlayerPrefs.SetInt(MoneyKey, totalSocMoney);
        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        currentDay = PlayerPrefs.GetInt(DayKey, currentDay);
        totalSocMoney = PlayerPrefs.GetInt(MoneyKey, totalSocMoney);
    }

    [ContextMenu("Reset Week Progress")]
    public void ResetWeekProgress()
    {
        currentDay = 1;
        totalSocMoney = 50000;

        SaveProgress();

        Debug.Log("Đã reset tiến trình tuần.");
    }

    public DayResult ApplyEndOfDay(int dayEarnedMoney, bool goalCompleted)
    {
        int goalBonus = goalCompleted ? dailyGoalBonus : 0;

        totalSocMoney += dayEarnedMoney;
        totalSocMoney += goalBonus;
        totalSocMoney -= dailyCareCost;

        bool gameOver = totalSocMoney <= 0;
        bool weekCompleted = !gameOver && currentDay >= daysToUnlockNextScene;

        DayResult result = new DayResult();

        result.day = currentDay;
        result.dayEarnedMoney = dayEarnedMoney;
        result.goalBonus = goalBonus;
        result.careCost = dailyCareCost;
        result.totalMoneyAfterDay = totalSocMoney;
        result.goalCompleted = goalCompleted;
        result.gameOver = gameOver;
        result.weekCompleted = weekCompleted;

        if (!gameOver && !weekCompleted)
        {
            currentDay++;
        }

        SaveProgress();

        return result;
    }
}
