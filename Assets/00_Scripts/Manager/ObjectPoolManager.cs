using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ObjectPoolManager : SingletonBehaviour<ObjectPoolManager>
{
    private readonly Dictionary<GameObject, Queue<GameObject>> pool = new();
    private readonly Dictionary<GameObject, GameObject> spawnedToPrefab = new();

    /// <summary>
    /// 제너릭 방식: 원하는 컴포넌트 타입으로 반환
    /// </summary>
    public T GetObject<T>(T prefab, Vector3 position, Quaternion rotation, float autoReturnTime = -1f)
        where T : Component
    {
        GameObject key = prefab.gameObject;

        if (!pool.ContainsKey(key))
            pool[key] = new Queue<GameObject>();

        GameObject obj = null;

        while (pool[key].Count > 0)
        {
            var candidate = pool[key].Dequeue();
            if (candidate != null && !candidate.Equals(null))
            {
                obj = candidate;
                break;
            }
        }

        if (obj == null)
        {
            obj = Instantiate(key);
        }

        obj.transform.SetPositionAndRotation(position, rotation);
        obj.transform.localScale = key.transform.localScale;
        obj.SetActive(true);

        if (obj.TryGetComponent<IPoolable>(out var poolable))
            poolable.OnGetFromPool();

        spawnedToPrefab[obj] = key;

        if (autoReturnTime > 0f)
            StartCoroutine(ReturnAfterSeconds(obj, autoReturnTime));

        return obj.GetComponent<T>();
    }

    /// <summary>
    /// GameObject 방식 (기존 방식 유지)
    /// </summary>
    public GameObject GetObject(GameObject prefab, Vector3 position, Quaternion rotation, float autoReturnTime = -1f)
    {
        return GetObject(prefab.transform, position, rotation, autoReturnTime).gameObject;
    }

    /// <summary>
    /// 반환
    /// </summary>
    public void ReturnToPool(GameObject obj)
    {
        if (obj == null || obj.Equals(null))
        {
            if (spawnedToPrefab.TryGetValue(obj, out var originalPrefab))
            {
                GameObject newobj = Instantiate(originalPrefab);
                newobj.SetActive(false);

                if (!pool.ContainsKey(originalPrefab))
                {
                    pool[originalPrefab] = new Queue<GameObject>();
                }

                pool[originalPrefab].Enqueue(newobj);
            }
            spawnedToPrefab.Remove(obj);
            return;
        }
        if (!spawnedToPrefab.TryGetValue(obj, out var prefab))
        {
            Debug.LogWarning("모르는 오브젝트 반환. Destroy처리");
            Destroy(obj);
            return;
        }

        if (obj.TryGetComponent<IPoolable>(out var poolable))
        {
            poolable.OnReturnToPool();
        }

        obj.SetActive(false);
        pool[prefab].Enqueue(obj);
        spawnedToPrefab.Remove(obj);
    }


    /// <summary>
    /// 일정 시간 후 자동 반환
    /// </summary>
    private IEnumerator ReturnAfterSeconds(GameObject obj, float seconds)
    {
        yield return new WaitForSeconds(seconds);

        if (obj != null && obj.activeSelf)
            ReturnToPool(obj);
    }
}