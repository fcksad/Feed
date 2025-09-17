using System;
using System.Collections.Generic;
using UnityEngine;

namespace Service.Locator
{
    [DefaultExecutionOrder(-990)]
    public class SceneServiceLocator : MonoBehaviour
    {
        private readonly Dictionary<Type, object> _services = new();
        private readonly HashSet<Type> _initialized = new();
        private readonly object _lock = new();

        public static SceneServiceLocator Current { get; private set; }

        private void Awake()
        {
            Current = this;
        }

        public bool Register<T>(T service, bool overwrite = false) where T : class
        {
            if (service == null) { Debug.LogError($"[SceneServiceLocator] {typeof(T).Name} is null"); return false; }
            lock (_lock)
            {
                var key = typeof(T);
                if (_services.ContainsKey(key) && !overwrite)
                {
                    Debug.LogWarning($"[SceneServiceLocator] {key.Name} already registered");
                    return false;
                }
                _services[key] = service;
                _initialized.Remove(key);
                return true;
            }
        }

        public bool TryGet<T>(out T service) where T : class
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

        public T Get<T>() where T : class
        {
            if (TryGet<T>(out var s)) return s;
            throw new InvalidOperationException($"[SceneServiceLocator] {typeof(T).Name} not found");
        }

        public void InitializeAll()
        {
            lock (_lock)
            {
                foreach (var (type, obj) in _services)
                {
                    if (_initialized.Contains(type)) continue;
                    if (obj is IInitializable init)
                    {
                        try { init.Initialize(); _initialized.Add(type); }
                        catch (Exception e) { Debug.LogError($"[SceneServiceLocator] Initialize failed for {type.Name}: {e}"); }
                    }
                    else
                    {
                        _initialized.Add(type);
                    }
                }
            }
        }

        public void DisposeAll()
        {
            lock (_lock)
            {
                var types = new List<Type>(_services.Keys);
                types.Reverse();
                foreach (var t in types)
                {
                    if (_services.TryGetValue(t, out var obj) && obj is IDisposable d)
                    {
                        try { d.Dispose(); } catch (Exception e) { Debug.LogError($"[SceneServiceLocator] Dispose failed for {t.Name}: {e}"); }
                    }
                }
                _initialized.Clear();
            }
        }

        public bool Unregister<T>(bool dispose = false) where T : class
        {
            lock (_lock)
            {
                var key = typeof(T);
                if (_services.TryGetValue(key, out var obj))
                {
                    if (dispose && obj is IDisposable d)
                    {
                        try { d.Dispose(); } catch (Exception e) { Debug.LogError($"[SceneServiceLocator] Dispose failed for {key.Name}: {e}"); }
                    }
                    _services.Remove(key);
                    _initialized.Remove(key);
                    return true;
                }
                return false;
            }
        }

        public void Clear(bool dispose = false)
        {
            if (dispose) DisposeAll();
            lock (_lock)
            {
                _services.Clear();
                _initialized.Clear();
            }
        }

        private void OnDestroy()
        {
            try { DisposeAll(); } finally { Clear(dispose: false); if (Current == this) Current = null; }
        }

        private IEnumerable<(Type, object)> _servicesEnumerable
        {
            get { foreach (var kv in _services) yield return (kv.Key, kv.Value); }
        }
    }
}
