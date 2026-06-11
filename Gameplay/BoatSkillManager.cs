using UnityEngine;

public class BoatSkillManager : MonoBehaviour
{
    [Header("Boat Reference")] // Tham chiếu đến BoatController để có thể điều chỉnh các thuộc tính của thuyền
    [SerializeField] private BoatController boatController;

    [Header("Level Data")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int currentExp = 0;
    [SerializeField] private int baseExpToNextLevel = 10;
    [SerializeField] private int expIncreasePerLevel = 5;

    [Header("Boat Bonus")]
    [SerializeField] private float speedBonusPerLevel = 0.1f; // Tăng 10% tốc độ mỗi cấp

    //nút reset để test
    [ContextMenu("Reset Boat Skill")]
    public void ResetBoatSkill()
    {
        currentLevel = 1;
        currentExp = 0;
        ApplyBoatBonus();
        SaveSkill();
    }

    public int CurrentLevel
    {
        get { return currentLevel; }
    }

    public int CurrentExp
    {
        get { return currentExp; }
    }

    public int ExpToNextLevel
    {
        get { return GetExpToNextLevel(); }
    }

    public float CurrentSpeedMultiplier
    {
        get { return 1f + (currentLevel - 1) * speedBonusPerLevel; }
    } 

    public int CurrentSpeedBonusPercent
    {
        get { return Mathf.RoundToInt((CurrentSpeedMultiplier - 1f) * 100f); }
    }

    private int GetExpToNextLevel()
    {
        return baseExpToNextLevel + (currentLevel - 1) * expIncreasePerLevel;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadSkill();
        ApplyBoatBonus();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddExp(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        currentExp += amount;

        while (currentExp >= GetExpToNextLevel())
        {
            currentExp -= GetExpToNextLevel();
            currentLevel++;
        }

        ApplyBoatBonus();
        SaveSkill();
    }
    
    private void ApplyBoatBonus()
    {
        if (boatController == null)
        {
            return;
        }

        float speedMultiplier = 1f + (currentLevel - 1) * speedBonusPerLevel;
        boatController.ApplySpeedMultiplier(speedMultiplier);
    }

    private void SaveSkill()
    {
        PlayerPrefs.SetInt("BoatSkill_Level", currentLevel); // PlayerPrefs là một hệ thống lưu trữ đơn giản của Unity, nó cho phép bạn lưu trữ dữ liệu dưới dạng key-value. Ở đây, chúng ta lưu cấp độ hiện tại của thuyền với key "BoatLevel" và kinh nghiệm hiện tại với key "BoatExp".
        PlayerPrefs.SetInt("BoatSkill_Exp", currentExp);
        PlayerPrefs.Save();
    }

    private void LoadSkill()
    {
        currentLevel = PlayerPrefs.GetInt("BoatSkill_Level", 1); 
        currentExp = PlayerPrefs.GetInt("BoatSkill_Exp", 0);
    }
}
