using UnityEngine;

public class MusicZone : MonoBehaviour
{
    public AudioClip areaMusic; // música específica da área

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (AudioManager.Instance == null)
            {
                Debug.LogWarning(
                    "[MusicZone] AudioManager.Instance is null, cannot play area music."
                );
                return;
            }

            if (areaMusic == null)
            {
                Debug.LogWarning(
                    $"[MusicZone] areaMusic clip is not assigned on '{gameObject.name}'."
                );
                return;
            }

            AudioManager.Instance.PlayMusic(areaMusic);
        }
    }
}
