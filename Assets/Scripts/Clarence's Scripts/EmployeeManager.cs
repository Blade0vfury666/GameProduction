using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EmployeeManager : MonoBehaviour
{
    public static EmployeeManager instance;

    [Header("Hire Employees UI (Main List)")]
    public Transform hireEmployeesContentList; 
    public TMP_Text hireRefreshTimerText;
    
    public float manualRefreshCost = 2500f; 
    private float hireRefreshTimer;

    [Header("Employee Prefab")]
    public EmployeeScript employeePrefab;

    [Header("Male Data Pools")]
    public string[] maleNames;
    public Sprite[] maleFaces;

    [Header("Female Data Pools")]
    public string[] femaleNames;
    public Sprite[] femaleFaces;

    [Header("Shared Skills")]
    public string[] randomSkills;

    [Header("Legendary Search UI")]
    public float specialistSearchCost = 100f; 
    
    [Tooltip("Time in SECONDS for the specialist search (600 = 10 mins)")]
    public float specialistSearchTime = 600f; 
    
    public Button searchSpecialistButton; 
    public GameObject legendaryPopupCanvas; 
    public Transform legendaryResultContainer; 
    public GameObject legendaryFailMessage; 
    public GameObject legendaryDismissButton; 
    public TMP_Text specialistTimerText; 

    [Header("Warnings")]
    public TMP_Text warningTXT; 
    private float warningTimer;

    private bool isSearchingLegendary;
    private float legendaryTimer;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        hireRefreshTimer = 600f; 
        CloseAndResetLegendaryUI(); 
        
        warningTXT.gameObject.SetActive(false);
        specialistTimerText.gameObject.SetActive(false);
        
        GenerateNewMarket();
    }

    void Update()
    {
        // --- WARNING TIMER LOGIC ---
        if (warningTimer > 0f)
        {
            warningTimer -= Time.deltaTime;
            if (warningTimer <= 0f)
            {
                warningTXT.gameObject.SetActive(false);
            }
        }

        // --- 1. MAIN LIST REFRESH TIMER ---
        hireRefreshTimer -= Time.deltaTime;
        
        int minutes = Mathf.FloorToInt(hireRefreshTimer / 60f);
        int seconds = Mathf.FloorToInt(hireRefreshTimer % 60f);
        
        hireRefreshTimerText.text = "Refresh in: " + minutes + "m " + seconds + "s";

        if (hireRefreshTimer <= 0f)
        {
            GenerateNewMarket();
            hireRefreshTimer = 600f; 
        }

        // --- 2. LEGENDARY SEARCH TIMER ---
        if (isSearchingLegendary == true)
        {
            legendaryTimer -= Time.deltaTime;
            
            int sMinutes = Mathf.CeilToInt(legendaryTimer / 60f); 
            
            specialistTimerText.text = "Searching. Time: " + sMinutes + "m";

            if (legendaryTimer <= 0f)
            {
                FinishLegendarySearch();
            }
        }
    }

    public void ShowWarning(string message)
    {
        warningTXT.text = message;
        warningTXT.gameObject.SetActive(true);
        warningTimer = 5f; 
    }

    // --- MAIN LIST LOGIC ---

    public void ClickManualRefresh() 
    {
        if (PlayerManager.instance.playerCash >= manualRefreshCost)
        {
            PlayerManager.instance.playerCash -= manualRefreshCost;
            GenerateNewMarket();
            hireRefreshTimer = 600f; 
        }
        else
        {
            ShowWarning("Insufficient Funds!");
        }
    }

    private void GenerateNewMarket()
    {
        foreach (Transform child in hireEmployeesContentList)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < 10; i++)
        {
            int randomLevel = 1;
            int rarityRoll = Random.Range(1, 101);

            if (rarityRoll <= 90) 
            {
                randomLevel = Random.Range(1, 21); // Common
            }
            else 
            {
                randomLevel = Random.Range(21, 41); // Rare
            }

            SpawnEmployee(randomLevel, hireEmployeesContentList, false);
        }
    }

    // --- LEGENDARY SPECIALIST LOGIC ---

    public void ClickSearchSpecialist() 
    {
        if (isSearchingLegendary == false)
        {
            // CHECK IF THERE IS ROOM FIRST, MASTER!
            if (PlayerManager.instance.hiredEmployees.Count < PlayerManager.instance.maxEmployeeSlots)
            {
                // THEN CHECK FUNDS
                if (PlayerManager.instance.playerGold >= specialistSearchCost)
                {
                    PlayerManager.instance.playerGold -= specialistSearchCost;
                    isSearchingLegendary = true;
                    
                    legendaryTimer = specialistSearchTime; 
                    
                    specialistTimerText.gameObject.SetActive(true);
                    searchSpecialistButton.interactable = false; 
                }
                else
                {
                    ShowWarning("Insufficient Funds!"); 
                }
            }
            else
            {
                ShowWarning("Upgrade Workspace!");
            }
        }
    }

    private void FinishLegendarySearch()
    {
        isSearchingLegendary = false;
        
        specialistTimerText.gameObject.SetActive(false);
        searchSpecialistButton.interactable = true; 
        
        legendaryPopupCanvas.SetActive(true);

        foreach (Transform child in legendaryResultContainer)
        {
            Destroy(child.gameObject);
        }

        int legendaryRoll = Random.Range(1, 101);

        if (legendaryRoll <= 5) // 5% Success!
        {
            legendaryFailMessage.SetActive(false);
            legendaryDismissButton.SetActive(false);
            
            int randomLevel = Random.Range(41, 61); 
            SpawnEmployee(randomLevel, legendaryResultContainer, true);
        }
        else // Fail...
        {
            legendaryFailMessage.SetActive(true);
            legendaryDismissButton.SetActive(true);
        }
    }

    public void CloseAndResetLegendaryUI()
    {
        legendaryPopupCanvas.SetActive(false);
        legendaryFailMessage.SetActive(false);
        legendaryDismissButton.SetActive(false);

        foreach (Transform child in legendaryResultContainer)
        {
            Destroy(child.gameObject);
        }
    }

    // --- SHARED SPAWNING LOGIC ---
    
    private void SpawnEmployee(int level, Transform targetFolder, bool isFreeRoll)
    {
        string rName = "";
        Sprite rFace = null;
        string rSkill = randomSkills[Random.Range(0, randomSkills.Length)];

        int genderFlip = Random.Range(0, 2); 

        if (genderFlip == 0) 
        {
            rName = maleNames[Random.Range(0, maleNames.Length)];
            rFace = maleFaces[Random.Range(0, maleFaces.Length)];
        }
        else 
        {
            rName = femaleNames[Random.Range(0, femaleNames.Length)];
            rFace = femaleFaces[Random.Range(0, femaleFaces.Length)];
        }

        EmployeeScript newStaff = Instantiate(employeePrefab, targetFolder);
        newStaff.SetupEmployee(rName, rFace, rSkill, level, isFreeRoll);
    }
}