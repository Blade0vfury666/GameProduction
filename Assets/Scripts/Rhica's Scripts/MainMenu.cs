using System.Collections;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public CanvasGroup logo;
    public GameObject logoScreen;
    public GameObject mainMenu;

    public float videoLength = 17f;
    public float fadeDuration = 1f;
    public float logoTime = 2f;

    void Start()
    {
        mainMenu.SetActive(false);
        logo.alpha = 0;

        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        // Wait for intro video
        yield return new WaitForSeconds(videoLength);

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
        yield return new WaitForSeconds(logoTime);

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
    }
}
