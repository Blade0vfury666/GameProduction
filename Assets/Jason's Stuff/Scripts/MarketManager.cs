using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MarketManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject marketPanel;
    public GameObject confirmPopup;
    
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
    
    void Start()
    {
        LoadAllEquipment();
        PopulateMarketUI();
        UpdateCurrentStatsUI();
        
        confirmPopup.SetActive(false);
        marketPanel.SetActive(false);
        
        if (confirmBuyButton != null)
            confirmBuyButton.onClick.AddListener(ConfirmPurchase);
        if (cancelBuyButton != null)
            cancelBuyButton.onClick.AddListener(ClosePopup);
    }
    
    void LoadAllEquipment()
    {
        Equipment[] items = Resources.LoadAll<Equipment>("Equipment");
        allEquipment.Clear();
        allEquipment.AddRange(items);
    }
    
    void PopulateMarketUI()
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

        if (inventoryContainer != null)
            PopulateInventoryUI();
    }
    
    void PopulateInventoryUI()  
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
            
            UpdateCurrentStatsUI();
            PopulateMarketUI();
            
            ClosePopup();
        }
        else
        {
            Debug.Log("Cannot afford " + selectedItem.itemName);
        }
    }
    
    public void EquipItem(Equipment item)
    {
        UnequipCategory(item.category);
        item.isEquipped = true;
        ApplyEquipmentBonuses();
        UpdateCurrentStatsUI();
        PopulateMarketUI();
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
            item.isEquipped = false;
            ApplyEquipmentBonuses();
            UpdateCurrentStatsUI();
            PopulateInventoryUI();  
            PopulateMarketUI();    
        }
    }
    
    void ApplyEquipmentBonuses()
    {
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
        }
    }
    
    void UpdateCurrentStatsUI()
    {
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
        foreach (Equipment item in allEquipment)
        {
            item.isOwned = false;
            item.isEquipped = false;
        }
        
        UpdateCurrentStatsUI();
        PopulateMarketUI();
        
        Debug.Log("All purchases have been reset!");
    }
}