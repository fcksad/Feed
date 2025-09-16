using System.Collections.Generic;
using System;
using UnityEngine;


namespace Service.Locator
{
    public class ServiceLocator : MonoBehaviour
    {
        private static readonly Dictionary<Type, object> _services = new();
        private static readonly object _lock = new();
        private static ServiceLocator _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public static bool Register<T>(T service, bool overwrite = false) where T : class
        {
            if (service == null) { Debug.LogError($"[ServiceLocator] {typeof(T).Name} is null"); return false; }
            lock (_lock)
            {
                var key = typeof(T);
                if (_services.ContainsKey(key) && !overwrite)
                {
                    Debug.LogWarning($"[ServiceLocator] {key.Name} already registered");
                    return false;
                }
                _services[key] = service;
                return true;
            }
        }

        public static T Get<T>() where T : class
        {
            if (TryGet<T>(out var s)) return s;
            throw new InvalidOperationException($"[ServiceLocator] {typeof(T).Name} not found");
        }

        public static bool TryGet<T>(out T service) where T : class
        {
            lock (_lock)
            {
                if (_services.TryGetValue(typeof(T), out var obj))
                {
                    service = obj as T;
                    return service != null;
                }
            }
            service = null;
            return false;
        }

        public static T GetOrRegister<T>(Func<T> factory) where T : class
        {
            if (TryGet(out T existing)) return existing;
            var created = factory();
            Register(created);
            return created;
        }

        public static bool Unregister<T>() where T : class
            => _services.Remove(typeof(T));

        public static void Clear() => _services.Clear();

        public static T RegisterComponent<T>() where T : Component
        {
            var go = new GameObject($"[{typeof(T).Name}]");
            DontDestroyOnLoad(go);
            var comp = go.AddComponent<T>();
            Register<T>(comp);
            return comp;
        }

        private void OnDestroy()
        {
            if (_instance == this) { _services.Clear(); _instance = null; }
        }
    }

}
