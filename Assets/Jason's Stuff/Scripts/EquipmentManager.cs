using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager instance;

    [Header("Equipment Bonuses")]
    public int totalLevelBonus;
    public int totalXPBonus;
    public int totalScopeBonus;  // Percentage (e.g., 10 = 10%)

    [Header("Scope Conversion Settings")]
    [Tooltip("How much flat scope per 1% bonus. 0.5 = 0.5 scope per 1%")]
    public float scopeConversionMultiplier = 0.5f;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        ApplyBonusesToPlayer();
    }

    public void ApplyEquipmentBonuses()
    {
        totalLevelBonus = 0;
        totalXPBonus = 0;
        totalScopeBonus = 0;

        Equipment[] allEquipment = Resources.LoadAll<Equipment>("Equipment");

        foreach (Equipment item in allEquipment)
        {
            if (item.isEquipped)
            {
                totalLevelBonus += item.flatLevelBonus;
                totalXPBonus += item.flatXPBonus;
                totalScopeBonus += item.flatScopeBonus;
            }
        }

        ApplyBonusesToPlayer();

        if (MarketManager.instance != null)
        {
            MarketManager.instance.UpdateCurrentStatsUI();
        }
    }

    void ApplyBonusesToPlayer()
    {
        if (PlayerManager.instance == null)
        {
            Debug.LogError("PlayerManager.instance is NULL!");
            return;
        }

        // Computers → pcSpec (flat)
        PlayerManager.instance.pcSpec = totalLevelBonus;

        // Chairs → chairLevel (flat percentage)
        PlayerManager.instance.chairLevel = totalXPBonus;

        // Servers → serverLevel
        // CONVERT percentage to flat value using scaling factor
        // MakeGameScript uses: maxLimit = ... + (serverLevel * 10f)
        // So we convert: (totalScopeBonus * scopeConversionMultiplier) gives the flat value
        float flatValue = totalScopeBonus * scopeConversionMultiplier;
        PlayerManager.instance.serverLevel = Mathf.RoundToInt(flatValue);
        
        Debug.Log("Scope Bonus: " + totalScopeBonus + "% → Flat Value: " + PlayerManager.instance.serverLevel);
    }

    public void EquipItem(Equipment item)
    {
        Equipment[] allEquipment = Resources.LoadAll<Equipment>("Equipment");
        foreach (Equipment e in allEquipment)
        {
            if (e.category == item.category && e.isEquipped)
            {
                e.isEquipped = false;
            }
        }

        item.isEquipped = true;
        ApplyEquipmentBonuses();
    }

    public void UnequipItem(Equipment item)
    {
        if (item.isEquipped)
        {
            item.isEquipped = false;
            ApplyEquipmentBonuses();
        }
    }

    public void ResetAllEquipment()
    {
        Equipment[] allEquipment = Resources.LoadAll<Equipment>("Equipment");
        foreach (Equipment item in allEquipment)
        {
            item.isOwned = false;
            item.isEquipped = false;
        }

        ApplyEquipmentBonuses();

        if (MarketManager.instance != null)
        {
            MarketManager.instance.UpdateCurrentStatsUI();
            MarketManager.instance.PopulateMarketUI();
            MarketManager.instance.PopulateInventoryUI();
        }

        Debug.Log("All equipment reset!");
    }

    public int GetTotalLevelBonus()
    {
        return totalLevelBonus;
    }

    public int GetTotalXPBonus()
    {
        return totalXPBonus;
    }

    public int GetTotalScopeBonus()
    {
        return totalScopeBonus;
    }

    public float GetScopeMultiplier()
    {
        return 1f + (totalScopeBonus / 100f);
    }
    public string GetEquippedName(string category)
    {
        Equipment[] allEquipment = Resources.LoadAll<Equipment>("Equipment");
        foreach (Equipment item in allEquipment)
        {
            if (item.category == category && item.isEquipped)
                return item.itemName;
        }
        return "N/A";
    }
}