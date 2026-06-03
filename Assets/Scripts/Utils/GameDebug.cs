using System.Diagnostics;

/// <summary>
/// Editor-only logging wrapper. Calls are stripped from non-editor builds
/// via the Conditional attribute, eliminating string allocations and info
/// leakage in production.
/// </summary>
public static class GameDebug
{
    [Conditional("UNITY_EDITOR")]
    public static void Log(string message)
    {
        UnityEngine.Debug.Log(message);
    }

    [Conditional("UNITY_EDITOR")]
    public static void Log(object message)
    {
        UnityEngine.Debug.Log(message);
    }
}
