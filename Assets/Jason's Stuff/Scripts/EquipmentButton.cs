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
        // Name now shows on the card
        if (nameText != null)
        {
            nameText.gameObject.SetActive(true);
            nameText.text = item.itemName;
        }
        // Everything else stays hidden on the card
        if (brandText != null) brandText.gameObject.SetActive(false);
        if (tierText != null) tierText.gameObject.SetActive(false);
        if (costText != null) costText.gameObject.SetActive(false);
        if (bonusText != null) bonusText.gameObject.SetActive(false);
        
        equippedOverlay.SetActive(item.isEquipped);
        ownedOverlay.SetActive(item.isOwned && !item.isEquipped);
        
        if (item.isOwned)
        {
            buyButton.gameObject.SetActive(false);
            equipButton.gameObject.SetActive(!item.isEquipped);
        }
        else
        {
            buyButton.gameObject.SetActive(true);
            equipButton.gameObject.SetActive(false);
            
            buyButton.interactable = true;
        }
        
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => marketManager.OnEquipmentClicked(item));
        
        equipButton.onClick.RemoveAllListeners();
        equipButton.onClick.AddListener(() => marketManager.OnEquipClicked(item));
    }
    
    public void SetupInventory(Equipment newItem, MarketManager manager)
    {
        item = newItem;
        marketManager = manager;
        
        if (iconImage != null && item.itemIcon != null)
            iconImage.sprite = item.itemIcon;
        
        if (nameText != null)
        {
            nameText.gameObject.SetActive(true);
            nameText.text = item.itemName;
        }
        
        if (brandText != null)
            brandText.gameObject.SetActive(false);
        
        if (tierText != null)
            tierText.gameObject.SetActive(false);
        
        if (bonusText != null)
            bonusText.gameObject.SetActive(false);
        
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
    }
}