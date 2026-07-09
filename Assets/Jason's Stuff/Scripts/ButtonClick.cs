using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonClick : MonoBehaviour
{
    [SerializeField] private string clickSoundName = "ButtonClick";

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(PlayClickSound);
    }

  private void PlayClickSound()
{
    Debug.Log("Button clicked, attempting to play: " + clickSoundName);
    AudioManager.Instance.PlaySFX(clickSoundName);
}
    
}