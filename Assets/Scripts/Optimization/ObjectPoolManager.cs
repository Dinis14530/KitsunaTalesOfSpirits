using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }

    private readonly Dictionary<int, Queue<GameObject>> pools = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static GameObject Spawn(
        GameObject prefab,
        Vector3 position,
        Quaternion rotation,
        Transform parent = null
    )
    {
        if (prefab == null)
            return null;

        EnsureInstance();
        return Instance.SpawnInternal(prefab, position, rotation, parent);
    }

    public static void Release(GameObject instance)
    {
        if (instance == null)
            return;

        EnsureInstance();
        Instance.ReleaseInternal(instance);
    }

    private GameObject SpawnInternal(
        GameObject prefab,
        Vector3 position,
        Quaternion rotation,
        Transform parent
    )
    {
        int poolId = prefab.GetInstanceID();

        if (!pools.TryGetValue(poolId, out Queue<GameObject> pool))
        {
            pool = new Queue<GameObject>();
            pools[poolId] = pool;
        }

        GameObject instance = pool.Count > 0 ? pool.Dequeue() : CreateInstance(prefab, poolId);
        PooledObject pooledObject = instance.GetComponent<PooledObject>();
        if (pooledObject != null)
            pooledObject.MarkAsInUse();

        Transform instanceTransform = instance.transform;
        instanceTransform.SetParent(parent, false);
        instanceTransform.SetPositionAndRotation(position, rotation);
        instance.SetActive(true);
        return instance;
    }

    private void ReleaseInternal(GameObject instance)
    {
        PooledObject pooledObject = instance.GetComponent<PooledObject>();

        if (pooledObject == null)
        {
            Destroy(instance);
            return;
        }

        if (!pooledObject.TryMarkAsReleased())
            return;

        if (!pools.TryGetValue(pooledObject.PoolId, out Queue<GameObject> pool))
        {
            pool = new Queue<GameObject>();
            pools[pooledObject.PoolId] = pool;
        }

        instance.SetActive(false);
        pool.Enqueue(instance);
    }

    private GameObject CreateInstance(GameObject prefab, int poolId)
    {
        GameObject instance = Instantiate(prefab);
        PooledObject pooledObject = instance.GetComponent<PooledObject>();

        if (pooledObject == null)
            pooledObject = instance.AddComponent<PooledObject>();

        pooledObject.SetPoolId(poolId);
        return instance;
    }

    private static void EnsureInstance()
    {
        if (Instance != null)
            return;

        GameObject managerObject = new GameObject("ObjectPoolManager");
        Instance = managerObject.AddComponent<ObjectPoolManager>();
        DontDestroyOnLoad(managerObject);
    }
}

public class PooledObject : MonoBehaviour
{
    public int PoolId { get; private set; }
    private bool isReleased = true;

    public void SetPoolId(int poolId)
    {
        PoolId = poolId;
    }

    public void MarkAsInUse()
    {
        isReleased = false;
    }

    public bool TryMarkAsReleased()
    {
        if (isReleased)
            return false;

        isReleased = true;
        return true;
    }
}
