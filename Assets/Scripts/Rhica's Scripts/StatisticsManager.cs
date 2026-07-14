using UnityEngine;

public class StatisticsManager : MonoBehaviour
{
    public static StatisticsManager Instance;

    [Header("Player Stats")]
    public int employeeLevel = 1;

    [Header("Equipment Bonuses")]
    public int pcBonus = 0;
    public int chairBonus = 0;
    public int serverBonus = 0;

    [Header("Equipment Names")]
    public string pcName = "NA";
    public string chairName = "NA";
    public string serverName = "NA";

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        UpdateEmployeeLevel();
    }

    public void UpdateEmployeeLevel()
    {
        if (PlayerManager.instance == null)
        {
            Debug.LogWarning("PlayerManager.instance is null!");
            return;
        }

        int totalLevel = 0;
        int employeeCount = PlayerManager.instance.hiredEmployees.Count;

        if (employeeCount == 0)
        {
            employeeLevel = 1;
            return;
        }

        foreach (HiredEmployeeScript emp in PlayerManager.instance.hiredEmployees)
        {
            totalLevel += emp.GetFinalLevel();
        }

        employeeLevel = totalLevel / employeeCount;
    }

    public int GetEmployeeCount()
    {
        if (PlayerManager.instance == null) return 0;
        return PlayerManager.instance.hiredEmployees.Count;
    }
}