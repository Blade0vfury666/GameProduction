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

    private string lastText = "";

    void OnEnable()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMusic(miniGameBGM);

        lastText = "";
    }

    void OnDisable()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.ReturnToPreviousMusic();
    }

    void Update()
    {
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
}