using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("Player Stats")]
    public int playerLevel = 1;
    public int totalEmployeeLevel = 0; // Sum of all employee actual levels
    
    [Header("Equipment Bonuses (Flat)")]
    public int equipmentLevelBonus = 0;      // From Computers
    public int equipmentXPBonus = 0;         // From Chairs (percent)
    public int equipmentScopeBonus = 0;      // From Servers
    
    [Header("Effective Stats (Calculated)")]
    public int effectiveEmployeeLevel;
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    public void SetEquipmentBonuses(int levelBonus, int xpBonus, int scopeBonus)
    {
        equipmentLevelBonus = levelBonus;
        equipmentXPBonus = xpBonus;
        equipmentScopeBonus = scopeBonus;
        
        RecalculateEffectiveStats();
    }
    
    void RecalculateEffectiveStats()
    {
        // Effective level = actual level + equipment bonus
        effectiveEmployeeLevel = totalEmployeeLevel + (equipmentLevelBonus * GetEmployeeCount());
    }
    
    int GetEmployeeCount()
    {
        // Return number of hired employees
        return 3; // Placeholder
    }
    
    public int GetMaxScope()
    {
        int baseScope = 50;
        int playerBonus = playerLevel * 2;
        int employeeBonus = GetEmployeeCount() * 5;
        
        return baseScope + equipmentScopeBonus + playerBonus + employeeBonus;
    }
}