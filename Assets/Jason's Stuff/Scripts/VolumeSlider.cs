using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public enum VolumeType { SFX, Music }

    [SerializeField] private Slider slider;
    [SerializeField] private VolumeType volumeType;

    private void Start()
    {
        // Initialize slider to whatever AudioManager currently has saved
        slider.value = volumeType == VolumeType.SFX 
            ? AudioManager.Instance.sfxVolume 
            : AudioManager.Instance.musicVolume;

        slider.onValueChanged.AddListener(HandleVolumeChanged);
    }

    private void HandleVolumeChanged(float value)
    {
        if (volumeType == VolumeType.SFX)
            AudioManager.Instance.SetSFXVolume(value);
        else
            AudioManager.Instance.SetMusicVolume(value);
    }

    private void OnDestroy()
    {
        slider.onValueChanged.RemoveListener(HandleVolumeChanged);
    }
}