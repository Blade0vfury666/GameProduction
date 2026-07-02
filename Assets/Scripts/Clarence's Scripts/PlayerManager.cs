using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    [Header("Player Stats")]
    public int playerLevel;
    public int pcSpec;
    public float playerCash;
    public float playerGold;
    public float playerXP;

    [Header("Leveling System (Auto-Calculates)")]
    public float nextLevelXP; // The lifetime XP target to hit the next level

    [Header("Workspace Settings")]
    public int maxEmployeeSlots = 5;

    [Header("Market Assets")]
    public int serverLevel;
    public int chairLevel;

    [Header("Hired Workforce")]
    public List<HiredEmployeeScript> hiredEmployees = new List<HiredEmployeeScript>();
    public Transform yourEmployeesContainer;
    public HiredEmployeeScript hiredEmployeePrefab;

    [Header("Office Character")]
    public EmployeeCharacter employeeCharacterPrefab;
    public Transform officeEmployeeContainer;

    [Header("UI References (DO NOT LEAVE EMPTY)")]
    public TMP_Text playerLevelText;
    public TMP_Text pcText;
    public TMP_Text cashText;
    public TMP_Text goldText;
    public TMP_Text playerXPText;

    void Awake()
    {
        instance = this;

        // Foolproof initialization: Player must start at least at Level 1
        if (playerLevel < 1)
        {
            playerLevel = 1;
        }

        // Calculate the first XP goal immediately when the game starts
        RecalculateXPThreshold();
    }

    void Update()
    {
        // 1. Check for Level Up (Using a 'while' loop in case they earn massive XP at once and skip multiple levels!)
        while (playerXP >= nextLevelXP)
        {
            playerLevel = playerLevel + 1;
            RecalculateXPThreshold();
        }

        // 2. Update UI Visuals
        playerLevelText.text = playerLevel.ToString();
        pcText.text = "PC Spec: Level " + pcSpec.ToString();

        cashText.text = Mathf.FloorToInt(playerCash).ToString();
        goldText.text = Mathf.FloorToInt(playerGold).ToString();

        // Show progress like an RPG (e.g., "XP: 150 / 532")
        playerXPText.text = "XP: " + Mathf.FloorToInt(playerXP) + " / " + Mathf.FloorToInt(nextLevelXP);
    }

    // This calculates the exact mathematical target required for the NEXT level based on your table
    private void RecalculateXPThreshold()
    {
        nextLevelXP = 0f;

        // Loop up to the current level to build the exact total XP needed
        for (int i = 1; i <= playerLevel; i++)
        {
            // Formula: Delta XP = 150 * (Level ^ 1.35)
            float deltaXP = 150f * Mathf.Pow(i, 1.35f);
            nextLevelXP = nextLevelXP + deltaXP;
        }
    }

    // Call this from your Hire button BEFORE attempting to hire, so you can
    // disable the button / show a locked message instead of silently failing.
    public bool CanHireEmployee()
    {
        // Still in the Basement (tier 0) - hiring isn't unlocked yet.
        if (WorkspaceManager.instance == null || WorkspaceManager.instance.CurrentTier < 1)
        {
            Debug.Log("Cannot hire yet — move to the Small Office to unlock hiring.");
            return false;
        }

        if (hiredEmployees.Count >= maxEmployeeSlots)
        {
            Debug.Log("Cannot hire — all employee slots are full.");
            return false;
        }

        return true;
    }

    public void HireNewEmployee(string eName, Sprite eFace, string eSkill, int eLevel)
    {
        if (!CanHireEmployee())
        {
            return;
        }

        // Creates employee UI card
        HiredEmployeeScript newHired = Instantiate(
            hiredEmployeePrefab,
            yourEmployeesContainer
        );
        newHired.SetupHiredEmployee(
            eName,
            eFace,
            eSkill,
            eLevel
        );
        hiredEmployees.Add(newHired);

        // Creates employee standing in office
        EmployeeCharacter newCharacter = Instantiate(
            employeeCharacterPrefab,
            officeEmployeeContainer
        );
        newCharacter.SetupCharacter(eFace);

        // PUT CHARACTER INTO OFFICE SLOT
        EmployeeSlotManager.instance.AddEmployee(newCharacter);

        // LINK the UI card to its physical character so firing can clean both up
        newHired.linkedCharacter = newCharacter;
    }

    public int GetTotalEffectiveEmployeeLevel()
    {
        int totalSum = 0;
        for (int i = 0; i < hiredEmployees.Count; i++)
        {
            int effectiveLevel = hiredEmployees[i].GetFinalLevel() + pcSpec;

            if (effectiveLevel > 60)
            {
                effectiveLevel = 60;
            }

            totalSum = totalSum + effectiveLevel;
        }
        return totalSum;
    }
}