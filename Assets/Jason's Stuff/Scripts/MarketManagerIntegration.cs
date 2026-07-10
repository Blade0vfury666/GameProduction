using UnityEngine;
using TMPro;

public class MarketEquipmentIntegration : MonoBehaviour
{
    public static MarketEquipmentIntegration instance;

    [Header("UI References - Drag Your Texts Here")]
    public TMP_Text serverBonusText;
    public TMP_Text chairBonusText;
    public TMP_Text pcBonusText;
    public TMP_Text totalLevelBonusText;
    public TMP_Text totalXPBonusText;
    public TMP_Text totalScopeBonusText;

    [Header("Bonus Settings")]
    public float qualityBonusPerPCLevel = 0.05f;      // 5% per PC level
    public float costReductionPerChairLevel = 0.01f;   // 1% per Chair level
    public float scopeBonusPerServerLevel = 10f;       // 10 per Server level
    public int maxEffectiveLevel = 60;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void OnEnable()
    {
        UpdateAllUI();
    }

    void Update()
    {
        // Update UI every frame while market is open
        UpdateAllUI();
    }

    public void UpdateAllUI()
    {
        if (EquipmentManager.instance == null) return;
        if (PlayerManager.instance == null) return;

        // Get equipment bonuses
        int pcBonus = EquipmentManager.instance.GetTotalLevelBonus();
        int chairBonus = EquipmentManager.instance.GetTotalXPBonus();
        int serverBonus = EquipmentManager.instance.GetTotalScopeBonus();

        // Update individual bonus texts
        if (pcBonusText != null)
            pcBonusText.text = "PC Bonus: +" + pcBonus + " Employee Level";

        if (chairBonusText != null)
            chairBonusText.text = "Chair Bonus: -" + chairBonus + "% Development Cost";

        if (serverBonusText != null)
            serverBonusText.text = "Server Bonus: +" + serverBonus + " Max Scope";

        // Update total stats
        if (totalLevelBonusText != null)
            totalLevelBonusText.text = "Total Level Bonus: +" + pcBonus;

        if (totalXPBonusText != null)
            totalXPBonusText.text = "Total XP Bonus: +" + chairBonus + "%";

        if (totalScopeBonusText != null)
            totalScopeBonusText.text = "Total Scope Bonus: +" + serverBonus;

        // Update PlayerManager with current bonuses (already done by EquipmentManager)
    }

   
    // METHODS FOR MAKEGAME SCRIPT TO USE
    

    public int GetMaxScopeWithBonus(int baseMaxScope)
    {
        if (EquipmentManager.instance == null) return baseMaxScope;
        
        int serverBonus = EquipmentManager.instance.GetTotalScopeBonus();
        return baseMaxScope + Mathf.FloorToInt(serverBonus * scopeBonusPerServerLevel);
    }

    public float GetAdjustedBudget(float baseBudget)
    {
        if (EquipmentManager.instance == null) return baseBudget;
        
        int chairBonus = EquipmentManager.instance.GetTotalXPBonus();
        float reduction = chairBonus * costReductionPerChairLevel;
        return baseBudget * (1f - reduction);
    }

    public float GetQualityMultiplier()
    {
        if (EquipmentManager.instance == null) return 1f;
        
        int pcBonus = EquipmentManager.instance.GetTotalLevelBonus();
        return 1f + (pcBonus * qualityBonusPerPCLevel);
    }

    public int GetEffectiveEmployeeLevel(int baseLevel)
    {
        if (EquipmentManager.instance == null) return baseLevel;
        
        int pcBonus = EquipmentManager.instance.GetTotalLevelBonus();
        int effectiveLevel = baseLevel + pcBonus;
        return Mathf.Min(effectiveLevel, maxEffectiveLevel);
    }

    // ============================================
    // METHODS FOR EMPLOYEE SCRIPT TO USE
    // ============================================

    public int GetPCBonus()
    {
        if (EquipmentManager.instance == null) return 0;
        return EquipmentManager.instance.GetTotalLevelBonus();
    }

    public int GetChairBonus()
    {
        if (EquipmentManager.instance == null) return 0;
        return EquipmentManager.instance.GetTotalXPBonus();
    }

    public int GetServerBonus()
    {
        if (EquipmentManager.instance == null) return 0;
        return EquipmentManager.instance.GetTotalScopeBonus();
    }

    // ============================================
    // UI REFRESH
    // ============================================

    public void RefreshUI()
    {
        UpdateAllUI();
    }
}