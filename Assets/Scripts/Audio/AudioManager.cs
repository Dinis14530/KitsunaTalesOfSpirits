using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private const string MasterVolumeKey = "MasterVolume";
    private const string LegacyMusicVolumeKey = "MusicVolume";

    public AudioSource musicSource; // Fonte de música principal
    public float fadeDuration = 1f; // tempo para fade

    private void Awake()
    {
        // Mantem apenas uma instancia global para controlo de musica entre cenas
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        float savedVolume = PlayerPrefs.GetFloat(
            MasterVolumeKey,
            PlayerPrefs.GetFloat(LegacyMusicVolumeKey, 1f)
        );
        SetVolume(savedVolume);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("[AudioManager] PlayMusic called with null clip.");
            return;
        }

        if (musicSource == null)
        {
            Debug.LogError("[AudioManager] musicSource is not assigned.");
            return;
        }

        StopAllCoroutines();
        StartCoroutine(FadeMusic(clip));
    }

    private System.Collections.IEnumerator FadeMusic(AudioClip newClip)
    {
        // Fade out
        float startVolume = musicSource.volume;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        musicSource.clip = newClip;
        musicSource.Play();

        // Fade in
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0, startVolume, t / fadeDuration);
            yield return null;
        }

        musicSource.volume = startVolume;
    }

    // Método para ajustar o volume mestre do jogo
    public void SetVolume(float volume)
    {
        AudioListener.volume = Mathf.Clamp01(volume);
    }
}
