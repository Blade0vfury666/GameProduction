using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public CanvasGroup logo;
    public GameObject logoScreen;
    public GameObject mainMenu;
    public GameObject skipButton;
    public GameObject videoObject;
    public RawImage videoRenderer;
    public GameObject logoPanel; // ← ADD THIS (the panel that contains the logo)

    public float videoLength = 17f;
    public float fadeDuration = 1f;
    public float logoTime = 2f;

    private bool isSkipped = false;

    void Start()
    {
        // Stop any auto-playing music
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic(0f);
        }

        mainMenu.SetActive(false);
        logo.alpha = 0;

        if (logoPanel != null)
            logoPanel.SetActive(false);

        if (skipButton != null)
            skipButton.SetActive(true);

        StartCoroutine(PlaySequence());
    }

    public void SkipIntro()
    {
        isSkipped = true;
        StopAllCoroutines();

        // Stop video and hide renderer (without restarting)
        VideoPlayer videoPlayer = GetComponent<VideoPlayer>();
        if (videoPlayer == null)
            videoPlayer = FindFirstObjectByType<VideoPlayer>();

        if (videoPlayer != null)
        {
            videoPlayer.Stop();
        }

        // Hide the renderer
        if (videoRenderer != null)
        {
            videoRenderer.enabled = false;
        }

        logo.alpha = 0;
        logoScreen.SetActive(false);
        mainMenu.SetActive(true);

        if (skipButton != null)
            skipButton.SetActive(false);

        PlayMainMenuBGM();
    }

    IEnumerator PlaySequence()
    {
        // Wait for intro video
        float elapsed = 0f;
        while (elapsed < videoLength && !isSkipped)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (isSkipped) yield break;

        // Hide renderer before logo appears
        if (videoRenderer != null)
        {
            videoRenderer.enabled = false;
        }

        // Show logo panel BEFORE fading in
        if (logoPanel != null)
            logoPanel.SetActive(true);

        // Fade logo in
        float t = 0;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            logo.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }

        logo.alpha = 1;

        // Hold logo
        t = 0;
        while (t < logoTime && !isSkipped)
        {
            t += Time.deltaTime;
            yield return null;
        }

        if (isSkipped) yield break;

        // Fade logo out
        t = 0;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            logo.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            yield return null;
        }

        logo.alpha = 0;

        // Hide logo screen
        logoScreen.SetActive(false);

        // Show menu
        mainMenu.SetActive(true);

        if (skipButton != null)
            skipButton.SetActive(false);
    }

    private void PlayMainMenuBGM()
    {
        MainMenuMusicStart musicStarter = FindFirstObjectByType<MainMenuMusicStart>();
        if (musicStarter != null)
        {
            musicStarter.PlayMusicNow();
        }
        else
        {
            Debug.LogWarning("MainMenuMusicStart not found!");
        }
    }
}