using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class QuitToMainMenu : MonoBehaviour
{
    public float fadeDuration = 0.5f;

    public void GoToMainMenu()
    {
        StartCoroutine(QuitWithFade());
    }

    IEnumerator QuitWithFade()
    {
        // Fade out music
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic(fadeDuration);
        }

        // Wait for fade
        yield return new WaitForSeconds(fadeDuration);

        // Load scene
        SceneManager.LoadScene("Main Menu");
    }
}