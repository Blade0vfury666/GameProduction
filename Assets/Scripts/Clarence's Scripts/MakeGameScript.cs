using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MakeGameScript : MonoBehaviour
{
    [Header("Input Fields")]
    public TMP_InputField nameInput;
    public TMP_InputField genreInput;
    public Slider scopeSlider;

    [Header("Slider UI Texts")]
    public TMP_Text minScopeText;
    public TMP_Text maxScopeText;
    public TMP_Text budgetPreviewText;
    public TMP_Text timePreviewText;
    
    public TMP_Text errorWarningText; 

    [Header("Spawning")]
    public GameEntityScript gamePrefab; 
    public Transform listParent;  

    private float liveBudget;
    private float liveTime;
    private int liveQuality;

    void OnEnable()
    {
        errorWarningText.gameObject.SetActive(false);

        // WIPE IT CLEAN: This resets the inputs every time you open the window
        nameInput.text = "";
        genreInput.text = "";
        
        nameInput.characterLimit = 30;
        genreInput.characterLimit = 30;

        // 1. DYNAMIC SCOPE LIMITS
        // We group the stats together to easily increase both min and max scope
        int statBonus = PlayerManager.instance.playerLevel + PlayerManager.instance.employeeCount + PlayerManager.instance.pcSpec;
        
        int minLimit = 1 + statBonus;
        int maxLimit = 10 + (statBonus * 2) + PlayerManager.instance.totalEmployeeLevel;

        // Apply limits to the physical slider
        scopeSlider.minValue = minLimit;
        scopeSlider.maxValue = maxLimit;
        
        // FIND THE MIDDLE
        int middlePoint = (minLimit + maxLimit) / 2;
        scopeSlider.value = middlePoint;

        // Update the visual text at the ends of the slider
        minScopeText.text = "" + minLimit;
        maxScopeText.text = "" + maxLimit;

        // Force the preview to calculate immediately
        UpdateSliderPreview(); 
    }

    public void UpdateSliderPreview()
    {
        int currentScope = Mathf.FloorToInt(scopeSlider.value);

        // 1. BUDGET FORMULA
        // Employees no longer cost extra money here. Only the Scope makes the game more expensive!
        liveBudget = currentScope * 100f;
        
        // 2. TIME FORMULA 
        // Employee count and totalEmployeeLevel (which is the combined level of all employees) heavily reduce time!
        liveTime = (currentScope * 10f) - 
                   (PlayerManager.instance.pcSpec * 2f) - 
                   (PlayerManager.instance.playerLevel * 1f) - 
                   (PlayerManager.instance.employeeCount * 4f) - 
                   (PlayerManager.instance.totalEmployeeLevel * 2f);
                   
        // Foolproof limit: Games can never take less than 3 seconds to make
        if (liveTime < 3f)
        {
            liveTime = 3f; 
        }

        // 3. QUALITY FORMULA 
        // Employee count and levels directly contribute to a much better game
        liveQuality = (currentScope * 5) + 
                      (PlayerManager.instance.pcSpec * 4) + 
                      (PlayerManager.instance.playerLevel * 3) + 
                      (PlayerManager.instance.employeeCount * 2) + 
                      (PlayerManager.instance.totalEmployeeLevel * 3);

        // Update the UI texts instantly
        budgetPreviewText.text = "Budget: $" + Mathf.FloorToInt(liveBudget);
        timePreviewText.text = "Time: " + Mathf.FloorToInt(liveTime) + "s";
    }

    public void ClickDevelopButton()
    {
        if (nameInput.text == "")
        {
            errorWarningText.text = "Please enter a Game Name!";
            errorWarningText.gameObject.SetActive(true);
            return; 
        }

        if (genreInput.text == "")
        {
            errorWarningText.text = "Please enter a Genre!";
            errorWarningText.gameObject.SetActive(true);
            return; 
        }

        if (PlayerManager.instance.playerCash < liveBudget)
        {
            errorWarningText.text = "Not enough cash!";
            errorWarningText.gameObject.SetActive(true);
            return;
        }

        errorWarningText.gameObject.SetActive(false);

        // Take the player's cash
        PlayerManager.instance.playerCash = PlayerManager.instance.playerCash - liveBudget;

        // Spawn the row directly into the UI list
        GameEntityScript spawnedGame = Instantiate(gamePrefab, listParent);

        // Handshake: Give the clone its specific stats
        spawnedGame.SetupGame(nameInput.text, genreInput.text, liveQuality, liveTime);

        // Turn off the UI Panel
        this.gameObject.SetActive(false); 
    }
}