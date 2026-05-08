using UnityEngine;

public class CameraZoomZone : MonoBehaviour
{
    public float zoomSize = 3.5f;
    public float zoomSpeed = 3f;
    public float pixelsPerUnit = 16f;

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
        float size = Mathf.Lerp(cam.orthographicSize, targetSize, zoomSpeed * Time.deltaTime);

        if (pixelsPerUnit > 0f)
        {
            float step = 1f / pixelsPerUnit;
            size = Mathf.Round(size / step) * step;
        }

        cam.orthographicSize = size;
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
