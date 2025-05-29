using System.Collections.Generic;
using UnityEngine;

public static class ResourceLoader
{
    /// <summary>
    /// ��������� ��� ������ ���� T �� ��������� ����� Resources.
    /// ������ ���� ������, ����� �������� ��� ������ ���� T �� ���� ����� Resources(������� �����������).
    /// �������� � ����� � � ��������.
    /// </summary>
    /// <typeparam name="T">��� ������ (��������, ScriptableObject, GameObject � �.�.)</typeparam>
    /// <param name="resourcePath">���� ������������ Resources (��������, "Components")</param>
    /// <returns>������ ��������� ��������</returns>
    public static List<T> GetAll<T>(string resourcePath = "") where T : UnityEngine.Object
    {
        var assets = Resources.LoadAll<T>(resourcePath);
        return new List<T>(assets);
    }

    /// <summary>
    /// ��������� ���� ����� ���� T �� ���������� ���� ������ ����� Resources.
    /// </summary>
    /// <typeparam name="T">��� ������</typeparam>
    /// <param name="resourcePath">���� � ����� ������������ ����� Resources (��������, "Components")</param>
    /// <returns>����������� ������ ��� null, ���� �� ������</returns>
    public static T Get<T>(string resourcePath = "") where T : UnityEngine.Object
    {
        var asset = Resources.Load<T>(resourcePath);
        if (asset == null)
        {
            UnityEngine.Debug.LogError($"[ResourceLoader] Could not load asset of type {typeof(T)} at path: {resourcePath}");
        }
        return asset;
    }


#if UNITY_EDITOR
   /* /// <summary>
    /// ��������� ������ ���������� ���� �� ���� � AssetDatabase (������� �����������).
    /// �������� ������ � Editor
    /// </summary>
    /// <typeparam name="T">��� ������ (��������, ScriptableObject, GameObject � �.�.)</typeparam>
    /// <param name="resourcePath">���� � AssetDatabase (��������, "Assets/MyConfigs")</param>
    /// <param name="typeFilter">������ ���� ��� AssetDatabase (��������, "ScriptableObject", "Prefab")</param>
    public static List<T> GetAll<T>(string resourcePath, string typeFilter) where T : UnityEngine.Object
    {
        List<T> assetList = new List<T>();

        string[] guids = AssetDatabase.FindAssets($"t:{typeFilter}", new[] { resourcePath });

        var loadedAssets = guids
            .Select(guid => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(asset => asset != null)
            .ToList();

        assetList.AddRange(loadedAssets);

        return assetList;
    }*/
#endif

}

public enum ResourceType
{

}

public static class ResourceLocation
{
    private static readonly Dictionary<ResourceType, string> _resourcePaths = new()
    {

    };


    public static string GetPathByType(ResourceType type)
    {
        return _resourcePaths[type];
    }
}
