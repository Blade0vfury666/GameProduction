using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HiredEmployeeScript : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text nameText;
    public TMP_Text skillText;
    public TMP_Text levelText;
    public Image faceImage;

    [Header("Rarity Backgrounds")]
    public GameObject commonImageObject;
    public GameObject rareImageObject;
    public GameObject legendaryImageObject;

    [Header("Live Stats")]
    public string employeeName;
    public Sprite employeeFace;
    public string employeeSkill;

    public int baseLevel;
    public int itemBonusLevel; 

    public void SetupHiredEmployee(string eName, Sprite eFace, string eSkill, int eLevel)
    {
        employeeName = eName;
        employeeFace = eFace;
        employeeSkill = eSkill;
        baseLevel = eLevel;
        itemBonusLevel = 0; 

        // --- PERMANENT RARITY VISUALS ---
        commonImageObject.SetActive(false);
        rareImageObject.SetActive(false);
        legendaryImageObject.SetActive(false);

        if (baseLevel <= 20)
        {
            commonImageObject.SetActive(true);
        }
        else if (baseLevel <= 40)
        {
            rareImageObject.SetActive(true);
        }
        else if (baseLevel <= 60)
        {
            legendaryImageObject.SetActive(true);
        }

        UpdateUI();
    }

    public void UpdateUI()
    {
        nameText.text = employeeName;
        faceImage.sprite = employeeFace;
        skillText.text = employeeSkill;

        // RAW NUMBER AND BLACK TEXT (Final level incorporates base + items)
        int finalLevel = GetFinalLevel();
        levelText.text = finalLevel.ToString();
        levelText.color = Color.black;
    }

    public int GetFinalLevel()
    {
        int finalLevel = baseLevel + itemBonusLevel;
        
        if (finalLevel > 60)
        {
            finalLevel = 60; // Keep the cap!
        }
        
        return finalLevel;
    }

    public void ClickFire()
    {
        PlayerManager.instance.hiredEmployees.Remove(this);
        Destroy(this.gameObject);
    }
}