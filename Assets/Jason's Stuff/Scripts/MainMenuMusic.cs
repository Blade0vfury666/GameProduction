using UnityEngine;
using UnityEngine.Video;

public class MainMenuMusicStart : MonoBehaviour
{
    [SerializeField] private VideoPlayer startupVideo;
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private float crossfadeDuration = 1.5f;

    private void OnEnable()
    {
        startupVideo.loopPointReached += OnVideoFinished;
    }

    private void OnDisable()
    {
        startupVideo.loopPointReached -= OnVideoFinished;
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        AudioManager.Instance.PlayMusic(mainMenuMusic, crossfadeDuration);
    }
}