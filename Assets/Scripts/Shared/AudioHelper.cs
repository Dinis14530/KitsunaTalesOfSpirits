using UnityEngine;

public static class AudioHelper
{
    public static void PlayWithRandomPitch(
        AudioSource source,
        AudioClip clip,
        float minPitch = 0.95f,
        float maxPitch = 1.05f
    )
    {
        if (source == null || clip == null)
            return;

        source.pitch = Random.Range(minPitch, maxPitch);
        source.PlayOneShot(clip);
    }
}
