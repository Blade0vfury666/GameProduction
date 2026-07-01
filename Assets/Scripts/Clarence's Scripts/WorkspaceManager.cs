using UnityEngine;
using UnityEngine.UI;
using TMP_Text = TMPro.TMP_Text;
using System.Collections;

public class WorkspaceManager : MonoBehaviour
{
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
    private bool smallPurchased;

    [Header("2. Medium Office")]
    public int mediumLevelReq = 25;
    public float mediumPrice = 350000f;
    public int mediumSlots = 10;
    public GameObject mediumBuyButton;
    public TMP_Text mediumStatusText;
    private bool mediumPurchased;

    [Header("3. Large Office")]
    public int largeLevelReq = 35;
    public float largePrice = 790000f;
    public int largeSlots = 15;
    public GameObject largeBuyButton;
    public TMP_Text largeStatusText;
    private bool largePurchased;

    [Header("4. AAA Studio")]
    public int AAALevelReq = 50;
    public float AAAPrice = 1240000f;
    public int AAASlots = 25;
    public GameObject AAABuyButton;
    public TMP_Text AAAStatusText;
    private bool AAAPurchased;

    void Start()
    {
        errorWarningPanel.SetActive(false);

        // Workspace visibility
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

        smallPurchased = false;
        smallBuyButton.SetActive(false);
        smallStatusText.text = "Requirement: Level " + smallLevelReq;
        smallStatusText.gameObject.SetActive(true);

        mediumPurchased = false;
        mediumBuyButton.SetActive(false);
        mediumStatusText.text = "Requirement: Level " + mediumLevelReq;
        mediumStatusText.gameObject.SetActive(true);

        largePurchased = false;
        largeBuyButton.SetActive(false);
        largeStatusText.text = "Requirement: Level " + largeLevelReq;
        largeStatusText.gameObject.SetActive(true);

        AAAPurchased = false;
        AAABuyButton.SetActive(false);
        AAAStatusText.text = "Requirement: Level " + AAALevelReq;
        AAAStatusText.gameObject.SetActive(true);
    }

    void Update()
    {
        if (smallPurchased == false)
        {
            if (PlayerManager.instance.playerLevel >= smallLevelReq)
            {
                smallBuyButton.SetActive(true);
                smallStatusText.gameObject.SetActive(false);
            }
        }

        if (mediumPurchased == false)
        {
            if (PlayerManager.instance.playerLevel >= mediumLevelReq)
            {
                mediumBuyButton.SetActive(true);
                mediumStatusText.gameObject.SetActive(false);
            }
        }

        if (largePurchased == false)
        {
            if (PlayerManager.instance.playerLevel >= largeLevelReq)
            {
                largeBuyButton.SetActive(true);
                largeStatusText.gameObject.SetActive(false);
            }
        }

        if (AAAPurchased == false)
        {
            if (PlayerManager.instance.playerLevel >= AAALevelReq)
            {
                AAABuyButton.SetActive(true);
                AAAStatusText.gameObject.SetActive(false);
            }
        }
    }

    // --- BUTTON CLICK FUNCTIONS ---

    public void ClickBuySmallOffice()
    {
        if (smallPurchased == false)
        {
            if (PlayerManager.instance.playerCash >= smallPrice)
            {
                PlayerManager.instance.playerCash = PlayerManager.instance.playerCash - smallPrice;
                PlayerManager.instance.maxEmployeeSlots = smallSlots;

                basement.SetActive(false);
                smallOffice.SetActive(true);

                // Carry existing employees over into the new office layout.
                EmployeeSlotManager.instance.ChangeOffice(smallOfficeSlots);

                smallPurchased = true;
                smallBuyButton.SetActive(false);

                smallStatusText.text = "Current Workspace";
                smallStatusText.gameObject.SetActive(true);
            }
            else
            {
                ShowError();
            }
        }
    }

    public void ClickBuyMediumOffice()
    {
        if (mediumPurchased == false)
        {
            if (PlayerManager.instance.playerCash >= mediumPrice)
            {
                PlayerManager.instance.playerCash = PlayerManager.instance.playerCash - mediumPrice;
                PlayerManager.instance.maxEmployeeSlots = mediumSlots;

                smallOffice.SetActive(false);
                mediumOffice.SetActive(true);

                // Carry existing employees over into the new office layout.
                EmployeeSlotManager.instance.ChangeOffice(mediumOfficeSlots);

                // Kill the Small Office permanently
                smallPurchased = true;
                smallBuyButton.SetActive(false);
                smallStatusText.gameObject.SetActive(false);

                mediumPurchased = true;
                mediumBuyButton.SetActive(false);

                mediumStatusText.text = "Current Workspace";
                mediumStatusText.gameObject.SetActive(true);
            }
            else
            {
                ShowError();
            }
        }
    }

    public void ClickBuyLargeOffice()
    {
        if (largePurchased == false)
        {
            if (PlayerManager.instance.playerCash >= largePrice)
            {
                PlayerManager.instance.playerCash = PlayerManager.instance.playerCash - largePrice;
                PlayerManager.instance.maxEmployeeSlots = largeSlots;

                smallOffice.SetActive(false);
                mediumOffice.SetActive(false);
                largeOffice.SetActive(true);

                // Carry existing employees over into the new office layout.
                EmployeeSlotManager.instance.ChangeOffice(largeOfficeSlots);

                // Kill Small and Medium permanently
                smallPurchased = true;
                smallBuyButton.SetActive(false);
                smallStatusText.gameObject.SetActive(false);

                mediumPurchased = true;
                mediumBuyButton.SetActive(false);
                mediumStatusText.gameObject.SetActive(false);

                largePurchased = true;
                largeBuyButton.SetActive(false);

                largeStatusText.text = "Current Workspace";
                largeStatusText.gameObject.SetActive(true);
            }
            else
            {
                ShowError();
            }
        }
    }

    public void ClickBuyAAAStudio()
    {
        if (AAAPurchased == false)
        {
            if (PlayerManager.instance.playerCash >= AAAPrice)
            {
                PlayerManager.instance.playerCash = PlayerManager.instance.playerCash - AAAPrice;
                PlayerManager.instance.maxEmployeeSlots = AAASlots;

                smallOffice.SetActive(false);
                mediumOffice.SetActive(false);
                largeOffice.SetActive(false);
                AAAOffice.SetActive(true);

                // Carry existing employees over into the new office layout.
                EmployeeSlotManager.instance.ChangeOffice(AAAOfficeSlots);

                // Kill Small, Medium, and Large permanently
                smallPurchased = true;
                smallBuyButton.SetActive(false);
                smallStatusText.gameObject.SetActive(false);

                mediumPurchased = true;
                mediumBuyButton.SetActive(false);
                mediumStatusText.gameObject.SetActive(false);

                largePurchased = true;
                largeBuyButton.SetActive(false);
                largeStatusText.gameObject.SetActive(false);

                AAAPurchased = true;
                AAABuyButton.SetActive(false);

                AAAStatusText.text = "Current Workspace";
                AAAStatusText.gameObject.SetActive(true);
            }
            else
            {
                ShowError();
            }
        }
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