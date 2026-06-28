using UnityEngine;
using TMPro;

public class MarketUIDisplay : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text cashText;
    public TMP_Text goldText;
    
    [Header("Display Settings")]
    public string cashPrefix = "Cash: ";
    public string goldPrefix = "Gold: ";
    public string suffix = "";
    
    void OnEnable()
    {
        UpdateUI();
    }
    
    void Update()
    {
        UpdateUI();
    }
    
    public void UpdateUI()
    {
        if (PlayerManager.instance == null) return;
        
        if (cashText != null)
        {
            cashText.text = cashPrefix + Mathf.FloorToInt(PlayerManager.instance.playerCash).ToString() + suffix;
        }
        
        if (goldText != null)
        {
            goldText.text = goldPrefix + Mathf.FloorToInt(PlayerManager.instance.playerGold).ToString() + suffix;
        }
    }
}