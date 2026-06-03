using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public Rigidbody2D targetRb;

    [Header("Look Ahead")]
    public float lookAheadDistance = 2f;
    public float lookAheadSmooth = 0.2f;
    private float currentLookAhead = 0f;
    private float targetLookAhead = 0f;
    private float lookVel = 0f;

    [Header("Vertical Offsets")]
    public float jumpOffset = 0.8f;
    public float fallOffset = -0.6f;

    [Header("Head Offset")]
    public float headOffsetY = 1.2f; // ajusta para a altura da cabeça do player

    [Header("Camera Bounds Object")]
    public BoxCollider2D cameraBounds;

    [Header("Pixel Perfect")]
    public float pixelsPerUnit = 16f;

    [Header("Rendering")]
    public bool disableMSAA = true;

    private Camera cam;
    private float camHalfWidth;
    private float camHalfHeight;

    private void Awake()
    {
        cam = GetComponent<Camera>();

        if (disableMSAA)
            cam.allowMSAA = false;

        camHalfHeight = cam.orthographicSize;
        camHalfWidth = cam.aspect * camHalfHeight;

        if (targetRb == null && target != null)
            targetRb = target.GetComponent<Rigidbody2D>();
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        // Usa a velocidade actual do alvo para antecipar o movimento da camera
        float velX = targetRb != null ? targetRb.linearVelocity.x : 0f;
        float velY = targetRb != null ? targetRb.linearVelocity.y : 0f;

        // --- LOOK AHEAD ---
        if (Mathf.Abs(velX) > 0.1f)
            targetLookAhead = Mathf.Sign(velX) * lookAheadDistance;
        else
            targetLookAhead = 0f;

        currentLookAhead = Mathf.SmoothDamp(
            currentLookAhead,
            targetLookAhead,
            ref lookVel,
            lookAheadSmooth
        );

        // --- VERTICAL OFFSET ---
        float extraVertical = 0f;
        // Compensa salto ou queda para manter o player melhor enquadrado
        if (velY > 0.05f)
            extraVertical = jumpOffset;
        else if (velY < -0.05f)
            extraVertical = fallOffset;

        // --- POSIÇÃO FINAL (agora com headOffsetY) ---
        Vector3 pos = new Vector3(
            target.position.x + currentLookAhead,
            target.position.y + extraVertical + headOffsetY,
            -10f
        );

        // --- LIMITES DA CAMERA ---
        if (cameraBounds != null)
        {
            Bounds b = cameraBounds.bounds;

            // Limita a camera para nao mostrar fora da area jogavel
            float minX = b.min.x + camHalfWidth;
            float maxX = b.max.x - camHalfWidth;
            float minY = b.min.y + camHalfHeight;
            float maxY = b.max.y - camHalfHeight;

            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
        }

        // Faz snap apos clamp para evitar subpixel no limite da area.
        if (pixelsPerUnit > 0f)
        {
            float step = 1f / pixelsPerUnit;
            pos.x = Mathf.Round(pos.x / step) * step;
            pos.y = Mathf.Round(pos.y / step) * step;
        }

        transform.position = pos;
    }
}
