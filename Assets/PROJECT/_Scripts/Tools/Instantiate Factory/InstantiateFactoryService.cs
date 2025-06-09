using UnityEngine;
using Zenject;

public class InstantiateFactoryService : IInstantiateFactoryService
{
    private DiContainer _container;
    private IPoolService _poolService;

    [Inject]
    public InstantiateFactoryService(DiContainer container, IPoolService poolService)
    {
        _container = container;
        _poolService = poolService;
    }

    /// <summary>
    /// ������������� ����� �������� ���������� �������.
    /// </summary>
    /// <typeparam name="T">��� MonoBehaviour �� �������</typeparam>
    /// <param name="prefab">������ � ����������� ���� T</param>
    /// <param name="parent">�������� � �������� (����� ���� null)</param>
    /// <param name="position">������� ������� (�� ��������� Vector3.zero)</param>
    /// <param name="rotation">������� (�� ��������� Quaternion.identity)</param>
    /// <param name="customName">���������� ��� GameObject (�����������)</param>
    public T Create<T>( T prefab, Transform parent = null,Vector3? position = null,Quaternion? rotation = null, string customName = null, string key = null) where T : MonoBehaviour
    {
        T instance;

        instance = _poolService.GetFromPool(prefab, key);

        var obj = instance.gameObject;

        obj.transform.SetParent(parent, false);
        obj.transform.position = position ?? Vector3.zero;
        obj.transform.rotation = rotation ?? Quaternion.identity;

        if (!string.IsNullOrEmpty(customName))
            obj.name = customName;

        return instance;
    }

    public void Release<T>(T instance, string key = null) where T : MonoBehaviour
    {
        _poolService.ReturnToPool(instance, key);
    }
}
