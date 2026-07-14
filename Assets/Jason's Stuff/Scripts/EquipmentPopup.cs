using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipmentDetailPopup : MonoBehaviour
{
    [Header("Text Fields")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI requirementText;
    public TextMeshProUGUI statBoostText;
    public TextMeshProUGUI costText;

    [Header("Buttons")]
    public Button yesButton;
    public Button noButton;

    private Equipment currentItem;
    private MarketManager marketManager;

    public void Show(Equipment item, MarketManager manager)
    {
        currentItem = item;
        marketManager = manager;

        gameObject.SetActive(true);

        if (nameText != null)
            nameText.text = item.itemName;

        if (descriptionText != null)
            descriptionText.text = item.description;

        if (requirementText != null)
        {
            if (item.requiredPlayerLevel > 0)
            {
                requirementText.gameObject.SetActive(true);
                requirementText.text = "Requirement: Level " + item.requiredPlayerLevel;

                // Check if player meets the requirement
                if (PlayerManager.instance != null)
                {
                    bool meetsRequirement = PlayerManager.instance.playerLevel >= item.requiredPlayerLevel;
                    requirementText.color = meetsRequirement ? Color.green : Color.red;
                }
                else
                {
                    requirementText.color = Color.red;
                }
            }
            else
            {
                requirementText.gameObject.SetActive(false);
            }
        }

        if (statBoostText != null)
        {
            if (item.category == "Computer")
                statBoostText.text = "+" + item.flatLevelBonus + " Employee Level";
            else if (item.category == "Chair")
                statBoostText.text = "+" + item.flatXPBonus + "% XP Gain";
            else if (item.category == "Server")
                statBoostText.text = "+" + item.flatScopeBonus + " Max Scope";
            else
                statBoostText.text = "";
        }

        if (costText != null)
        {
            if (item.cashCost > 0)
                costText.text = item.cashCost + " Cash";
            else if (item.goldCost > 0)
                costText.text = item.goldCost + " Gold";
        }

        if (yesButton != null)
        {
            yesButton.onClick.RemoveAllListeners();
            yesButton.onClick.AddListener(() => marketManager.ConfirmPurchase());
        }

        if (noButton != null)
        {
            noButton.onClick.RemoveAllListeners();
            noButton.onClick.AddListener(() => marketManager.ClosePopup());
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        currentItem = null;
    }
}