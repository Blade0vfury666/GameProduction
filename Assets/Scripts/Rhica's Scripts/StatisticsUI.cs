using TMPro;
using UnityEngine;

public class StatisticsUI : MonoBehaviour
{
    public StatisticsManager stats;

    public TMP_Text employeeLevelText;
    public TMP_Text pcBonusText;
    public TMP_Text chairBonusText;
    public TMP_Text serverBonusText;

    void Start()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        employeeLevelText.text =
            "Employee Level: " + stats.employeeLevel;

        pcBonusText.text =
            stats.pcName + " (+" + stats.pcBonus + ")";

        chairBonusText.text =
            stats.chairName + " (+" + stats.chairBonus + "% XP)";

        serverBonusText.text =
            stats.serverName + " (+" + stats.serverBonus + "% Scope)";
    }
}