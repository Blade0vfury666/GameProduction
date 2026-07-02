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
    public TMP_Text errorWarningText; 

    [Header("Mini Game Connection")]
    public GameObject miniGamePanel;
    public MiniGame1 miniGameScript;

    private float liveBudget;

    void OnEnable()
    {
        errorWarningText.gameObject.SetActive(false);

        // Wipe inputs clean every time you open the window
        nameInput.text = "";
        genreInput.text = "";
        
        nameInput.characterLimit = 30;
        genreInput.characterLimit = 30;

        // 1. DYNAMIC SCOPE LIMITS (Phase A Formula)
        int minLimit = 1 + PlayerManager.instance.playerLevel;
        
        int totalEffectiveLevel = PlayerManager.instance.GetTotalEffectiveEmployeeLevel();
        int maxLimit = Mathf.FloorToInt(10f + (PlayerManager.instance.playerLevel * 2f) + (totalEffectiveLevel * 0.5f) + (PlayerManager.instance.serverLevel * 10f));

        if (maxLimit <= minLimit)
        {
            maxLimit = minLimit + 1; // Foolproof safety check
        }

        scopeSlider.minValue = minLimit;
        scopeSlider.maxValue = maxLimit;
        
        // Default to the middle point
        int middlePoint = (minLimit + maxLimit) / 2;
        scopeSlider.value = middlePoint;

        minScopeText.text = minLimit.ToString();
        maxScopeText.text = maxLimit.ToString();

        UpdateSliderPreview(); 
    }

    public void UpdateSliderPreview()
    {
        int currentScope = Mathf.FloorToInt(scopeSlider.value);

        // Budget Formula (Phase A)
        liveBudget = currentScope * 100f;
        
        budgetPreviewText.text = "Budget: $" + Mathf.FloorToInt(liveBudget);
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

        // Instantly deduct the player's cash
        PlayerManager.instance.playerCash = PlayerManager.instance.playerCash - liveBudget;

        // Setup the Minigame, open it, and close the setup window
        miniGamePanel.SetActive(true);
        miniGameScript.InitializeMiniGame(nameInput.text, genreInput.text, Mathf.FloorToInt(scopeSlider.value));

        this.gameObject.SetActive(false); 
    }
}