using UnityEngine;

public interface IPoolService
{
    T GetFromPool<T>(T prefab) where T : Component;
    void ReturnToPool<T>(T instance) where T : Component;
    void ReleaseAll();
}
