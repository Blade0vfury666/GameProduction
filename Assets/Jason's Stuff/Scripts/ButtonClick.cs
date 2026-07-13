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

        if (AudioManager.Instance == null)
        {
            Debug.LogError("AudioManager.Instance is NULL! Make sure AudioManager exists in the scene.");
            return;
        }

        AudioManager.Instance.PlaySFX(clickSoundName);
    }
}