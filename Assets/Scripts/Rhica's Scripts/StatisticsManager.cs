using UnityEngine;

public class StatisticsManager : MonoBehaviour
{
    public static StatisticsManager Instance;

    [Header("Player Stats")]
    public int employeeLevel = 1;

    [Header("Equipment Bonuses")]
    public int pcBonus = 1;
    public int chairBonus = 5;
    public int serverBonus = 10;

    [Header("Equipment Names")]
    public string pcName = "Starterstation";
    public string chairName = "RedPanther";
    public string serverName = "Extra HDDs";

    private void Awake()
    {
        Instance = this;
    }
}
