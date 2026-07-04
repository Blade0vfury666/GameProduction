using UnityEngine;
using UnityEngine.UI;
using TMP_Text = TMPro.TMP_Text;
using System.Collections;

public class WorkspaceManager : MonoBehaviour
{
    public static WorkspaceManager instance;

    [Header("Main Background")]
    public Image backgroundImage;

    [Header("Main Workspace")]
    public GameObject basement;
    public GameObject smallOffice;
    public GameObject mediumOffice;
    public GameObject largeOffice;
    public GameObject AAAOffice;

    [Header("Global UI Connections")]
    public GameObject errorWarningPanel;
    private Coroutine errorCoroutine;

    [Header("Employee Slot Layouts")]
    [Tooltip("Empty child Transforms marking where employees stand in the basement.")]
    public Transform[] basementSlots;
    [Tooltip("Empty child Transforms marking where employees stand in the Small Office.")]
    public Transform[] smallOfficeSlots;
    [Tooltip("Empty child Transforms marking where employees stand in the Medium Office.")]
    public Transform[] mediumOfficeSlots;
    [Tooltip("Empty child Transforms marking where employees stand in the Large Office.")]
    public Transform[] largeOfficeSlots;
    [Tooltip("Empty child Transforms marking where employees stand in the AAA Studio.")]
    public Transform[] AAAOfficeSlots;

    [Header("1. Small Office")]
    public int smallLevelReq = 10;
    public float smallPrice = 30000f;
    public int smallSlots = 5;
    public GameObject smallBuyButton;
    public TMP_Text smallStatusText;

    [Header("2. Medium Office")]
    public int mediumLevelReq = 25;
    public float mediumPrice = 350000f;
    public int mediumSlots = 10;
    public GameObject mediumBuyButton;
    public TMP_Text mediumStatusText;

    [Header("3. Large Office")]
    public int largeLevelReq = 35;
    public float largePrice = 790000f;
    public int largeSlots = 15;
    public GameObject largeBuyButton;
    public TMP_Text largeStatusText;

    [Header("4. AAA Studio")]
    public int AAALevelReq = 50;
    public float AAAPrice = 1240000f;
    public int AAASlots = 25;
    public GameObject AAABuyButton;
    public TMP_Text AAAStatusText;

    // --- SINGLE SOURCE OF TRUTH FOR OWNERSHIP ---
    // 0 = Basement, 1 = Small, 2 = Medium, 3 = Large, 4 = AAA
    private int currentTier = 0;

    // Public read-only access so other scripts (e.g. PlayerManager) can check
    // whether the player has moved past the Basement yet.
    public int CurrentTier
    {
        get { return currentTier; }
    }

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        errorWarningPanel.SetActive(false);

        currentTier = 0;

        // Workspace visibility - only Basement active at the start
        basement.SetActive(true);
        smallOffice.SetActive(false);
        mediumOffice.SetActive(false);
        largeOffice.SetActive(false);
        AAAOffice.SetActive(false);

        // Make sure the slot manager starts out using the basement's layout.
        if (EmployeeSlotManager.instance != null)
        {
            EmployeeSlotManager.instance.ChangeOffice(basementSlots);
        }

        RefreshOfficeUI();
    }

    void Update()
    {
        RefreshOfficeUI();
    }

    // Centralized UI logic: for every tier above the one currently owned,
    // show the buy button once the level requirement is met, otherwise show
    // the requirement text. Tiers at or below currentTier never show a buy
    // button, so a skipped tier can never be purchased after the fact.
    private void RefreshOfficeUI()
    {
        int playerLevel = PlayerManager.instance.playerLevel;

        UpdateTierUI(1, smallLevelReq, smallBuyButton, smallStatusText, playerLevel);
        UpdateTierUI(2, mediumLevelReq, mediumBuyButton, mediumStatusText, playerLevel);
        UpdateTierUI(3, largeLevelReq, largeBuyButton, largeStatusText, playerLevel);
        UpdateTierUI(4, AAALevelReq, AAABuyButton, AAAStatusText, playerLevel);
    }

    private void UpdateTierUI(int tier, int levelReq, GameObject buyButton, TMP_Text statusText, int playerLevel)
    {
        if (tier < currentTier)
        {
            // A higher tier has already been purchased - this one is permanently locked out.
            buyButton.SetActive(false);
            statusText.gameObject.SetActive(false);
            return;
        }

        if (tier == currentTier)
        {
            // This is the office currently in use.
            buyButton.SetActive(false);
            statusText.text = "Current Workspace";
            statusText.gameObject.SetActive(true);
            return;
        }

        // tier > currentTier: still purchasable if the level requirement is met.
        if (playerLevel >= levelReq)
        {
            buyButton.SetActive(true);
            statusText.gameObject.SetActive(false);
        }
        else
        {
            buyButton.SetActive(false);
            statusText.text = "Requirement: Level " + levelReq;
            statusText.gameObject.SetActive(true);
        }
    }

    // --- BUTTON CLICK FUNCTIONS ---

    public void ClickBuySmallOffice()
    {
        TryBuyOffice(1, smallPrice, smallSlots, smallOffice, smallOfficeSlots);
    }

    public void ClickBuyMediumOffice()
    {
        TryBuyOffice(2, mediumPrice, mediumSlots, mediumOffice, mediumOfficeSlots);
    }

    public void ClickBuyLargeOffice()
    {
        TryBuyOffice(3, largePrice, largeSlots, largeOffice, largeOfficeSlots);
    }

    public void ClickBuyAAAStudio()
    {
        TryBuyOffice(4, AAAPrice, AAASlots, AAAOffice, AAAOfficeSlots);
    }

    // Shared purchase logic. Deactivates every office before activating the
    // one being bought, so it's impossible for two offices to be visible at
    // the same time, no matter what order tiers are purchased in.
    private void TryBuyOffice(int tier, float price, int slots, GameObject officeObject, Transform[] officeSlots)
    {
        // Already own this tier or a higher one - ignore the click.
        if (tier <= currentTier)
        {
            return;
        }

        if (PlayerManager.instance.playerCash < price)
        {
            ShowError();
            return;
        }

        PlayerManager.instance.playerCash -= price;
        PlayerManager.instance.maxEmployeeSlots = slots;

        // Deactivate every workspace, then activate only the one just bought.
        basement.SetActive(false);
        smallOffice.SetActive(false);
        mediumOffice.SetActive(false);
        largeOffice.SetActive(false);
        AAAOffice.SetActive(false);
        officeObject.SetActive(true);

        currentTier = tier;

        // Carry existing employees over into the new office layout.
        if (EmployeeSlotManager.instance != null)
        {
            EmployeeSlotManager.instance.ChangeOffice(officeSlots);
        }

        RefreshOfficeUI();
    }

    // --- ERROR COOLDOWN SYSTEM ---

    private void ShowError()
    {
        errorWarningPanel.SetActive(true);

        if (errorCoroutine != null)
        {
            StopCoroutine(errorCoroutine);
        }

        errorCoroutine = StartCoroutine(HideErrorAfterDelay());
    }

    private IEnumerator HideErrorAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        errorWarningPanel.SetActive(false);
        errorCoroutine = null;
    }
}