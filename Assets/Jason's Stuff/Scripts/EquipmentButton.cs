using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipmentButton : MonoBehaviour
{
    [Header("UI Elements")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI brandText;
    public TextMeshProUGUI tierText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI bonusText;
    public GameObject ownedOverlay;
    public GameObject equippedOverlay;
    public Button buyButton;
    public Button equipButton;
    
    private Equipment item;
    private MarketManager marketManager;
    
    public void Setup(Equipment newItem, MarketManager manager)
    {
        item = newItem;
        marketManager = manager;
        
        iconImage.sprite = item.itemIcon;
        nameText.text = item.itemName;
        brandText.text = item.brand;
        tierText.text = item.tier;
        
        if (item.cashCost > 0)
            costText.text = item.cashCost + " Cash";
        else if (item.goldCost > 0)  
            costText.text = item.goldCost + " Gold";  
        
        if (item.category == "Computer")
            bonusText.text = "+" + item.flatLevelBonus + " Emp. Level";
        else if (item.category == "Chair")
            bonusText.text = "+" + item.flatXPBonus + "% XP Gain";
        else if (item.category == "Server")
            bonusText.text = "+" + item.flatScopeBonus + " Max Scope";
        
        equippedOverlay.SetActive(item.isEquipped);
        ownedOverlay.SetActive(item.isOwned && !item.isEquipped);
        
        bool meetsLevelRequirement = PlayerManager.instance != null && 
                                      PlayerManager.instance.playerLevel >= item.requiredPlayerLevel;
        
        if (item.isOwned)
        {
            buyButton.gameObject.SetActive(false);
            equipButton.gameObject.SetActive(!item.isEquipped);
        }
        else
        {
            buyButton.gameObject.SetActive(true);
            equipButton.gameObject.SetActive(false);
            
            bool canAfford = true;
            if (PlayerManager.instance != null)
            {
                if (item.cashCost > 0 && PlayerManager.instance.playerCash < (float)item.cashCost)
                    canAfford = false;
                if (item.goldCost > 0 && PlayerManager.instance.playerGold < (float)item.goldCost)
                    canAfford = false;
            }
            
            
            buyButton.interactable = true;
            
            if (!meetsLevelRequirement)
            {
                costText.text = "Requires Level " + item.requiredPlayerLevel;
                costText.color = Color.red;
            }
            else
            {
                costText.color = Color.white;
            }
        }
        
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => marketManager.OnEquipmentClicked(item));
        
        equipButton.onClick.RemoveAllListeners();
        equipButton.onClick.AddListener(() => marketManager.OnEquipClicked(item));
        
      
        Image lockOverlay = transform.Find("LockOverlay")?.GetComponent<Image>();
        if (lockOverlay != null)
        {
            lockOverlay.gameObject.SetActive(!meetsLevelRequirement && !item.isOwned);
        }
    }
    
    public void SetupInventory(Equipment newItem, MarketManager manager)
    {
        item = newItem;
        marketManager = manager;
        
        if (iconImage != null && item.itemIcon != null)
            iconImage.sprite = item.itemIcon;
        
        if (nameText != null)
            nameText.text = item.itemName;
        
        if (brandText != null)
            brandText.text = item.brand;
        
        if (tierText != null)
            tierText.text = item.tier;
        
        if (bonusText != null)
        {
            if (item.category == "Computer")
                bonusText.text = "+" + item.flatLevelBonus + " Employee Level";
            else if (item.category == "Chair")
                bonusText.text = "+" + item.flatXPBonus + "% XP Gain";
            else if (item.category == "Server")
                bonusText.text = "+" + item.flatScopeBonus + " Max Scope";
        }
        
        if (costText != null)
            costText.gameObject.SetActive(false);
        
        if (equippedOverlay != null)
            equippedOverlay.SetActive(item.isEquipped);
        
        if (ownedOverlay != null)
            ownedOverlay.SetActive(false);
        
        if (buyButton != null)
            buyButton.gameObject.SetActive(false);
        
        if (equipButton != null)
        {
            equipButton.gameObject.SetActive(true);
            gameObject.SetActive(true);
            
            TextMeshProUGUI buttonText = equipButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                if (item.isEquipped)
                {
                    buttonText.text = "EQUIPPED";
                    equipButton.interactable = false;
                }
                else
                {
                    buttonText.text = "EQUIP";
                    equipButton.interactable = true;
                }
            }
            
            equipButton.onClick.RemoveAllListeners();
            equipButton.onClick.AddListener(() => marketManager.OnEquipClicked(item));
        }
        
        
        Image lockOverlay = transform.Find("LockOverlay")?.GetComponent<Image>();
        if (lockOverlay != null)
        {
            lockOverlay.gameObject.SetActive(false);
        }
    }
}