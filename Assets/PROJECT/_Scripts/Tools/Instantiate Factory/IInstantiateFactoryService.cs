using UnityEngine;

public interface IInstantiateFactoryService
{  /// <summary>
   /// ������������� ����� �������� ���������� �������.
   /// </summary>
   /// <typeparam name="T">��� MonoBehaviour �� �������</typeparam>
   /// <param name="prefab">������ � ����������� ���� T</param>
   /// <param name="parent">�������� � �������� (����� ���� null)</param>
   /// <param name="position">������� ������� (�� ��������� Vector3.zero)</param>
   /// <param name="rotation">������� (�� ��������� Quaternion.identity)</param>
   /// <param name="customName">���������� ��� GameObject (�����������)</param>
    T Create<T>(T prefab, Transform parent = null, Vector3? position = null, Quaternion? rotation = null, string customName = null) where T : MonoBehaviour;

    public void Release<T>(T instance) where T : MonoBehaviour;


}
