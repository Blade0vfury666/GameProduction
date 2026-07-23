using UnityEngine;
using UnityEngine.SceneManagement;

public class Mainmenu : MonoBehaviour
{
    public void PlayGame()
    {
        // Loads the scene normally. SaveGameManager will find the file and load it.
        SceneManager.LoadSceneAsync(1);
    }

    public void NewGame()
    {
        // 1. Delete the save data if it exists to prevent errors on fresh installs
        if (ES3.FileExists())
        {
            ES3.DeleteFile();
        }

        // 2. Load the scene. SaveGameManager won't find a file and will start fresh.
        SceneManager.LoadSceneAsync(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}