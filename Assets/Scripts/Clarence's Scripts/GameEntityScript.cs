using UnityEngine;
using TMPro;

public class GameEntityScript : MonoBehaviour
{
    [Header("Always Visible UI")]
    public TMP_Text titleText;
    public TMP_Text genreText;
    public TMP_Text statusText;

    [Header("Published Container")]
    public GameObject publishedContainer;
    public TMP_Text moneyRateText;
    public TMP_Text xpRateText;
    public GameObject publishedAdButtonObject; 
    public TMP_Text pubSpeedBoostTimerText;    

    [Header("Balance Settings")]
    // MASSIVELY NERFED DEFAULT VALUES FOR A BALANCED ECONOMY!
    public float moneyPerQualityPoint = 0.2f; 
    public float xpPerQualityPoint = 0.05f;

    public float GetAdBoostTimer() { return pubAdBoostTimer; }
    public float GetBaseMoneyRate() { return baseMoneyRate; }
    public float GetBaseXPRate() { return baseXPRate; }

    private bool isPublished;
    private float baseMoneyRate;
    private float baseXPRate;
    private float oneSecondTimer;
    private float pubAdBoostTimer;

    // The Instanced Handshake (Phase C & D Math)
    public void SetupGame(string finalName, string finalGenre, int scope, float finalAccuracy)
    {
        titleText.text = finalName;
        genreText.text = finalGenre;
        statusText.text = "Status: Published";

        // Gather mathematical workforce variables (Includes Employee Level & PC Spec)
        int totalEffectiveLevel = PlayerManager.instance.GetTotalEffectiveEmployeeLevel();
        float teamSafetyNet = totalEffectiveLevel / 12.0f;
        
        // REBALANCED QUALITY SCORE MATH
        // Scope still gives base score (higher difficulty = more base money)
        float baseMath = (scope * 2f) + (PlayerManager.instance.playerLevel * 1.5f);
        float qualityScore = ((finalAccuracy / 100f) * baseMath) + teamSafetyNet;

        // ==========================================
        // CHANGED: Server Level added directly here!
        // Employees, PC, Chairs, and Servers ALL buff the final cash/xp output
        // without touching the difficulty!
        // ==========================================
        float multiSystem = (1f + (totalEffectiveLevel / 100f)) 
                          * (1f + (PlayerManager.instance.chairLevel * 0.05f))
                          * (1f + (PlayerManager.instance.serverLevel * 0.10f));

        baseMoneyRate = qualityScore * multiSystem * moneyPerQualityPoint;
        baseXPRate = qualityScore * multiSystem * xpPerQualityPoint;

        isPublished = true;
        publishedContainer.SetActive(true);
        publishedAdButtonObject.SetActive(true);
        pubSpeedBoostTimerText.gameObject.SetActive(false); 
        
        UpdateRateTexts(1f);
    }

    void Update()
    {
        // ==========================================
        // PUBLISHED & EARNING (Loop)
        // ==========================================
        if (isPublished == true)
        {
            float currentEarningsMultiplier = 1f;

            if (pubAdBoostTimer > 0f)
            {
                pubAdBoostTimer = pubAdBoostTimer - Time.deltaTime;
                currentEarningsMultiplier = 3f; 
                
                pubSpeedBoostTimerText.text = "3x CASH & XP BOOST: " + Mathf.CeilToInt(pubAdBoostTimer) + "s";

                if (pubAdBoostTimer <= 0f)
                {
                    publishedAdButtonObject.SetActive(true);
                    pubSpeedBoostTimerText.gameObject.SetActive(false);
                    UpdateRateTexts(1f); 
                }
            }

            oneSecondTimer = oneSecondTimer + Time.deltaTime;
            
            if (oneSecondTimer >= 1f)
            {
                oneSecondTimer = 0f; 
                
                PlayerManager.instance.playerCash = PlayerManager.instance.playerCash + (baseMoneyRate * currentEarningsMultiplier);
                PlayerManager.instance.playerXP = PlayerManager.instance.playerXP + (baseXPRate * currentEarningsMultiplier);
            }
        }
    }

    private void UpdateRateTexts(float multiplier)
    {
        float activeMoney = baseMoneyRate * multiplier;
        float activeXP = baseXPRate * multiplier;
        
        // NOW USING FLOATS FOR UI (".00")
        moneyRateText.text = "+$" + activeMoney.ToString("F2") + "/sec";
        xpRateText.text = "+" + activeXP.ToString("F2") + " XP/sec";
    }

    public void ClickPublishedAdButton()
    {
        pubAdBoostTimer = 180f;
        
        publishedAdButtonObject.SetActive(false);
        pubSpeedBoostTimerText.gameObject.SetActive(true);
        
        UpdateRateTexts(3f); 
    }

    public void LoadExistingGame(string finalName, string finalGenre, float loadedMoneyRate, float loadedXPRate, float adBoostTimer)
    {
        titleText.text = finalName;
        genreText.text = finalGenre;
        statusText.text = "Status: Published";
        
        baseMoneyRate = loadedMoneyRate;
        baseXPRate = loadedXPRate;
        pubAdBoostTimer = adBoostTimer;
        
        isPublished = true;
        publishedContainer.SetActive(true);
        
        if (pubAdBoostTimer > 0f)
        {
            publishedAdButtonObject.SetActive(false);
            pubSpeedBoostTimerText.gameObject.SetActive(true);
            UpdateRateTexts(3f);
        }
        else
        {
            publishedAdButtonObject.SetActive(true);
            pubSpeedBoostTimerText.gameObject.SetActive(false); 
            UpdateRateTexts(1f);
        }
    }
}