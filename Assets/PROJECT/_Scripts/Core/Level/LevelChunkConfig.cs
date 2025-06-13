using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelInfo
{
    [field: SerializeField] public string Type { get; private set; }
    [field: SerializeField] public LevelChunk Prefab { get; private set; }
    [field: SerializeField] public Vector3 Offset { get; private set; }
    [field: SerializeField] public float Height { get; private set; } = 3f;
    [field: SerializeField] public int Weight { get; private set; } = 1;

    public void OnValidate()
    {
        if (Type != Prefab?.ChunkType && Prefab.ChunkType != "")
        {
            Type = Prefab.ChunkType;
        }
    }
}

[CreateAssetMenu(fileName = "LevelChunk", menuName = "Configs/Levels/LevelChunk")]
public class LevelChunkConfig : ScriptableObject
{
    [field: SerializeField] public List<LevelInfo> Levels { get; private set; } = new List<LevelInfo>();

    public LevelInfo GetLevelInfoByType(string type)
    {
            foreach (var level in Levels)
                if (level.Type == type)
                    return level;

        Debug.LogError($"Chunk type not found: {type}");
        return null;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        foreach (var level in Levels)
        {
            level.OnValidate();
        }
    }
#endif
}
