using UnityEngine;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    [Header("Player Stats")]
    public int playerLevel;
    public int pcSpec;
    public float playerCash;
    public float playerGold;
    public float playerXP;
    
    [Header("Testing Variables")]
    public int employeeCount;
    public int totalEmployeeLevel;

    [Header("UI References (DO NOT LEAVE EMPTY)")]
    public TMP_Text playerLevelText;
    public TMP_Text pcText;
    public TMP_Text cashText;
    public TMP_Text goldText;
    public TMP_Text playerXPText; 

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        playerLevelText.text = playerLevel.ToString();
        pcText.text = "PC Spec: Level " + pcSpec.ToString();
        
        cashText.text = Mathf.FloorToInt(playerCash).ToString();
        goldText.text = Mathf.FloorToInt(playerGold).ToString();
        playerXPText.text = "XP: " + Mathf.FloorToInt(playerXP).ToString();
    }
}