using UnityEngine;
using TMPro;

public class MiniGameAudio : MonoBehaviour
{
    [Header("Reference to the minigame's combo text")]
    [SerializeField] private TMP_Text comboText;

    [Header("SFX Names (must match AudioManager's Sfx Library)")]
    [SerializeField] private string perfectSFX = "SliderPerfect";
    [SerializeField] private string goodSFX = "SliderGood";
    [SerializeField] private string missSFX = "SliderMiss";

    [Header("BGM Settings")]
    [SerializeField] private AudioClip miniGameBGM;

    [Header("Panel Reference")]
    [SerializeField] private GameObject miniGamePanel;

    private string lastText = "";
    private bool isBGMPlaying = false;

    void Update()
    {
        // ============================================
        // BGM: Auto-play when panel is active
        // ============================================
        if (miniGamePanel != null)
        {
            if (miniGamePanel.activeSelf && !isBGMPlaying)
            {
                PlayBGM();
            }
            else if (!miniGamePanel.activeSelf && isBGMPlaying)
            {
                StopBGM();
            }
        }

        // ============================================
        // SFX: Detect combo text changes
        // ============================================
        if (comboText == null) return;
        if (comboText.text == lastText) return;

        lastText = comboText.text;

        switch (comboText.text)
        {
            case "PERFECT!":
                AudioManager.Instance.PlaySFX(perfectSFX);
                break;
            case "HIT!":
                AudioManager.Instance.PlaySFX(goodSFX);
                break;
            case "MISS!":
                AudioManager.Instance.PlaySFX(missSFX);
                break;
        }
    }

    void PlayBGM()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("AudioManager.Instance is null!");
            return;
        }

        if (miniGameBGM == null)
        {
            Debug.LogWarning("miniGameBGM is not assigned!");
            return;
        }

        if (isBGMPlaying) return;

        AudioManager.Instance.PlayMusic(miniGameBGM);
        isBGMPlaying = true;
        Debug.Log("MiniGame BGM started: " + miniGameBGM.name);
    }

    void StopBGM()
    {
        if (AudioManager.Instance == null) return;
        if (!isBGMPlaying) return;

        AudioManager.Instance.StopMusic();
        isBGMPlaying = false;
        Debug.Log("MiniGame BGM stopped");
    }

    void OnEnable()
    {
        // Reset state when enabled
        isBGMPlaying = false;
    }

    void OnDisable()
    {
        StopBGM();
    }
}