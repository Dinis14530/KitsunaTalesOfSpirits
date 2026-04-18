using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Canvas fpsCanvas;
    [SerializeField] private TMP_Text fpsText;

    [Header("Atualização")]
    [SerializeField] private float updateInterval = 0.25f;

    private float unscaledTimeAccumulator;
    private int frameCounter;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        EnsureUI();
    }

    private void Update()
    {
        frameCounter++;
        unscaledTimeAccumulator += Time.unscaledDeltaTime;

        if (unscaledTimeAccumulator < updateInterval)
            return;

        float fps = frameCounter / unscaledTimeAccumulator;

        if (fpsText != null)
        {
            fpsText.text = $"FPS: {fps:0}";
            fpsText.color = GetColorForFps(fps);
        }

        frameCounter = 0;
        unscaledTimeAccumulator = 0f;
    }

    private void EnsureUI()
    {
        if (fpsCanvas == null)
            fpsCanvas = GetComponentInChildren<Canvas>(true);

        if (fpsCanvas == null)
        {
            GameObject canvasObject = new GameObject("FPSCanvas");
            canvasObject.transform.SetParent(transform, false);

            fpsCanvas = canvasObject.AddComponent<Canvas>();
            fpsCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            fpsCanvas.sortingOrder = 5000;

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            canvasObject.AddComponent<GraphicRaycaster>();
        }

        if (fpsText == null)
        {
            GameObject textObject = new GameObject("FPSText");
            textObject.transform.SetParent(fpsCanvas.transform, false);

            fpsText = textObject.AddComponent<TextMeshProUGUI>();
            fpsText.fontSize = 24f;
            fpsText.alignment = TextAlignmentOptions.TopLeft;
            fpsText.color = Color.white;
            fpsText.raycastTarget = false;
            fpsText.text = "FPS: --";

            RectTransform rectTransform = fpsText.rectTransform;
            rectTransform.anchorMin = new Vector2(0f, 1f);
            rectTransform.anchorMax = new Vector2(0f, 1f);
            rectTransform.pivot = new Vector2(0f, 1f);
            rectTransform.anchoredPosition = new Vector2(16f, -16f);
            rectTransform.sizeDelta = new Vector2(220f, 40f);
        }
    }

    private Color GetColorForFps(float fps)
    {
        if (fps >= 50f)
            return new Color(0.35f, 0.9f, 0.45f);

        if (fps >= 30f)
            return new Color(1f, 0.8f, 0.25f);

        return new Color(1f, 0.35f, 0.35f);
    }
}