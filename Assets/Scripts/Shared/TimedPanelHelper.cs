using System.Collections;
using TMPro;
using UnityEngine;

public static class TimedPanelHelper
{
    public static Coroutine Show(
        MonoBehaviour host,
        GameObject panel,
        float duration,
        Coroutine previous = null,
        TMP_Text label = null,
        string text = null
    )
    {
        if (panel == null)
            return null;

        if (label != null && text != null)
            label.text = text;

        if (previous != null)
            host.StopCoroutine(previous);

        return host.StartCoroutine(ShowRoutine(panel, duration));
    }

    private static IEnumerator ShowRoutine(GameObject panel, float duration)
    {
        panel.SetActive(true);
        yield return new WaitForSeconds(duration);
        panel.SetActive(false);
    }

    public static void Cleanup(MonoBehaviour host, GameObject panel, ref Coroutine routine)
    {
        if (routine != null)
        {
            host.StopCoroutine(routine);
            routine = null;
        }

        if (panel != null)
            panel.SetActive(false);
    }
}
