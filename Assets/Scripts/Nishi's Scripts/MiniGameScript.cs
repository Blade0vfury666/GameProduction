using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MiniGame1 : MonoBehaviour
{
    [Header("Spawning (Assign these in Inspector!)")]
    public GameEntityScript gamePrefab;
    public Transform listParent;

    [Header("References")]
    public RectTransform trackArea;
    public RectTransform marker;
    public RectTransform successZone;

    [Header("UI")]
    public Slider progressBar;
    public GameObject resultPopup;
    public GameObject miniGamePanel;
    public TMP_Text breakdownText;
    public TMP_Text comboText;

    [Header("Base Difficulty Settings (Easiest)")]
    public float baseMarkerSpeed = 300f;
    public float baseZoneWidth = 150f;
    public float baseZoneMoveRange = 200f; // Put back to 200 so it moves wildly again!

    [Header("Max Difficulty Settings (Hardest)")]
    public float maxMarkerSpeed = 800f;
    public float minZoneWidth = 60f;
    public float scopeDifficultyCap = 150f;

    [Header("Progress")]
    public float progress = 0f;
    public float progressPerPerfect = 12f;
    public float progressPerGood = 10f;
    public float progressPerMiss = 2f;

    private float activeMarkerSpeed;
    private float activeZoneMoveRange;

    [Header("Events")]
    public UnityEvent onComplete;

    private bool movingRight = true;
    private bool finished = false;

    // Output Variables
    private string currentGameName;
    private string currentGameGenre;
    private int currentGameScope;
    private float finalAccuracy;

    private int perfectHits;
    private int goodHits;
    private int missHits;

    public void InitializeMiniGame(string gameName, string gameGenre, int gameScope)
    {
        currentGameName = gameName;
        currentGameGenre = gameGenre;
        currentGameScope = gameScope;

        // 1. CALCULATE DIFFICULTY PERCENTAGE FOR SPEED & WIDTH
        float difficultyPercent = gameScope / scopeDifficultyCap;

        if (difficultyPercent > 1f)
        {
            difficultyPercent = 1f;
        }

        activeMarkerSpeed = Mathf.Lerp(baseMarkerSpeed, maxMarkerSpeed, difficultyPercent);
        float activeZoneWidth = Mathf.Lerp(baseZoneWidth, minZoneWidth, difficultyPercent);

        Vector2 zoneSize = successZone.sizeDelta;
        zoneSize.x = activeZoneWidth;
        successZone.sizeDelta = zoneSize;

        // 2. FOOLPROOF OUT-OF-BOUNDS FIX WITH YOUR OLD AGGRESSIVE MATH
        float trackHalfWidth = trackArea.rect.width / 2f;
        float zoneHalfWidth = activeZoneWidth / 2f;
        float absoluteMaxSafeRange = trackHalfWidth - zoneHalfWidth;

        // We use your old math so it jumps far right from Level 1
        float calculatedMoveRange = baseZoneMoveRange + (gameScope * 8f);

        // But if it tries to jump out of bounds, we hard-stop it at the edge of the track!
        if (calculatedMoveRange > absoluteMaxSafeRange)
        {
            calculatedMoveRange = absoluteMaxSafeRange;
        }

        activeZoneMoveRange = calculatedMoveRange;

        // Reset all states
        progress = 0f;
        perfectHits = 0;
        goodHits = 0;
        missHits = 0;
        finished = false;

        resultPopup.SetActive(false);
        comboText.text = "";

        UpdateUI();
    }

    void Update()
    {
        if (finished == true)
        {
            return;
        }

        MoveMarker();

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            EvaluateHit();
        }

        CheckCompletion();
    }

    void MoveMarker()
    {
        float halfWidth = trackArea.rect.width / 2f;
        float speedMultiplier = 1f;

        if (progress >= 90f)
            speedMultiplier = 1.6f;
        else if (progress >= 70f)
            speedMultiplier = 1.3f;

        float currentSpeed = activeMarkerSpeed * speedMultiplier;
        Vector2 pos = marker.anchoredPosition;

        if (movingRight == true)
        {
            pos.x = pos.x + (currentSpeed * Time.deltaTime);
        }
        else
        {
            pos.x = pos.x - (currentSpeed * Time.deltaTime);
        }

        if (pos.x >= halfWidth)
        {
            pos.x = halfWidth;
            movingRight = false;
        }
        else if (pos.x <= -halfWidth)
        {
            pos.x = -halfWidth;
            movingRight = true;
        }

        marker.anchoredPosition = pos;
    }

    void EvaluateHit()
    {
        float markerX = marker.anchoredPosition.x;
        float zoneMin = successZone.anchoredPosition.x - (successZone.rect.width / 2f);
        float zoneMax = successZone.anchoredPosition.x + (successZone.rect.width / 2f);

        if (markerX < zoneMin || markerX > zoneMax)
        {
            missHits++;
            progress = progress + progressPerMiss;
            ShowCombo("MISS!", Color.red);

            MoveSuccessZone();
            UpdateUI();
            return;
        }

        float center = successZone.anchoredPosition.x;
        float distance = Mathf.Abs(markerX - center);

        if (distance < successZone.rect.width * 0.2f)
        {
            perfectHits++;
            progress = progress + progressPerPerfect;
            ShowCombo("PERFECT!", Color.yellow);
        }
        else
        {
            goodHits++;
            progress = progress + progressPerGood;
            ShowCombo("HIT!", Color.green);
        }

        MoveSuccessZone();
        UpdateUI();
    }

    void MoveSuccessZone()
    {
        float randomX = Random.Range(-activeZoneMoveRange, activeZoneMoveRange);

        Vector2 pos = successZone.anchoredPosition;
        pos.x = randomX;
        successZone.anchoredPosition = pos;
    }

    void ShowCombo(string message, Color color)
    {
        comboText.text = message;
        comboText.color = color;

        StopAllCoroutines();
        StartCoroutine(ClearCombo());
    }

    IEnumerator ClearCombo()
    {
        yield return new WaitForSeconds(0.6f);
        comboText.text = "";
    }

    void UpdateUI()
    {
        if (progress > 100f)
        {
            progress = 100f;
        }

        if (progressBar != null)
        {
            progressBar.value = progress / 100f;
        }
    }

    void CheckCompletion()
    {
        if (progress >= 100f)
        {
            FinishGame();
        }
    }

    void FinishGame()
    {
        finished = true;

        int totalHits = perfectHits + goodHits + missHits;
        finalAccuracy = 0f;

        if (totalHits > 0)
        {
            float hitScore = (perfectHits * 1f) + (goodHits * 0.6f);
            finalAccuracy = (hitScore / totalHits) * 100f;
        }

        if (finalAccuracy > 100f)
        {
            finalAccuracy = 100f;
        }

        resultPopup.SetActive(true);

        breakdownText.text =
            "RESULTS\n\n" +
            "Flawless Features: " + perfectHits + "\n" +
            "Stable Features: " + goodHits + "\n" +
            "Bug Reports: " + missHits + "\n" +
            "Accuracy: " + Mathf.RoundToInt(finalAccuracy) + "%";
    }

    public void ClosePopup()
    {
        GameEntityScript spawnedGame = Instantiate(gamePrefab, listParent);
        spawnedGame.SetupGame(currentGameName, currentGameGenre, currentGameScope, finalAccuracy);

        resultPopup.SetActive(false);
        miniGamePanel.SetActive(false);
    }
}