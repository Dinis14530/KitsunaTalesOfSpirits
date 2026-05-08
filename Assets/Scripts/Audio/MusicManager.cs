using UnityEngine;

public class MusicZone : MonoBehaviour
{
    public AudioClip areaMusic; // música específica da área

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.Instance.PlayMusic(areaMusic);
        }
    }
}
