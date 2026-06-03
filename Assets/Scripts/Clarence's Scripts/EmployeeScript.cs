using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EmployeeScript : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text nameText;
    public TMP_Text skillText;
    public TMP_Text levelText;
    public TMP_Text costText;
    public Image faceImage;
    public Button actionButton; // This is the "Hire" or "Claim" button

    private int myLevel;
    private int hireCost;

    // The Manager calls this exactly once when the employee is spawned
    public void SetupEmployee(string eName, Sprite eFace, string eSkill, int eLevel)
    {
        nameText.text = eName;
        faceImage.sprite = eFace;
        skillText.text = eSkill;
        myLevel = eLevel;
        levelText.text = "Lv: " + myLevel;

        // Calculate a one-time hire cost based on level
        hireCost = myLevel * 100;
        
        // If this employee was spawned in the SSR popup, it's free to claim!
        if (this.transform.parent.name == "SSR_Spawn_Point")
        {
            hireCost = 0;
            costText.text = "FREE";
        }
        else
        {
            costText.text = "$" + hireCost;
        }
    }

    // Connect this to the Button on the prefab
    public void ClickAction()
    {
        // Check if player has enough money (Bypass this if it's the free SSR claim)
        if (PlayerManager.instance.playerCash >= hireCost)
        {
            PlayerManager.instance.playerCash = PlayerManager.instance.playerCash - hireCost;

            // 1. Tell Tycoon Stats to update
            PlayerManager.instance.employeeCount = PlayerManager.instance.employeeCount + 1;
            PlayerManager.instance.totalEmployeeLevel = PlayerManager.instance.totalEmployeeLevel + myLevel;

            // 2. Move this UI physically to the "Your Employees" window!
            Transform hiredList = GameObject.Find("Your_Employees_Container").transform;
            this.transform.SetParent(hiredList);

            // 3. Disable the button so they can't be hired twice
            actionButton.gameObject.SetActive(false);
            costText.gameObject.SetActive(false);

            // 4. If this was the SSR popup, close the popup window automatically
            GameObject ssrPopup = GameObject.Find("SSR_Popup_Canvas");
            if (ssrPopup != null)
            {
                ssrPopup.SetActive(false);
            }
        }
    }
}