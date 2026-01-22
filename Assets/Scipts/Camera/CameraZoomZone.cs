using UnityEngine;

public class CameraZoomZone : MonoBehaviour
{
    public float zoomSize = 3.5f;     
    public float zoomSpeed = 3f;       
    private Camera cam;
    private bool playerInside = false;
    private float defaultSize;

    void Start()
    {
        cam = Camera.main;
        defaultSize = cam.orthographicSize;
    }

    void Update()
    {
        float targetSize = playerInside ? zoomSize : defaultSize;
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, zoomSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInside = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInside = false;
    }
}
