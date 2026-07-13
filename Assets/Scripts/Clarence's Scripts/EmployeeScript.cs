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
    public Button hireButton; 

    [Header("Rarity Backgrounds")]
    public GameObject commonImageObject;
    public GameObject rareImageObject;
    public GameObject legendaryImageObject;

    [Header("Live Stats")]
    public int employeeLevel; 
    private int hireCost;
    private bool isLegendarySpawn; 
    
    private string employeeName;
    private Sprite employeeFace;
    private string employeeSkill;

    public void SetupEmployee(string eName, Sprite eFace, string eSkill, int eLevel, bool isFreeRoll)
    {
        employeeName = eName;
        employeeFace = eFace;
        employeeSkill = eSkill;
        isLegendarySpawn = isFreeRoll; 

        nameText.text = employeeName;
        faceImage.sprite = employeeFace;
        skillText.text = employeeSkill;
        
        employeeLevel = eLevel;
        if (employeeLevel > 60)
        {
            employeeLevel = 60;
        }

        levelText.text = employeeLevel.ToString();
        levelText.color = Color.black;

        commonImageObject.SetActive(false);
        rareImageObject.SetActive(false);
        legendaryImageObject.SetActive(false);

        if (employeeLevel <= 20)
        {
            commonImageObject.SetActive(true);
        }
        else if (employeeLevel <= 40)
        {
            rareImageObject.SetActive(true);
        }
        else if (employeeLevel <= 60)
        {
            legendaryImageObject.SetActive(true);
        }

        if (isLegendarySpawn == true)
        {
            hireCost = 0;
            costText.text = ""; 
        }
        else
        {
            hireCost = employeeLevel * 12500; 
            costText.text = "$" + hireCost.ToString("N0"); 
        }
    }

    public void ClickHire()
    {
        if (PlayerManager.instance.playerCash >= hireCost)
        {
            if (PlayerManager.instance.hiredEmployees.Count < PlayerManager.instance.maxEmployeeSlots)
            {
                //  SUCCESSFUL HIRE
                PlayerManager.instance.playerCash -= hireCost;
                PlayerManager.instance.HireNewEmployee(employeeName, employeeFace, employeeSkill, employeeLevel);

                // Play success SFX
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySFX("ThankYou");

                if (isLegendarySpawn == true)
                {
                    EmployeeManager.instance.CloseAndResetLegendaryUI();
                }

                Destroy(this.gameObject);
            }
            else
            {
                //  NO SLOTS AVAILABLE
                EmployeeManager.instance.ShowWarning("Upgrade Workspace!");
            }
        }
        else
        {
            //  NOT ENOUGH MONEY
            EmployeeManager.instance.ShowWarning("Insufficient Funds!");
        }
    }
}