using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    private const string MasterVolumeKey = "MasterVolume";
    private const string LegacyMusicVolumeKey = "MusicVolume";

    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat(
            MasterVolumeKey,
            PlayerPrefs.GetFloat(LegacyMusicVolumeKey, 1f)
        );

        slider.value = savedVolume;
        AudioManager.Instance.SetVolume(savedVolume);

        // Adiciona listener
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float value)
    {
        AudioManager.Instance.SetVolume(value);
        PlayerPrefs.SetFloat(MasterVolumeKey, value);
    }
}
