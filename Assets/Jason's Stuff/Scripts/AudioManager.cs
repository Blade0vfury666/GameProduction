using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource ClickSFXSource;
    [SerializeField] private AudioSource MainMusicSource;

    private void Awake()
    {
        // Simple singleton so any script can call AudioManager.Instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
       ClickSFXSource.PlayOneShot(clip, volume);
    }

    public void PlayMusic(AudioClip clip, bool loop = true, float volume = 1f)
    {
        if (clip == null) return;
        if (MainMusicSource.clip == clip && MainMusicSource.isPlaying) return; // avoid restarting same track

        MainMusicSource.clip = clip;
        MainMusicSource.volume = volume;
        MainMusicSource.loop = loop;
        MainMusicSource.Play();
    }

    public void StopMusic()
    {
        MainMusicSource.Stop();
    }
}