using System.Collections.Generic;
using UnityEngine;

public abstract class StateTracker<T> : MonoBehaviour
    where T : StateTracker<T>
{
    public static T Instance { get; private set; }

    private readonly HashSet<string> trackedIds = new HashSet<string>();

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = (T)this;
    }

    public void MarkTracked(string id)
    {
        if (string.IsNullOrEmpty(id))
            return;

        if (trackedIds.Add(id))
            Debug.Log($"{GetType().Name}: tracked {id}");
    }

    public bool IsTracked(string id)
    {
        if (string.IsNullOrEmpty(id))
            return false;

        return trackedIds.Contains(id);
    }

    public List<string> GetTrackedIds()
    {
        return new List<string>(trackedIds);
    }

    public void SetTrackedIds(List<string> ids)
    {
        trackedIds.Clear();
        if (ids == null)
            return;

        foreach (var id in ids)
        {
            if (!string.IsNullOrEmpty(id))
                trackedIds.Add(id);
        }

        Debug.Log($"{GetType().Name}: loaded {trackedIds.Count} entries");
    }

    public void ClearAll()
    {
        trackedIds.Clear();
    }
}
