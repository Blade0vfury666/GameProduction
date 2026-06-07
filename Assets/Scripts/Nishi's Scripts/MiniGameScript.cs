using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class MiniGame1 : MonoBehaviour
{
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

    [Header("Settings")]
    public float markerSpeed = 300f;

    [Header("Progress")]
    public float progress = 0f;
    public float progressPerPerfect = 12f;
    public float progressPerGood = 10f;
    public float progressPerMiss = 2f;

    [Header("Zone Movement")]
    public float zoneMoveRange = 200f;

    [Header("Stats")]
    public int perfectHits;
    public int goodHits;
    public int missHits;

    [Header("Events")]
    public UnityEvent onComplete;

    private bool movingRight = true;
    private bool finished = false;

    void Start()
    {
        progress = 0f;

        perfectHits = 0;
        goodHits = 0;
        missHits = 0;

        resultPopup.SetActive(false);
        comboText.text = "";

        UpdateUI();
    }

    void Update()
    {
        if (finished)
            return;

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

        float currentSpeed = markerSpeed * speedMultiplier;

        Vector2 pos = marker.anchoredPosition;

        pos.x += (movingRight ? 1 : -1) * currentSpeed * Time.deltaTime;

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

        float zoneMin = successZone.anchoredPosition.x - successZone.rect.width / 2f;
        float zoneMax = successZone.anchoredPosition.x + successZone.rect.width / 2f;

        if (markerX < zoneMin || markerX > zoneMax)
        {
            missHits++;
            progress += progressPerMiss;

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
            progress += progressPerPerfect;

            ShowCombo("PERFECT!", Color.yellow);
        }
        else
        {
            goodHits++;
            progress += progressPerGood;

            ShowCombo("HIT!", Color.green);
        }

        MoveSuccessZone();
        UpdateUI();
    }

    void MoveSuccessZone()
    {
        float randomX = Random.Range(-zoneMoveRange, zoneMoveRange);

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
        progress = Mathf.Clamp(progress, 0, 100);

        if (progressBar != null)
            progressBar.value = progress / 100f;
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

        float accuracy = 0f;

        if (totalHits > 0)
        {
            accuracy =
                (perfectHits * 1f + goodHits * 0.6f) /
                totalHits;
        }

        int quality = Mathf.RoundToInt(accuracy * 100f);
        quality = Mathf.Clamp(quality, 0, 100);

        resultPopup.SetActive(true);

        breakdownText.text =
            $"RESULTS\n\n" +
            $"Flawless Features: {perfectHits}\n" +
            $"Stable Features: {goodHits}\n" +
            $"Bug Reports: {missHits}\n" +
            $"Game Quality: {quality}%";
    }


    public void ClosePopup()
    {
        resultPopup.SetActive(false);
        miniGamePanel.SetActive(false);
    }
}