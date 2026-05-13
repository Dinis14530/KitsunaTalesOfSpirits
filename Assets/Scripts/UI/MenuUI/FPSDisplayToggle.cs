using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class FPSDisplayToggle : MonoBehaviour
{
    private const string ShowFpsPrefKey = "ShowFPS";
    private const int DefaultShowFps = 1;

    [SerializeField]
    private FPSDisplay fpsDisplay;

    private Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
    }

    private void Start()
    {
        if (fpsDisplay == null)
            fpsDisplay = FPSDisplay.GetExisting();

        if (fpsDisplay != null)
            toggle.SetIsOnWithoutNotify(fpsDisplay.IsDisplayEnabled);
        else
            toggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt(ShowFpsPrefKey, DefaultShowFps) == 1);

        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnDestroy()
    {
        if (toggle != null)
            toggle.onValueChanged.RemoveListener(OnToggleChanged);
    }

    public void OnToggleChanged(bool enabled)
    {
        if (fpsDisplay == null)
            fpsDisplay = FPSDisplay.GetExisting();

        if (fpsDisplay != null)
        {
            fpsDisplay.SetDisplayEnabled(enabled);
            return;
        }

        PlayerPrefs.SetInt(ShowFpsPrefKey, enabled ? 1 : 0);
        PlayerPrefs.Save();
    }
}
