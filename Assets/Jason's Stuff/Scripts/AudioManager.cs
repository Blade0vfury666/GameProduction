using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SFXEntry
{
    public string name;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("SFX")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private List<SFXEntry> sfxLibrary;
    private Dictionary<string, AudioClip> sfxDictionary;

    [Header("Music (2 sources for crossfading)")]
    [SerializeField] private AudioSource musicSourceA;
    [SerializeField] private AudioSource musicSourceB;
    [SerializeField] private float defaultCrossfadeDuration = 1.5f;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 1f;

    private AudioSource activeMusicSource;
    private AudioSource inactiveMusicSource;
    private Coroutine crossfadeCoroutine;
    private AudioClip currentClip;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        activeMusicSource = musicSourceA;
        inactiveMusicSource = musicSourceB;
        activeMusicSource.loop = true;
        inactiveMusicSource.loop = true;
        inactiveMusicSource.volume = 0f;

        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        activeMusicSource.volume = musicVolume;

        sfxDictionary = new Dictionary<string, AudioClip>();
        foreach (var entry in sfxLibrary)
        {
            if (!sfxDictionary.ContainsKey(entry.name))
                sfxDictionary[entry.name] = entry.clip;
        }
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, volume * sfxVolume);
    }

    public void PlaySFX(string clipName, float volume = 1f)
    {
        if (sfxDictionary.TryGetValue(clipName, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip, volume * sfxVolume);
        }
        else
        {
            Debug.LogWarning($"SFX '{clipName}' not found in library.");
        }
    }

    // ============================================
    // FIXED: PlayMusic with duplicate check
    // ============================================
    public void PlayMusic(AudioClip clip, float? fadeDuration = null)
    {
        if (clip == null) return;

        // Don't restart the same clip if it's already playing
        if (clip == currentClip && activeMusicSource.isPlaying)
        {
            Debug.Log("Music already playing: " + clip.name);
            return;
        }

        Debug.Log("Playing music: " + clip.name);
        currentClip = clip;
        float duration = fadeDuration ?? defaultCrossfadeDuration;

        if (crossfadeCoroutine != null)
            StopCoroutine(crossfadeCoroutine);

        crossfadeCoroutine = StartCoroutine(CrossfadeTo(clip, duration));
    }

    private IEnumerator CrossfadeTo(AudioClip newClip, float duration)
    {
        // Ensure the inactive source is ready
        inactiveMusicSource.clip = newClip;
        inactiveMusicSource.volume = 0f;
        inactiveMusicSource.Play();

        float startVolumeActive = activeMusicSource.volume;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float ratio = t / duration;

            activeMusicSource.volume = Mathf.Lerp(startVolumeActive, 0f, ratio);
            inactiveMusicSource.volume = Mathf.Lerp(0f, musicVolume, ratio);

            yield return null;
        }

        activeMusicSource.Stop();
        activeMusicSource.volume = 0f;
        inactiveMusicSource.volume = musicVolume;

        // Swap sources
        (activeMusicSource, inactiveMusicSource) = (inactiveMusicSource, activeMusicSource);
        crossfadeCoroutine = null;
    }

    public void StopMusic(float fadeDuration = 1f)
    {
        if (crossfadeCoroutine != null)
            StopCoroutine(crossfadeCoroutine);

        crossfadeCoroutine = StartCoroutine(FadeOutAndStop(fadeDuration));
        currentClip = null;
    }

    private IEnumerator FadeOutAndStop(float duration)
    {
        float startVolume = activeMusicSource.volume;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            activeMusicSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        activeMusicSource.Stop();
        activeMusicSource.volume = 0f;
        crossfadeCoroutine = null;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        activeMusicSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }
}