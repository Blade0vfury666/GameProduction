using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EmployeeManager : MonoBehaviour
{
    [Header("Market UI")]
    public Transform marketContainer;
    public TMP_Text marketTimerText;
    private float marketRefreshTimer;

    [Header("Prefabs")]
    public EmployeeScript rarePrefab;
    public EmployeeScript superRarePrefab;
    public EmployeeScript superSuperRarePrefab;

    [Header("Data Pools (Fill in Inspector)")]
    public string[] randomNames;
    public Sprite[] randomFaces;
    public string[] randomSkills;

    [Header("SSR Specialist Search")]
    public GameObject ssrPopupCanvas; // The canvas that blocks everything
    public Transform ssrSpawnPoint;   // Empty object inside the popup
    public GameObject ssrFailMessage; // Text saying "Failed"
    public GameObject ssrOkButton;    // Button to close if failed
    public TMP_Text specialistTimerText; // Shows on your main screen

    private bool isSearchingSSR;
    private float ssrTimer;

    void Start()
    {
        marketRefreshTimer = 600f; // 10 minutes
        ssrPopupCanvas.SetActive(false);
        GenerateNewMarket();
    }

    void Update()
    {
        // --- 1. MARKET REFRESH TIMER ---
        marketRefreshTimer = marketRefreshTimer - Time.deltaTime;
        
        // Convert to minutes and seconds for the UI
        int minutes = Mathf.FloorToInt(marketRefreshTimer / 60f);
        int seconds = Mathf.FloorToInt(marketRefreshTimer % 60f);
        marketTimerText.text = "Refresh in: " + minutes + "m " + seconds + "s";

        if (marketRefreshTimer <= 0f)
        {
            GenerateNewMarket();
            marketRefreshTimer = 600f; // Reset to 10 mins
        }

        // --- 2. SSR SEARCH TIMER ---
        if (isSearchingSSR == true)
        {
            ssrTimer = ssrTimer - Time.deltaTime;
            
            int sMinutes = Mathf.FloorToInt(ssrTimer / 60f);
            int sSeconds = Mathf.FloorToInt(ssrTimer % 60f);
            specialistTimerText.text = "Searching: " + sMinutes + "m " + sSeconds + "s";

            if (ssrTimer <= 0f)
            {
                FinishSSRSearch();
            }
        }
    }

    // --- MARKET LOGIC ---

    public void ClickManualRefresh() // Costs 2500 Cash
    {
        if (PlayerManager.instance.playerCash >= 2500f)
        {
            PlayerManager.instance.playerCash = PlayerManager.instance.playerCash - 2500f;
            GenerateNewMarket();
            marketRefreshTimer = 600f; // Reset the 10 min timer early
        }
    }

    private void GenerateNewMarket()
    {
        // 1. Destroy old unhired employees in the market
        foreach (Transform child in marketContainer)
        {
            Destroy(child.gameObject);
        }

        // 2. Spawn 10 new ones
        for (int i = 0; i < 10; i++)
        {
            EmployeeScript chosenPrefab = rarePrefab;
            int randomLevel = 1;

            // Roll Rarity: 1 to 100
            int rarityRoll = Random.Range(1, 101);

            if (rarityRoll <= 85) // 85% Chance for Rare
            {
                chosenPrefab = rarePrefab;
                randomLevel = Random.Range(1, 21); // Levels 1 - 20
            }
            if (rarityRoll > 85) // 15% Chance for Super Rare
            {
                chosenPrefab = superRarePrefab;
                randomLevel = Random.Range(21, 41); // Levels 21 - 40
            }

            SpawnEmployee(chosenPrefab, randomLevel, marketContainer);
        }
    }

    // --- SSR SPECIALIST LOGIC ---

    public void ClickSearchSpecialist() // Costs 100 Gold
    {
        if (isSearchingSSR == false)
        {
            if (PlayerManager.instance.playerGold >= 100f)
            {
                PlayerManager.instance.playerGold = PlayerManager.instance.playerGold - 100f;
                isSearchingSSR = true;
                ssrTimer = 300f; // 5 minutes
                specialistTimerText.gameObject.SetActive(true);
            }
        }
    }

    private void FinishSSRSearch()
    {
        isSearchingSSR = false;
        specialistTimerText.gameObject.SetActive(false);
        
        // Turn on the Popup Blocker Canvas
        ssrPopupCanvas.SetActive(true);

        // Wipe anything that might be sitting in the spawn point from last time
        foreach (Transform child in ssrSpawnPoint)
        {
            Destroy(child.gameObject);
        }

        // Roll for SSR: 5% Chance
        int ssrRoll = Random.Range(1, 101);

        if (ssrRoll <= 5) // Success!
        {
            ssrFailMessage.SetActive(false);
            ssrOkButton.SetActive(false);

            int randomLevel = Random.Range(41, 61); // Levels 41 - 60
            SpawnEmployee(superSuperRarePrefab, randomLevel, ssrSpawnPoint);
        }
        else // Fail...
        {
            ssrFailMessage.SetActive(true);
            ssrOkButton.SetActive(true);
        }
    }

    public void ClickCloseSSRPopup()
    {
        ssrPopupCanvas.SetActive(false);
    }

    // --- SHARED SPAWNING LOGIC ---
    
    private void SpawnEmployee(EmployeeScript prefabToSpawn, int level, Transform targetFolder)
    {
        // Pick random visual data
        string rName = randomNames[Random.Range(0, randomNames.Length)];
        Sprite rFace = randomFaces[Random.Range(0, randomFaces.Length)];
        string rSkill = randomSkills[Random.Range(0, randomSkills.Length)];

        // Spawn it into the target folder
        EmployeeScript newStaff = Instantiate(prefabToSpawn, targetFolder);
        
        // Handshake
        newStaff.SetupEmployee(rName, rFace, rSkill, level);
    }
}