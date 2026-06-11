using UnityEngine;

public class BoatDayTimeManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private BoatMissionUI missionUI;

    [Header("Time Settings")]
    [SerializeField] private int startHour = 7;
    [SerializeField] private int startMinute = 0;

    [SerializeField] private int endHour = 17;
    [SerializeField] private int endMinute = 0;

    [SerializeField] private float secondsPerGameMinute = 1; // 1 real second = 1 game minute

    private int currentTotalMinutes; 
    private int endTotalMinutes;
    private float timer;
    private bool isDayEnded;

    public bool IsDayEnded
    {
        get { return isDayEnded; }
    }

    public int CurrentHour
    {
        get { return currentTotalMinutes / 60; }
    }

    public int CurrentMinute
    {
        get { return currentTotalMinutes % 60; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        currentTotalMinutes = startHour * 60 + startMinute;
        endTotalMinutes = endHour * 60 + endMinute; 
        
        UpdateTimeUI();
    }

    // Update is called once per frame
    private void Update()
    {
        if (isDayEnded)
        {
            return;
        }
        
        timer += Time.deltaTime; 

        while (timer >= secondsPerGameMinute) 
        {
            timer -= secondsPerGameMinute; 
            AdvanceOneMinute(); 

            if (isDayEnded) 
            {
                break;
            }
        }
    }

    private void AdvanceOneMinute() 
    {
        currentTotalMinutes++;

        if (currentTotalMinutes >= endTotalMinutes)
        {
            currentTotalMinutes = endTotalMinutes;
            isDayEnded = true;
        }

        UpdateTimeUI();
    }

    private void UpdateTimeUI()
    {
        if (missionUI != null)
        {
            missionUI.SetTime(CurrentHour, CurrentMinute);
        }
    }
}
