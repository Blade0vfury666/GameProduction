using UnityEngine;
using TMPro;

public class GameEntityScript : MonoBehaviour
{
    [Header("Always Visible UI")]
    public TMP_Text titleText;
    public TMP_Text genreText;
    public TMP_Text statusText;

    [Header("Development Container (Assign the Empty Object)")]
    public GameObject developmentContainer;
    public TMP_Text devTimeRemainingText;   
    public TMP_Text speedBoostTimerText;    
    public TMP_Text goldCostText;
    public GameObject devAdButtonObject;    
    public GameObject goldButtonObject;

    [Header("Published Container (Assign the Empty Object)")]
    public GameObject publishedContainer;
    public TMP_Text moneyRateText;
    public TMP_Text xpRateText;
    public GameObject publishedAdButtonObject; 
    public TMP_Text pubSpeedBoostTimerText;    

    [Header("Balance Settings (Editable!)")]
    public float moneyPerQualityPoint = 2.5f;
    public float xpPerQualityPoint = 0.5f;
    public float goldCostPerSecond = 0.5f;

    private bool isPublished;
    private float timeRemaining;
    
    private float baseMoneyRate;
    private float baseXPRate;
    private float oneSecondTimer;

    private float devAdBoostTimer;
    private float pubAdBoostTimer;
    
    private int currentGoldCost;

    public void SetupGame(string finalName, string finalGenre, int finalQuality, float devSeconds)
    {
        isPublished = false;
        
        titleText.text = finalName;
        genreText.text = finalGenre;
        statusText.text = "Status: Developing";

        timeRemaining = devSeconds;

        baseMoneyRate = finalQuality * moneyPerQualityPoint;
        baseXPRate = finalQuality * xpPerQualityPoint;

        developmentContainer.SetActive(true);
        publishedContainer.SetActive(false);
        
        devAdButtonObject.SetActive(true);
        goldButtonObject.SetActive(true);
        speedBoostTimerText.gameObject.SetActive(false); 
        pubSpeedBoostTimerText.gameObject.SetActive(false); 
    }

    void Update()
    {
        // ==========================================
        // PHASE 1: DEVELOPING
        // ==========================================
        if (isPublished == false)
        {
            float timeSpeed = 1f;

            if (devAdBoostTimer > 0f)
            {
                devAdBoostTimer = devAdBoostTimer - Time.deltaTime;
                timeSpeed = 3f; 
                
                speedBoostTimerText.text = "Boost: " + Mathf.CeilToInt(devAdBoostTimer) + "s";
                
                if (devAdBoostTimer <= 0f)
                {
                    devAdButtonObject.SetActive(true);
                    speedBoostTimerText.gameObject.SetActive(false);
                }
            }

            timeRemaining = timeRemaining - (Time.deltaTime * timeSpeed);
            devTimeRemainingText.text = Mathf.CeilToInt(timeRemaining) + "s remaining";

            currentGoldCost = Mathf.CeilToInt(timeRemaining * goldCostPerSecond);
            if (currentGoldCost < 1) 
            {
                currentGoldCost = 1;
            }
            goldCostText.text = currentGoldCost + " Gold";

            if (timeRemaining <= 0f)
            {
                PublishGame();
            }
        }

        // ==========================================
        // PHASE 2: PUBLISHED & EARNING
        // ==========================================
        if (isPublished == true)
        {
            float currentEarningsMultiplier = 1f;

            if (pubAdBoostTimer > 0f)
            {
                pubAdBoostTimer = pubAdBoostTimer - Time.deltaTime;
                currentEarningsMultiplier = 3f; 
                
                // Show the 3x boost text specifically on the timer text!
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

    private void PublishGame()
    {
        isPublished = true;
        statusText.text = "Status: Published";

        developmentContainer.SetActive(false);
        publishedContainer.SetActive(true);
        
        publishedAdButtonObject.SetActive(true);
        pubSpeedBoostTimerText.gameObject.SetActive(false);

        UpdateRateTexts(1f);
    }

    private void UpdateRateTexts(float multiplier)
    {
        float activeMoney = baseMoneyRate * multiplier;
        float activeXP = baseXPRate * multiplier;
        
        moneyRateText.text = "+$" + Mathf.FloorToInt(activeMoney) + "/sec";
        xpRateText.text = "+" + Mathf.FloorToInt(activeXP) + " XP/sec";
    }

    public void ClickDevelopmentAdButton()
    {
        devAdBoostTimer = 300f;
        
        devAdButtonObject.SetActive(false);
        speedBoostTimerText.gameObject.SetActive(true);
    }

    public void ClickGoldButton()
    {
        if (PlayerManager.instance.playerGold >= currentGoldCost)
        {
            PlayerManager.instance.playerGold = PlayerManager.instance.playerGold - currentGoldCost;
            timeRemaining = 0f; 
        }
    }

    public void ClickPublishedAdButton()
    {
        pubAdBoostTimer = 180f;
        
        publishedAdButtonObject.SetActive(false);
        pubSpeedBoostTimerText.gameObject.SetActive(true);
        
        UpdateRateTexts(3f); 
    }
}