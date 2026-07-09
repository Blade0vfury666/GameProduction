using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    [SerializeField] private AudioClip BGM;
    [SerializeField] private float crossfadeDuration = 1.5f;

    private void OnEnable()
    {
        AudioManager.Instance.PlayMusic(BGM, crossfadeDuration);
    }
}