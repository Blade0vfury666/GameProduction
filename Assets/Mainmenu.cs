using UnityEngine;
using UnityEngine.SceneManagement;

public class Mainmenu : MonoBehaviour
{
   public void PlayGame()
   {
    SceneManager.LoadSceneAsync(1);
   }
}
