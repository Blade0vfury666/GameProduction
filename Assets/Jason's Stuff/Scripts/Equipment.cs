using UnityEngine;

[CreateAssetMenu(fileName = "NewEquipment", menuName = "Studio Rise/Equipment")]
public class Equipment : ScriptableObject
{
    public string itemName;
    public string category; // "Computer", "Chair", "Server"
    public string brand;
    public string tier;
    
    // Computers
    public int flatLevelBonus;    // +1 to +5 Employee Level
    
    // Chairs
    public int flatXPBonus;       // +2% to +10% XP Gain  
    
    // Servers
    public int flatScopeBonus;    // +10 to +50 Max Scope  
    
    public int cashCost;
    public int goldCost;
    
    public Sprite itemIcon;
    public string description;
    
    public bool isOwned;
    public bool isEquipped;

    public int requiredPlayerLevel;
}