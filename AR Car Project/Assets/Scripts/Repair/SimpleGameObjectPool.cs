using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Minimal stack pool for UI prefab instances to avoid repeated Instantiate/Destroy churn.
/// </summary>
public sealed class SimpleGameObjectPool
{
    readonly Stack<GameObject> _available = new Stack<GameObject>();
    readonly GameObject _prefab;
    readonly Transform _parent;

    public SimpleGameObjectPool(GameObject prefab, Transform parent)
    {
        _prefab = prefab;
        _parent = parent;
    }

    public GameObject Get()
    {
        while (_available.Count > 0)
        {
            var go = _available.Pop();
            if (go != null)
            {
                go.SetActive(true);
                return go;
            }
        }

        return Object.Instantiate(_prefab, _parent, false);
    }

    public void Release(GameObject instance)
    {
        if (instance == null)
            return;
        instance.SetActive(false);
        instance.transform.SetParent(_parent, false);
        _available.Push(instance);
    }

    public void ClearDestroyed()
    {
        // No-op: pool assumes releases are valid; optional cleanup could scan stack
    }
}
