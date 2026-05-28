using UnityEngine;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    // Static instance so ANY script in the scene can read or change these stats instantly
    public static PlayerManager instance;

    // Player Core Stats
    public int playerLevel;
    public int pcSpec;
    public int playerCash;
    public int researchPoints;
    public int userSkillLevel;

    // UI Display Objects (Drag your TextMeshPro components here in the Inspector)
    public TMP_Text levelText;
    public TMP_Text pcText;
    public TMP_Text cashText;
    public TMP_Text researchPointsText;
    public TMP_Text userSkillLevelText;

    void Awake()
    {
        instance = this;

        // Set starting stats for a solo developer
        playerLevel = 1;
        pcSpec = 1;
        playerCash = 100;
        researchPoints = 0;
        userSkillLevel = 1;
    }

    void Update()
    {
        // Foolproof UI updates: forces text fields to always show the exact variable numbers
        levelText.text = "" + playerLevel;
        pcText.text = "PC Spec: Level " + pcSpec;
        cashText.text = "" + playerCash;
        researchPointsText.text = "Research Points: " + researchPoints;
        userSkillLevelText.text = "user skill: " + userSkillLevel;
    }
}