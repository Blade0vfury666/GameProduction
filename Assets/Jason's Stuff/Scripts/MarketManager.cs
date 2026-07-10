using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

// Manages the in-game marketplace, including item display, purchasing, equipping,

public class MarketManager : MonoBehaviour
{
    public static MarketManager instance;

    [Header("UI Panels")]
    public GameObject marketPanel;
    public GameObject confirmPopup;

    [Header("Error Warning")]
    public GameObject errorWarningPanel;
    public TextMeshProUGUI errorWarningText;
    private Coroutine errorCoroutine;
    
    [Header("Item Containers")]
    public Transform computersContainer;
    public Transform chairsContainer;
    public Transform serversContainer;
    public Transform inventoryContainer;  
    
    [Header("Item Prefab")]
    public GameObject equipmentButtonPrefab;
    
    [Header("Current Stats Display")]
    public TextMeshProUGUI currentLevelBonusText;
    public TextMeshProUGUI currentXPBonusText;
    public TextMeshProUGUI currentScopeBonusText;
    
    [Header("Player Resources")]
    public TextMeshProUGUI cashText;
    public TextMeshProUGUI goldText;  
    
    [Header("Confirmation Popup")]
    public TextMeshProUGUI confirmItemName;
    public TextMeshProUGUI confirmItemCost;
    public Button confirmBuyButton;
    public Button cancelBuyButton;
    
    private List<Equipment> allEquipment = new List<Equipment>();
    private Equipment selectedItem;
    
    void Awake()
    {
        instance = this;
    }
    
    void Start()  // Load Equipment UI
    {
        LoadAllEquipment();
        PopulateMarketUI();
        UpdateCurrentStatsUI();
        UpdatePlayerResourcesUI();
        
        confirmPopup.SetActive(false);
        marketPanel.SetActive(false);

        if (errorWarningPanel != null)
            errorWarningPanel.SetActive(false);
        
        if (confirmBuyButton != null)
            confirmBuyButton.onClick.AddListener(ConfirmPurchase);
        if (cancelBuyButton != null)
            cancelBuyButton.onClick.AddListener(ClosePopup);
    }
    
    void OnEnable()
    {
        UpdatePlayerResourcesUI();
    }
    
    void LoadAllEquipment()
    {
        Equipment[] items = Resources.LoadAll<Equipment>("Equipment");
        allEquipment.Clear();
        allEquipment.AddRange(items);
    }
    
    public void PopulateMarketUI()  /// Loads all Equipment ScriptableObjects from the Resources/Equipment folder.
    {
        ClearContainer(computersContainer);
        ClearContainer(chairsContainer);
        ClearContainer(serversContainer);
        
        foreach (Equipment item in allEquipment)
        {
            GameObject buttonObj = Instantiate(equipmentButtonPrefab, GetContainerByCategory(item.category));
            EquipmentButton button = buttonObj.GetComponent<EquipmentButton>();
            button.Setup(item, this);
        }

        if (inventoryContainer != null) // Populate the inventory section
            PopulateInventoryUI();
    }
    
    public void PopulateInventoryUI()  
    {
        ClearContainer(inventoryContainer);
        
        foreach (Equipment item in allEquipment)
        {
            if (item.isOwned)
            {
                GameObject buttonObj = Instantiate(equipmentButtonPrefab, inventoryContainer);
                buttonObj.SetActive(true);
                EquipmentButton button = buttonObj.GetComponent<EquipmentButton>();
                button.SetupInventory(item, this);
            }
        }
    }
    
    Transform GetContainerByCategory(string category)
    {
        switch (category)
        {
            case "Computer": return computersContainer;
            case "Chair": return chairsContainer;
            case "Server": return serversContainer;
            default: return computersContainer;
        }
    }
    
    void ClearContainer(Transform container)
    {
        if (container == null) return;
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }
    
    public void OnEquipmentClicked(Equipment item)
    {
        if (item.isOwned)
        {
            Debug.Log(item.itemName + " is already owned.");
            return;
        }

        if (PlayerManager.instance != null && PlayerManager.instance.playerLevel < item.requiredPlayerLevel)
        {
            ShowError("Requires Level " + item.requiredPlayerLevel);
            return;
        }

        bool canAfford = true;
        if (PlayerManager.instance != null)
        {
            if (item.cashCost > 0 && PlayerManager.instance.playerCash < (float)item.cashCost)
                canAfford = false;
            if (item.goldCost > 0 && PlayerManager.instance.playerGold < (float)item.goldCost)
                canAfford = false;
        }

        if (!canAfford)
        {
            ShowError("Not enough " + (item.cashCost > 0 ? "Cash" : "Gold") + "!");
            return;
        }
        
        selectedItem = item;
        ShowConfirmPopup();
    }
    
    public void OnEquipClicked(Equipment item)
    {
        if (!item.isOwned)
        {
            Debug.Log(item.itemName + " is not owned yet. Buy it first.");
            return;
        }
        
        EquipItem(item);
        UpdateCurrentStatsUI();
        PopulateMarketUI();
        PopulateInventoryUI();
    }
    
    void ShowConfirmPopup()
    {
        confirmPopup.SetActive(true);
        confirmItemName.text = selectedItem.itemName;
        
        string costText = "";
        if (selectedItem.cashCost > 0)
            costText = selectedItem.cashCost + " Cash";
        if (selectedItem.goldCost > 0)  
            costText = selectedItem.goldCost + " Gold";  
            
        confirmItemCost.text = costText;
    }
    
    public void ConfirmPurchase()
    {
        if (selectedItem == null) return;
        if (PlayerManager.instance == null)
        {
            Debug.LogError("PlayerManager.instance is NULL!");
            return;
        }
        
        if (PlayerManager.instance.playerLevel < selectedItem.requiredPlayerLevel)
        {
            ShowError("Requires Level " + selectedItem.requiredPlayerLevel);
            ClosePopup();
            return;
        }
        
        bool canAfford = true;
        
        if (selectedItem.cashCost > 0 && PlayerManager.instance.playerCash < (float)selectedItem.cashCost)
            canAfford = false;
        if (selectedItem.goldCost > 0 && PlayerManager.instance.playerGold < (float)selectedItem.goldCost)
            canAfford = false;
        
        if (canAfford)
        {
            if (selectedItem.cashCost > 0)
                PlayerManager.instance.playerCash -= (float)selectedItem.cashCost;
            if (selectedItem.goldCost > 0)
                PlayerManager.instance.playerGold -= (float)selectedItem.goldCost;
            
            selectedItem.isOwned = true;
            EquipItem(selectedItem);
            
            UpdatePlayerResourcesUI();
            UpdateCurrentStatsUI();
            PopulateMarketUI();
            
            ClosePopup();
        }
        else
        {
            ShowError("Not enough " + (selectedItem.cashCost > 0 ? "Cash" : "Gold") + "!");
        }
    }

  
    // ERROR WARNING SYSTEM
    
    private void ShowError(string message)
    {
        if (errorWarningPanel == null) return;

        errorWarningPanel.SetActive(true);
        if (errorWarningText != null)
            errorWarningText.text = message;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX("PurchaseError");

        if (errorCoroutine != null)
            StopCoroutine(errorCoroutine);

        errorCoroutine = StartCoroutine(HideErrorAfterDelay());
    }

    private IEnumerator HideErrorAfterDelay()
    {
        yield return new WaitForSeconds(2.5f);
        errorWarningPanel.SetActive(false);
        errorCoroutine = null;
    }
    
    public void EquipItem(Equipment item)
    {
        if (EquipmentManager.instance != null)
        {
            EquipmentManager.instance.EquipItem(item);
        }
        else
        {
            UnequipCategory(item.category);
            item.isEquipped = true;
            ApplyEquipmentBonuses();
        }
        
        UpdatePlayerResourcesUI();
        UpdateCurrentStatsUI();
        PopulateMarketUI();
        PopulateInventoryUI();
    }
    
    void UnequipCategory(string category)
    {
        foreach (Equipment item in allEquipment)
        {
            if (item.category == category && item.isEquipped)
            {
                item.isEquipped = false;
            }
        }
    }
    
    public void OnUnequipClicked(Equipment item)
    {
        if (item.isEquipped)
        {
            if (EquipmentManager.instance != null)
            {
                EquipmentManager.instance.UnequipItem(item);
            }
            else
            {
                item.isEquipped = false;
                ApplyEquipmentBonuses();
            }
            
            UpdatePlayerResourcesUI();
            UpdateCurrentStatsUI();
            PopulateInventoryUI();  
            PopulateMarketUI();    
        }
    }
    
    void ApplyEquipmentBonuses()
    {
        if (EquipmentManager.instance != null)
        {
            EquipmentManager.instance.ApplyEquipmentBonuses();
            return;
        }
        
        int totalLevelBonus = 0;
        int totalXPBonus = 0;
        int totalScopeBonus = 0;
        
        foreach (Equipment item in allEquipment)
        {
            if (item.isEquipped)
            {
                totalLevelBonus += item.flatLevelBonus;
                totalXPBonus += item.flatXPBonus;
                totalScopeBonus += item.flatScopeBonus;
            }
        }
        
        if (PlayerManager.instance != null)
        {
            PlayerManager.instance.pcSpec = totalLevelBonus;
            PlayerManager.instance.chairLevel = totalXPBonus;
            PlayerManager.instance.serverLevel = totalScopeBonus;
        }
    }
    
    
    // CURRENCY FORMATTING - Only above 1 Million
   
    public string FormatCurrency(float amount)
    {
        // Only format if amount is 1,000,000 or higher
        if (amount >= 1000000f)
        {
            if (amount >= 1000000000000f)
                return (amount / 1000000000000f).ToString("0.##") + "T";
            if (amount >= 1000000000f)
                return (amount / 1000000000f).ToString("0.##") + "B";
            if (amount >= 1000000f)
                return (amount / 1000000f).ToString("0.##") + "M";
        }
        
        // Below 1 million: show full number (no decimal)
        return Mathf.FloorToInt(amount).ToString();
    }
    
    public void UpdatePlayerResourcesUI()
    {
        if (PlayerManager.instance == null) return;
        
        if (cashText != null)
        {
            cashText.text = FormatCurrency(PlayerManager.instance.playerCash);
        }
        
        if (goldText != null)
        {
            goldText.text = FormatCurrency(PlayerManager.instance.playerGold);
        }
    }
    
    public void UpdateCurrentStatsUI()
    {
        int totalLevelBonus = 0;
        int totalXPBonus = 0;
        int totalScopeBonus = 0;
        
        if (EquipmentManager.instance != null)
        {
            totalLevelBonus = EquipmentManager.instance.GetTotalLevelBonus();
            totalXPBonus = EquipmentManager.instance.GetTotalXPBonus();
            totalScopeBonus = EquipmentManager.instance.GetTotalScopeBonus();
        }
        else
        {
            foreach (Equipment item in allEquipment)
            {
                if (item.isEquipped)
                {
                    totalLevelBonus += item.flatLevelBonus;
                    totalXPBonus += item.flatXPBonus;
                    totalScopeBonus += item.flatScopeBonus;
                }
            }
        }
        
        if (currentLevelBonusText != null)
            currentLevelBonusText.text = "+" + totalLevelBonus + " Employee Level";
        if (currentXPBonusText != null)
            currentXPBonusText.text = "+" + totalXPBonus + "% XP Gain";
        if (currentScopeBonusText != null)
            currentScopeBonusText.text = "+" + totalScopeBonus + " Max Scope";
    }
    
    public void ClosePopup()
    {
        confirmPopup.SetActive(false);
        selectedItem = null;
    }
    
    public void OpenMarket()
    {
        marketPanel.SetActive(true);
        UpdatePlayerResourcesUI();
        UpdateCurrentStatsUI();
        PopulateMarketUI();
        PopulateInventoryUI();
    }
    
    public void CloseMarket()
    {
        marketPanel.SetActive(false);
    }

    public void ResetAllPurchases()
    {
        if (EquipmentManager.instance != null)
        {
            EquipmentManager.instance.ResetAllEquipment();
        }
        else
        {
            foreach (Equipment item in allEquipment)
            {
                item.isOwned = false;
                item.isEquipped = false;
            }
        }
        
        UpdatePlayerResourcesUI();
        UpdateCurrentStatsUI();
        PopulateMarketUI();
        PopulateInventoryUI();
        
        Debug.Log("All purchases have been reset!");
    }
}