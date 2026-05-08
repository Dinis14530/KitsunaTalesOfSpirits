using TMPro;
using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    private const string ShowFpsPrefKey = "ShowFPS";

    public static FPSDisplay Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private Canvas fpsCanvas;
    [SerializeField] private TMP_Text fpsText;

    [Header("Atualização")]
    [SerializeField] private float updateInterval = 0.25f;

    [Header("Estado")]
    [SerializeField] private bool showByDefault = true;

    private float unscaledTimeAccumulator;
    private int frameCounter;
    private bool isDisplayEnabled;

    public bool IsDisplayEnabled => isDisplayEnabled;

    public static FPSDisplay GetExisting()
    {
        if (Instance != null)
            return Instance;

        return FindObjectOfType<FPSDisplay>(true);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        ResolveUIReferences();

        bool savedShowValue = PlayerPrefs.GetInt(ShowFpsPrefKey, showByDefault ? 1 : 0) == 1;
        SetDisplayEnabled(savedShowValue, false);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Update()
    {
        if (!isDisplayEnabled)
            return;

        if (fpsText == null)
            return;

        frameCounter++;
        unscaledTimeAccumulator += Time.unscaledDeltaTime;

        if (unscaledTimeAccumulator < updateInterval)
            return;

        float fps = frameCounter / unscaledTimeAccumulator;

        if (fpsText != null)
        {
            fpsText.text = $"FPS: {fps:0}";
        }

        frameCounter = 0;
        unscaledTimeAccumulator = 0f;
    }

    public void SetDisplayEnabled(bool enabled)
    {
        SetDisplayEnabled(enabled, true);
    }

    private void SetDisplayEnabled(bool enabled, bool savePreference)
    {
        isDisplayEnabled = enabled;

        if (fpsCanvas != null)
            fpsCanvas.gameObject.SetActive(enabled);
        else if (fpsText != null)
            fpsText.enabled = enabled;

        frameCounter = 0;
        unscaledTimeAccumulator = 0f;

        if (savePreference)
        {
            PlayerPrefs.SetInt(ShowFpsPrefKey, enabled ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    private void ResolveUIReferences()
    {
        if (fpsCanvas == null)
            fpsCanvas = GetComponentInChildren<Canvas>(true);

        if (fpsCanvas == null)
            return;

        if (fpsText == null)
            fpsText = fpsCanvas.GetComponentInChildren<TMP_Text>(true);
    }
}