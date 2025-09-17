using System.Collections.Generic;
using System;
using UnityEngine;


namespace Service.Locator
{
    public class ServiceLocator : MonoBehaviour
    {
        private static readonly Dictionary<Type, object> _services = new();
        private static readonly HashSet<Type> _initialized = new();
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
                _initialized.Remove(key);
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

        public static void InitializeAll()
        {
            lock (_lock)
            {
                foreach (var (type, obj) in _services)
                {
                    if (_initialized.Contains(type)) continue;
                    if (obj is IInitializable init)
                    {
                        try { init.Initialize(); _initialized.Add(type); }
                        catch (Exception e) { Debug.LogError($"[ServiceLocator] Initialize failed for {type.Name}: {e}"); }
                    }
                    else
                    {
                        _initialized.Add(type); 
                    }
                }
            }
        }


        public static void DisposeAll()
        {
            lock (_lock)
            {
                var types = new List<Type>(_services.Keys);
                types.Reverse();
                foreach (var t in types)
                {
                    if (_services.TryGetValue(t, out var obj) && obj is IDisposable d)
                    {
                        try { d.Dispose(); }
                        catch (Exception e) { Debug.LogError($"[ServiceLocator] Dispose failed for {t.Name}: {e}"); }
                    }
                }
                _initialized.Clear();
            }
        }


        public static bool Unregister<T>(bool dispose = false) where T : class
        {
            lock (_lock)
            {
                var key = typeof(T);
                if (_services.TryGetValue(key, out var obj))
                {
                    if (dispose && obj is IDisposable d)
                    {
                        try { d.Dispose(); } catch (Exception e) { Debug.LogError($"[ServiceLocator] Dispose failed for {key.Name}: {e}"); }
                    }
                    _services.Remove(key);
                    _initialized.Remove(key);
                    return true;
                }
                return false;
            }
        }

        public static void Clear(bool dispose = false)
        {
            if (dispose) DisposeAll();
            lock (_lock)
            {
                _services.Clear();
                _initialized.Clear();
            }
        }

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
            if (_instance == this)
            {
                try { DisposeAll(); } finally { Clear(dispose: false); _instance = null; }
            }
        }

        private static IEnumerable<(Type, object)> _servicesEnumerable
        {
            get { foreach (var kv in _services) yield return (kv.Key, kv.Value); }
        }
    }

}
