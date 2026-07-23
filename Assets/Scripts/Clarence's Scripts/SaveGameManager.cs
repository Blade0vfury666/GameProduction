using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SavedEmployeeData
{
    public string empName;
    public string faceName;
    public string skill;
    public int level;
}

[System.Serializable]
public class SavedMarketEmployeeData
{
    public string empName;
    public string faceName;
    public string skill;
    public int level;
    public int cost; 
}

[System.Serializable]
public class SavedGameData
{
    public string title;
    public string genre;
    public float moneyRate;
    public float xpRate;
    public float adBoostTimer; 
}

[System.Serializable]
public class SavedEquipmentData
{
    public string itemName;
    public bool isOwned;
    public bool isEquipped;
}

public class SaveGameManager : MonoBehaviour
{
    public static SaveGameManager instance;

    [Header("Loading References")]
    public GameEntityScript gamePrefab;
    public Transform publishedGamesContainer;
    public GameObject tutorialContainer;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        Invoke(nameof(LoadGame), 0.1f);
    }

    public void SaveGame()
    {
        ES3.Save("tutorialFinished", !tutorialContainer.activeSelf);

        ES3.Save("playerLevel", PlayerManager.instance.playerLevel);
        ES3.Save("playerCash", PlayerManager.instance.playerCash);
        ES3.Save("playerGold", PlayerManager.instance.playerGold);
        ES3.Save("playerXP", PlayerManager.instance.playerXP);

        ES3.Save("workspaceTier", WorkspaceManager.instance.CurrentTier);

        Equipment[] allEquipment = Resources.LoadAll<Equipment>("Equipment");
        List<SavedEquipmentData> equipmentSave = new List<SavedEquipmentData>();
        foreach(var item in allEquipment)
        {
            equipmentSave.Add(new SavedEquipmentData { itemName = item.itemName, isOwned = item.isOwned, isEquipped = item.isEquipped });
        }
        ES3.Save("equipment", equipmentSave);

        List<SavedEmployeeData> employeeSave = new List<SavedEmployeeData>();
        foreach(var emp in PlayerManager.instance.hiredEmployees)
        {
            employeeSave.Add(new SavedEmployeeData {
                empName = emp.employeeName,
                faceName = emp.employeeFace != null ? emp.employeeFace.name : "",
                skill = emp.employeeSkill,
                level = emp.baseLevel
            });
        }
        ES3.Save("employees", employeeSave);

        List<SavedGameData> gamesSave = new List<SavedGameData>();
        GameEntityScript[] activeGames = publishedGamesContainer.GetComponentsInChildren<GameEntityScript>();
        foreach(var game in activeGames)
        {
            gamesSave.Add(new SavedGameData {
                title = game.titleText.text,
                genre = game.genreText.text,
                moneyRate = game.GetBaseMoneyRate(),
                xpRate = game.GetBaseXPRate(),
                adBoostTimer = game.GetAdBoostTimer() 
            });
        }
        ES3.Save("publishedGames", gamesSave);

        ES3.Save("hireRefreshTimer", EmployeeManager.instance.hireRefreshTimer);
        ES3.Save("isSearchingLegendary", EmployeeManager.instance.isSearchingLegendary);
        ES3.Save("legendaryTimer", EmployeeManager.instance.legendaryTimer);

        List<SavedMarketEmployeeData> marketSave = new List<SavedMarketEmployeeData>();
        foreach(var marketEmp in EmployeeManager.instance.marketEmployeesList) 
        {
            marketSave.Add(new SavedMarketEmployeeData {
                empName = marketEmp.employeeName,
                faceName = marketEmp.employeeFace != null ? marketEmp.employeeFace.name : "",
                skill = marketEmp.employeeSkill,
                level = marketEmp.baseLevel,
                cost = marketEmp.hireCost
            });
        }
        ES3.Save("marketEmployees", marketSave);

        Debug.Log("Game Saved Successfully!");
    }

    public void LoadGame()
    {
        // --- THE ACTUAL FIX ---
        // 1. We MUST scrub the ScriptableObjects clean immediately when the scene starts.
        Equipment[] allEquipment = Resources.LoadAll<Equipment>("Equipment");
        foreach(var eqAsset in allEquipment)
        {
            eqAsset.isOwned = false;
            eqAsset.isEquipped = false;
        }

        // 2. NOW we check if a save file exists. If it's a New Game, it stops here, and the items stay clean!
        if (!ES3.KeyExists("playerLevel")) return;
        // ----------------------

        bool tutorialFinished = ES3.Load<bool>("tutorialFinished", false);
        if (tutorialFinished == true)
        {
            tutorialContainer.SetActive(false);
        }

        PlayerManager.instance.playerLevel = ES3.Load<int>("playerLevel", 1);
        PlayerManager.instance.playerCash = ES3.Load<float>("playerCash", 0f);
        PlayerManager.instance.playerGold = ES3.Load<float>("playerGold", 0f);
        PlayerManager.instance.playerXP = ES3.Load<float>("playerXP", 0f);

        int savedTier = ES3.Load<int>("workspaceTier", 1);
        WorkspaceManager.instance.LoadSavedTier(savedTier);

        // 3. Equipment (We already loaded allEquipment at the top, so we just apply the save data now)
        List<SavedEquipmentData> equipmentSave = ES3.Load<List<SavedEquipmentData>>("equipment", new List<SavedEquipmentData>());
        foreach(var savedItem in equipmentSave)
        {
            foreach(var eqAsset in allEquipment)
            {
                if (eqAsset.itemName == savedItem.itemName)
                {
                    eqAsset.isOwned = savedItem.isOwned;
                    eqAsset.isEquipped = savedItem.isEquipped;
                    break;
                }
            }
        }
        
        if (EquipmentManager.instance != null) EquipmentManager.instance.ApplyEquipmentBonuses();
        if (MarketManager.instance != null) 
        {
            MarketManager.instance.PopulateInventoryUI();
            MarketManager.instance.PopulateMarketUI();
            MarketManager.instance.UpdateCurrentStatsUI();
        }

        List<SavedEmployeeData> employeeSave = ES3.Load<List<SavedEmployeeData>>("employees", new List<SavedEmployeeData>());
        foreach(var savedEmp in employeeSave)
        {
            Sprite faceSprite = EmployeeManager.instance.GetFaceSprite(savedEmp.faceName);
            PlayerManager.instance.HireNewEmployee(savedEmp.empName, faceSprite, savedEmp.skill, savedEmp.level);
        }

        List<SavedGameData> gamesSave = ES3.Load<List<SavedGameData>>("publishedGames", new List<SavedGameData>());
        foreach(var savedGame in gamesSave)
        {
            GameEntityScript newGame = Instantiate(gamePrefab, publishedGamesContainer);
            newGame.LoadExistingGame(savedGame.title, savedGame.genre, savedGame.moneyRate, savedGame.xpRate, savedGame.adBoostTimer);
        }

        EmployeeManager.instance.hireRefreshTimer = ES3.Load<float>("hireRefreshTimer", 600f);
        EmployeeManager.instance.isSearchingLegendary = ES3.Load<bool>("isSearchingLegendary", false);
        EmployeeManager.instance.legendaryTimer = ES3.Load<float>("legendaryTimer", 0f);
        
        if (EmployeeManager.instance.isSearchingLegendary)
        {
            EmployeeManager.instance.specialistTimerText.gameObject.SetActive(true);
            EmployeeManager.instance.searchSpecialistButton.interactable = false;
        }

        if (ES3.KeyExists("marketEmployees"))
        {
            EmployeeManager.instance.ClearMarketList(); 
            
            List<SavedMarketEmployeeData> marketSave = ES3.Load<List<SavedMarketEmployeeData>>("marketEmployees", new List<SavedMarketEmployeeData>());
            foreach(var savedMarketEmp in marketSave)
            {
                Sprite faceSprite = EmployeeManager.instance.GetFaceSprite(savedMarketEmp.faceName);
                EmployeeManager.instance.SpawnMarketEmployeeUI(savedMarketEmp.empName, faceSprite, savedMarketEmp.skill, savedMarketEmp.level, savedMarketEmp.cost);
            }
        }

        Debug.Log("Game Loaded Successfully!");
    }
}