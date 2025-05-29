using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PoolService : IPoolService, IInitializable
{
    private Dictionary<string, Queue<GameObject>> _pool = new();
    private Dictionary<string, Transform> _holders = new();

    private readonly DiContainer _container;
    private Transform _root;

    [Inject]    
    public PoolService(DiContainer container)
    {
        _container = container;
    }

    public void Initialize()
    {
        var go = new GameObject("[PoolService]");
        Object.DontDestroyOnLoad(go);
        _root = go.transform;
    }

    public T GetFromPool<T>(T prefab) where T : Component
    {
        string key = typeof(T).Name;

        if (!_pool.ContainsKey(key))
            _pool[key] = new Queue<GameObject>();

        if (!_holders.ContainsKey(key))
        {
            var holder = new GameObject($"[Pool] {key}").transform;
            holder.SetParent(_root);
            _holders[key] = holder;
        }

        GameObject obj;

        if (_pool[key].Count > 0)
        {
            obj = _pool[key].Dequeue();
        }
        else
        {
            obj = _container.InstantiatePrefab(prefab.gameObject);
        }

        obj.SetActive(true);
        return obj.GetComponent<T>();
    }

    public void ReturnToPool<T>(T instance) where T : Component
    {
        if (!instance.gameObject.activeInHierarchy) return;

        string key = typeof(T).Name;

        if (!_pool.ContainsKey(key))
            _pool[key] = new Queue<GameObject>();

        if (!_holders.ContainsKey(key))
        {
            var holder = new GameObject($"[Pool] {key}").transform;
            holder.SetParent(_root);
            _holders[key] = holder;
        }

        if (instance is IPoolable poolable)
            poolable.OnDespawn();

        instance.gameObject.SetActive(false);
        instance.transform.SetParent(_holders[key], false);
        _pool[key].Enqueue(instance.gameObject);
    }

    public void ReleaseAll()
    {
        foreach (var queue in _pool.Values)
        {
            while (queue.Count > 0)
            {
                var obj = queue.Dequeue();
                Object.Destroy(obj);
            }
        }

        _pool.Clear();

        foreach (var holder in _holders.Values)
            Object.Destroy(holder.gameObject);

        _holders.Clear();
    }
}
